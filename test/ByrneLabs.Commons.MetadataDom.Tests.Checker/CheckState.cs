﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ByrneLabs.Commons.MetadataDom.Tests.Checker
{
    public class CheckState
    {
        private static readonly string[] _ignoredErrorsRegex =
        {
            @"^Could not find Method [\.\w]+\._VtblGap\d+_\d+\(\) with reflection$",
            @"^\.HasDefaultValue has a value of False in metadata but a value of True in reflection$",
            @"^\.MetadataToken has a value of 0 in metadata but a value of 134217728 in reflection$",
            @"^<module>\.FullName has a value of .+ in metadata but a value of .+ in reflection$"
        };
        private static readonly string[] _likelyFrameworkBugErrorsRegex =
        {
            @"ByrneLabs\.Commons\.MetadataDom\.BadMetadataException: Method .+ has \d+ parameters but \d+ parameter types were found"
        };
        private readonly List<CodeElement> _checkedMetadataElements = new List<CodeElement>();
        private readonly List<Tuple<CodeElement, object>> _comparedElements = new List<Tuple<CodeElement, object>>();
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

        public CheckState()
        {
            StartTime = DateTime.Now;
        }

        public Assembly Assembly { get; set; }

        public ImmutableArray<CodeElement> CheckedMetadataElements
        {
            get
            {
                lock (_checkedMetadataElements)
                {
                    return _checkedMetadataElements.ToImmutableArray();
                }
            }
        }

        public ImmutableArray<IMember> ComparedMetadataMembers
        {
            get
            {
                lock (_comparedElements)
                {
                    return _comparedElements.Select(checkedElement => checkedElement.Item1).OfType<IMember>().Distinct().ToImmutableArray();
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
                            _errorLogText = string.Join(Environment.NewLine, Errors);
                            _errorLogTextDirty = false;
                        }
                        return _errorLogText;
                    }
                }
            }
        }

        public string UnfilteredErrorLogText
        {
            get
            {
                lock (_errors)
                {
                    lock (_exceptions)
                    {
                        if (_unfilteredErrorLogTextDirty)
                        {
                            _unfilteredErrorLogText = string.Join(Environment.NewLine, UnfilteredErrors);
                            _unfilteredErrorLogTextDirty = false;
                        }
                        return _unfilteredErrorLogText;
                    }
                }
            }
        }

        public ImmutableArray<string> UnfilteredErrors
        {
            get
            {
                lock (_errors)
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

                    return errorMessages.ToImmutableArray();
                }
            }
        }

        public ImmutableArray<string> Errors
        {
            get
            {
                var filteredErrorMessages = UnfilteredErrors.Where(errorMessage => !_ignoredErrorsRegex.Any(ignoredErrorRegex => Regex.IsMatch(errorMessage, ignoredErrorRegex)) && !_likelyFrameworkBugErrorsRegex.Any(ignoredErrorRegex => Regex.IsMatch(errorMessage, ignoredErrorRegex)));
                return filteredErrorMessages.ToImmutableArray();
            }
        }

        public TimeSpan? ExecutionTime => FinishTime.HasValue ? FinishTime.Value.Subtract(StartTime) : (TimeSpan?)null;

        public bool FailedValidation => Errors.Any() && !Faulted;

        public bool Faulted
        {
            get
            {
                lock (_exceptions)
                {
                    return _exceptions.Any();
                }
            }
        }

        public bool FaultedAssemblyCopy
        {
            get
            {
                lock (_exceptions)
                {
                    return _exceptions.Any(exception => exception.Item1 == CheckPhase.MoveAssembly);
                }
            }
        }

        public bool FaultedAssemblyLoad
        {
            get
            {
                lock (_exceptions)
                {
                    return !NonDotNetAssembly && !IncompleteAssemblyLoad && _exceptions.Any(exception => exception.Item1 == CheckPhase.AssemblyLoad);
                }
            }
        }

        public bool FaultedMetadataCheck
        {
            get
            {
                lock (_exceptions)
                {
                    return _exceptions.Any(exception => exception.Item1 == CheckPhase.MetadataCheck);
                }
            }
        }

        public bool FaultedMetadataLoad
        {
            get
            {
                lock (_exceptions)
                {
                    return _exceptions.Any(exception => exception.Item1 == CheckPhase.MetadataLoad);
                }
            }
        }

        public bool FaultedReflectionComparison
        {
            get
            {
                lock (_exceptions)
                {
                    return !IncompleteAssemblyLoad && _exceptions.Any(exception => exception.Item1 == CheckPhase.ReflectionComparison);
                }
            }
        }

        public bool LikelyFrameworkBugFound
        {
            get
            {
                lock (_exceptions)
                {
                    return Errors.Any(errorMessage => !_likelyFrameworkBugErrorsRegex.Any(ignoredErrorRegex => Regex.IsMatch(errorMessage, ignoredErrorRegex)));
                }
            }
        }

        public DateTime? FinishTime { get; set; }

        public bool IncompleteAssemblyLoad => LogText.Contains("System.IO.FileNotFoundException: Could not load file or assembly") || LogText.Contains("This suggests the assembly also has a native image assembly") || LogText.Contains("System.IO.FileLoadException: Could not load file or assembly");

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

        public string UnfilteredLogText
        {
            get
            {
                lock (_errors)
                {
                    lock (_exceptions)
                    {
                        if (_unfilteredLogTextDirty)
                        {
                            _unfilteredLogText = UnfilteredErrorLogText + Environment.NewLine + Environment.NewLine + (ExecutionTime.HasValue ? $"{Environment.NewLine}Analysis finished in {ExecutionTime.Value.TotalSeconds} seconds{Environment.NewLine}" : Environment.NewLine);
                            _unfilteredLogTextDirty = false;
                        }
                        return _unfilteredLogText;
                    }
                }
            }
        }

        public Metadata Metadata { get; set; }

        public bool NonDotNetAssembly => LogText.Contains("The module was expected to contain an assembly manifest");

        public DateTime StartTime { get; }

        public bool Success => !Errors.Any() && !LikelyFrameworkBugFound;

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

        public bool HasBeenChecked(CodeElement codeElement)
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

        public bool HaveBeenCompared(CodeElement metadataElement, object reflectionElement)
        {
            lock (_comparedElements)
            {
                var key = new Tuple<CodeElement, object>(metadataElement, reflectionElement);
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
