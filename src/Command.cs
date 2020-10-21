using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static script_reader.Config;

namespace script_reader {
    public class Command {
        public static string UnixCommand(string exe, string args, bool ignore = false) {
            if (!ignore) {
                exe = GetConfigValue(exe);
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                    args = $"/c \"\"{exe}\" {args}\"";
                    exe = @"C:\Windows\System32\cmd.exe";
                }
            }
            try {
                var proc = new Process {
                    StartInfo = new ProcessStartInfo {
                        FileName = exe,
                        Arguments = args,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };
                proc.Start();
                string output = proc.StandardOutput.ReadToEnd();
                proc.WaitForExit();
                return output;
            } catch (Win32Exception e) {
                Console.WriteLine("An error occured: " + e);
                return "err";
            }
        }

        private static string GetConfigValue(string exe) {
            switch (exe) {
                case "python2":
                    return GetAppSetting("script-reader:python2Location");
                case "python3":
                    return GetAppSetting("script-reader:python3Location");
                case "python3Venv":
                    return GetAppSetting("script-reader:python3VenvLocation");
            }
            return exe;
        }
    }
}