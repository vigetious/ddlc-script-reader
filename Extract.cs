using System;
using System.IO;
using static script_reader.Command;


namespace script_reader {
    public class Extract {
        static string config = Directory.GetCurrentDirectory() + "/config";
        public FileInfo[] rpyFiles { get; set; }
        public Extract(string folder) {
            Console.WriteLine("Creating temporary extraction folder...");
            DirectoryInfo tempExtractionFolder = Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/temp");
            Console.WriteLine("Extracting root .rpa...");
            ExtractRpa(folder, "temp");
            // in case there are any folders in the rpa file.
            FileInfo[] rpaFiles = tempExtractionFolder.GetFiles("*.rpa", SearchOption.AllDirectories);
            Console.WriteLine("Extracted root .rpa. Checking for other .rpa files contained in the root file...");
            for (int i = 0; i < rpaFiles.Length; i++) {
                ExtractRpa(rpaFiles[i].FullName, "temp");
            }
            Console.WriteLine("Extracted all sub .rpa files. Decompiling .rpyc files...");
            UnixCommand("python2", $"config/unrpyc/unrpyc.py ./temp/");
            Console.WriteLine("Decompiled .rpyc files.");
            rpyFiles = tempExtractionFolder.GetFiles("*.rpy", SearchOption.AllDirectories);
        }

        static void ExtractRpa(string folder, string extractionLocation) {
            UnixCommand($"{config}/venv/bin/python3", $"-m unrpa {folder} -p {extractionLocation}/");
            //\"{tempExtractionFolder.ToString().Replace(" ", "\\ ")}\"
        }

        static void ExtractRpyc() {
            UnixCommand("python2", $"config/unrpyc/unrpyc.py ./temp/");
        }
    }
}