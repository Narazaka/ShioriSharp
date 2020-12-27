using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EnumWithValues {
    [EnumWithValues("MyE")]
    public enum MyEnum {
        [EnumValue(1, "aa")]
        One,
        Two,
    }

    [Generator]
    public class SourceGenerator : ISourceGenerator {
        const string AttributeClassesSource = @"
using System;

namespace EnumWithValues {
    [AttributeUsage(AttributeTargets.Enum)]
    public class EnumWithValuesAttribute : Attribute {
        public string Name { get; }
        public bool DefaultNumberConvert { get; }
        public EnumWithValuesAttribute(string name, bool defaultNumberConvert = false) {
            Name = name;
            DefaultNumberConvert = defaultNumberConvert;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class EnumValueAttribute : Attribute {
        public object?[] Value { get; }
        public EnumValueAttribute(params object?[] value) => Value = value;
    }
}
";

        public void Initialize(GeneratorInitializationContext context) {
#if DEBUG
            if (!System.Diagnostics.Debugger.IsAttached) {
                // System.Diagnostics.Debugger.Launch();
            }
#endif
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context) {
            try {
                ExecuteCore(context);
            } catch (Exception ex) {
                System.Diagnostics.Trace.WriteLine(ex.ToString());
            }
        }
        void ExecuteCore(GeneratorExecutionContext context) {
            context.AddSource("EnumWithValues.cs", SourceText.From(AttributeClassesSource, Encoding.UTF8));
            if (context.SyntaxReceiver is not SyntaxReceiver receiver)
                return;
            if (context.Compilation is not Compilation compilation)
                return;
            var options = (compilation as CSharpCompilation)!.SyntaxTrees[0].Options as CSharpParseOptions;
            compilation = compilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText(SourceText.From(AttributeClassesSource, Encoding.UTF8), options));
            var enumWithValuesAttributeSymbol = compilation.GetTypeByMetadataName("EnumWithValues.EnumWithValuesAttribute");
            var enumValueAttributeSymbol = compilation.GetTypeByMetadataName("EnumWithValues.EnumValueAttribute");
            var enums = new List<EnumDeclaration>();
            foreach (var syntax in receiver.EnumDeclarationSyntaxes) {
                var model = compilation.GetSemanticModel(syntax.SyntaxTree);
                var symbol = ModelExtensions.GetDeclaredSymbol(model, syntax)!;
                var attrs = symbol.GetAttributes();
                var (name, defaultNumberConvert) = attrs
                    .Where(attr => attr.AttributeClass!.Equals(enumWithValuesAttributeSymbol, SymbolEqualityComparer.Default))
                    .Select(attr => ((string)attr.ConstructorArguments[0].Value!, (bool?)attr.ConstructorArguments[1].Value))
                    .FirstOrDefault();
                if (name is not null) {
                    string? namespaceName = null;
                    if (symbol.ContainingNamespace is INamespaceSymbol nss)
                        namespaceName = nss.Name;
                    enums.Add(new() { Name = symbol.Name, Namespace = namespaceName, StructName = name, DefaultNumberConvert = defaultNumberConvert ?? false });
                }
            }
            foreach (var syntax in receiver.FieldDeclarationSyntaxes) {
                if (syntax.Parent is not EnumDeclarationSyntax parent)
                    continue;
                var model = compilation.GetSemanticModel(syntax.SyntaxTree);
                var symbol = ModelExtensions.GetDeclaredSymbol(model, syntax)!;
                var attrs = symbol.GetAttributes();
            }
        }

        class EnumDeclaration {
            public string? Namespace { get; set; }
            public string Name { get; set; } = "";
            public string StructName { get; set; } = "";
            public bool DefaultNumberConvert { get; set; }
        }

        class EnumField {
            public string Name { get; set; } = "";
            public object?[] Values { get; set; } = new object?[0];
        }

        const string I = "    ";

        string Namespaced(string? namespaceName, string code) {
            if (namespaceName is null)
                return code;
            return $@"
namespace {namespaceName} {{
{code}
}}
";
        }

        string StructCode(string structName, string enumName, IList<Type> valueTypes, IEnumerable<EnumField> fields) {
            return $@"
    public struct {structName} : IEquatable<{structName}> {{
{FieldCode(structName, enumName, fields)}

        public {enumName} Enum {{ get; }}

        {structName}({enumName} @enum) => Enum = @enum;

        public bool Equals({structName} other) => Enum == other.Enum;

{ValueOperatorCode(structName, enumName, valueTypes, fields)}
    }}
";
        }

        string FieldCode(string structName, string enumName, IEnumerable<EnumField> fields) {
            var code = new StringBuilder();
            foreach (var field in fields)
                code.Append($"{I}{I}public static readonly {structName} {field.Name} = new({enumName}.{field.Name});").AppendLine();
            return code.ToString();
        }

        string ValueOperatorCode(string structName, string enumName, IList<Type> valueTypes, IEnumerable<EnumField> fields) {
            var code = new StringBuilder();
            for (var i = 0; i < valueTypes.Count; ++i) {
                ToValueOperatorCode(code, structName, enumName, valueTypes, fields, i);
                FromValueOperatorCode(code, structName, enumName, valueTypes, fields, i);
            }
            return code.ToString();
        }

        StringBuilder ToValueOperatorCode(StringBuilder code, string structName, string enumName, IList<Type> valueTypes, IEnumerable<EnumField> fields, int valueIndex) {
            code.Append($"{I}{I}public static implicit operator {valueTypes[valueIndex].FullName}({structName} enumStruct) {{").AppendLine();
            code.Append($"{I}{I}{I}switch (enumStruct.Enum) {{").AppendLine();
            foreach (var field in fields)
                code.Append($"{I}{I}{I}{I}case {enumName}.{field.Name}: return {field.Values[valueIndex]};").AppendLine();
            code.Append($"{I}{I}{I}{I}default: throw new InvalidCastException();").AppendLine();
            code.Append($"{I}{I}{I}}}").AppendLine();
            code.Append($"{I}{I}}}").AppendLine();
            return code;
        }

        StringBuilder FromValueOperatorCode(StringBuilder code, string structName, string enumName, IList<Type> valueTypes, IEnumerable<EnumField> fields, int valueIndex) {
            code.Append($"{I}{I}public static implicit operator {structName}({valueTypes[valueIndex].FullName} value) {{").AppendLine();
            code.Append($"{I}{I}{I}switch (value) {{").AppendLine();
            foreach (var field in fields)
                code.Append($"{I}{I}{I}{I}case {field.Values[valueIndex]}: return {enumName}.{field.Name};").AppendLine();
            code.Append($"{I}{I}{I}{I}default: throw new InvalidCastException();").AppendLine();
            code.Append($"{I}{I}{I}}}").AppendLine();
            code.Append($"{I}{I}}}").AppendLine();
            return code;
        }
    }

    internal class SyntaxReceiver : ISyntaxReceiver {
        internal List<EnumDeclarationSyntax> EnumDeclarationSyntaxes { get; } = new();
        internal List<FieldDeclarationSyntax> FieldDeclarationSyntaxes { get; } = new();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode) {
            switch (syntaxNode) {
                case EnumDeclarationSyntax syntax when syntax.AttributeLists.Count > 0:
                    EnumDeclarationSyntaxes.Add(syntax);
                    break;
                case FieldDeclarationSyntax syntax when syntax.AttributeLists.Count > 0:
                    FieldDeclarationSyntaxes.Add(syntax);
                    break;
            }
        }
    }

    [AttributeUsage(AttributeTargets.Enum)]
    public class EnumWithValuesAttribute : Attribute {
        public string Name { get; }
        public bool DefaultNumberConvert { get; }
        public EnumWithValuesAttribute(string name, bool defaultNumberConvert = true) {
            Name = name;
            DefaultNumberConvert = defaultNumberConvert;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class EnumValueAttribute : Attribute {
        public object?[] Value { get; }
        public EnumValueAttribute(params object?[] value) => Value = value;
    }

    public struct MyEnumStruct : IEquatable<MyEnumStruct> {
        public static readonly MyEnumStruct One = new(MyEnum.One);
        public static readonly MyEnumStruct Two = new(MyEnum.Two);

        public MyEnum Enum { get; }

        MyEnumStruct(MyEnum @enum) => Enum = @enum;

        public bool Equals(MyEnumStruct other) => Enum == other.Enum;

        public static implicit operator int(MyEnumStruct enumStruct) {
            switch (enumStruct.Enum) {
                case MyEnum.One:
                    return 1;
                case MyEnum.Two:
                    return 2;
                default:
                    throw new InvalidCastException();
            }
        }

        public static implicit operator MyEnumStruct(int value) {
            switch (value) {
                case 1:
                    return One;
                case 2:
                    return Two;
                default:
                    throw new InvalidCastException();
            }
        }
    }

    public class FFF {
        void F() {
            var mye = MyEnumStruct.One;
            MyEnumStruct foo = 1;
            Console.WriteLine($"{mye == 1}");
        }
    }
}

