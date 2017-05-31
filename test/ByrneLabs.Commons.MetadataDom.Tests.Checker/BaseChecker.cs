using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using ByrneLabs.Commons.MetadataDom.TypeSystem;

namespace ByrneLabs.Commons.MetadataDom.Tests.Checker
{
    internal class BaseChecker
    {
        private readonly CheckState _checkState = new CheckState();

        protected BaseChecker(IReadOnlyList<string> args)
        {
            if (args.Count > 3)
            {
                throw new ArgumentException("Only a base directory, assembly file name, and PDB file name can be provided");
            }

            if (args.Count == 0)
            {
                AssemblyFile = new FileInfo(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "ByrneLabs.Commons.MetadataDom.Tests.SampleToParse", "bin", "default", "netstandard1.6", "ByrneLabs.Commons.MetadataDom.Tests.SampleToParse.dll"));
            }
            else if (args.Count > 1)
            {
                BaseDirectory = new DirectoryInfo(args[0]);
                AssemblyFile = new FileInfo(args[1]);
            }
            else
            {
                AssemblyFile = new FileInfo(args[0]);
            }
            if (args.Count == 3)
            {
                PdbFile = new FileInfo(args[2]);
            }
        }

        protected BaseChecker(DirectoryInfo baseDirectory, FileInfo assemblyFile, FileInfo pdbFile = null)
        {
            AssemblyFile = assemblyFile;
            BaseDirectory = baseDirectory;
            PdbFile = pdbFile;
        }

        protected FileInfo AssemblyFile { get; }

        protected DirectoryInfo BaseDirectory { get; }

        protected FileInfo PdbFile { get; }

        private DirectoryInfo FailedValidationDirectory => new DirectoryInfo(Path.Combine(BaseDirectory.FullName, "FailedValidation"));

        private DirectoryInfo FaultedLoadAssemblyDirectory => new DirectoryInfo(Path.Combine(BaseDirectory.FullName, "FaultedLoadAssembly"));

        private DirectoryInfo FaultedMetadataCheckDirectory => new DirectoryInfo(Path.Combine(BaseDirectory.FullName, "FaultedMetadataCheck"));

        private DirectoryInfo FaultedMetadataLoadDirectory => new DirectoryInfo(Path.Combine(BaseDirectory.FullName, "FaultedMetadataLoad"));

        private DirectoryInfo FaultedReflectionComparisonDirectory => new DirectoryInfo(Path.Combine(BaseDirectory.FullName, "FaultedMetadataComparison"));

        private DirectoryInfo IncompleteAssemblyLoadDirectory => new DirectoryInfo(Path.Combine(BaseDirectory.FullName, "IncompleteAssemblyLoad"));

        private DirectoryInfo LikelyFrameworkBugFoundDirectory => new DirectoryInfo(Path.Combine(BaseDirectory.FullName, "LikelyFrameworkBugFound"));

        private DirectoryInfo NonDotNetAssembliesDirectory => new DirectoryInfo(Path.Combine(BaseDirectory.FullName, "NonDotNetAssemblies"));

        private DirectoryInfo NotRunTestDirectory => new DirectoryInfo(Path.Combine(BaseDirectory.FullName, "NotRun"));

        private DirectoryInfo PassedAssemblyDirectory => new DirectoryInfo(Path.Combine(BaseDirectory.FullName, "Passed"));

        public static CheckState CheckOnlyMetadata(FileInfo assemblyFile, FileInfo pdbFile = null)
        {
            var checker = new BaseChecker(null, assemblyFile, pdbFile);
            return checker.CheckOnlyMetadata();
        }

        public CheckState Check()
        {
            if (!AssemblyFile.Exists)
            {
                throw new ArgumentException($"Assembly file does not exist: {AssemblyFile}");
            }

            CheckPhaseMetadataLoad();

            if (!_checkState.Faulted)
            {
                CheckPhaseMetadataCheck();
            }
            if (!_checkState.Faulted)
            {
                CheckPhaseAssemblyLoad();
            }
            if (!_checkState.Faulted)
            {
                CheckPhaseReflectionComparison();
            }

            _checkState.FinishTime = DateTime.Now;

            if (BaseDirectory != null && AssemblyFile.DirectoryName.StartsWith(BaseDirectory.FullName))
            {
                CopyAssemblyToResultsFolder();
            }
            Console.WriteLine(_checkState.LogText);

            return _checkState;
        }

        public CheckState CheckOnlyMetadata()
        {
            CheckPhaseMetadataLoad();

            if (!_checkState.Faulted)
            {
                CheckPhaseMetadataCheck();
            }

            _checkState.FinishTime = DateTime.Now;

            return _checkState;
        }

        protected virtual System.Reflection.Assembly LoadAssembly() => null;

