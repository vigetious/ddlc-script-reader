using System;
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
                generateConfig();
            } else {
                
            }
        }

        static void generateConfig() {
            DirectoryInfo configDirectory = Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/config");
            string python3Directory;
            if (Environment.OSVersion.Platform == PlatformID.Unix) {
                if (unixCommand("python2", "-V").StartsWith("Python 2")) {
                    Console.WriteLine("Python 2 is installed. Checking if Python 3 is also installed...");
                    if (unixCommand("python3", "-V").StartsWith("Python 3")) {
                        Console.WriteLine("Python 3 is installed. Creating virtual environment...");
                        unixCommand("python3", "-m venv venv");
                        Console.WriteLine("Virtual environment created. Installing/downloading dependencies...");
                        python3Directory = configDirectory + "/venv/bin/python";
                        unixCommand("./venv/bin/python", "-m pip install unrpa");
                        using (var webClient = new WebClient()) {
                            webClient.DownloadFile("https://raw.githubusercontent.com/CensoredUsername/unrpyc/master/unrpyc.py", $"{configDirectory}/unrpyc.py");
                        }
                        Console.WriteLine("Dependencies installed/downloaded.");
                    } else {
                        Console.WriteLine("Python 3 is either not installed.");
                    }
                } else {
                    Console.WriteLine("Python 2 is either not installed.");
                }
            }
        }

        static string unixCommand(string exe, string args) {
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
        }
    }
}