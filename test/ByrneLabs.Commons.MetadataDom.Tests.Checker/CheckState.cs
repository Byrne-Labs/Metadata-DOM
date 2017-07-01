using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using ByrneLabs.Commons.MetadataDom.TypeSystem;

namespace ByrneLabs.Commons.MetadataDom.Tests.Checker
{
    internal class CheckState
    {
        private static readonly string[] _allFilterRegex;
        private static readonly string[] _ignoredErrorsRegex =
        {
            @"^<module>\.FullName has a value of .+ in metadata but a value of .+ in reflection$",
            @"System\.BadImageFormatException: Invalid method header: 0x[0-9A-F][0-9A-F](?: 0x[0-9A-F][0-9A-F])?\s*at System\.Reflection\.Metadata\.MethodBodyBlock\.Create\(BlobReader reader\)",
            @"System.NotSupportedException: This method will be supported in a future version",
            @"System.NotSupportedException: This method is not valid on metadata",
            @"ReflectionOnly has a value of True in metadata but a value of False in reflection",
            "ModuleHandle has a value of System.ModuleHandle in metadata but a value of System.ModuleHandle in reflection",
            @"at System.Reflection.TypeDelegator\.get_IsSZArray\(\)",
            @"at System.Reflection.Assembly\.get_Evidence\(\)",
            @"at System.Reflection.Assembly\.get_PermissionSet\(\)",
            "Method may only be called on a Type for which Type.IsGenericParameter is true.",
            @"\.GetCustomAttributesData\(\) has \d+ items in metadata but \d+ items in reflection"
        };
        private static readonly string[] _invalidErrorsRegex =
        {
            @"^Could not find Method [\.\w]+\._VtblGap\d+_\d+\(\) with reflection$",
            @"^Could not find Method [\.\w]+\._VtblGap\d+_\d+ with reflection$"
        };
        private static readonly string[] _likelyFrameworkBugErrorsRegex =
        {
            @"ByrneLabs\.Commons\.MetadataDom\.BadMetadataException: Method .+ has \d+ parameters but \d+ parameter types were found",
            @"HasDefaultValue has a value of False in metadata but a value of True in reflection",
            @"MetadataToken has a value of \d+ in metadata but a value of \d+ in reflection",
            @"<\w+?>\{[0-9A-F]{8}-[0-9A-F]{4}-[0-9A-F]{4}-[0-9A-F]{4}-[0-9A-F]{12}\}.+?Name has a value of <\w+?>\{[0-9A-F]{8}-[0-9A-F]{4}-[0-9A-F]{4}-[0-9A-F]{4}-[0-9A-F]{12}\}.*? in metadata but a value of <\w+?>\{[0-9A-F]{8}-[0-9A-F]{4}-[0-9A-F]{4}-[0-9A-F]{4}-[0-9A-F]{12}\}.*? in reflection",
            @"System\.FormatException: Encountered an invalid type for a default value\."
        };
        private readonly List<IManagedCodeElement> _checkedMetadataElements = new List<IManagedCodeElement>();
        private readonly List<Tuple<IManagedCodeElement, object>> _comparedElements = new List<Tuple<IManagedCodeElement, object>>();
        private readonly List<string> _errors = new List<string>();
        private readonly List<Tuple<CheckPhase, Exception, object>> _exceptions = new List<Tuple<CheckPhase, Exception, object>>();
        private string _errorLogText;
        private bool _errorLogTextDirty = true;
        private string _logText;
        private bool _logTextDirty = true;
        private string _unfilteredErrorLogText;
        private bool _unfilteredErrorLogTextDirty = true;
        private string _unfilteredLogText;
        private bool _unfilteredLogTextDirty = true;
#pragma warning disable CA1810 // Initialize reference type static fields inline -- This needs to be in a static initializer so that the field declarations are not dependent on order. -- Jonathan Byrne 06/30/2017
        static CheckState()
#pragma warning restore CA1810 // Initialize reference type static fields inline
        {
            _allFilterRegex = _invalidErrorsRegex.Union(_ignoredErrorsRegex).Union(_likelyFrameworkBugErrorsRegex).ToArray();
        }

