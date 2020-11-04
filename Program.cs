using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using static renpy_tools.Extract;
using static renpy_tools.Update;

namespace renpy_tools {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine(args[0]);
            var arguments = ParseCommandArguments(args);
            if (arguments.Item2.Contains("-update")) {
                CheckForUpdates();
            }
            int wpm = 250;
            if (arguments.Item1.ContainsKey("-wpm")) {
                wpm = int.Parse(arguments.Item1["-wpm"]);
            }

            if (arguments.Item1.ContainsKey("-rm")) {
                if (arguments.Item2.Contains("hard")) {
                    ForcedCleanUp(true);
                } else {
                    ForcedCleanUp(false);
                }
            }

            ConfigChecks(arguments.Item2.Contains("init"));
            var extract = ExtractFiles(arguments.Item1.ContainsKey("-dir") ? arguments.Item1["-dir"] : args[0]);
            ReadFiles readFiles = new ReadFiles(extract.rpyFiles, arguments.Item2);
            int minutesToRead = readFiles.ScriptBuilder.totalNumberOfWords / wpm;
            if (arguments.Item2.Contains("-json")) {
                Console.WriteLine(JsonifyOutput(readFiles.ScriptBuilder, minutesToRead));
            } else {
                Console.WriteLine($"Total number of words: {readFiles.ScriptBuilder.totalNumberOfWords}");
                Console.WriteLine($"Time to read: {minutesToRead / 60}h {minutesToRead % 60}m");
                if (!arguments.Item2.Contains("-keepRPA")) {
                    Console.WriteLine("Cleaning up...");
                }
            }
            if (!arguments.Item2.Contains("-keepRPA")) {
                CleanUp();
            }

            if (Directory.Exists(Directory.GetCurrentDirectory() + "/zippedFolder")) {
                Directory.Delete(Directory.GetCurrentDirectory() + "/zippedFolder", true);
            }
        }

        private static void ConfigChecks(bool config) {
            string folder = "";
            foreach (var directory in Directory.GetDirectories(Directory.GetCurrentDirectory())) {
                string dir = new DirectoryInfo(directory).Name;
                if (dir == "config") {
                    folder = Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/config").FullName;
                }
            }

            if (config) {
                if (folder != "") {
                    Console.WriteLine(
                        "Configuration already found. Would you like to generate new configuration (if your local config version is broken)? (y or n).");
                    if (Console.ReadLine()?.ToLower() == "y") {
                        new Initialize();
                        Environment.Exit(0);
                    }
                } else {
                    Console.WriteLine("No existing configuration found. Creating configuration...");
                    new Initialize();
                    Environment.Exit(0);
                }
            } else {
                if (folder == "") {
                    Console.WriteLine(
                        "No existing configuration found. Would you like to run the automated setup? (y or n).");
                    string confirmation = Console.ReadLine()?.ToLower();
                    Console.WriteLine("This can be run later manually by providing the 'init' argument.");
                    if (confirmation == "y") {
                        new Initialize();
                        Environment.Exit(0);
                    } else {
                        Console.WriteLine("Okay. Exiting the program.");
                        Environment.Exit(0);
                    }
                }
            }
        }

        private static Extract ExtractFiles(string directory) {
            FileInfo[] rpaFolder;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                directory = directory.Replace("\"", "").Replace("'", "");
            }
            FileAttributes attr = File.GetAttributes(directory);
            if ((attr & FileAttributes.Directory) != FileAttributes.Directory) {
                ExtractCompressedFile(directory, Directory.GetCurrentDirectory() + "/zippedFolder");
                directory = Directory.GetCurrentDirectory() + "/zippedFolder";
            }
            try {
                rpaFolder = new DirectoryInfo(directory).GetFiles("*.rpa", SearchOption.AllDirectories);
            } catch (IOException) {
                throw new SystemException("Incorrect or no directory given.");
            }

            foreach (var file in rpaFolder) {
                if (file.FullName.EndsWith(".rpa")) {
                    Console.WriteLine("Configuration found. RPA file found. Extracting...");
                    return new Extract(rpaFolder);
                }

                Console.WriteLine("ERROR: No .rpa files found in directory.");
                Environment.Exit(13);
                return null;
            }

            Console.WriteLine("No files found in directory.");
            Environment.Exit(1);
            return null;
        }

        private static void CleanUp() {
            Directory.Delete(Directory.GetCurrentDirectory() + "/extracted", true);
        }

        private static void ForcedCleanUp(bool hard) {
            if (hard) {
                Console.WriteLine(
                    "Are you sure you want to hard remove all files/folders created by this program? (y or n).");
            } else {
                Console.WriteLine(
                    "Are you sure you want to soft remove all files/folders created by this program? (y or n).");
            }

            string confirmation = Console.ReadLine()?.ToLower();
            if (confirmation == "y") {
                Console.WriteLine("Understood, removing all files/folders...");
                if (Directory.Exists(Directory.GetCurrentDirectory() + "/extracted")) {
                    Directory.Delete(Directory.GetCurrentDirectory() + "/extracted", true);
                } 
                if (Directory.Exists(Directory.GetCurrentDirectory() + "/config")) {
                    Directory.Delete(Directory.GetCurrentDirectory() + "/config", true);
                }
                if (Directory.Exists(Directory.GetCurrentDirectory() + "/zippedFolder")) {
                    Directory.Delete(Directory.GetCurrentDirectory() + "/zippedFolder", true);
                }

                if (hard) {
                    if (Directory.Exists(Directory.GetCurrentDirectory() + "/scriptBackups")) {
                        Directory.Delete(Directory.GetCurrentDirectory() + "/scriptBackups", true);
                    }
                    if (File.Exists(Directory.GetCurrentDirectory() + "/script")) {
                        File.Delete(Directory.GetCurrentDirectory() + "/script");
                    }
                }
            }

            Console.WriteLine("Done.");
            Environment.Exit(0);
        }

        private static Tuple<Dictionary<string, string>, List<string>> ParseCommandArguments(string[] args) {
            Dictionary<string, string> namedArguments = new Dictionary<string, string>();
            List<string> arguments = new List<string>();
            if (args.Length != 0) {
                for (var x = 0; x < args.Length; x++) {
                    if (args[x].StartsWith("-") && args.Length > x + 1 && !args[x + 1].StartsWith("-")) {
                        namedArguments.Add(args[x], args[x + 1]);
                    } else {
                        arguments.Add(args[x]);
                    }
                }
            }

            return new Tuple<Dictionary<string, string>, List<string>>(namedArguments, arguments);
        }

        private static string JsonifyOutput(ScriptBuilder output, int minutesToRead) {
            Console.Clear();
            Parent parent = new Parent();
            parent.TotalWords = output.totalNumberOfWords;
            parent.Time = new Time();
            parent.Time.Hours = minutesToRead / 60;
            parent.Time.Minutes = minutesToRead % 60;
            return JsonConvert.SerializeObject(parent);
        }
    }
}