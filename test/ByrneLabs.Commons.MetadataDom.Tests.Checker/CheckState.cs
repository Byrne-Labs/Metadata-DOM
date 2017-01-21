﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ByrneLabs.Commons.MetadataDom.Tests.Checker
{
    public class CheckState
    {
        private readonly List<CodeElement> _checkedMetadataElements = new List<CodeElement>();
        private readonly List<Tuple<CodeElement, object>> _comparedElements = new List<Tuple<CodeElement, object>>();
        private readonly List<string> _errors = new List<string>();
        private readonly List<Tuple<CheckPhase, Exception, object>> _exceptions = new List<Tuple<CheckPhase, Exception, object>>();
        private string _errorLogText;
        private bool _errorLogTextDirty = true;
        private string _logText;
        private bool _logTextDirty = true;

        public CheckState()
        {
            StartTime = DateTime.Now;
        }

        public Assembly Assembly { get; set; }

        public IEnumerable<CodeElement> CheckedMetadataElements
        {
            get
            {
                lock (_checkedMetadataElements)
                {
                    return new ReadOnlyCollection<CodeElement>(_checkedMetadataElements);
                }
            }
        }

        public IEnumerable<IMember> ComparedMetadataMembers
        {
            get
            {
                lock (_comparedElements)
                {
                    return new ReadOnlyCollection<IMember>(_comparedElements.Select(checkedElement => checkedElement.Item1).OfType<IMember>().Distinct().ToList());
                }
            }
        }

        public IEnumerable<MemberInfo> ComparedReflectionMembers
        {
            get
            {
                lock (_comparedElements)
                {
                    return new ReadOnlyCollection<MemberInfo>(_comparedElements.Select(checkedElement => checkedElement.Item2).OfType<MemberInfo>().Distinct().ToList());
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

        public IEnumerable<string> Errors
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

                    return errorMessages;
                }
            }
        }

        public TimeSpan? ExecutionTime => FinishTime.HasValue ? FinishTime.Value.Subtract(StartTime) : (TimeSpan?) null;

        public bool FailedValidation
        {
            get
            {
                lock (_errors)
                {
                    return _errors.Any() && !Faulted;
                }
            }
        }

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

        public Metadata Metadata { get; set; }

        public bool NonDotNetAssembly => LogText.Contains("The module was expected to contain an assembly manifest");

        public DateTime StartTime { get; }

        public bool Success => !Errors.Any();

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
                _logTextDirty = true;
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
                _logTextDirty = true;
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
