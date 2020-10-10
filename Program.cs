using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using static script_reader.Command;

namespace script_reader {
    class Program {
        static void Main(string[] args) {
            var arguments = ParseCommandArguments(args);
            int wpm = 250;
            if (arguments.Item1.ContainsKey("-wpm")) {
                wpm = int.Parse(arguments.Item1["-wpm"]);
            }

            if (arguments.Item1.ContainsKey("-rm")) {
                if (arguments.Item2.Contains("hard")) {
                    //Console.WriteLine("hard");
                    ForcedCleanUp(true);
                } else {
                    ForcedCleanUp(false);
                }
            }
            ConfigChecks(arguments.Item2.Contains("init"));
            var extract = ExtractFiles(arguments.Item1.ContainsKey("-dir") ? arguments.Item1["-dir"] : args[0]);
            ReadFiles readFiles = new ReadFiles(extract.rpyFiles);
            int hoursToRead = (readFiles.ScriptBuilder.totalNumberOfWords / wpm) / 60;
            Console.WriteLine($"Total number of words: {readFiles.ScriptBuilder.totalNumberOfWords}");
            Console.WriteLine($"Time to read: {hoursToRead}h");
            CleanUp();
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
            Console.WriteLine("Cleaning up...");
            Directory.Delete(Directory.GetCurrentDirectory() + "/temp", true);
        }

        private static void ForcedCleanUp(bool hard) {
            if (hard) {
                Console.WriteLine("Are you sure you want to hard remove all files/folders created by this program? (y or n).");
            } else {
                Console.WriteLine("Are you sure you want to soft remove all files/folders created by this program? (y or n).");
            }
            
            string confirmation = Console.ReadLine()?.ToLower();
            if (confirmation == "y") {
                Console.WriteLine("Understood, removing all files/folders...");
                if (Directory.Exists(Directory.GetCurrentDirectory() + "/temp")) {
                    Directory.Delete(Directory.GetCurrentDirectory() + "/temp", true);
                } else if (Directory.Exists(Directory.GetCurrentDirectory() + "/config")) {
                    Directory.Delete(Directory.GetCurrentDirectory() + "/config", true);
                }
                if (hard) {
                    if (Directory.Exists(Directory.GetCurrentDirectory() + "/scriptBackups")) {
                        Directory.Delete(Directory.GetCurrentDirectory() + "/scriptBackups", true);
                    }
                }
            }
            Console.WriteLine("Done.");
            Environment.Exit(0);
        }

        private static Tuple<Dictionary<string, string>, List<string>> ParseCommandArguments(string[] args) {
            Dictionary<string, string> namedArguments = new Dictionary<string, string>();
            List<string> arguments = new List<string>();
            string prefix = "-";
            if (args.Length != 0) {
                for (var x = 0; x < args.Length; x++) {
                    if (args[x].StartsWith(prefix)) {
                        namedArguments.Add(args[x], args[x + 1]);
                    } else {
                        arguments.Add(args[x]);
                    }
                }
            }
            return new Tuple<Dictionary<string, string>, List<string>>(namedArguments, arguments);
        }
    }
}