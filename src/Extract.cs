using System;
using System.IO;
using SharpCompress.Common;
using SharpCompress.Readers;
using static renpy_tools.Command;


namespace renpy_tools {
    public class Extract {
        static string config = Directory.GetCurrentDirectory() + "/config";
        public FileInfo[] rpyFiles { get; set; }

        public Extract(FileInfo[] folder) {
            Console.WriteLine("Creating temporary extraction folder...");
            DirectoryInfo tempExtractionFolder =
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/extracted");
            Console.WriteLine("Extracting all .rpa files...");
            foreach (var rpaFile in folder) {
                ExtractRpa(rpaFile.FullName, "extracted");
            }

            FileInfo[] rpaFiles = tempExtractionFolder.GetFiles("*.rpa", SearchOption.AllDirectories);
            Console.WriteLine("Extracted root .rpa. Checking for other .rpa files contained in the root file...");
            for (int i = 0; i < rpaFiles.Length; i++) {
                ExtractRpa(rpaFiles[i].FullName, "extracted");
            }

            Console.WriteLine("Extracted all sub .rpa files. Decompiling .rpyc files...");
            UnixCommand("python2", $"config/unrpyc/unrpyc-master/unrpyc.py ./extracted/");
            foreach (var rpycFile in tempExtractionFolder.GetFiles("*.rpyc", SearchOption.AllDirectories)) {
                rpycFile.Delete();
            }

            Console.WriteLine("Decompiled .rpyc files.");
            rpyFiles = tempExtractionFolder.GetFiles("*.rpy", SearchOption.AllDirectories);
        }

        static void ExtractRpa(string folder, string extractionLocation) {
            UnixCommand($"python3Venv", $"-m unrpa \"{folder}\" -p {extractionLocation}/");
        }

        static void ExtractRpyc() {
            UnixCommand("python2", $"config/unrpyc/unrpyc-master/unrpyc.py ./extracted/");
        }

        public static void ExtractCompressedFile(string file, string extractedDirectory) {
            try {
                using (Stream stream = File.OpenRead(file))
                using (var reader = ReaderFactory.Open(stream)) {
                    while (reader.MoveToNextEntry()) {
                        if (!reader.Entry.IsDirectory) {
                            Console.SetCursorPosition(0, Console.CursorTop - 1);
                            Console.WriteLine("Extracted " + reader.Entry.Key);
                            reader.WriteEntryToDirectory(extractedDirectory,
                                new ExtractionOptions {
                                    ExtractFullPath = true,
                                    Overwrite = true
                                });
                        }
                    }
                }
            } catch (InvalidOperationException) {
                Console.WriteLine(
                    "File is not a supported compressed file type. Currently supported file types: Zip, GZip, BZip2, Tar, Rar, LZip, XZ.");
                Environment.Exit(1);
            }
        }
    }
}