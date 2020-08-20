using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;

namespace script_reader {
    class Program {
        static void Main(string[] args) {
            
            /*var proc = new Process() {
                StartInfo = new ProcessStartInfo {
                    FileName = "./venv/bin/python",
                    Arguments = "-m pip install unrpa",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            proc.Start();
            string output = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();
            Console.WriteLine(output);
            
            Console.WriteLine(Environment.OSVersion.Platform);*/

            if (args[0] != "init") {
                foreach (var directory in Directory.GetDirectories(Directory.GetCurrentDirectory())) {
                    string dir = new DirectoryInfo(directory).Name;
                    if (dir == "config") {
                        Console.WriteLine("Configuration already found. Would you like to generate new configuration (if your local config version is broken)? (y or n).");
                        if (Console.ReadLine()?.ToLower() == "y") {
                            Initialize init = new Initialize();
                            break;
                        } else {
                            break;
                        }
                    }
                }
            } else {
                
            }
        }
    }
}