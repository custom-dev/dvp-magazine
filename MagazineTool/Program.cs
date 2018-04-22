using System;
using System.IO;

namespace Developpez.MagazineTool
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                string inputFile = args[0];
                string outputDirectory = Path.GetDirectoryName(inputFile);

                Generator generator = Generator.Create(inputFile, outputDirectory);

                if (generator.Generate())
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