        public CheckState()
        {
            StartTime = DateTime.Now;
        }

        public System.Reflection.Assembly Assembly { get; set; }

        public ImmutableArray<IManagedCodeElement> CheckedMetadataElements
        {
            get
            {
                lock (_checkedMetadataElements)
                {
                    return _checkedMetadataElements.ToImmutableArray();
                }
            }
        }

        public ImmutableArray<IMemberInfo> ComparedMetadataMembers
        {
            get
            {
                lock (_comparedElements)
                {
                    return _comparedElements.Select(checkedElement => checkedElement.Item1).OfType<IMemberInfo>().Distinct().ToImmutableArray();
                }
            }
        }

        public ImmutableArray<MemberInfo> ComparedReflectionMembers
        {
            get
            {
                lock (_comparedElements)
                {
                    return _comparedElements.Select(checkedElement => checkedElement.Item2).OfType<MemberInfo>().Distinct().ToImmutableArray();
                }
            }
        }

        public string ErrorLogText
        {
            get
            {
                lock (_errors)
                {
                    lock (_exceptions)
                    {
                        if (_errorLogTextDirty)
                        {
                            _errorLogText = string.Join(Environment.NewLine + Environment.NewLine, Errors);
                            _errorLogTextDirty = false;
                        }
                        return _errorLogText;
                    }
                }
            }
        }

        public ImmutableArray<string> Errors
        {
            get
            {
                var filteredErrorMessages = UnfilteredErrors.Where(errorMessage => !_ignoredErrorsRegex.Any(ignoredErrorRegex => Regex.IsMatch(errorMessage, ignoredErrorRegex)));
                return filteredErrorMessages.ToImmutableArray();
            }
        }

        public TimeSpan? ExecutionTime => FinishTime.HasValue ? FinishTime.Value.Subtract(StartTime) : (TimeSpan?) null;

        public bool FailedValidation => Errors.Any() && !Faulted && !LikelyFrameworkBugFound;

        public bool Faulted => FilteredExceptions.Any();

        public bool FaultedAssemblyCopy => FilteredExceptions.Any(exception => exception.Item1 == CheckPhase.MoveAssembly);

        public bool FaultedAssemblyLoad => !NonDotNetAssembly && !IncompleteAssemblyLoad && FilteredExceptions.Any(exception => exception.Item1 == CheckPhase.AssemblyLoad);

        public bool FaultedMetadataCheck => FilteredExceptions.Any(exception => exception.Item1 == CheckPhase.MetadataCheck);

        public bool FaultedMetadataLoad => FilteredExceptions.Any(exception => exception.Item1 == CheckPhase.MetadataLoad);

        public bool FaultedReflectionComparison => !IncompleteAssemblyLoad && FilteredExceptions.Any(exception => exception.Item1 == CheckPhase.ReflectionComparison);

        public DateTime? FinishTime { get; set; }

        public bool IncompleteAssemblyLoad => LogText.Contains("System.IO.FileNotFoundException: Could not load file or assembly") || LogText.Contains("This suggests the assembly also has a native image assembly") || LogText.Contains("System.IO.FileLoadException: Could not load file or assembly");

        public bool LikelyFrameworkBugFound => Errors.Any() && Errors.All(errorMessage => _likelyFrameworkBugErrorsRegex.Any(ignoredErrorRegex => Regex.IsMatch(errorMessage, ignoredErrorRegex)));

        public string LogText
        {
            get
            {
                lock (_errors)
                {
                    lock (_exceptions)
                    {
                        if (_logTextDirty)
                        {
                            _logText = ErrorLogText + Environment.NewLine + Environment.NewLine + (ExecutionTime.HasValue ? $"{Environment.NewLine}Analysis finished in {ExecutionTime.Value.TotalSeconds} seconds{Environment.NewLine}" : Environment.NewLine);
                            _logTextDirty = false;
                        }
                        return _logText;
                    }
                }
            }
        }

        public Metadata Metadata { get; set; }

        public bool NonDotNetAssembly => LogText.Contains("The module was expected to contain an assembly manifest");

