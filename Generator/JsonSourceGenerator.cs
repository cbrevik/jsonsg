using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.CodeDom.Compiler;
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
            IEnumerable<AdditionalText> settingsFiles = context.AdditionalFiles.Where(at => at.Path.EndsWith(".json"));
            foreach (var file in settingsFiles)
            {
                var fileName = Path.GetFileNameWithoutExtension(file.Path);
                var jsonDocument = JsonDocument.Parse(file.GetText().ToString());
                var sb = new StringBuilder($@"
namespace MyGenerated 
{{
    public class {fileName} 
    {{
        ");
                foreach (var prop in jsonDocument.RootElement.EnumerateObject())
                {
                    sb.AppendLine($"public {GetPropertyType(prop.Value.ValueKind)} {prop.Name} {{ get; set; }}");
                }

                sb.AppendLine("} }");

                var content = sb.ToString();

                context.AddSource($"Json_{fileName}.cs", SourceText.From(content, Encoding.UTF8));
            }
        }

        static string GetPropertyType(JsonValueKind kind) =>
            kind switch
            {
                JsonValueKind.String => "string",
                JsonValueKind.Number => "int",
                _ => "string"
            };

        public void Initialize(InitializationContext context)
        {
        }
    }
}
