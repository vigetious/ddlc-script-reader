using System;
using System.IO;
using System.Net;
using LibGit2Sharp;
using static script_reader.Command;

namespace script_reader {
    public class Initialize {
        private string ConfigDirectory { get; set; }
        public Initialize() {
            Console.WriteLine("Generating new configuration...");
            Console.WriteLine("WARNING: A lot of files will be created in the directory of this program. Make sure you have moved the program to an appropriate location (not on your Desktop or Downloads folder). Proceed? (y or n).");
            if (Console.ReadLine()?.ToLower() == "y") {
                Console.WriteLine("ok");
                GenerateConfig();
            } else {
                Console.WriteLine("err");
            }
        }

        static void GenerateConfig() {
            DirectoryInfo configDirectory = Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/config");
            if (Environment.OSVersion.Platform == PlatformID.Unix) {
                // os is unix based
                if (UnixCommand("python2", "-V") != "err") {
                    Console.WriteLine("Python 2 is installed. Checking if Python 3 is also installed...");
                    if (UnixCommand("python3", "-V") != "err") {
                        Console.WriteLine("Python 3 is installed. Creating virtual environment...");
                        UnixCommand("python3", $"-m venv config/venv");
                        Console.WriteLine("Virtual environment created. Installing/downloading dependencies...");
                        UnixCommand($"{configDirectory}/venv/bin/python", "-m pip install unrpa");
                        Console.WriteLine("Installed unrpa.");
                        Repository.Clone("https://github.com/CensoredUsername/unrpyc.git", $"{configDirectory}/unrpyc");
                        Console.WriteLine("Downloaded unrpyc.");
                        Console.WriteLine("Dependencies installed/downloaded.");
                        Console.WriteLine("Initialization finished. Now re-run the program with an RPA file.");
                    } else {
                        Console.WriteLine("Python 3 is either not installed.");
                    }
                } else {
                    Console.WriteLine("Python 2 is either not installed.");
                }
            } else if (Environment.OSVersion.Platform == PlatformID.Win32NT) {
                // os is windows based
                //Command("cmd.exe", "/c python -V");
                Console.WriteLine("finished");
            } else {
                Console.WriteLine("unknown platform");
            }
        }
    }
}