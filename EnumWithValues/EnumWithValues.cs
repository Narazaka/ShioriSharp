using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
                    .Select(attr => ((string)attr.ConstructorArguments[0].Value!, (bool)attr.ConstructorArguments[1].Value!))
                    .FirstOrDefault();
                if (name is null)
                    continue;
                var enumDeclaration = new EnumDeclaration() { Accessibility = symbol.DeclaredAccessibility, EnumName = symbol.Name, EnumFullname = symbol.ToString(), StructName = name, DefaultNumberConvert = defaultNumberConvert };
                foreach (var memberSyntax in syntax.Members) {
                    var memberModel = compilation.GetSemanticModel(memberSyntax.SyntaxTree);
                    var memberSymbol = ModelExtensions.GetDeclaredSymbol(memberModel, memberSyntax)!;
                    var memberAttrs = memberSymbol.GetAttributes();
                    var valueConstants = memberAttrs
                        .Where(attr => attr.AttributeClass!.Equals(enumValueAttributeSymbol, SymbolEqualityComparer.Default))
                        .Select(attr => attr.ConstructorArguments[0].Values!)
                        .FirstOrDefault();
                    enumDeclaration.Members.Add(new() { Name = memberSymbol.Name, Values = valueConstants });
                }
                enums.Add(enumDeclaration);
            }
            foreach (var e in enums) {
                e.DetectTypes();
                var code = StructCode(e.Accessibility, e.StructName, e.EnumName, e.Types ?? new(), e.Members);
                if (e.Namespace is string ns)
                    code = Namespaced(ns, code);
                context.AddSource($"{e.StructFullname}.cs", SourceText.From(code, Encoding.UTF8));
            }
        }

        class EnumDeclaration {
            public Accessibility Accessibility { get; set; }
            public string EnumFullname { get; set; } = "";
            public string EnumName { get; set; } = "";
            public string? Namespace { get => EnumFullname == EnumName ? null : EnumFullname.Substring(0, EnumFullname.Length - EnumName.Length - 1); }
            public string StructName { get; set; } = "";
            public string StructFullname { get => $"{Namespace}_{StructName}"; }
            public bool DefaultNumberConvert { get; set; }
            public List<EnumMember> Members { get; set; } = new();
            public List<ITypeSymbol>? Types { get; set; }
            public void DetectTypes() {
                if (Members.Count == 0) {
                    Types = new();
                    return;
                }
                var types = Members[0].Values.Select(value => value.Type!).ToList();
                foreach (var member in Members) {
                    if (!member.Values.Select(value => value.Type!).SequenceEqual(types, SymbolEqualityComparer.Default)) {
                        return;
                    }
                }
                Types = types;
            }
        }

        class EnumMember {
            public string Name { get; set; } = "";
            public ImmutableArray<TypedConstant> Values { get; set; } = new();
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

        string AccessibilityToString(Accessibility accessibility) =>
            string.Join(" ", accessibility.ToString().Split(new string[] { "And" }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.ToLower()));

        string StructCode(Accessibility accessibility, string structName, string enumName, IList<ITypeSymbol> valueTypes, IEnumerable<EnumMember> members) {
            return $@"
    using System;

    {AccessibilityToString(accessibility)} struct {structName} : IEquatable<{structName}> {{
{FieldCode(structName, enumName, members)}

        public {enumName} Enum {{ get; }}

        {structName}({enumName} @enum) => Enum = @enum;

        public bool Equals({structName} other) => Enum == other.Enum;

{ValueOperatorCode(structName, enumName, valueTypes, members)}
    }}
";
        }

        string FieldCode(string structName, string enumName, IEnumerable<EnumMember> members) {
            var code = new StringBuilder();
            foreach (var member in members)
                code.Append($"{I}{I}public static readonly {structName} {member.Name} = new({enumName}.{member.Name});").AppendLine();
            return code.ToString();
        }

        string ValueOperatorCode(string structName, string enumName, IList<ITypeSymbol> valueTypes, IEnumerable<EnumMember> members) {
            var code = new StringBuilder();
            for (var i = 0; i < valueTypes.Count; ++i) {
                ToValueOperatorCode(code, structName, enumName, valueTypes, members, i);
                FromValueOperatorCode(code, structName, enumName, valueTypes, members, i);
            }
            return code.ToString();
        }

        StringBuilder ToValueOperatorCode(StringBuilder code, string structName, string enumName, IList<ITypeSymbol> valueTypes, IEnumerable<EnumMember> members, int valueIndex) {
            code.Append($"{I}{I}public static implicit operator {valueTypes[valueIndex]}({structName} enumStruct) {{").AppendLine();
            code.Append($"{I}{I}{I}switch (enumStruct.Enum) {{").AppendLine();
            foreach (var member in members)
                code.Append($"{I}{I}{I}{I}case {enumName}.{member.Name}: return {member.Values[valueIndex].ToCSharpString()};").AppendLine();
            code.Append($"{I}{I}{I}{I}default: throw new InvalidCastException();").AppendLine();
            code.Append($"{I}{I}{I}}}").AppendLine();
            code.Append($"{I}{I}}}").AppendLine();
            return code;
        }

        StringBuilder FromValueOperatorCode(StringBuilder code, string structName, string enumName, IList<ITypeSymbol> valueTypes, IEnumerable<EnumMember> members, int valueIndex) {
            code.Append($"{I}{I}public static implicit operator {structName}({valueTypes[valueIndex]} value) {{").AppendLine();
            code.Append($"{I}{I}{I}switch (value) {{").AppendLine();
            foreach (var member in members)
                code.Append($"{I}{I}{I}{I}case {member.Values[valueIndex].ToCSharpString()}: return {member.Name};").AppendLine();
            code.Append($"{I}{I}{I}{I}default: throw new InvalidCastException();").AppendLine();
            code.Append($"{I}{I}{I}}}").AppendLine();
            code.Append($"{I}{I}}}").AppendLine();
            return code;
        }
    }

    internal class SyntaxReceiver : ISyntaxReceiver {
        internal List<EnumDeclarationSyntax> EnumDeclarationSyntaxes { get; } = new();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode) {
            switch (syntaxNode) {
                case EnumDeclarationSyntax syntax when syntax.AttributeLists.Count > 0:
                    EnumDeclarationSyntaxes.Add(syntax);
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

