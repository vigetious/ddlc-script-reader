using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace script_reader {
    static class Config {
        public static void AddOrUpdateAppSetting<T>(string sectionPathKey, T value) {
            try {
                var filePath = Path.Combine(AppContext.BaseDirectory, "config/appsettings.json");
                if (!File.Exists(filePath)) {
                    using StreamWriter sw = File.AppendText(filePath);
                    sw.Write("{}");
                }

                string json = File.ReadAllText(filePath);
                dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

                SetValueRecursively(sectionPathKey, jsonObj, value);

                string output =
                    Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(filePath, output);
            } catch (Exception ex) {
                Console.WriteLine("Error writing app settings | {0}", ex.Message);
            }
        }

        public static dynamic GetAppSetting(string path) {
            dynamic configuration = null;
            try {
                configuration = new ConfigurationBuilder()
                    .AddJsonFile(Path.Combine(AppContext.BaseDirectory, "config/appsettings.json"))
                    .Build();
            } catch (FileNotFoundException) {
                Console.WriteLine("ERROR: Configuration file not found. Did you run \"script-reader init\"?");
                Environment.Exit(1);
            }

            return configuration[path];
        }

        private static void SetValueRecursively<T>(string sectionPathKey, dynamic jsonObj, T value) {
            var remainingSections = sectionPathKey.Split(":", 2);

            var currentSection = remainingSections[0];
            if (remainingSections.Length > 1) {
                var nextSection = remainingSections[1];
                jsonObj[currentSection] ??= new JObject();
                SetValueRecursively(nextSection, jsonObj[currentSection], value);
            } else {
                jsonObj[currentSection] = value;
            }
        }
    }
}