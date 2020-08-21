using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace script_reader {
    public class Initialize {
        private string ConfigDirectory { get; set; }
        public Initialize() {
            Console.WriteLine("Generating new configuration...");
            Console.WriteLine("WARNING: A lot of files will be created in the directory of this program. Make sure you have moved the program to an appropriate location (not on your Desktop or Downloads folder). Proceed? (y or n).");
            if (Console.ReadLine()?.ToLower() == "y") {
                GenerateConfig();
            } else {
                
            }
        }

        static void GenerateConfig() {
            DirectoryInfo configDirectory = Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/config");
            if (Environment.OSVersion.Platform == PlatformID.Unix) {
                // os is unix based
                if (Command("python2", "-V") != "err") {
                    Console.WriteLine("Python 2 is installed. Checking if Python 3 is also installed...");
                    if (Command("python3", "-V").StartsWith("P")) {
                        Console.WriteLine("Python 3 is installed. Creating virtual environment...");
                        Command("python3", $"-m venv config/venv");
                        Console.WriteLine("Virtual environment created. Installing/downloading dependencies...");
                        Command($"{configDirectory}/venv/bin/python", "-m pip install unrpa");
                        Console.WriteLine("Installed unrpa.");
                        using (var webClient = new WebClient()) {
                            webClient.DownloadFile("https://raw.githubusercontent.com/CensoredUsername/unrpyc/master/unrpyc.py", $"{configDirectory}/unrpyc.py");
                        }
                        Console.WriteLine("Downloaded unrpyc.py.");
                        Console.WriteLine("Dependencies installed/downloaded.");
                        Console.WriteLine("Initialization finished. Ready!");
                    } else {
                        Console.WriteLine("Python 3 is either not installed.");
                    }
                } else {
                    Console.WriteLine("Python 2 is either not installed.");
                }
            } else if (Environment.OSVersion.Platform == PlatformID.Win32NT) {
                // os is windows based
                Command("cmd.exe", "python -V");
                Console.WriteLine("finished");
            } else {
                Console.WriteLine("unknown platform");
            }
        }

        static string Command(string exe, string args) {
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
                Console.WriteLine("An error has occured:");
                throw;
            }
        }
    }
}