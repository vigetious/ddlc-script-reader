using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace renpy_tools {
    public class ReadFiles {
        public ScriptBuilder ScriptBuilder { get; set; }
        public ReadFiles(FileInfo[] files, List<string> commands) {
            List<string> characters = new List<string>();
            for (int i = 0; i < files.Length; i++) {
                if (files[i].Name == "definitions.rpy") {
                    characters = getCharacters(files[i]);
                    break;
                }
            }

            Console.WriteLine("Reading the script from each file...");
            FileInfo fi = new FileInfo("script");

            if (fi.Exists) {
                CheckBackups(fi);
            }

            ScriptBuilder = new ScriptBuilder(files, characters, fi, commands);
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

        private static void CheckBackups(FileInfo fi) {
            if (Directory.Exists(fi.DirectoryName + "/scriptBackups") && Directory.EnumerateFiles(fi.DirectoryName + "/scriptBackups").Count() != 0) {
                var backupFiles = new DirectoryInfo(fi.DirectoryName + "/scriptBackups").EnumerateFiles();
                List<int> backupFileNumbers = new List<int>();
                foreach (var file in backupFiles) {
                    if (file.Name.EndsWith(")")) {
                        int start = file.Name.IndexOf("(") + 1;
                        backupFileNumbers.Add(int.Parse(file.Name.Substring(start, file.Name.IndexOf(")") - start)));
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