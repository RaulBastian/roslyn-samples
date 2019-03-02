using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basic
{
    class Car
    {
    }


    class Program
    {
        private static string sample_class = "" +
            "namespace models{" +
                "public class Product {" +
                "}" +
            "}";

        static void Main(string[] args)
        {
            //1) Parse and get the root syntax node
            var node = CSharpSyntaxTree.ParseText(sample_class).GetRoot();

            //2) Get its class and namespace declaration
            var classDeclaration = node.DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault();
            var namespaceDeclaration = node.DescendantNodes().OfType<NamespaceDeclarationSyntax>().FirstOrDefault();

            //3) By using the class declaration, we create the new view model class
            var view_model_class = GetViewModelClass(classDeclaration);

            //4) Parse and get the view model's syntax root node
            var viewModelNode = CSharpSyntaxTree.ParseText(view_model_class).GetRoot();

            //5) Get the view model's class declaration.
            var viewModelNodeClassDeclaration = viewModelNode.DescendantNodes().OfType<ClassDeclarationSyntax>().FirstOrDefault();

            //6) Add the class declaration to the namespace
            var newNamespace = namespaceDeclaration.AddMembers(viewModelNodeClassDeclaration).NormalizeWhitespace();

            var basePath = $"{Environment.CurrentDirectory}\\..\\..\\";

            if(!Directory.Exists(Path.Combine(basePath,"ViewModels")))
            {
                Directory.CreateDirectory(Path.Combine(basePath, "ViewModels"));
            }

            File.WriteAllText(Path.Combine(basePath, "ViewModels", "ViewModels.cs"), newNamespace.ToString());
        }



        /// <summary>
        /// Returns a view model class inheriting from PRISM's BindableBase
        /// </summary>
        /// <param name="classDeclaration"></param>
        /// <returns></returns>
        static string GetViewModelClass(ClassDeclarationSyntax classDeclaration)
        {
            var viewModelClass = $"{classDeclaration.Identifier.Text}ViewModel";

            return $"public class {viewModelClass} : BindableBase{{ }}";
        }

    }
}
