using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace TextualSmellDetector
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //Console.WriteLine(Directory.GetCurrentDirectory());
            var tree = CSharpSyntaxTree.ParseText(
                File.ReadAllText(@"Program.cs"));
            if (tree.GetDiagnostics().Any())
            {
                throw new InvalidOperationException();
            }
            var root = tree.GetRoot();
            var children = root.ChildNodes();
            var namespaces = children.Where(x => x is NamespaceDeclarationSyntax);
            foreach (var node in namespaces)
            {
                var namespaceNode = node.ChildNodes()
                    .First(x => x is IdentifierNameSyntax || x is QualifiedNameSyntax);// IdentifierNameSyntax or Qualified
     
                var namespaceName = namespaceNode switch
                {
                    IdentifierNameSyntax ins => ins.ChildTokens().First(x => x.Kind() == SyntaxKind.IdentifierToken).Text,
                    QualifiedNameSyntax qns => qns.ToString(),
                    _ => throw new InvalidOperationException()
                };
                var components = new List<CodeComponent>();
                // in future, needs record implementation
                // TODO: inner class
                var classes = node.ChildNodes()
                    .Where(x => x is ClassDeclarationSyntax || x is StructDeclarationSyntax);

                var enums = node.ChildNodes().Where(x => x is EnumDeclarationSyntax);
                foreach (var classNode in classes)
                {
                    var ls = new List<CodeComponent>();

                    var className = classNode.ChildTokens().First(x => x.Kind() == SyntaxKind.IdentifierToken).Text;
                    var methods = classNode.ChildNodes()
                        .Where(x => x is MethodDeclarationSyntax);
                    var fields = classNode.ChildNodes()
                        .Where(x => x is FieldDeclarationSyntax);
                    var properties = classNode.ChildNodes()
                        .Where(x => x is PropertyDeclarationSyntax);
                    foreach (var method in methods)
                    {
                        var lss = GetIdentifiers(method);
                        var methodName = method.ChildTokens().First(x => x.Kind() == SyntaxKind.IdentifierToken).Text;
                        ls.Add(new MethodComponent(methodName, lss));
                    }

                    foreach (var field in fields)
                    {
                        var f = GetIdentifiers(field);
                        var fieldName = f.First();
                        ls.Add(new FieldComponent(fieldName));
                    }

                    foreach (var property in properties)
                    {
                        var props = property.ChildNodes()
                            .First(x => x is AccessorListSyntax || x is ArrowExpressionClauseSyntax);
                        var propName = property.ChildTokens()
                            .First(x => x.Kind() == SyntaxKind.IdentifierToken).Text;
                        var getterName = $"Get{propName}";
                        var setterName = $"Set{propName}";

                        if (props is AccessorListSyntax als)
                        {
                            var accessors = props.ChildNodes().OfType<AccessorDeclarationSyntax>().ToList();
                            var getter = accessors
                                .FirstOrDefault(x => x.Kind() == SyntaxKind.GetAccessorDeclaration);
                            var setter = accessors
                                .FirstOrDefault(x => x.Kind() == SyntaxKind.SetAccessorDeclaration);
                            if (getter is { })
                            {
                                var lss = GetIdentifiers(getter);
                                ls.Add(new MethodComponent(getterName, lss));
                            }

                            if (setter is { })
                            {
                                var lss = GetIdentifiers(setter);
                                ls.Add(new MethodComponent(setterName, lss));
                            }
                        }
                        else if (props is ArrowExpressionClauseSyntax aecs)
                        {
                            var lss = GetIdentifiers(aecs);
                            ls.Add(new MethodComponent(getterName, lss));
                        }
                    }
                    var clscmp = new ClassComponent(className, ls);

                    components.Add(clscmp);
                }

                foreach (var enumNodes in enums)
                {
                    var enumName = enumNodes.ChildTokens().First(x => x.Kind() == SyntaxKind.IdentifierToken).Text;
                    var idents = GetIdentifiers(enumNodes);
                    var enumcmp = new EnumComponent(enumName, idents);
                    components.Add(enumcmp);
                }

                // Blob

                var classComponents = components.OfType<ClassComponent>();
                foreach (var component in classComponents)
                {
                    CalculateBlobSmell(component);
                }

            }

        }

        private string test;
        private string Test2
        {
            get { return test; }
            set { test = value; }
        }
        private string test3 { get; set; }
        private string test4 => "d";

        static IReadOnlyList<string> GetIdentifiers(SyntaxNode node)
        {
            var list = new List<string>();
            var nodes = node.ChildNodes();
            list.AddRange(nodes.Select(GetIdentifiers)
                .Aggregate(new List<string>() as IEnumerable<string>,
                    (current,
                        next) => current.Concat(next)));
            var tokens = node.ChildTokens();
            list.AddRange(tokens.Where(x => x.Kind() == SyntaxKind.IdentifierToken && x.Text != "var")
                .Select(x => x.Text));

            return list;
        }

        private static double CalculateBlobSmell(ClassComponent cmp)
        {
            var className = cmp.ClassName;
            var methodComponents = cmp.Children.OfType<MethodComponent>();
            cmp.GetMethodTermDictionary();
            var mat = new MethodMethodLsiMatrix(cmp);
            mat.Calculate();
            return 3;
        }
    }

    class Unko
    {

    }

    enum MyUnkoEnum
    {
        Test = 1,
        TEst = 44
    }
}

namespace MyNamespace.MyName2.MyName4  
{

}
