using Microsoft.AspNetCore.Razor.Language;
using Microsoft.AspNetCore.Razor.Language.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Developpez.MagazineTool
{
    public static class TemplateManager
    {
        public abstract class Template<T> where T : new()
        {
            private TextWriter _writer = new StringWriter();

            public T Model
            {
                get; set;
            }

            public void WriteLiteral(string literal)
            {
                _writer.Write(literal);
            }

            public void Write(object obj)
            {
                _writer.Write(obj);
            }

            public void WriteLine()
            {
                _writer.WriteLine();
            }

            public void WriteLine(string str)
            {
                _writer.WriteLine(str);
            }

            public void WriteLine(string str, params object[] p)
            {
                _writer.WriteLine(String.Format(str, p));
            }

            public string EscapeChar(string str)
            {
                string s = str
                    .Replace("&", @"\&")
                    .Replace("%", @"\%");
                return s;
            }

            public abstract Task ExecuteAsync();

            public override string ToString()
            {
                return _writer.ToString();
            }
        }

        private static Dictionary<string, Type> _cache = new Dictionary<string, Type>();
        public static string GetTemplate(string path)
        {
            string manifestName = String.Format("Developpez.MagazineTool.Templates.{0}", path.Replace('/', '.'));
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(manifestName))
            {
                if (stream != null)
                {
                    using (TextReader reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }

            return String.Empty;
        }

        public static byte[] GetBinaryFile(string path)
        {
            string manifestName = String.Format("Developpez.MagazineTool.Templates.{0}", path.Replace('/', '.'));
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(manifestName))
            {
                if (stream != null)
                {
                    byte[] data = new byte[stream.Length];
                    stream.Read(data, 0, data.Length);

                    return data;
                }
            }

            return null;
        }

        public static string RenderTemplate<T>(string path, T model) where T : new()
        {
            Type type = null;

            if (!_cache.ContainsKey(path))
            {
                string templateContent = GetTemplate(path);
                //RazorEngine.Engine.Razor.Compile(templateContent, path, typeof(T));
                type = CompileTemplate<T>(path, templateContent);
                _cache.Add(path, type);
            }
            else
            {
                type = _cache[path];
            }

            Template<T> template = (Template<T>)Activator.CreateInstance(type);
            template.Model = model;
            Task task = template.ExecuteAsync();
            task.Wait();
            string renderedString = template.ToString();
            return renderedString;
        }

        private static Type CompileTemplate<T>(string templatePath, string content) where T : new()
        {
            string cs = GetTemplateSourceCode<T>(templatePath, content);
            string dllName = String.Format("Compiled.{0}", templatePath);
            string path = Path.Combine(Path.GetFullPath("."), dllName + ".dll");

            Assembly asm = CompileAndLoadDll(path, cs);
            return asm.GetType("MyNamespace.Template");            
        }

        private static string GetTemplateSourceCode<T>(string templatePath, string content)
        {
            // customize the default engine a little bit
            var engine = RazorEngine.Create(engineBuilder =>
            {
                InheritsDirective.Register(engineBuilder);
                engineBuilder.SetNamespace("MyNamespace");
                engineBuilder.Build();
            });

            // points to the local path
            var project = RazorProject.Create(".");
            var te = new RazorTemplateEngine(engine, project);

            // get a razor-templated file. My "hello.txt" template file is defined like this:
            //
            // @inherits RazorTemplate.MyTemplate
            // Hello @Model.Name, welcome to Razor World!
            //
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(String.Format("@inherits Developpez.MagazineTool.TemplateManager.Template<{0}>", typeof(T).FullName));
            builder.Append(content);
            RazorSourceDocument sourceDocument = RazorSourceDocument.Create(builder.ToString(), templatePath);
            RazorCodeDocument codeDocument = RazorCodeDocument.Create(sourceDocument);

            // parse and generate C# code, outputs it on the console

            //var cs = te.GenerateCode(item);
            var cs = te.GenerateCode(codeDocument);
            return cs.GeneratedCode;
        }

        private static Assembly CompileAndLoadDll(string path, string sourceCode)
        {
            // now, use roslyn, parse the C# code
            var tree = CSharpSyntaxTree.ParseText(sourceCode);
            string dllName = Path.GetFileName(path);
            var compilation = CSharpCompilation.Create(dllName, new[] { tree },
                new[]
                {
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                    MetadataReference.CreateFromFile(Assembly.GetExecutingAssembly().Location),
                    MetadataReference.CreateFromFile(Path.Combine(Path.GetDirectoryName(typeof(object).Assembly.Location), "System.Collections.dll")),
                    MetadataReference.CreateFromFile(Path.Combine(Path.GetDirectoryName(typeof(object).Assembly.Location), "System.Core.dll")),
                    MetadataReference.CreateFromFile(Path.Combine(Path.GetDirectoryName(typeof(object).Assembly.Location), "System.Runtime.dll")),
                    MetadataReference.CreateFromFile(Path.Combine(Path.GetDirectoryName(typeof(object).Assembly.Location), "System.Linq.dll")),
                    MetadataReference.CreateFromFile(Path.Combine(Path.GetDirect‌​oryName(typeof(object).Assembly.Location‌​), "netstandard.dll"))
                },
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                );


            // compile the dll

            var result = compilation.Emit(path);
            if (!result.Success)
            {
                Console.Error.WriteLine(string.Join(Environment.NewLine, result.Diagnostics));
                return null;
            }

            Assembly asm = Assembly.LoadFile(path);
            return asm;
        }
    }        
}

