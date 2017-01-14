using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ByrneLabs.Commons.MetadataDom.Tests.Checker
{
    public class BaseChecker
    {
        private readonly CheckState _checkState = new CheckState();

        protected BaseChecker(IReadOnlyList<string> args)
        {
            if (args.Count < 2)
            {
                throw new ArgumentException("A base directory and assembly file name must be provided");
            }
            if (args.Count > 3)
            {
                throw new ArgumentException("Only a base directory, assembly file name, and PDB file name can be provided");
            }

            BaseDirectory = new DirectoryInfo(args[0]);
            AssemblyFile = new FileInfo(args[0]);
            if (args.Count == 3)
            {
                PdbFile = new FileInfo(args[3]);
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

        private DirectoryInfo FailedValidationDirectory => new DirectoryInfo(Path.Combine(BaseDirectory.FullName, "../FailedValidationTests"));

        private DirectoryInfo FaultedLoadAssemblyDirectory => new DirectoryInfo(Path.Combine(BaseDirectory.FullName, "../FaultedLoadAssemblyTests"));

        private DirectoryInfo FaultedMetadataCheckDirectory => new DirectoryInfo(Path.Combine(BaseDirectory.FullName, "../FaultedMetadataCheckTests"));

        private DirectoryInfo FaultedMetadataLoadDirectory => new DirectoryInfo(Path.Combine(BaseDirectory.FullName, "../FaultedMetadataLoadTests"));

        private DirectoryInfo FaultedReflectionComparisonDirectory => new DirectoryInfo(Path.Combine(BaseDirectory.FullName, "../FaultedMetadataComparisonTests"));

        private DirectoryInfo PassedAssemblyDirectory => new DirectoryInfo(Path.Combine(BaseDirectory.FullName, "../PassedTests"));

        private DirectoryInfo TestAssemblyDirectory => new DirectoryInfo(Path.Combine(BaseDirectory.FullName, "../TestAssemblies"));

        public static CheckState CheckOnlyMetadata(FileInfo assemblyFile, FileInfo pdbFile = null)
        {
            var checker = new BaseChecker(null, assemblyFile, pdbFile);
            return checker.CheckOnlyMetadata();
        }

        public CheckState Check()
        {
            CheckPhaseMetadataLoad();

            if (!_checkState.Faulted)
            {
                CheckPhaseAssemblyLoad();
            }
            if (!_checkState.Faulted)
            {
                CheckPhaseMetadataCheck();
            }
            if (!_checkState.Faulted)
            {
                CheckPhaseReflectionComparison();
            }

            _checkState.FinishTime = DateTime.Now;

            if (BaseDirectory != null)
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
                if (_checkState.FaultedReflectionComparison)
                {
                    CopyAssembly(FaultedReflectionComparisonDirectory);
                }
                if (_checkState.FailedValidation)
                {
                    CopyAssembly(FailedValidationDirectory);
                }
                if (_checkState.Success)
                {
                    CopyAssembly(PassedAssemblyDirectory);
                }

                var assemblyDirectory = AssemblyFile.Directory;
                while (!assemblyDirectory.GetFileSystemInfos().Any())
                {
                    assemblyDirectory.Delete();
                    assemblyDirectory = assemblyDirectory.Parent;
                }
            }

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

        protected virtual Assembly LoadAssembly() => null;

        private void CheckPhaseAssemblyLoad()
        {
            try
            {
                _checkState.Assembly = LoadAssembly();
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
        private void CopyAssembly(DirectoryInfo baseDirectory)
        {
            var newFileLocation = new FileInfo(AssemblyFile.FullName.ToLower().Replace(TestAssemblyDirectory.FullName.ToLower(), baseDirectory.FullName));
            newFileLocation.Directory.Create();
            AssemblyFile.MoveTo(newFileLocation.FullName);

            var logFileName = newFileLocation.FullName.Substring(0, newFileLocation.FullName.Length - 4) + ".log";

            File.WriteAllText(logFileName, _checkState.LogText);
        }
    }
}
