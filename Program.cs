using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;

namespace script_reader {
    class Program {
        static void Main(string[] args) {
            /*try {
                var proc = new Process() {
                    StartInfo = new ProcessStartInfo {
                        FileName = "pythonn",
                        Arguments = "awd",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };
                proc.Start();
                string output = proc.StandardOutput.ReadToEnd();
                proc.WaitForExit();
                Console.WriteLine(proc.ExitCode);
            } catch (Win32Exception e) {
                Console.WriteLine(1);
            }*/
            
            if (args[0] != "init") {
                var folderExists = false;
                foreach (var directory in Directory.GetDirectories(Directory.GetCurrentDirectory())) {
                    string dir = new DirectoryInfo(directory).Name;
                    if (dir == "config") {
                        folderExists = true;
                    }
                }

                if (folderExists) {
                    Console.WriteLine("Configuration already found. Would you like to generate new configuration (if your local config version is broken)? (y or n).");
                    if (Console.ReadLine()?.ToLower() == "y") {
                        Initialize init = new Initialize();
                    }
                } else {
                    Console.WriteLine("No existing configuration found. Creating configuration...");
                    Initialize init = new Initialize();
                }
            }
        }
    }
}