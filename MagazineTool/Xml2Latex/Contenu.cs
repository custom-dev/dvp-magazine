using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Developpez.MagazineTool
{
    public class Contenu
    {
        private XElement _xRoot;
        private ContenuContexteEnum _contexte;
        private ArticleType _article;

        private Contenu(ArticleType article, XElement xRoot, ContenuContexteEnum contexte)
        {
            _xRoot = xRoot;
            _contexte = contexte;
            _article = article;
        }

        public static Contenu Create(ArticleType article, XElement xRoot, ContenuContexteEnum contexte)
        {
            return new Contenu(article, xRoot, contexte);
        }

        public string ToLatex()
        {
            using (StringWriter writer = new StringWriter())
            {
                ContenuState state = new ContenuState();
                switch (_contexte)
                {
                    case ContenuContexteEnum.Contenu:
                        TranslateContenu(_article, writer, _xRoot, state);
                        break;
                    case ContenuContexteEnum.ZoneRedigee:
                        TranslateZoneRedigee(_article, writer, _xRoot, state);
                        break;
                }

                return writer.ToString();
            }
        }

        private static void TranslateContenu(ArticleType article, TextWriter writer, XElement xElement, ContenuState state)
        {
            IEnumerable<XNode> xNodes = xElement.Nodes();
            state.Sections.Push(0);

            foreach (XNode xNode in xNodes)
            {
                switch (xNode.NodeType)
                {
                    case XmlNodeType.Element:
                        {
                            XElement xChildElement = xNode as XElement;
                            switch(xChildElement.Name.ToString())
                            {
                                case "section":
                                    TranslateSection(article, writer, xChildElement, state);
                                    break;
                                default:
                                    throw new NotImplementedException(xChildElement.Name.ToString());
                            }
                        }
                        break;
                    default:
                        throw new NotImplementedException(xNode.NodeType.ToString());
                }
            }

            state.Sections.Pop();
        }

        private static void TranslateZoneRedigee(ArticleType article, TextWriter writer, XElement xElement, ContenuState state)
        {
            if (xElement != null)
            {
                IEnumerable<XNode> xNodes = xElement.Nodes();
                foreach (XNode xChildNode in xNodes)
                {
                    switch (xChildNode.NodeType)
                    {
                        case XmlNodeType.Element:
                            {
                                XElement xChildElement = xChildNode as XElement;
                                switch (xChildElement.Name.ToString())
                                {
                                    case "annotation-kitoodvp":
                                        break;
                                    case "b":
                                        TranslateB(article, writer, xChildElement, state);
                                        break;
                                    case "br":
                                        TranslateBr(writer, xChildElement, state);
                                        break;
                                    case "code":
                                        TranslateCode(article, writer, xChildElement, state);
                                        break;
                                    case "font":
                                        TranslateFont(article, writer, xChildElement, state);
                                        break;
                                    case "i":
                                        TranslateI(article, writer, xChildElement, state);
                                        break;
                                    case "image":
                                        TranslateImage(article, writer, xChildElement, state);
                                        break;
                                    case "inline":
                                        TranslateInline(writer, xChildElement, state);
                                        break;
                                    case "important":
                                        TranslateImportant(article, writer, xChildElement, state);
                                        break;
                                    case "link":
                                        TranslateLink(article, writer, xChildElement, state);
                                        break;
                                    case "liste":
                                        TranslateListe(article, writer, xChildElement, state);
                                        break;
                                    case "paragraph":
                                        TranslateParagraphe(article, writer, xChildElement, state);
                                        break;
                                    case "renvoi":
                                        TranslateRenvoi(writer, xChildElement, state);
                                        break;
                                    case "signet":
                                        TranslateSignet(writer, xChildElement, state);
                                        break;
                                    case "sub":
                                        TranslateSub(article, writer, xChildElement, state);
                                        break;
                                    case "sup":
                                        TranslateSup(article, writer, xChildElement, state);
                                        break;
                                    default:
                                        throw new NotImplementedException(xChildElement.Name.ToString());
                                }
                                break;
                            }
                        case XmlNodeType.Text:
                            XText xChildText = xChildNode as XText;
                            writer.Write(EscapeChar(xChildText.Value));
                            break;
                        default:
                            throw new NotImplementedException(xChildNode.NodeType.ToString());
                    }
                }
            }
        }

        private static void TranslateSection(ArticleType article, TextWriter writer, XElement xElement, ContenuState state)
        {
            XElement xTitle = xElement.XPathSelectElement("title");
            XAttribute xID = xElement.Attribute("id");
            XAttribute xNoNumber = xElement.Attribute("noNumber");
            XAttribute xSautDePagepdf = xElement.Attribute("sautDePagePdf");
            IEnumerable<XElement> xSections = xElement.Elements("section");
            int sectionDeep;
            string title = xTitle != null ? xTitle.Value : String.Empty;

            state.Stack.Push("section");
            state.Sections.Push(state.Sections.Pop() + 1);
            sectionDeep = state.Stack.Where(x => x == "section").Count();

            if (MustIncludeSection(state, article))
            {
                switch (sectionDeep)
                {
                    case 1:
                        //writer.WriteLine(@"\begin{{multicols}}{{2}}[{0}]", !String.IsNullOrEmpty(title) ? String.Format(@"\subsection{{{0}}}", EscapeChar(title)) : String.Empty);
                        writer.WriteLine(@"\subsection{{{0}}}", EscapeChar(title));
                        break;
                    case 2:
                        if (!String.IsNullOrEmpty(title))
                        {
                            writer.WriteLine(@"\subsubsection{{{0}}}", EscapeChar(title));
                        }
                        break;
                    case 3:
                        if (!String.IsNullOrEmpty(title))
                        {
                            writer.WriteLine(@"\subsubsubsection{{{0}}}", EscapeChar(title));
                        }
                        break;
                    case 4:
                        if (!String.IsNullOrEmpty(title))
                        {
                            writer.WriteLine(@"\paragraph{{{0}}}", EscapeChar(title));
                        }
                        break;
                    default:
                        throw new NotImplementedException();
                }

                IEnumerable<XNode> xNodes = xElement.Nodes();

                bool hasSectionChildren = xElement.Element("section") != null;

                if (hasSectionChildren)
                {
                    state.Sections.Push(0);
                }

                foreach (XNode xChildNode in xNodes)
                {
                    switch (xChildNode.NodeType)
                    {
                        case XmlNodeType.Element:
                            {
                                XElement xChildElement = xChildNode as XElement;
                                switch (xChildElement.Name.ToString())
                                {
                                    case "animation":
                                        TranslateAnimation(writer, xChildElement, state);                                        
                                        break;
                                    case "citation":
                                        TranslateCitation(article, writer, xChildElement, state);
                                        break;
                                    case "code":
                                        TranslateCode(article, writer, xChildElement, state);
                                        break;
                                    case "image":
                                        TranslateImage(article, writer, xChildElement, state);
                                        break;
                                    case "imgtext":
                                        TranslateImgText(article, writer, xChildElement, state);
                                        break;
                                    case "liste":
                                        TranslateListe(article, writer, xChildElement, state);
                                        break;
                                    case "paragraph":
                                        TranslateParagraphe(article, writer, xChildElement, state);
                                        break;
                                    case "rich-imgtext":
                                        TranslateRichImgText(article, writer, xChildElement, state);
                                        break;
                                    case "section":
                                        TranslateSection(article, writer, xChildElement, state);
                                        break;
                                    case "signet":
                                        TranslateSignet(writer, xChildElement, state);
                                        break;
                                    case "tableau":
                                        TranslateTableau(article, writer, xChildElement, state);
                                        break;
                                    case "title":
                                        break;
                                    default:
                                        throw new NotImplementedException(xChildElement.Name.ToString());
                                }
                                break;
                            }
                        default:
                            throw new NotImplementedException(xChildNode.NodeType.ToString());
                    }
                }

                if (hasSectionChildren)
                {
                    state.Sections.Pop();
                }

                switch (sectionDeep)
                {
                    case 1:
                        //writer.WriteLine(@"\end{multicols}");
                        writer.WriteLine();
                        break;
                    case 2:
                    case 3:
                    case 4:
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            state.Stack.Pop();
        }

        private static bool MustIncludeSection(ContenuState state, ArticleType article)
        {
            if (article.Sections != null && article.Sections.Count > 0)
            {
                SectionType section = null;
                List<SectionType> sections = article.Sections;

                foreach(int numeroSection in state.Sections.Reverse())
                {
                    // Si pas de précision sur les sous-sections, on les inclus toutes
                    if (sections == null || sections.Count == 0)
                    {
                        return true;
                    }

                    section = sections.FirstOrDefault(x => x.Numero == numeroSection);
                    if (section == null)
                    {
                        return false;
                    }
                    else
                    {
                        sections = section.Section;
                    }
                }
            }

            return true;
        }

        private static void TranslateParagraphe(ArticleType article, TextWriter writer, XElement xElement, ContenuState state)
        {
            state.Stack.Push("paragraph");
            TranslateTexte(article, writer, xElement, state);

            if (xElement.NextNode != null)
            {
                writer.WriteLine();
                writer.WriteLine();
            }
            state.Stack.Pop();
        }

        private static void TranslateLienForum(TextWriter writer, XElement xElement, ContenuState state)
        {
            // Rien à faire !
        }

        private static void TranslateB(ArticleType article, TextWriter writer, XElement xElement, ContenuState state)
        {
            state.Stack.Push("b");
            writer.Write(@"\textbf{");
            TranslateTexte(article, writer, xElement, state);
            writer.Write("}");
            state.Stack.Pop();
        }

        private static void TranslateI(ArticleType article, TextWriter writer, XElement xElement, ContenuState state)
        {
            state.Stack.Push("i");
            writer.Write(@"\textit{");
            TranslateTexte(article, writer, xElement, state);
            writer.Write("}");
            state.Stack.Pop();
        }

        private static void TranslateImportant(ArticleType article, TextWriter writer, XElement xElement, ContenuState state)
        {
            state.Stack.Push("i");
            writer.Write(@"\textit{\textbf{");
            TranslateTexte(article, writer, xElement, state);
            writer.Write("}}");
            state.Stack.Pop();
        }

        private static void TranslateImage(ArticleType article, TextWriter writer, XElement xElement, ContenuState state)
        {
            XAttribute xAlign = xElement.Attribute("align");
            XAttribute xSrc = xElement.Attribute("src");
            XAttribute xLegende = xElement.Attribute("legende");
            XAttribute xZoom = xElement.Attribute("zoom");
            XAttribute xAlt = xElement.Attribute("alt");
            XAttribute xHref = xElement.Attribute("href");
            XAttribute xTitre = xElement.Attribute("titre");
            string align = xAlign != null ? xAlign.Value : String.Empty;
            string src = xSrc != null ? xSrc.Value : String.Empty;
            string scale = "0.4";

            // Modification de l'extension si c'est un gif
            if (src.ToLower().EndsWith(".gif"))
            {
                src = src.Substring(0, src.Length - ".gif".Length) + ".png";
            }

            if (article.Images != null)
            {
                ImageType image = article.Images.FirstOrDefault(x => x.Src == src);

                if (image != null)
                {
                    scale = image.Taille;
                }
            }

            state.Stack.Push("image");
    
            if (state.Stack.FirstOrDefault(x => x == "tableau") != null ||
                state.Stack.FirstOrDefault(x => x == "rich-imgtexte") != null ||
                state.Stack.FirstOrDefault(x => x == "paragraph") != null)
            {                
                writer.Write(@"\includegraphics[scale={1}]{{\DVPGetImagePath/{0}}}", src, scale);
            }
            else
            {
                if (xLegende == null || String.IsNullOrEmpty(xLegende.Value))
                {
                    writer.WriteLine(@"\begin{figure}[H]");
                }
                else
                {
                    writer.WriteLine(@"\begin{figure}[H]");
                }
                
                writer.WriteLine(@"\center");

                writer.WriteLine(@"\includegraphics[scale={1}]{{\DVPGetImagePath/{0}}}", src, scale);
                if (xLegende != null && !String.IsNullOrEmpty(xLegende.Value))
                {
                    writer.WriteLine(@"\caption{{{0}}}", EscapeChar(xLegende.Value));
                }
                writer.WriteLine(@"\end{figure}");
                writer.WriteLine();
            }
            
            state.Stack.Pop();
        }

        private static void TranslateTexte(ArticleType article, TextWriter writer, XElement xElement, ContenuState state)
        {
            IEnumerable<XNode> xNodes = xElement.Nodes();
            foreach (XNode xChildNode in xNodes)
            {
                switch (xChildNode.NodeType)
                {
                    case XmlNodeType.Element:
                        {
                            XElement xChildElement = xChildNode as XElement;
                            switch (xChildElement.Name.ToString())
                            {
                                case "annotation-kitoodvp":
                                    break;
                                case "b":
                                    TranslateB(article, writer, xChildElement, state);
                                    break;
                                case "br":
                                    TranslateBr(writer, xChildElement, state);
                                    break;
                                case "code":
                                    TranslateCode(article, writer, xChildElement, state);
                                    break;
                                case "font":
                                    TranslateFont(article, writer, xChildElement, state);
                                    break;
                                case "i":
                                    TranslateI(article, writer, xChildElement, state);
                                    break;
                                case "image":
                                    TranslateImage(article, writer, xChildElement, state);
                                    break;
                                case "important":
                                    TranslateImportant(article, writer, xChildElement, state);
                                    break;
                                case "inline":
                                    TranslateInline(writer, xChildElement, state);
                                    break;
                                case "lien-forum":
                                    TranslateLienForum(writer, xChildElement, state);
                                    break;
                                case "link":
                                    TranslateLink(article, writer, xChildElement, state);
                                    break;
                                case "liste":
                                    TranslateListe(article, writer, xChildElement, state);
                                    break;
                                case "noteBasPage":
                                    TranslateNoteBasPage(article, writer, xChildElement, state);
                                    break;
                                case "paragraph":
                                    TranslateParagraphe(article, writer, xChildElement, state);
                                    break;
                                case "renvoi":
                                    TranslateRenvoi(writer, xChildElement, state);
                                    break;
                                case "signet":
                                    TranslateSignet(writer, xChildElement, state);
                                    break;
                                case "sub":
                                    TranslateSub(article, writer, xChildElement, state);
                                    break;
                                case "sup":
                                    TranslateSup(article, writer, xChildElement, state);
                                    break;
                                case "u":
                                    TranslateU(article, writer, xChildElement, state);
                                    break;
                                default:
                                    throw new NotImplementedException(xChildElement.Name.ToString());
                            }
                            break;
                        }
                    case XmlNodeType.Text:
                    case XmlNodeType.CDATA:
                        {
                            XText xChildText = xChildNode as XText;
                            writer.Write(EscapeChar(xChildText.Value));
                            break;
                        }
                    default:
                        throw new NotImplementedException(xChildNode.NodeType.ToString());
                }
            }
        }

        private static void TranslateListe(ArticleType article, TextWriter writer, XElement xElement, ContenuState state)
        {
            IEnumerable<XElement> xElements = xElement.Elements("element");

            state.Stack.Push("liste");
            state.Stack.Push("element");

            writer.WriteLine(@"\begin{itemize}");
            foreach (XElement xItem in xElements)
            {
                writer.Write(@"\item ");
                TranslateZoneRedigee(article, writer, xItem, state);
                writer.WriteLine();
            }
            
            writer.WriteLine(@"\end{itemize}");
            writer.WriteLine();

            state.Stack.Pop();
            state.Stack.Pop();
        }

        private static void TranslateLink(ArticleType article, TextWriter writer, XElement xElement, ContenuState state)
        {
            XAttribute xHref = xElement.Attribute("href");
            XAttribute xTarget = xElement.Attribute("target");
            XAttribute xOnClick = xElement.Attribute("onclick");
            XAttribute xTitle = xElement.Attribute("title");
            XAttribute xLangue = xElement.Attribute("langue");

            string rawUrl = EscapeChar(xHref.Value);
            //TranslateTexte(article, writer, xElement, state);

            //if (xHref != null)
            //{
            //    writer.WriteLine(@" \href{{{0}}}{{\lien}}", xHref.Value);
            //}
            writer.Write(String.Format(@"\href{{{0}}}{{", xHref.Value));
            TranslateTexte(article, writer, xElement, state);            
            writer.Write(@"}");
            writer.Write(String.Format(@"\footnote{{\lien : \texttt{{{0}}}}}", rawUrl));

        }

        private static void TranslateRenvoi(TextWriter writer, XElement xElement, ContenuState state)
        {
            XAttribute xID = xElement.Attribute("id");
            string texte = xElement.Value;

            writer.Write(EscapeChar(texte));
        }

        private static void TranslateTableau(ArticleType article, TextWriter writer, XElement xElement, ContenuState state)
        {
            XElement info = xElement.XPathSelectElement("tableau/ligne/colonne[1]/image[@src='./images/100002010000002000000020F2813DC4.png']");

            if (info != null)
            {
                XElement xText = xElement.XPathSelectElement("tableau/ligne/colonne[2]");
                state.Stack.Push("tableau");
                state.Stack.Push("ligne");
                state.Stack.Push("colonne");
                writer.WriteLine(@"\begin{info}{}");
                TranslateZoneRedigee(article, writer, xElement, state);
                writer.WriteLine(@"\end{info}");
                state.Stack.Pop();
                state.Stack.Pop();
                state.Stack.Pop();
            }
            else
            {
                XAttribute xBorder = xElement.Attribute("border");
                bool hasBorder = xBorder != null && xBorder.Value != "0";                
                IEnumerable<XElement> xFirstLine = xElement.XPathSelectElements("ligne[1]/colonne");
                IEnumerable<XElement> xLignes = xElement.XPathSelectElements("ligne");
                string[] formatColonne = new string[xFirstLine.Count()];
                string formatColonnes;

                int count = 0;
                state.Stack.Push("tableau");

                foreach (XElement xCell in xFirstLine)
                {
                    //                    formatColonne[count] = "m{3.5cm}";
                    formatColonne[count] = "l";
                    count++;
                }

                if (hasBorder)
                {
                    formatColonnes = String.Format("|{0}|", String.Join("|", formatColonne));
                }
                else
                {
                    formatColonnes = String.Join(String.Empty, formatColonne);
                }

                writer.WriteLine(@"\begin{center}");
                writer.WriteLine(@"\begin{{tabular}}{{{0}}}", formatColonnes);

                if (hasBorder)
                {
                    writer.WriteLine(@"\hline");
                }

                foreach(XElement xLigne in xLignes)
                {
                    IEnumerable<XElement> xColonnes = xLigne.XPathSelectElements("colonne");
                    bool firstCell = true;

                    foreach(XElement xCell in xColonnes)
                    {
                        if (!firstCell)
                        {
                            writer.Write("&");
                        }

                        TranslateTexte(article, writer, xCell, state);
                        firstCell = false;    
                    }
                    writer.WriteLine(@"\\");
                }

                if (hasBorder)
                {
                    writer.WriteLine(@"\hline");
                }
                writer.WriteLine(@"\end{tabular}");
                writer.WriteLine(@"\end{center}");
                writer.WriteLine();

                state.Stack.Pop();

            }
        }

        private static void TranslateImgText(ArticleType article, TextWriter writer, XElement xElement, ContenuState state)
        {
            XAttribute xType = xElement.Attribute("type");
            XAttribute xSrc = xElement.Attribute("src");
            XAttribute xBorder = xElement.Attribute("border");
            XAttribute xFond = xElement.Attribute("fond");

            string type = String.Empty;

            switch(xType.Value)
            {
                case "idea":
                    type = "idee";
                    break;
                case "warning":
                    type = "attention";
                    break;
                case "info":
                    type = "info";
                    break;
                case "error":
                    type = "danger";
                    break;
                default:
                    throw new NotImplementedException();
            }

            state.Stack.Push("imgtexte");
            writer.WriteLine(@"\begin{{{0}}}{{}}", type);
            TranslateTexte(article, writer, xElement, state);
            writer.WriteLine(@"\end{{{0}}}", type);
            writer.WriteLine();

            state.Stack.Pop();
        }

        private static void TranslateBr(TextWriter writer, XElement xElement, ContenuState state)
        {
            writer.WriteLine();
        }

        private static void TranslateFont(ArticleType article, TextWriter writer, XElement xElement, ContenuState state)
        {
            TranslateTexte(article, writer, xElement, state);
        }

        private static void TranslateCitation(ArticleType article, TextWriter writer, XElement xElement, ContenuState state)
        {
            state.Stack.Push("citation");
            writer.WriteLine(@"\begin{quote}");
            TranslateTexte(article, writer, xElement, state);
            writer.WriteLine(@"\end{quote}");
            state.Stack.Pop();
        }

        private static void TranslateNoteBasPage(ArticleType article, TextWriter writer, XElement xElement, ContenuState state)
        {
            state.Stack.Push("noteBasPage");
            writer.Write(@"\footnote{");
            TranslateTexte(article, writer, xElement, state);
            writer.Write("}");
            state.Stack.Pop();
        }

        private static void TranslateInline(TextWriter writer, XElement xElement, ContenuState state)
        {
            state.Stack.Push("inline");
            writer.Write(@"\textit{");
            writer.Write(EscapeChar(xElement.Value));
            writer.Write("}");
            state.Stack.Pop();
        }

        private static void TranslateSub(ArticleType article, TextWriter writer, XElement xElement, ContenuState state)
        {
            state.Stack.Push("sub");
            //writer.Write("_{");
            writer.Write(@"\textsubscript{");
            TranslateTexte(article, writer, xElement, state);
            writer.Write("}");
            state.Stack.Pop();
        }

        private static void TranslateSup(ArticleType article, TextWriter writer, XElement xElement, ContenuState state)
        {
            state.Stack.Push("sup");
            //writer.Write("^{");
            writer.Write(@"\textsuperscript{");
            TranslateTexte(article, writer, xElement, state);
            writer.Write("}");
            state.Stack.Pop();
        }

        private static void TranslateRichImgText(ArticleType article, TextWriter writer, XElement xElement, ContenuState state)
        {
            XAttribute xType = xElement.Attribute("type");
            XAttribute xSrc = xElement.Attribute("src");
            XAttribute xBorder = xElement.Attribute("border");
            XAttribute xFond = xElement.Attribute("fond");

            string type = String.Empty;

            switch (xType.Value)
            {
                case "idea":
                    type = "idee";
                    break;
                case "warning":
                    type = "attention";
                    break;
                case "info":
                    type = "info";
                    break;
                default:
                    throw new NotImplementedException();
            }

            state.Stack.Push("rich-imgtexte");
            writer.WriteLine(@"\begin{{{0}}}{{}}", type);
            TranslateTexte(article, writer, xElement, state);
            writer.WriteLine(@"\end{{{0}}}", type);
            state.Stack.Pop();
        }

        private static void TranslateCode(ArticleType article, TextWriter writer, XElement xElement, ContenuState state)
        {
            XAttribute xLanguage = xElement.Attribute("langage");
            state.Stack.Push("code");
            writer.Write(@"\begin{lstlisting}");
            
            if (xLanguage != null)
            {
                switch(xLanguage.Value.ToLower())
                {
                    case "assembly-r2000":
                        writer.Write(@"[language={[Motorola68k]Assembler}]");
                        break;
                    case "c":
                        writer.Write(@"[language=C]");
                        break;
                    case "cpp":
                        writer.Write("[language=C++]");
                        break;
                    case "csharp":
                        writer.Write(@"[language={[Sharp]C}]");
                        break;
                    case "java":
                        writer.Write("[language=Java]");
                        break;
                    case "javascript":
                        //writer.Write("[language=Java]");
                        break;
                    case "makefile":
                        writer.Write("[language=make]");
                        break;
                    case "other":
                        break;
                    case "php":
                        writer.Write("[language=PHP]");
                        break;
                    case "qml":
                        break;
                    case "qt":
                        writer.Write("[language=C++]");
                        break;
                    case "shell":
                        writer.Write("[language=sh]");
                        break;
                    case "text":
                        break;
                    default:
                        writer.Write(@"[language={0}]", xLanguage.Value);
                        break;
            }
        }

            writer.WriteLine();

            foreach (XNode xChildNode in xElement.Nodes())
            {
                switch (xChildNode.NodeType)
                {
                    case XmlNodeType.Text:
                    case XmlNodeType.CDATA:
                        XText xChildText = xChildNode as XText;
                        writer.Write(xChildText.Value);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }            

            //TranslateTexte(article, writer, xElement, state);
            writer.WriteLine(@"\end{lstlisting}");
            state.Stack.Pop();
        }

        private static void TranslateU(ArticleType article, TextWriter writer, XElement xElement, ContenuState state)
        {
            state.Stack.Push("u");
            writer.Write(@"\underline{");
            TranslateTexte(article, writer, xElement, state);
            writer.Write("}");
            state.Stack.Pop();
        }

        private static void TranslateSignet(TextWriter writer, XElement xElement, ContenuState state)
        {
            XAttribute xID = xElement.Attribute("id");
            writer.Write(@"\label{{{0}}}", xID.Value);            
        }

        private static void TranslateAnimation(TextWriter writer, XElement xElement, ContenuState state)
        {
            //Rien à faire
        }

        private static string EscapeChar(string str)
        {
            return str.Replace("&", @"\&").
                Replace("#", @"\#").
                Replace("_", @"\_").
                Replace("°", @"\no").
                Replace("\u009c", @"\oe{}").
                Replace("…", @"\dots").
                Replace("’", @"'").
                Replace("«", @"\og{}").
                Replace("»", @"\fg{}").
                Replace("%", @"\%").
                Replace("$", @"\$");     
        }
    }
}
