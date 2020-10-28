using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static renpy_tools.Config;

namespace renpy_tools {
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
                    return GetAppSetting("renpy-toolsrenpy-tools:python2Location");
                case "python3":
                    return GetAppSetting("renpy-toolsrenpy-tools:python3Location");
                case "python3Venv":
                    return GetAppSetting("renpy-toolsrenpy-tools:python3VenvLocation");
            }
            return exe;
        }
    }
}