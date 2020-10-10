using System;
using System.ComponentModel;
using System.Diagnostics;

namespace script_reader {
    public class Command {
        public static string UnixCommand(string exe, string args) {
            try {
                var proc = new Process() {
                    StartInfo = new ProcessStartInfo {
                        FileName = exe,
                        Arguments = args,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };
                proc.Start();
                string output = proc.StandardOutput.ReadToEnd();
                proc.WaitForExit();
                return output;
            } catch (Win32Exception e) {
                Console.WriteLine("Fatal error: Process could not be run. File name or arguments incorrect.");
                return "err";
            }
        }
    }
}