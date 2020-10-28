using System;
using System.Collections.Generic;
using System.IO;

namespace renpy_tools {
    public class ScriptBuilder {
        public int totalNumberOfWords { get; set; }
        public List<string> script { get; set; }

        public ScriptBuilder(FileInfo[] files, List<string> characters, FileInfo fi, List<string> commands) {
            script = new List<string>();
            for (int i = 0; i < files.Length; i++) {
                using (StreamReader sr = files[i].OpenText()) {
                    var buildScript = BuildScript(sr, characters);
                    foreach (var line in buildScript) {
                        script.Add(line);
                    }
                }
            }

            totalNumberOfWords = BuildTotalNumberOfWords(script);
            if (commands.Contains("-keepScript")) {
                Console.WriteLine("Writing backup file...");
                BackupScript(script, fi);
            }
        }
        
        private static List<string> BuildScript(StreamReader sr, List<string> characters) {
            List<string> script = new List<string>();
            var s = "";
            while ((s = sr.ReadLine()) != null) {
                string potentialCharacter = null;
                var potentialCharacterSplit = s.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (potentialCharacterSplit.Length > 1) {
                    potentialCharacter = potentialCharacterSplit[0];
                }

                if (!string.IsNullOrEmpty(potentialCharacter)) {
                    if (s.Trim().StartsWith('"') && s.Trim().EndsWith('"')) {
                        if (s.Trim().Contains(' ')) {
                            script.Add(s.Trim());
                        }
                    } else if (s.Trim().EndsWith('"') && characters.Contains(potentialCharacter)) {
                        script.Add(s.Trim().Split('"')[1]);
                    }
                }
            }
            return script;
        }

        private static int BuildTotalNumberOfWords(List<string> script) {
            int numberOfWords = 0;
            foreach (var line in script) {
                numberOfWords += line.Split(" ").Length;
            }

            return numberOfWords;
        }

        private static void BackupScript(List<string> script, FileInfo fi) {
            using (StreamWriter sw = fi.AppendText()) {
                foreach (var line in script) {
                    sw.WriteLine(line);
                }
            }
        }
    }
}