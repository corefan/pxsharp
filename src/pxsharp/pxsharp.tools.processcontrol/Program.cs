using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PxSharp.Tools.ProcessControl {
    class Program {
        static void Main (string[] args) {
            switch (args[0]) {
                case "start": {
                        if (args.Length == 3)
                            Thread.Sleep(int.Parse(args[2]));

                        ProcessStartInfo c = new ProcessStartInfo(args[1]);
                        Process p = new Process();
                        p.StartInfo = c;
                        p.Start();
                    }
                    break;

                case "stop": {
                        foreach (Process p in Process.GetProcessesByName(args[1])) {
                            p.Kill();
                        }
                    }
                    break;
            }
        }
    }
}
