using System;
using System.Collections.Generic;
using System.IO;

namespace script_reader {
    class Program {
        static void Main(string[] args) {
            var arguments = ParseCommandArguments(args);
            ConfigChecks(arguments.Item2.Contains("init"));
            var extract = ExtractFiles(args);
            ReadFiles readFiles = new ReadFiles(extract.rpyFiles);
            int hoursToRead = (readFiles.ScriptBuilder.totalNumberOfWords / 250) / 60;
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
            FileInfo[] rpaFolder;
            try {
                rpaFolder = new DirectoryInfo(args[0]).GetFiles("*.rpa", SearchOption.AllDirectories);
            } catch (IOException) {
                throw new SystemException("Incorrect directory given.");
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

        private static Tuple<Dictionary<string, string>, List<string>> ParseCommandArguments(string[] args) {
            Dictionary<string, string> namedArguments = new Dictionary<string, string>();
            List<string> arguments = new List<string>();
            string prefix = "-";
            if (args.Length != 0) {
                for (var x = 0; x < args.Length; x++) {
                    if (args[x].StartsWith(prefix)) {
                        namedArguments.Add(args[x], args[x + 1]);
                        x++;
                    } else {
                        arguments.Add(args[x]);
                    }
                }
            }
            return new Tuple<Dictionary<string, string>, List<string>>(namedArguments, arguments);
        }
    }
}