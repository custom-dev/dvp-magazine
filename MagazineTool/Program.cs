using System;
using System.IO;
using System.Text;

namespace Developpez.MagazineTool
{
    class Program
    {
        static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            if (args.Length == 1)
            {
                string inputFile = args[0];
                string outputDirectory = Path.GetDirectoryName(inputFile);

                Generator generator = Generator.Create(inputFile, outputDirectory);

                //if (generator.GenerateEPub())
                //{
                //    Console.Write("Génération EPUB réussie");
                //}
                //else
                //{
                //    Console.Write("Echec de la génération EPUB");
                //}

                if (generator.GenerateLatex())
                {
                    Console.WriteLine("Génération réussie");
                }
                else
                {
                    Console.WriteLine("Echec de la génération");
                }
            }
            else
            {
                ShowHelp();
            }            
        }

        private static void ShowHelp()
        {
            string help = TemplateManager.GetTemplate("help.txt");
            Console.WriteLine(help);
        }
    }
}
