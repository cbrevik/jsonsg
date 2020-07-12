using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Generator1
{
    [Generator]
    public class JsonSourceGenerator : ISourceGenerator
    {
        public void Execute(SourceGeneratorContext context)
        {
            // Using the context, get any additional files that end in .xmlsettings
            IEnumerable<AdditionalText> settingsFiles = context.AdditionalFiles.Where(at => at.Path.EndsWith(".json"));
            foreach (var file in settingsFiles)
            {
                var fileName = Path.GetFileNameWithoutExtension(file.Path);
                var source = GetClass(
                    fileName,
                    JsonDocument.Parse(file.GetText().ToString()).RootElement
                );
                context.AddSource($"Json_{fileName}.cs", SourceText.From(source, Encoding.UTF8));
            }
        }

        static string GetClass(string name, JsonElement el)
        {
            var sb = new StringBuilder($@"
    public class {name} 
    {{");
            foreach (var prop in el.EnumerateObject())
            {
                sb.AppendLine($"public {GetPropertyType(prop.Value.ValueKind)} {prop.Name} {{ get; set; }}");
            }

            sb.AppendLine("}");

            return sb.ToString();
        }

        static string GetPropertyType(JsonValueKind kind) =>
            kind switch
            {
                JsonValueKind.String => "string",
                _ => "int"
            };

        public void Initialize(InitializationContext context)
        {
        }
    }
}
