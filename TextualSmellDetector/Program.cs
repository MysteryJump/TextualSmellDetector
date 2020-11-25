using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace TextualSmellDetector
{
    class Program
    {
        private static double _threshold = 0.65;
        private static bool _isCsv = false;
        private static bool _isFsharp = false;
        static int Main(string[] args)
        {
            var targetFolder = @"D:\source\repos\eShopOnContainers\src\Services\Ordering\Ordering.API\Application\Validations";//args[0];
            targetFolder = @"D:\source\repos\Utf8Json";
            targetFolder = null;
            //targetFolder = @"D:\source\repos\runtime";
            //targetFolder = @"D:\Documents\Sourcecode\zerochsharp";

            foreach (var s in args)
            {
                if (s.StartsWith("-c") || s.StartsWith("--csv"))
                {
                    _isCsv = true;
                }
                else if (s.StartsWith("-t") || s.StartsWith("--threshold"))
                {
                    var str = s.Replace("-t:", "").Replace("--threshold:", "");
                    if (double.TryParse(str, out var ans))
                    {
                        if (ans <= 1 && ans >= 0)
                        {
                            _threshold = ans;
                        }
                        else
                        {
                            Console.WriteLine("given number does not meet requirement");
                            return -1;
                        }
                    }
                    else
                    {
                        Console.WriteLine("cannot parse given numeric");
                        return -1;
                    }
                }
                else if (s.StartsWith("-f") || s.StartsWith("--fsharp"))
                {
                    _isFsharp = true;
                }
                else if (s.StartsWith("--help") || s.StartsWith("-h"))
                {
                    Console.WriteLine("Usages: tesmdet [options | target folder]" + Environment.NewLine +
                                      "options: " + Environment.NewLine +
                                      "\t-c, --csv: outputs csv text" + Environment.NewLine +
                                      "\t-t:[0,1], --threshold:[0,1]: use alternative threshold (default: 0.65)" + Environment.NewLine +
                                      //  "\t-f, --fsharp: analysis fsharp instead of csharp (experimental)" + Environment.NewLine + 
                                      "\t-h, --help: show help page");
                }
                else if (s.StartsWith("-"))
                {
                    Console.WriteLine("unknown option, use --help to show help");
                    return -1;
                }
                else
                {
                    targetFolder = s;
                }
            }

            if (string.IsNullOrWhiteSpace(targetFolder))
            {
                Console.WriteLine("cannot detect target directory");
                return -1;
            }

            try
            {
                var target = Directory.EnumerateFiles(targetFolder,
                     _isFsharp ? "*.fs" : "*.cs",
                    enumerationOptions: new EnumerationOptions()
                    {
                        RecurseSubdirectories = true,
                        ReturnSpecialDirectories = false
                    });
                Solve(target);
                return 0;
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("Directory not found");
                return -1;
            }
            catch (Exception e)
            {
                Console.WriteLine("Internal error");
                Console.WriteLine(e);
                return -100;
            }

        }

        static void Solve(IEnumerable<string> list)
        {
            if (_isCsv)
            {
                Console.WriteLine("namespace,class,probability,smelled,loc,token_count,unique_token,method_count");
            }
            var count = 0;
            var smelled = 0;
            Parallel.ForEach(list, path =>
            {

                //});
                //foreach (var path in list)
                //{
                var tree = CSharpSyntaxTree.ParseText(
                    File.ReadAllText(
                        path /*@"LsiMatrix.cs"*/
                        //@"D:\source\repos\aspnetcore\src\Hosting\Hosting\test\Fakes\StartupCtorThrows.cs"
                        ));
                if (tree.GetDiagnostics().Any(x => x.WarningLevel == 0))
                {
                    return;
                    //continue;
                }

                Console.Error.WriteLine($"DEBUG: {path}");

                var root = tree.GetRoot();
                var children = root.ChildNodes();
                var namespaces = children.Where(x => x is NamespaceDeclarationSyntax);
                foreach (var node in namespaces)
                {
                    var namespaceNode = node.ChildNodes()
                        .First(x => x is IdentifierNameSyntax ||
                                    x is QualifiedNameSyntax); // IdentifierNameSyntax or Qualified

                    var namespaceName = namespaceNode switch
                    {
                        IdentifierNameSyntax ins => ins.ChildTokens()
                            .First(x => x.Kind() == SyntaxKind.IdentifierToken)
                            .Text,
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

                        var className = classNode.ChildTokens().First(x => x.Kind() == SyntaxKind.IdentifierToken)
                            .Text;
                        var methods = classNode.ChildNodes()
                            .Where(x => x is MethodDeclarationSyntax);
                        var constructors = classNode.ChildNodes()
                            .Where(x => x is ConstructorDeclarationSyntax);
                        var fields = classNode.ChildNodes()
                            .Where(x => x is FieldDeclarationSyntax);
                        var properties = classNode.ChildNodes()
                            .Where(x => x is PropertyDeclarationSyntax);
                        foreach (var method in methods)
                        {
                            var lss = GetIdentifiers(method);
                            if (lss.Count < 2 || lss.Count == 2 && lss.Contains("Main") && lss.Contains("args"))
                            {
                                continue;
                            }

                            var methodName = method.ChildTokens().First(x => x.Kind() == SyntaxKind.IdentifierToken)
                                .Text;
                            ls.Add(new MethodComponent(methodName, lss));
                        }

                        foreach (var constructor in constructors)
                        {
                            var lss = GetIdentifiers(constructor);
                            if (lss.Count < 2)
                            {
                                continue;
                            }

                            var methodName = $"{className}.Constructor";
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
                                    if (lss.Count > 5)
                                    {
                                        ls.Add(new MethodComponent(getterName, lss));
                                    }

                                }

                                if (setter is { })
                                {
                                    var lss = GetIdentifiers(setter);
                                    if (lss.Count > 5)
                                    {
                                        ls.Add(new MethodComponent(setterName, lss));
                                    }

                                }
                            }
                            else if (props is ArrowExpressionClauseSyntax aecs)
                            {
                                var lss = GetIdentifiers(aecs);
                                if (lss.Count > 5)
                                {
                                    ls.Add(new MethodComponent(getterName, lss));
                                }
                            }
                        }

                        var clscmp = new ClassComponent(className, ls, namespaceName,
                            classNode.ToFullString().Count(x => x == '\n'));

                        components.Add(clscmp);
                    }

                    foreach (var enumNodes in enums)
                    {
                        var enumName = enumNodes.ChildTokens().First(x => x.Kind() == SyntaxKind.IdentifierToken)
                            .Text;
                        var idents = GetIdentifiers(enumNodes);
                        var enumcmp = new EnumComponent(enumName, idents);
                        components.Add(enumcmp);
                    }

                    // Blob

                    var classComponents = components.OfType<ClassComponent>();


                    foreach (var component in classComponents)
                    {
                        var ans = CalculateBlobSmell(component, out var test);
                        Interlocked.Increment(ref count);
                        if (test < -0.9)
                        {
                            continue;
                        }

                        if (_isCsv)
                        {
                            Console.WriteLine(
                                $"{component.Namespace},{component.ClassName},{test},{(ans ? 1 : 0)}," +
                                $"{component.Loc},{component.TokenCount},{component.UniqueTokenCount},{component.Children.Count(x => x is MethodComponent)}");
                        }
                        else if (ans)
                        {
                            Console.WriteLine(
                                $"Class: {component.ClassName} ({component.Namespace}) ({test:F5}, LOC: {component.Loc}, TokenCount: {component.TokenCount})," +
                                $" ({Interlocked.Increment(ref smelled)}/{count})");

                        }
                    }

                }

                //}

                //Console.WriteLine(Directory.GetCurrentDirectory());
            });

        }

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

        private static bool CalculateBlobSmell(ClassComponent cmp, out double num)
        {
            var className = cmp.ClassName;
            var methodComponents = cmp.Children.OfType<MethodComponent>();
            cmp.GetMethodTermDictionary();
            var mat = new MethodMethodLsiMatrix(cmp);
            num = mat.Calculate();
            return num > _threshold;

        }
    }


}
