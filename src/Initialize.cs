using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using LibGit2Sharp;
using static script_reader.Command;

namespace script_reader {
    public class Initialize {
        private string ConfigDirectory { get; set; }

        public Initialize() {
            Console.WriteLine("Generating new configuration...");
            Console.WriteLine(
                "WARNING: A lot of files will be created in the directory of this program. Make sure you have moved the program to an appropriate location (not on your Desktop or Downloads folder). Proceed? (y or n).");
            if (Console.ReadLine()?.ToLower() == "y") {
                Console.WriteLine("ok");
                GenerateConfig();
            } else {
                Console.WriteLine("err");
            }
        }

        static void GenerateConfig() {
            DirectoryInfo configDirectory = Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/config");
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||
                RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD)) {
                // os is unix based
                if (UnixCommand("python2", "-V") != "err") {
                    Console.WriteLine("Python 2 is installed. Checking if Python 3 is also installed...");
                    if (UnixCommand("python3", "-V") != "err") {
                        Console.WriteLine("Python 3 is installed. Creating virtual environment...");
                        UnixCommand("python3", "-m venv config/venv");
                        Console.WriteLine("Virtual environment created. Installing/downloading dependencies...");
                        UnixCommand($"{configDirectory}/venv/bin/python", "-m pip install unrpa");
                        Console.WriteLine("Installed unrpa.");
                        try {
                            Repository.Clone("https://github.com/CensoredUsername/unrpyc.git",
                                $"{configDirectory}/unrpyc");
                        } catch (NameConflictException) {
                            Console.WriteLine("unrpyc already exists.");
                        }

                        Console.WriteLine("Downloaded unrpyc.");
                        Console.WriteLine("Dependencies installed/downloaded.");
                        Console.WriteLine("Initialization finished. Now re-run the program with an RPA file.");
                    } else {
                        Console.WriteLine("Python 3 is either not installed.");
                    }
                } else {
                    Console.WriteLine("Python 2 is either not installed.");
                }
            } else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                WindowsGetPythonEnvironments(UnixCommand(@"C:\Windows\System32\cmd.exe", "/c py --list-paths"));
                /*if (UnixCommand(@"C:\Windows\System32\cmd.exe", "/c py --list-paths").Length > 0) {
                    Console.WriteLine("Python 2 is installed. Checking if Python 3 is aso installed...");
                } else {
                    throw new SystemException("Python 2 is not installed.");
                }*/
                // os is windows based
                //Command("cmd.exe", "/c python -V");
                Console.WriteLine("finished");
            } else {
                Console.WriteLine("unknown platform");
            }
        }

        static Tuple<string, string> WindowsGetPythonEnvironments(string response) {
            SystemException autoDetectFailure = new SystemException(
                "Unable to auto-detect a Python environment. Please enter the environments path in the configuration file. Check documentation for more information.");
            string python2Location = "";
            string python3Location = "";
            string[] environments = response.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            foreach (var environmentLine in environments) {
                int startIndex = environmentLine.IndexOf("-");
                if (startIndex == -1) {
                    throw autoDetectFailure;
                }

                int endIndex = environmentLine.IndexOf(" ", startIndex);
                if (endIndex == -1) {
                    throw autoDetectFailure;
                }

                string pythonVersion = environmentLine.Substring(startIndex + 1, endIndex);
                if (pythonVersion.StartsWith("2") && python2Location == "") {
                    int newStartIndex = environmentLine.IndexOf(" ", endIndex);
                    if (newStartIndex == -1) {
                        python2Location = PythonManualPathEntry(2);
                    } else {
                        python2Location = environmentLine.Substring(newStartIndex + 1, environmentLine.Length - newStartIndex - 1)
                            .Trim();
                    }
                } else if (pythonVersion.StartsWith("3") && python3Location == "") {
                    int newerStartIndex = environmentLine.IndexOf(" ", endIndex);
                    if (newerStartIndex == -1) {
                        python3Location = PythonManualPathEntry(3);
                    } else {
                        python3Location = environmentLine
                            .Substring(newerStartIndex + 1, environmentLine.Length - newerStartIndex - 1).Trim();
                    }
                }
            }
            Tuple<string, string> returnTuple = new Tuple<string, string>(python2Location, python3Location);
            
            return returnTuple;
        }

        private static string PythonManualPathEntry(int version) {
            Console.WriteLine($"Python {version} could not be found. Please the the path to the Python {version} executable (e.g. 'C:/Program Files/Python{version}':");
            string path = "";
            while (true) {
                path = Console.ReadLine();
                if (UnixCommand(@"C:\Windows\System32\cmd.exe", $"/c {path} -V").Length > 0) {
                    Console.WriteLine("Incorrect path. Try again.");
                } else {
                    break;
                }
            }
            return path;
        }
    }
}