        public DateTime StartTime { get; }

        public bool Success => !Errors.Any() && !LikelyFrameworkBugFound;

        public string UnfilteredErrorLogText
        {
            get
            {
                if (_unfilteredErrorLogTextDirty)
                {
                    _unfilteredErrorLogText = string.Join(Environment.NewLine + Environment.NewLine, UnfilteredErrors);
                    _unfilteredErrorLogTextDirty = false;
                }
                return _unfilteredErrorLogText;
            }
        }

        public ImmutableArray<string> UnfilteredErrors
        {
            get
            {
                lock (_errors)
                {
                    lock (_exceptions)
                    {
                        var errorMessages = _errors.ToList();
                        foreach (var exception in _exceptions)
                        {
                            try
                            {
                                errorMessages.Add($"On code element \"{exception.Item3}\", exception was thrown:{Environment.NewLine}{exception.Item2}");
                            }
                            catch
                            {
                                errorMessages.Add($"On a code element of type \"{exception.Item3.GetType().FullName}\", exception was thrown:{Environment.NewLine}{exception.Item2}");
                            }
                        }

                        return errorMessages.Where(errorMessage => !_invalidErrorsRegex.Any(invalidErrorRegex => Regex.IsMatch(errorMessage, invalidErrorRegex))).ToImmutableArray();
                    }
                }
            }
        }

        public string UnfilteredLogText
        {
            get
            {
                if (_unfilteredLogTextDirty)
                {
                    _unfilteredLogText = UnfilteredErrorLogText + Environment.NewLine + Environment.NewLine + (ExecutionTime.HasValue ? $"{Environment.NewLine}Analysis finished in {ExecutionTime.Value.TotalSeconds} seconds{Environment.NewLine}" : Environment.NewLine);
                    _unfilteredLogTextDirty = false;
                }
                return _unfilteredLogText;
            }
        }

        private IEnumerable<Tuple<CheckPhase, Exception, object>> FilteredExceptions
        {
            get
            {
                lock (_exceptions)
                {
                    return _exceptions.Where(exception => !_allFilterRegex.Any(filterRegex => Regex.IsMatch(exception.ToString(), filterRegex)));
                }
            }
        }

        public void AddError(string error)
        {
            lock (_errors)
            {
                _errors.Add(error);
                _errorLogTextDirty = true;
                _logTextDirty = true;
            }
        }

        public void AddErrors(IEnumerable<string> errors)
        {
            lock (_errors)
            {
                _errors.AddRange(errors);
                _errorLogTextDirty = true;
                _unfilteredErrorLogTextDirty = true;
                _logTextDirty = true;
                _unfilteredLogTextDirty = true;
            }
        }

        public void AddException(Exception exception, object codeElement, CheckPhase checkPhase)
        {
            lock (_exceptions)
            {
                var realException = exception;
                while (realException is TargetInvocationException && realException.InnerException != null)
                {
                    realException = realException.InnerException;
                }

                _exceptions.Add(new Tuple<CheckPhase, Exception, object>(checkPhase, exception, codeElement));
                _errorLogTextDirty = true;
                _unfilteredErrorLogTextDirty = true;
                _logTextDirty = true;
                _unfilteredLogTextDirty = true;
            }
        }

        public bool HasBeenChecked(IManagedCodeElement codeElement)
        {
            lock (_checkedMetadataElements)
            {
                var hasBeenChecked = _checkedMetadataElements.Contains(codeElement);
                if (!hasBeenChecked)
                {
                    _checkedMetadataElements.Add(codeElement);
                }

                return hasBeenChecked;
            }
        }

        public bool HaveBeenCompared(IManagedCodeElement metadataElement, object reflectionElement)
        {
            lock (_comparedElements)
            {
                var key = new Tuple<IManagedCodeElement, object>(metadataElement, reflectionElement);
                var haveBeenCompared = _comparedElements.Contains(key);
                if (!haveBeenCompared)
                {
                    _comparedElements.Add(key);
                }

                return haveBeenCompared;
            }
        }
    }
}
