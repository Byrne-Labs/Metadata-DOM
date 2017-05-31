using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ByrneLabs.Commons.MetadataDom.Tests
{
    public class SampleBuild
    {
        private static readonly string[] _dotNetCoreDebugTypes = { "none", "portable", "embedded" };
        private static readonly string[] _dotNetCoreVersions = { "netcoreapp1.0", "netcoreapp1.1" };
        private static readonly string[] _dotNetFrameworkDebugTypes = { "none", "full", "pdbonly" };
        private static readonly string[] _dotNetFrameworkVersions = { "v2.0", "v3.0", "v3.5", "v4.0", "v4.5", "v4.5.1", "v4.5.2", "v4.6", "v4.6.1", "v4.6.2" };
        private static readonly string[] _dotNetStandardVersions = { "netstandard1.0", "netstandard1.1", "netstandard1.2", "netstandard1.3", "netstandard1.4", "netstandard1.5", "netstandard1.6" };
        private static readonly string[] _fileAlignments = { "512", "1024", "2048", "4096", "8192" };
        private static readonly string[] _outputTypes = { "Library", "Exe", "Module", "Winexe" };
        private static readonly string[] _platforms = { "AnyCPU", "x86", "x64" };
        private static readonly Random _random = new Random();
        private static readonly DirectoryInfo _sampleProjectBuildDirectory = new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, $"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}ByrneLabs.Commons.MetadataDom.Tests.SampleToParse", "bin"));
        private static readonly DirectoryInfo _sampleProjectDirectory = new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, $"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}ByrneLabs.Commons.MetadataDom.Tests.SampleToParse"));

        public SampleBuild(string targetFramework, bool debugSymbols, bool optimize, string fileAlignment, int languageVersion, string platform, string outputType, string debugType)
        {
            TargetFramework = targetFramework;
            DebugSymbols = debugSymbols;
            Optimize = optimize;
            FileAlignment = fileAlignment;
            LanguageVersion = languageVersion;
            Platform = platform;
            OutputType = outputType;
            DebugType = debugType;
        }

        public static IEnumerable<FileInfo> BuiltSamples =>
            _sampleProjectBuildDirectory.EnumerateFiles("*.dll", SearchOption.AllDirectories)
                .Union(_sampleProjectBuildDirectory.EnumerateFiles("*.exe", SearchOption.AllDirectories))
                .Union(_sampleProjectBuildDirectory.EnumerateFiles("*.netmodule", SearchOption.AllDirectories)).ToList();

        public string BuildName
        {
            get
            {
                var buildName = new StringBuilder();
                buildName.Append(TargetFramework).Append("-CSharp").Append(LanguageVersion).Append("-");
                if (DebugSymbols)
                {
                    buildName.Append("Debug-");
                }
                if (Optimize)
                {
                    buildName.Append("Optimized-");
                }
                buildName.Append(DebugType).Append("-").Append(FileAlignment).Append("-").Append(OutputType);

                return buildName.ToString();
            }
        }

        public bool DebugSymbols { get; }

        public string DebugType { get; }

        public string FileAlignment { get; }

        public bool IsFramework => _dotNetFrameworkVersions.Contains(TargetFramework);

        public int LanguageVersion { get; }

        public bool Optimize { get; }

        public string OutputType { get; }

        public string Platform { get; }

        public string TargetFramework { get; }

        public static IEnumerable<FileInfo> GetSampleAssemblies(int sampleCount = 100)
        {
            if (_sampleProjectBuildDirectory.EnumerateFiles("failure.txt", SearchOption.AllDirectories).Any())
            {
                throw new InvalidOperationException("Some sample builds have failed");
            }

            var assembliesNeeded = sampleCount - BuiltSamples.Count();
            var builtDirectories = _sampleProjectBuildDirectory.EnumerateDirectories();

            var sampleBuilds = new List<SampleBuild>();
            while (sampleBuilds.Count < assembliesNeeded)
            {
                var build = CreateRandomBuild();
                if (!sampleBuilds.Contains(build) || builtDirectories.Any(directory => directory.Name.Equals(build.BuildName)))
                {
                    sampleBuilds.Add(build);
                }
            }

            var passed = true;
            Parallel.ForEach(sampleBuilds, sampleBuild => passed &= sampleBuild.Build());
            if (!passed)
            {
                throw new InvalidOperationException("Some sample builds have failed");
            }

            return BuiltSamples.OrderBy(x => _random.Next()).Take(sampleCount).ToList();
        }

        private static SampleBuild CreateRandomBuild()
        {
            var allFrameworks = _dotNetFrameworkVersions.Union(_dotNetCoreVersions).Union(_dotNetStandardVersions).ToArray();
            var targetFramework = allFrameworks[_random.Next(allFrameworks.Length)];
            var sampleBuild = new SampleBuild
            (
                targetFramework,
                _random.Next(2) == 0,
                _random.Next(2) == 0,
                _fileAlignments[_random.Next(_fileAlignments.Length)],
                _random.Next(6) + 2,
                _platforms[_random.Next(_platforms.Length)],
                _outputTypes[_random.Next(_outputTypes.Length)],
                _dotNetFrameworkVersions.Contains(targetFramework) ? _dotNetFrameworkDebugTypes[_random.Next(_dotNetFrameworkDebugTypes.Length)] : _dotNetCoreDebugTypes[_random.Next(_dotNetCoreDebugTypes.Length)]
            );
            return sampleBuild;
        }

        private static string GetDotNetRuntimePath() => @"C:\Program Files\dotnet\dotnet.exe";

        private static string GetMsBuildPath() => @"C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild\15.0\Bin\amd64\msbuild.exe";

        public bool Build()
        {
            var success = true;
            if (!IsFramework)
            {
                var restoreProcessStartInfo = new ProcessStartInfo(GetDotNetRuntimePath())
                {
                    Arguments = $"restore {GetBuildArguments()}",
                    WorkingDirectory = _sampleProjectDirectory.FullName,
                    RedirectStandardOutput = true
                };
                success &= RunTask(restoreProcessStartInfo);
            }
            var processStartInfo = new ProcessStartInfo(GetMsBuildPath())
            {
                Arguments = GetBuildArguments(),
                WorkingDirectory = _sampleProjectDirectory.FullName,
                RedirectStandardOutput = true
            };
            success &= RunTask(processStartInfo);

            return success;
        }

        public override bool Equals(object obj)
        {
            var castObj = obj as SampleBuild;
            return ReferenceEquals(this, obj) || castObj != null &&
                   string.Equals(castObj.OutputType, OutputType) &&
                   string.Equals(castObj.TargetFramework, TargetFramework) &&
                   castObj.LanguageVersion == LanguageVersion &&
                   string.Equals(castObj.Platform, Platform) &&
                   string.Equals(castObj.DebugType, DebugType) &&
                   string.Equals(castObj.FileAlignment, FileAlignment) &&
                   castObj.DebugSymbols == DebugSymbols &&
                   castObj.Optimize == Optimize;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 91;
                hash = hash * 17 + OutputType.GetHashCode();
                hash = hash * 17 + TargetFramework.GetHashCode();
                hash = hash * 17 + LanguageVersion.GetHashCode();
                hash = hash * 17 + Platform.GetHashCode();
                hash = hash * 17 + DebugType.GetHashCode();
                hash = hash * 17 + DebugSymbols.GetHashCode();
                hash = hash * 17 + FileAlignment.GetHashCode();
                hash = hash * 17 + Optimize.GetHashCode();
                return hash;
            }
        }

        private string GetBuildArguments()
        {
            var arguments = new StringBuilder();
            arguments.Append("/property:OutputType=").Append(OutputType).Append(" ");
            arguments.Append("/property:DebugType=").Append(DebugType).Append(" ");
            arguments.Append("/property:FileAlignment=").Append(FileAlignment).Append(" ");
            arguments.Append("/property:LangVersion=").Append(LanguageVersion).Append(" ");
            arguments.Append("/property:Platform=").Append(Platform).Append(" ");
            arguments.Append("/property:DebugSymbols=").Append(DebugSymbols.ToString().ToLower()).Append(" ");
            if (_dotNetFrameworkVersions.Contains(TargetFramework))
            {
                arguments.Append("/property:TargetFrameworkVersion=").Append(TargetFramework).Append(" ");
            }
            else
            {
                arguments.Append("/property:TargetFramework=").Append(TargetFramework).Append(" ");
            }
            arguments.Append("/property:Optimize=").Append(Optimize.ToString().ToLower()).Append(" ");
            arguments.Append("/property:BuildName=").Append(BuildName).Append(" ");

            if (_dotNetFrameworkVersions.Contains(TargetFramework))
            {
                arguments.Append("ByrneLabs.Commons.MetadataDom.Tests.SampleToParse.NetFramework.csproj");
            }
            else
            {
                arguments.Append("ByrneLabs.Commons.MetadataDom.Tests.SampleToParse.NetCore.csproj");
            }

            return arguments.ToString();
        }

        private bool RunTask(ProcessStartInfo processStartInfo)
        {
            using (var process = new Process())
            using (var outputWaitHandle = new AutoResetEvent(false))
            {
                var output = new StringBuilder();

                process.StartInfo = processStartInfo;
                process.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data == null)
                    {
                        outputWaitHandle.Set();
                    }
                    else
                    {
                        output.AppendLine(e.Data);
                    }
                };
                process.Start();
                process.BeginOutputReadLine();
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    var failureMessage = new StringBuilder();
                    failureMessage.Append("WorkingDirectory: \"").Append(processStartInfo.WorkingDirectory).AppendLine("\"");
                    failureMessage.Append("Command: \"").Append(processStartInfo.FileName).Append("\" ").AppendLine(processStartInfo.Arguments).AppendLine();
                    failureMessage.AppendLine("Output:");
                    failureMessage.AppendLine(output.ToString());
                    Debug.WriteLine(failureMessage.ToString());
                    var failureLogFile = new FileInfo(Path.Combine(_sampleProjectDirectory.FullName, "bin", BuildName, "failure.txt"));
                    failureLogFile.Directory.Create();
                    File.AppendAllText(failureLogFile.FullName, failureMessage.ToString());
                }

                return process.ExitCode == 0;
            }
        }
    }
}
