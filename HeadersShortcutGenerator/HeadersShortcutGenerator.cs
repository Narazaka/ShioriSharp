using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ShioriSharp.SourceGenerator {
    [Generator]
    public sealed class HeadersShortcutGenerator : ISourceGenerator {
        const string AttributeClassSource = @"
using System;

namespace ShioriSharp.SourceGenerator {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class HeaderShortcutAttribute : Attribute {
        public string Name { get; }
        public string? Comment { get; }
        public HeaderShortcutAttribute(string name, string? comment = null) {
            Name = name;
            Comment = comment;
        }
    }
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class HeadersShortcutAttribute : Attribute {
        public string[] Name { get; }
        public HeadersShortcutAttribute(params string[] name) => Name = name;
    }

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ReferenceHeadersShortcutAttribute : Attribute {
        public int Max { get; }
        public ReferenceHeadersShortcutAttribute(int max) => Max = max;
    }
}
";
        public void Initialize(GeneratorInitializationContext context) {
            // System.Diagnostics.Debugger.Launch();
            context.RegisterForSyntaxNotifications(() => new HeaderShortcutSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context) {
            try {
                ExecuteCore(context);
            } catch (Exception ex) {
                System.Diagnostics.Trace.WriteLine(ex.ToString());
            }
        }

        void ExecuteCore(GeneratorExecutionContext context) {
            context.AddSource("HeadersShortcutAttribute.cs", SourceText.From(AttributeClassSource, Encoding.UTF8));

            if (context.SyntaxReceiver is not HeaderShortcutSyntaxReceiver receiver)
                return;
            if (context.Compilation is not Compilation compilation)
                return;
            var options = (compilation as CSharpCompilation)!.SyntaxTrees[0].Options as CSharpParseOptions;
            compilation = compilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText(SourceText.From(AttributeClassSource, Encoding.UTF8), options));
            var headerShortcutAttributeSymbol = compilation.GetTypeByMetadataName("ShioriSharp.SourceGenerator.HeaderShortcutAttribute");
            var headersShortcutAttributeSymbol = compilation.GetTypeByMetadataName("ShioriSharp.SourceGenerator.HeadersShortcutAttribute");
            var referenceHeadersShortcutAttributeSymbol = compilation.GetTypeByMetadataName("ShioriSharp.SourceGenerator.ReferenceHeadersShortcutAttribute");
            foreach (var classDeclarationSyntax in receiver.ClassDeclarationSyntaxes) {
                var model = compilation.GetSemanticModel(classDeclarationSyntax.SyntaxTree);
                var classSymbol = ModelExtensions.GetDeclaredSymbol(model, classDeclarationSyntax)!;
                var attrs = classSymbol.GetAttributes();
                var max = attrs
                    .Where(attr => attr.AttributeClass!.Equals(referenceHeadersShortcutAttributeSymbol, SymbolEqualityComparer.Default))
                    .Select(attr => (int?)attr.ConstructorArguments[0].Value!)
                    .FirstOrDefault();
                var nameAndComments = attrs
                    .Where(attr => attr.AttributeClass!.Equals(headerShortcutAttributeSymbol, SymbolEqualityComparer.Default))
                    .Select(attr => ((string)attr.ConstructorArguments[0].Value!, (string?)attr.ConstructorArguments[1].Value));
                var names = attrs
                    .Where(attr => attr.AttributeClass!.Equals(headersShortcutAttributeSymbol, SymbolEqualityComparer.Default))
                    .SelectMany(attr => attr.ConstructorArguments[0].Values)
                    .Select(value => (string)value.Value!);
                var shortcuts = new StringBuilder();
                if (max is int maxIndex) {
                    for (var i = 0; i <= maxIndex; ++i)
                        shortcuts.Append($"\t\t/** <summary>Reference{i} header (SHIORI/2.2-2.6,3.x)</summary> */ public string? Reference{i} {{ get => Get(\"Reference{i}\"); set => Set(\"Reference{i}\", value); }}").AppendLine();
                }
                foreach (var (name, comment) in nameAndComments)
                    shortcuts.Append($"\t\t/** <summary>{comment}</summary> */ public string? {name} {{ get => Get(\"{name}\"); set => Set(\"{name}\", value); }}").AppendLine();
                foreach (var name in names)
                    shortcuts.Append($"\t\tpublic string? {name} {{ get => Get(\"{name}\"); set => Set(\"{name}\", value); }}").AppendLine();
                if (max is not null || names.Any()) {
                    context.AddSource(
                        $"{classSymbol.ToDisplayString()}_shortcuts.cs",
                        SourceText.From($@"
namespace {classSymbol.ContainingNamespace.ToDisplayString()} {{
    public partial class {classSymbol.Name} {{
{shortcuts}
    }}
}}
", Encoding.UTF8)
                    );
                }
            }
        }
    }

    internal class HeaderShortcutSyntaxReceiver : ISyntaxReceiver {
        internal List<ClassDeclarationSyntax> ClassDeclarationSyntaxes { get; } = new();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode) {
            if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax && classDeclarationSyntax.AttributeLists.Count > 0)
                ClassDeclarationSyntaxes.Add(classDeclarationSyntax);
        }
    }
}
