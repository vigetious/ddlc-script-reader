using System;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace renpy_tools {
    public class Update {
        public static void CheckForUpdates() {
            if (NetworkInterface.GetIsNetworkAvailable()) {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", "renpy-tools");
                var response = client.GetAsync(new Uri("https://api.github.com/repositories/289041239/releases/latest")).Result;

                if (response.IsSuccessStatusCode) {
                    var responseContent = response.Content;
                    string responseString = responseContent.ReadAsStringAsync().Result;
                    var parsed = JObject.Parse(responseString);
                    if (parsed["draft"] != null) {
                        if (!bool.Parse(parsed["draft"].ToString())) {
                            var oldVersion = parsed["tag_name"];
                            var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
                            if (currentVersion != null && oldVersion != null) {
                                if (currentVersion.ToString().Equals(oldVersion.ToString())) {
                                    Console.WriteLine("You are on the latest version!");
                                } else {
                                    Console.WriteLine($"New version detected! Download it here: {parsed["url"]}");
                                }
                            }
                        }
                    }
                } else {
                    Console.WriteLine($"Error checking for updates: {response.StatusCode}");
                    Environment.Exit(0);
                }
            } else {
                Console.WriteLine("Not connected to the internet. Please reconnect and then re-run 'update'.");
                Environment.Exit(0);
            }
            Environment.Exit(1);
        }
    }
}