using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace script_reader {
    public class ReadFiles {
        public ReadFiles(FileInfo[] files) {
            List<string> characters = new List<string>();
            for (int i = 0; i < files.Length; i++) {
                if (files[i].Name == "definitions.rpy") {
                    characters = getCharacters(files[i]);
                    break;
                }
            }

            Console.WriteLine("Reading the script from each file...");
            FileInfo fi = new FileInfo("script");
            
            CheckBackups(fi);
            
            for (int i = 0; i < files.Length; i++) {
                using (StreamReader sr = files[i].OpenText()) {
                    //int numberOfWords = ScriptBuilder<int>(sr, characters);
                    using (StreamWriter sw = fi.AppendText()) {
                        foreach (var VARIABLE in ScriptBuilder<List<string>>(sr, characters)) {
                            sw.WriteLine(VARIABLE);
                        }
                    }
                }
            }
        }

        private static List<string> getCharacters(FileInfo definitions) {
            List<string> characters = new List<string>();
            using (StreamReader sr = definitions.OpenText()) {
                string s = "";
                while ((s = sr.ReadLine()) != null) {
                    if (s.Trim().StartsWith("define") &&
                        (s.Trim().Contains("Character") || s.Trim().Contains("DynamicCharacter"))) {
                        characters.Add(s.Trim().Split(' ')[1]);
                    }
                }
            }

            Console.WriteLine("Retrieved character names.");
            return characters;
        }

        private static dynamic ScriptBuilder<T>(StreamReader sr, List<string> characters) {
            List<string> script = new List<string>();
            int numberOfWords = 0;
            var s = "";
            while ((s = sr.ReadLine()) != null) {
                string potentialCharacter = null;
                try {
                    potentialCharacter = s.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries)[0];
                } catch (IndexOutOfRangeException) {
                }

                if (!string.IsNullOrEmpty(potentialCharacter)) {
                    if (s.Trim().StartsWith('"') && s.Trim().EndsWith('"')) {
                        if (s.Trim().Contains(' ')) {
                            numberOfWords += s.Trim().Split(" ").Length;
                            script.Add(s.Trim());
                        }
                    } else if (s.Trim().EndsWith('"') && characters.Contains(potentialCharacter)) {
                    }
                }
            }

            if (typeof(T) == typeof(int)) {
                return numberOfWords;
            }

            if (typeof(T) == typeof(List<string>)) {
                return script;
            }

            return null;
        }

        private static void CheckBackups(FileInfo fi) {
            if (fi.Exists) {
                if (Directory.Exists(fi.DirectoryName + "/scriptBackups")) {
                    var backupFiles = new DirectoryInfo(fi.DirectoryName + "/scriptBackups").EnumerateFiles();
                    List<int> backupFileNumbers = new List<int>();
                    foreach (var file in backupFiles) {
                        if (file.Name.EndsWith(")")) {
                            backupFileNumbers.Add(int.Parse(file.Name[^2].ToString()));
                        }
                    }
                    fi.CopyTo($"{fi.DirectoryName}/scriptBackups/{fi.Name}({backupFileNumbers.Max() + 1})");
                } else {
                    fi.CopyTo(
                        $"{Directory.CreateDirectory(fi.DirectoryName + "/scriptBackups").FullName}/{fi.Name}(1)");
                }
                fi.Delete();
            }
        }
    }
}