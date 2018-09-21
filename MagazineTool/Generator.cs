using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Developpez.MagazineTool
{
    public class Generator
    {
        private readonly string COUVERTURE_FILENAME = "couverture.tex";
        private readonly string EDITORIAL_FILENAME = "editorial.tex";
        private readonly string ARTICLE_FILENAME = "article.tex";
        private readonly string MAGAZINE_FILENAME = "magazine.tex";

        private string _inputFile;
        private string _outputDirectory;
        private MagazineType _magazine;
        private DirectoryInfo _rootDirectory;
        private DirectoryInfo _sourceDirectory;
        private DirectoryInfo _inputDirectory;
        private DirectoryInfo _inputLogoDirectory;
        private DirectoryInfo _magazineDirectory;
        private DirectoryInfo _articlesDirectory;

        private Generator()
        {

        }

        public static Generator Create(string inputFile, string outputDirectory)
        {
            Generator generator = new Generator();
            bool ok = true;

            generator._inputFile = inputFile;
            generator._outputDirectory = outputDirectory;

            ok = generator.ParseInputFile();
            ok = ok && generator.InitializeOutputDirectoryHierarchy();
            //ok = ok && CreateXmlFromUrl();
            ok = ok && generator.LoadArticlesFromXml();
            
            return ok ? generator : null;
        }        

        public string InputFile { get { return _inputFile; } }

        public string GetArticleXmlFile(ArticleType article)
        {
            DirectoryInfo dir = GetArticleSourceDirectory(article);
            string xmlFilePath = Path.Combine(dir.FullName, article.FileName);
            return xmlFilePath;
        }

        public DirectoryInfo GetArticleSourceDirectory(ArticleType article)
        {
            return _sourceDirectory.CreateSubdirectory(article.ID);
        }

        public bool GenerateLatex()
        {
            bool ok = true;

            //ok = ParseInputFile();
            //ok = ok && InitializeOutputDirectoryHierarchy();
            //ok = ok && LoadArticlesFromXml();            
            ok = ok && CopyInputFiles();
            ok = ok && GenerateMagazine();
            ok = ok && GenerateCouverture();
            ok = ok && GenerateEditorial();
            ok = ok && GenerateArticles();
            return ok;
        }

        public bool GenerateEPub()
        {
            GeneratorEPub generator = GeneratorEPub.Create(this, _magazine);
            return generator.Generate();
        }

        public bool ParseInputFile()
        {
            try
            {
                _magazine = MagazineType.LoadFromFile(_inputFile);

                if (_magazine == null)
                {
                    Console.Error.WriteLine("Erreur lors de la lecture du fichier d'entré");
                    return false;
                }
                else
                {
                    if (_magazine.Couverture != null && _magazine.Couverture.Articles != null)
                    {
                        for(int i = 0; i < _magazine.Couverture.Articles.Count; ++i)
                        {
                            string articleRef = _magazine.Couverture.Articles[i].Ref;
                            ArticleType article = _magazine.GetArticleByID(articleRef);
                            _magazine.Couverture.Articles[i] = article;
                        }
                    }

                    return true;
                }
            }
            catch(Exception ex)
            {
                Console.Error.WriteLine(ex.FullTrace());
                return false;
            }
        }

        

        public bool LoadArticlesFromXml()
        {
            bool ok = true;
            foreach (MagazineTypeCategorie categorie in _magazine.Categories)
            {
                foreach (ArticleType article in categorie.Articles)
                {
                    string xmlFilePath = GetArticleXmlFile(article);
                    XDocument xDoc = XDocument.Load(xmlFilePath);
                    ok = ok && article.LoadFromDeveloppezXML(categorie, xDoc);
                }
            }
            return ok;
        }

        public bool CreateXmlFromUrl()
        {
            bool ok = true;
            foreach (MagazineTypeCategorie categorie in _magazine.Categories)
            {
                foreach (ArticleType article in categorie.Articles)
                {
                    string xmlFilePath = this.GetArticleXmlFile(article);

                    if (!File.Exists(xmlFilePath))
                    {
                        if (String.IsNullOrEmpty(article.URL))
                        {
                            ok = false;
                        }
                        else
                        {
                            if (article.URL.StartsWith("https://www.developpez.net/forums/"))
                            {
                                ok = PostToXml.PostToXmlConverter.CreateXmlFromUrl(this, article);                                
                            }
                            else
                            {
                                ok = false;
                            }
                        }
                    }
                }
            }
            return ok;
        }

        public DirectoryInfo RootDirectory { get { return _rootDirectory; } }

        public bool InitializeOutputDirectoryHierarchy()
        {
            string magazinePath = String.Format("mag{0}", _magazine.Numero);
            _rootDirectory = Directory.CreateDirectory(_outputDirectory);
            //_sourceDirectory = Directory.CreateDirectory("sources");
            _inputDirectory = _rootDirectory.CreateSubdirectory("input");
            _sourceDirectory = _rootDirectory.CreateSubdirectory(@"sources");
            _inputLogoDirectory = _inputDirectory.CreateSubdirectory("logos_images");
            _magazineDirectory = _rootDirectory.CreateSubdirectory(magazinePath);
            _articlesDirectory = _magazineDirectory.CreateSubdirectory("articles");

            if (_articlesDirectory == null)
            {
                Console.Error.WriteLine("Erreur lors de la création de l'arborescence de sortie");
                return false;
            }
            else
            {
                return true;
            }            
        }

        public bool CopyInputFiles()
        {
            string[] files = new string[]
            {
                "environnements.tex",
                "macros.tex",
                "preambule.tex"                
            };

            string[] images = new string[]
            {
                "access.png",
                "ajax.png",
                "Algorithmique.png",
                "alm.png",
                "assembleur.png",
                "attention.jpg",
                "attention.png",
                "c++.png",
                "CC_by-nc-sa.png",
                "cloud-computing.png",
                "clubdesdevp.png",
                "conception.png",
                "danger.png",
                "delphi.png",
                "dotnet.png",
                "idee2.png",
                "imagedefondassembleur.jpg",
                "imagedefondconception.png",
                "imagedefondcpp.jpg",
                "imagedefondclub.jpg",
                "imagedefonddevweb.jpg",
                "imagedefondjava.jpg",
                "imagedefondos.jpg",
                "imagedefondhardware.png",
                "imagedefondpascal.jpg",
                "imagedefondqt.png",
                "imagedefondrose.jpg",
                "information3.png",
                "java.png",
                "leclub.png",
                "logomag.png",
                "php.png",
                "qt.png",
                "systemes-embarques.png",
                "windows.png"
            };

            foreach(string file in files)
            {
                string outputFile = Path.Combine(_inputDirectory.FullName, file);
                string fileContent = TemplateManager.GetTemplate(String.Format("input/{0}", file));
                File.WriteAllText(outputFile, fileContent);
            }

            foreach (string file in images)
            {
                string outputFile = Path.Combine(_inputLogoDirectory.FullName, file);
                byte[] fileContent = TemplateManager.GetBinaryFile(String.Format("input/logos_images/{0}", file));
                File.WriteAllBytes(outputFile, fileContent);
            }
            return true;
        }

        public bool GenerateMagazine()
        {
            string content = TemplateManager.RenderTemplate(MAGAZINE_FILENAME, _magazine);
            string outputFileName = Path.Combine(_rootDirectory.FullName, String.Format("mag{0}.tex", _magazine.Numero));

            File.WriteAllText(outputFileName, content);
            return true;
        }

        public bool GenerateCouverture()
        {
            string content = TemplateManager.RenderTemplate(COUVERTURE_FILENAME, _magazine);
            string outputFileName = Path.Combine(_magazineDirectory.FullName, COUVERTURE_FILENAME);

            File.WriteAllText(outputFileName, content);
            return true;
        }

        public bool GenerateEditorial()
        {
            StringBuilder benevolesBuilder = new StringBuilder();
            HashSet<string> participants = new HashSet<string>();
            List<string> listeOrdonnee;

            foreach (var categorie in _magazine.Categories)
            {
                foreach (var article in categorie.Articles)
                {
                    foreach (var auteur in article.Auteurs)
                    {
                        if (auteur.Role == AuteurRoleEnum.Auteur ||
                            auteur.Role == AuteurRoleEnum.Traducteur ||
                            auteur.Role == AuteurRoleEnum.Correcteur)
                        {
                            participants.Add(auteur.NomComplet);
                        }
                    }
                }
            }

            listeOrdonnee = participants.OrderBy(x => x).ToList();
            benevolesBuilder.AppendLine(@"\begin{itemize}");
            for(int i = 0; i < listeOrdonnee.Count; ++i)
            {
                string auteur = listeOrdonnee[i];
                benevolesBuilder.Append(String.Format(@"\item {0}", auteur));

                if (i == listeOrdonnee.Count - 1)
                {
                    benevolesBuilder.Append(".");
                }
                else
                {
                    benevolesBuilder.Append(" ;");
                }

                benevolesBuilder.AppendLine();
            }
            benevolesBuilder.Append(@"\end{itemize}");

            _magazine.Editorial.Edito = _magazine.Editorial.Edito.Replace("[LISTE_BENEVOLES]", benevolesBuilder.ToString());
            string content = TemplateManager.RenderTemplate(EDITORIAL_FILENAME, _magazine);
            string outputFileName = Path.Combine(_magazineDirectory.FullName, EDITORIAL_FILENAME);

            File.WriteAllText(outputFileName, content);
            return true;
        }

        public bool GenerateArticles()
        {
            foreach (MagazineTypeCategorie categorie in _magazine.Categories)
            {
                foreach (ArticleType article in categorie.Articles)
                {
                    DirectoryInfo articleDirectory = _articlesDirectory.CreateSubdirectory(article.ID);
                    DirectoryInfo imageDirectory = articleDirectory.CreateSubdirectory("images");

                    IEnumerable<XElement> images = article.XDocument.XPathSelectElements("//image");
                    
                    foreach(XElement image in images)
                    {
                        XAttribute xSrc = image.Attribute("src");
                        DirectoryInfo sourceDirectory = GetArticleSourceDirectory(article);
                        string src = xSrc.Value;
                        string sourceFileName = Path.Combine(sourceDirectory.FullName, src);
                        string destination = String.Format("images/{0}", Path.GetFileName(sourceFileName));
                        File.Copy(sourceFileName, Path.Combine(imageDirectory.FullName, Path.GetFileName(destination)), true);
                        image.SetAttributeValue("src", destination);
                    }

                    string content = TemplateManager.RenderTemplate(ARTICLE_FILENAME, article);
                    string outputFileName = Path.Combine(articleDirectory.FullName, ARTICLE_FILENAME);

                    File.WriteAllText(outputFileName, content);
                }
            }
            
            return true;
        }
    }
}