        private void CheckPhaseAssemblyLoad()
        {
            try
            {
                _checkState.Assembly = LoadAssembly();

                if (!string.Equals(_checkState.Metadata.AssemblyDefinition.FullName.ToUpperInvariant(), _checkState.Assembly.FullName.ToUpperInvariant()))
                {
                    throw new InvalidOperationException($"The metadata assembly has the name \"{_checkState.Metadata.AssemblyDefinition.FullName}\", but the reflection assembly has the name \"{_checkState.Assembly.FullName}\"");
                }

                // get all types to see if it can resolve everything
                // ReSharper disable once UnusedVariable
                foreach (var method in _checkState.Assembly.DefinedTypes.SelectMany(definedType => definedType.DeclaredMembers.OfType<MethodBase>()))
                {
                    method.GetParameters();
                }

                if (_checkState.Assembly.DefinedTypes.Count() != _checkState.Metadata.AssemblyDefinition.DefinedTypes.Count())
                {
                    throw new InvalidOperationException($"The metadata assembly has {_checkState.Metadata.AssemblyDefinition.DefinedTypes.Count()} defined types but the reflection assembly has {_checkState.Assembly.DefinedTypes.Count()} defined.  This suggests the assembly also has a native image assembly.");
                }
            }
            catch (ReflectionTypeLoadException exception)
            {
                _checkState.AddException(exception, null, CheckPhase.AssemblyLoad);
                foreach (var loadException in exception.LoaderExceptions)
                {
                    _checkState.AddException(loadException, null, CheckPhase.AssemblyLoad);
                }
            }
            catch (Exception exception)
            {
                _checkState.AddException(exception, null, CheckPhase.AssemblyLoad);
            }
        }

        private void CheckPhaseMetadataCheck()
        {
            try
            {
                var metadataChecker = new MetadataChecker(_checkState);
                metadataChecker.Check();
            }
            catch (Exception exception)
            {
                _checkState.AddException(exception, null, CheckPhase.MetadataCheck);
            }
        }

        private void CheckPhaseMetadataLoad()
        {
            try
            {
                _checkState.Metadata = PdbFile == null ? new Metadata(AssemblyFile) : new Metadata(AssemblyFile, PdbFile);
            }
            catch (Exception exception)
            {
                _checkState.AddException(exception, null, CheckPhase.MetadataLoad);
            }
        }

        private void CheckPhaseReflectionComparison()
        {
            try
            {
                var reflectionComparison = new ReflectionComparison(_checkState);
                reflectionComparison.Check();
            }
            catch (Exception exception)
            {
                _checkState.AddException(exception, null, CheckPhase.ReflectionComparison);
            }
        }

        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "Must be a directory, not a file")]
        private void CopyAssembly(DirectoryInfo baseDirectory, bool useUnfilteredLog = false)
        {
            try
            {
                string logFileName;
                if (baseDirectory != null)
                {
                    string folderName;
                    if (_checkState.Metadata?.AssemblyDefinition != null)
                    {
                        folderName = $"{_checkState.Metadata.AssemblyDefinition.GetName().Name}--{_checkState.Metadata.AssemblyDefinition.GetName().Version}--{_checkState.Metadata.AssemblyDefinition.GetName().CultureName}";
                    }
                    else
                    {
                        folderName = AssemblyFile.DirectoryName.Substring(NotRunTestDirectory.FullName.Length + 1);
                    }
                    var newDirectory = new DirectoryInfo(Path.Combine(baseDirectory.FullName, folderName));
                    try
                    {
                        newDirectory.Create();
                    }
                    catch
                    {
                        newDirectory = new DirectoryInfo(Path.Combine(baseDirectory.FullName, AssemblyFile.DirectoryName.Substring(NotRunTestDirectory.FullName.Length + 1)));
                        newDirectory.Create();
                    }
                    foreach (var file in AssemblyFile.Directory.EnumerateFiles())
                    {
                        file.CopyTo(Path.Combine(newDirectory.FullName, file.Name), true);
                    }

                    logFileName = Path.Combine(newDirectory.FullName, "tests.log");
                }
                else
                {
                    logFileName = Path.Combine(AssemblyFile.DirectoryName, "tests.log");
                }

                File.WriteAllText(logFileName, useUnfilteredLog ? _checkState.UnfilteredLogText : _checkState.LogText);
            }
            catch (Exception exception)
            {
                _checkState.AddException(exception, baseDirectory, CheckPhase.MoveAssembly);
            }
        }

        private void CopyAssemblyToResultsFolder()
        {
            if (_checkState.FaultedAssemblyLoad)
            {
                CopyAssembly(FaultedLoadAssemblyDirectory);
            }
            if (_checkState.FaultedMetadataLoad)
            {
                CopyAssembly(FaultedMetadataLoadDirectory);
            }
            if (_checkState.FaultedMetadataCheck)
            {
                CopyAssembly(FaultedMetadataCheckDirectory);
            }
            if (_checkState.IncompleteAssemblyLoad)
            {
                CopyAssembly(IncompleteAssemblyLoadDirectory);
            }
            if (_checkState.FaultedReflectionComparison)
            {
                CopyAssembly(FaultedReflectionComparisonDirectory);
            }
            if (_checkState.FailedValidation)
            {
                CopyAssembly(FailedValidationDirectory);
            }
            if (_checkState.FaultedAssemblyCopy)
            {
                CopyAssembly(null);
            }
            if (_checkState.LikelyFrameworkBugFound)
            {
                CopyAssembly(LikelyFrameworkBugFoundDirectory, true);
            }
            if (_checkState.Success)
            {
                CopyAssembly(PassedAssemblyDirectory, true);
            }
        }
    }
}
