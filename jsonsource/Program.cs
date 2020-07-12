using System;
using System.IO;
using System.Text.Json;

namespace jsonsource
{
    class Program
    {
        static void Main(string[] args)
        {
            var jsonString = File.ReadAllText("./Spotify/me.json");
            var jsonDocument = JsonDocument.Parse(jsonString);

            PrintElement(jsonDocument.RootElement);
        }

        static void PrintElement(JsonElement el)
        {
            switch (el.ValueKind)
            {
                case JsonValueKind.Object:
                    Console.WriteLine($"Object:");
                    foreach (var prop in el.EnumerateObject())
                    {
                        PrintProperty(prop);
                    }
                    break;
                case JsonValueKind.String:
                    Console.WriteLine($"String value: {el.GetString()}");
                    break;
                default:
                    Console.WriteLine("No idea");
                    break;
            }
        }

        static void PrintProperty(JsonProperty prop)
        {
            Console.WriteLine($"Prop name: {prop.Name}");
            PrintElement(prop.Value);
        }
    }
}
