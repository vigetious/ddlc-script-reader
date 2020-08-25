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
            ConfigChecks(args);
            var extract = ExtractFiles(args);
        }

        private static void ConfigChecks(string[] args) {
            string folder = "";
            foreach (var directory in Directory.GetDirectories(Directory.GetCurrentDirectory())) {
                string dir = new DirectoryInfo(directory).Name;
                if (dir == "config") {
                    folder = Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/config").FullName;
                }
            }

            if (args[0] == "init") {
                if (folder != "") {
                    Console.WriteLine(
                        "Configuration already found. Would you like to generate new configuration (if your local config version is broken)? (y or n).");
                    if (Console.ReadLine()?.ToLower() == "y") {
                        Initialize init = new Initialize();
                    }
                } else {
                    Console.WriteLine("No existing configuration found. Creating configuration...");
                    Initialize init = new Initialize();
                }
            } else {
                if (folder == "") {
                    Console.WriteLine(
                        "No existing configuration found. Would you like to run the automated setup? (y or n).");
                    string confirmation = Console.ReadLine()?.ToLower();
                    Console.WriteLine("This can be run later manually by providing the 'init' argument.");
                    if (confirmation == "y") {
                        Initialize init = new Initialize();
                    } else {
                        Console.WriteLine("Okay. Exiting the program.");
                        Environment.Exit(0);
                    }
                }
            }
        }

        private static Extract ExtractFiles(string[] args) {
            if (args[0].EndsWith(".rpa")) {
                Console.WriteLine("Configuration found. RPA file found. Extracting...");
                return new Extract(args[0]);
            }
            Console.WriteLine("ERROR: Incorrect file type supplied. Make sure the file is an .rpa file.");
            Environment.Exit(13);
            return null;
        }
    }
}