using System;
using System.IO;
using System.Runtime.InteropServices;
using LibGit2Sharp;
using static script_reader.Command;

namespace script_reader {
    public class Initialize {
        private string ConfigDirectory { get; set; }
        public string python2Location { get; set; }
        private string python3Location { get; set; }

        public Initialize() {
            Console.WriteLine("Generating new configuration...");
            Console.WriteLine(
                "WARNING: A lot of files will be created in the directory of this program. Make sure you have moved the program to an appropriate location (not on your Desktop or Downloads folder). Proceed? (y or n).");
            if (Console.ReadLine()?.ToLower() == "y") {
                Tuple<string, string> pythonPaths = GenerateConfig();
                python2Location = pythonPaths.Item1;
                python3Location = pythonPaths.Item2;
            } else {
                Console.WriteLine("err");
            }
        }

        static Tuple<string, string> GenerateConfig() {
            string python2Location = "";
            string python3Location = "";
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
                        Console.WriteLine("Python 3 is not installed.");
                    }
                } else {
                    Console.WriteLine("Python 2 is not installed.");
                }
            } else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                //WindowsGetPythonEnvironments(UnixCommand(@"C:\Windows\System32\cmd.exe", "/c py --list-paths"));
                Console.WriteLine(UnixCommand(@"C:\Windows\System32\cmd.exe",
                    "/c py -2 -c \"import sys; print(sys.version_info.major)\""));
                if (UnixCommand(@"C:\Windows\System32\cmd.exe",
                    "/c py -2 -c \"import sys; print(sys.version_info.major)\"").StartsWith("2")) {
                    Console.WriteLine("Python 2 is installed. Checking if Python 3 is also installed...");
                    python2Location = UnixCommand(@"C:\Windows\System32\cmd.exe",
                        "/c py -2 -c \"import sys; print(sys.executable)\"");
                } else {
                    python2Location = PythonManualPathEntry(2);
                }
                if (UnixCommand(@"C:\Windows\System32\cmd.exe",
                    "/c py -3 -c \"import sys; print(sys.version_info.major)\"").StartsWith("3")) {
                    Console.WriteLine("Python 3 is installed. Creating virtual environment...");
                    python3Location = UnixCommand(@"C:\Windows\System32\cmd.exe",
                        "/c py -3 -c \"import sys; print(sys.executable)\"");
                } else {
                    python3Location = PythonManualPathEntry(3);
                }
                UnixCommand(@"C:\Windows\System32\cmd.exe",
                    "/c py -3 -m venv config/venv");
                Console.WriteLine("Virtual environment created. Installing/downloading dependencies...");
                UnixCommand(@"C:\Windows\System32\cmd.exe", $"/c {configDirectory}/venv/bin/python -m pip install unrpa");
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
                Console.WriteLine("Platform not recognised. Please leave an issue in the Github repo to report your OS as missing support.");
                Environment.Exit(1);
            }
            return new Tuple<string, string>(python2Location, python3Location);
        }

        private static string PythonManualPathEntry(int version) {
            Console.WriteLine(
                $"Python {version} could not be found. Please type the path to the Python {version} executable (e.g. 'C:/Program Files/Python{version}/python.exe':");
            string path = "";
            while (true) {
                path = Console.ReadLine();
                string response = UnixCommand(path, "-c \"import sys; print(sys.version_info.major)\"");
                if (response == "err") {
                    Console.WriteLine("Try again.");
                }

                if (response.Length == 0) {
                    Console.WriteLine("Incorrect path. Try again.");
                } else if (response != version.ToString()) {
                    Console.WriteLine(
                        $"Python version {response} detected. Incorrect Python version given. The path must lead to a Python {version} executable. Try again.");
                } else {
                    break;
                }
            }

            return path;
        }
    }
}