using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace Developpez.MagazineTool
{
    partial class ArticleType
    {
        [XmlIgnore]
        public XDocument XDocument { get; private set; }

        [XmlIgnore]
        public MagazineTypeCategorie Categorie { get; private set; }

        [XmlIgnore]
        public Contenu Synopsis { get; private set; }

        [XmlIgnore]
        public Contenu Contenu { get; private set; }
        
        public bool LoadFromDeveloppezXML(MagazineTypeCategorie categorie, XDocument xDoc)
        {
            bool ok = true;
            XElement xRoot = xDoc.Root;
            this.XDocument = xDoc;
            this.Categorie = categorie;


            if (xRoot.Name == "document")
            {
                XElement xTitre = xRoot.XPathSelectElement("entete/titre/article");
                XElement xSynopsis = xRoot.XPathSelectElement("synopsis");
                XElement xRubrique = xRoot.XPathSelectElement("entete/rubrique");
                XElement xSummary = xRoot.XPathSelectElement("summary");
                XElement xURL = xRoot.XPathSelectElement("entete/urlhttp");
                IEnumerable<XElement> xAuteurs = xRoot.XPathSelectElements("authorDescriptions/authorDescription");
                List<AuteurType> auteurs = new List<AuteurType>();

                this.Titre = xTitre.Value;
                this.Synopsis = Contenu.Create(this, xSynopsis, ContenuContexteEnum.ZoneRedigee);
                this.Rubrique = CategorieExtension.FromID(xRubrique.Value).ToLabel();
                this.Contenu = Contenu.Create(this, xSummary, ContenuContexteEnum.Contenu);
                this.URL = xURL.Value;

                foreach(XElement xAuteur in xAuteurs)
                {
                    AuteurType auteur = AuteurType.LoadFromDeveloppezXML(xAuteur);

                    if (auteur != null)
                    {
                        auteurs.Add(auteur);
                    }
                }

                this.Auteurs = auteurs;
            }
            else
            {
                ok = false;
            }

            return ok;
        }

        public string DisplayAuteurs()
        {
            return DisplayAuteurs(this.Auteurs.Where(x => x.Role == AuteurRoleEnum.Auteur).ToList());
        }

        private static string DisplayAuteurs(List<AuteurType> liste)
        {
            string[] auteurs = liste.Select(x => x.NomComplet).ToArray();
            StringBuilder builder = new StringBuilder();

            if (auteurs.Length > 1)
            {
                for(int i = 0; i < auteurs.Length - 1; ++i)
                {
                    if (i > 0)
                    {
                        builder.Append(", ");
                    }

                    builder.Append(auteurs[i]);
                }
                builder.Append(" et ");
                builder.Append(auteurs.Last());
            }
            else
            {
                if (auteurs.Length == 1)
                {
                    builder.Append(auteurs[0]);
                }
            }

            return builder.ToString();
        }

        public string DisplayFooter()
        {
            StringBuilder builder = new StringBuilder();
            string auteurs = DisplayAuteurs(this.Auteurs.Where(x => x.Role == AuteurRoleEnum.Auteur).ToList());
            string traducteurs = DisplayAuteurs(this.Auteurs.Where(x => x.Role == AuteurRoleEnum.Traducteur).ToList());
            string nomAuteursEtTraducteurs;

            if (!String.IsNullOrEmpty(traducteurs))
            {
                nomAuteursEtTraducteurs = String.Format("{0} et traduit par {1}", auteurs, traducteurs);
            }
            else
            {
                nomAuteursEtTraducteurs = auteurs;
            }

            switch (this.Source)
            {
                case SourceEnum.Actualite:
                    {
                        if (this.Complet == false)
                        {
                            builder.Append(String.Format("Retrouvez l'actualité complète de {0} en ligne : ", nomAuteursEtTraducteurs));
                        }
                        else
                        {
                            builder.Append(String.Format("Retrouvez l'actualité de {0} en ligne : ", nomAuteursEtTraducteurs));
                        }
                        break;
                    }
                case SourceEnum.Article:
                    {
                        if (this.Complet == false)
                        {
                            builder.Append(String.Format("Retrouvez l'intégralité de l'article de {0} en ligne : ", nomAuteursEtTraducteurs));
                        }
                        else
                        {
                            builder.Append(String.Format("Retrouvez l'article de {0} en ligne : ", nomAuteursEtTraducteurs));
                        }
                        break;
                    }
            }

            builder.AppendLine(String.Format(@"\href{{{0}}}{{{0}}}", this.URL));
            //builder.Append(String.Format(@"\footnote{{\lien : \texttt{{{0}}}}}", this.URL));

            return builder.ToString();
        }
    }
}
