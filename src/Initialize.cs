using System;
using System.IO;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using static renpy_tools.Command;
using static renpy_tools.Config;
using static renpy_tools.Extract;

namespace renpy_tools {
    public class Initialize {
        public Initialize() {
            Console.WriteLine("Generating new configuration...");
            Console.WriteLine(
                "WARNING: A lot of files will be created in the directory of this program. Make sure you have moved the program to an appropriate location (not on your Desktop or Downloads folder). Proceed? (y or n).");
            while (true) {
                if (Console.ReadLine()?.ToLower() == "y") {
                    Tuple<string, string> pythonPaths = GenerateConfig();
                    AddOrUpdateAppSetting("renpy-tools:python2Location", pythonPaths.Item1);
                    AddOrUpdateAppSetting("renpy-tools:python3Location", pythonPaths.Item2);
                    break;
                }

                Console.WriteLine("Incorrect input. Try again.");
            }
        }

        static Tuple<string, string> GenerateConfig() {
            string python2Location = "";
            string python3Location = "";
            DirectoryInfo configDirectory = Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/config");
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||
                RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD)) {
                // os is unix based
                if (UnixCommand("python2", "-c \"import sys; print(sys.version_info.major)\"", true).StartsWith("2")) {
                    Console.WriteLine("Python 2 is installed. Checking if Python 3 is also installed...");
                    python2Location = UnixCommand("python2", "-c \"import sys; print(sys.executable)\"", true).Trim();
                } else {
                    python2Location = PythonManualPathEntry(2).Trim();
                }

                if (UnixCommand("python3", "-V", true) != "err") {
                    Console.WriteLine("Python 3 is installed. Creating virtual environment...");
                    python3Location = UnixCommand("python3", "-c \"import sys; print(sys.executable)\"", true).Trim();
                } else {
                    python3Location = PythonManualPathEntry(3).Trim();
                }

                UnixCommand(python3Location, "-m venv config/venv", true);
                Console.WriteLine("Virtual environment created. Installing/downloading dependencies...");
                AddOrUpdateAppSetting("renpy-tools:python3VenvLocation", UnixCommand($"{configDirectory}/venv/bin/python",
                    "-c \"import sys; print(sys.executable)\"", true).Trim());
                UnixCommand($"{configDirectory}/venv/bin/python", "-m pip install unrpa", true);
                Console.WriteLine("Installed unrpa.");
                if (!Directory.Exists($"{configDirectory}/unrpyc")) {
                    Download("https://github.com/vigetious/unrpyc/archive/master.zip", $"{configDirectory}/unrpyc", "unrpyc");
                }
            } else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                if (UnixCommand(@"C:\Windows\System32\cmd.exe",
                    "/c py -2 -c \"import sys; print(sys.version_info.major)\"", true).StartsWith("2")) {
                    python2Location = UnixCommand(@"C:\Windows\System32\cmd.exe",
                        "/c py -2 -c \"import sys; print(sys.executable)\"", true).Trim();
                    Console.WriteLine("Python 2 is installed. Checking if Python 3 is also installed...");
                } else {
                    python2Location = PythonManualPathEntry(2).Trim();
                }

                if (UnixCommand(@"C:\Windows\System32\cmd.exe",
                    "/c py -3 -c \"import sys; print(sys.version_info.major)\"", true).StartsWith("3")) {
                    python3Location = UnixCommand(@"C:\Windows\System32\cmd.exe",
                        "/c py -3 -c \"import sys; print(sys.executable)\"", true).Trim();
                    Console.WriteLine("Python 3 is installed. Creating virtual environment...");
                } else {
                    python3Location = PythonManualPathEntry(3).Trim();
                }

                UnixCommand(@"C:\Windows\System32\cmd.exe",
                    $"/c \"{python3Location}\" -m venv config/venv", true);
                Console.WriteLine("Virtual environment created. Installing/downloading dependencies...");
                AddOrUpdateAppSetting("renpy-tools:python3VenvLocation", UnixCommand(@"C:\Windows\System32\cmd.exe",
                    $"/c \"\"{configDirectory}/venv/Scripts/python\" -c \"import sys; print(sys.executable)\"", true).Trim());
                UnixCommand(@"C:\Windows\System32\cmd.exe",
                    $"/c \"\"{configDirectory}/venv/Scripts/python\" -m pip install unrpa\"", true);
                Console.WriteLine("Installed unrpa.");
                if (!Directory.Exists($"{configDirectory}/unrpyc")) {
                    Download("https://github.com/vigetious/unrpyc/archive/master.zip", $"{configDirectory}/unrpyc", "unrpyc");
                }
            } else {
                Console.WriteLine(
                    "Platform not recognised. Please leave an issue in the Github repo to report your OS as missing support.");
                Environment.Exit(1);
            }
            Console.WriteLine("Downloaded unrpyc.");
            Console.WriteLine("Dependencies installed/downloaded.");
            Console.WriteLine("Initialization finished. Moving the program file will require running `init` again. Now re-run the program with an RPA file.");

            return new Tuple<string, string>(python2Location, python3Location);
        }

        private static string PythonManualPathEntry(int version) {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||
                RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD)) {
                Console.WriteLine(
                    $"Python {version} could not be found. Please type the path to Python {version} (e.g. '/usr/bin/python3.7'):");
            } else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                Console.WriteLine(
                    $"Python {version} could not be found. Please type the path to the Python {version} executable (e.g. 'C:/Program Files/Python{version}/python.exe'):");
            }

            string path;
            while (true) {
                path = Console.ReadLine();
                string response = UnixCommand(path, "-c \"import sys; print(sys.version_info.major)\"", true);
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

        private static void Download(string repo, string destination, string name = null) {
            Console.WriteLine(name != null ? $"Downloading {name}..." : "Downloading file...");
            if (NetworkInterface.GetIsNetworkAvailable()) {
                HttpClient client = new HttpClient();
                var contentBytes = client.GetByteArrayAsync(new Uri(repo)).Result;
                MemoryStream stream = new MemoryStream(contentBytes);
                FileStream file = new FileStream($"{destination}Zip", FileMode.Create, FileAccess.Write);
                stream.WriteTo(file);
                file.Close();
                stream.Close();
            } else {
                Console.WriteLine("Not connected to the internet. Please reconnect and then re-run 'init'.");
                Environment.Exit(0);
            }
            ExtractCompressedFile($"{destination}Zip", destination);
        }
    }
}