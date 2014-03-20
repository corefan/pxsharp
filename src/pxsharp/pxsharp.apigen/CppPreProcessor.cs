using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace PxSharp.ApiGen {
    class CppPreProcessor {
        string compiler;
        string[] includes;
        string[] defines;
        string[] options;

        public CppPreProcessor (string compilerPath, string[] includeDirs, string[] defineSymbols) {
            compiler = '"' + compilerPath + '"';
            includes = includeDirs ?? new string[0];
            defines = defineSymbols ?? new string[0];
            options = new[] { "/EP" };
        }

        public string Run (string inputFile) {
            var arguments = 
                String.Join(" ",
                    includes.Select(x => "/I " + '"' + x + '"')
                    .Concat(defines.Select(x => "/D " + x))
                    .Concat(options)
                    .Concat(new[] { '"' + inputFile + '"' })
                .ToArray());

            var result = 
                new StringBuilder(1 << 16);

            var proc = new Process();
            proc.StartInfo.FileName = compiler;
            proc.StartInfo.Arguments = arguments;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.UseShellExecute = false;
            proc.OutputDataReceived += (s, e) => result.AppendLine(e.Data);
            proc.ErrorDataReceived += (s, e) => Console.WriteLine(e.Data);

            proc.Start();
            proc.BeginErrorReadLine();
            proc.BeginOutputReadLine();
            proc.WaitForExit();

            return result.ToString();
        }
    }
}
