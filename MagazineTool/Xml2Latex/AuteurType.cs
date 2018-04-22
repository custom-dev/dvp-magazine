using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Developpez.MagazineTool
{
    partial class AuteurType
    {
        internal static AuteurType LoadFromDeveloppezXML(XElement xAuteur)
        {
            AuteurType auteur = new AuteurType();
            XElement xFullName = xAuteur.XPathSelectElement("fullname");
            XElement xURL = xAuteur.XPathSelectElement("url");
            XAttribute xRole = xAuteur.Attribute("role");

            if (xFullName != null)
            {
                auteur.NomComplet = xFullName.Value;
            }

            if (xURL != null)
            {
                auteur.URL = xURL.Value;
            }

            if (xRole != null)
            {
                switch(xRole.Value)
                {
                    case "auteur":
                        auteur.Role = AuteurRoleEnum.Auteur;
                        break;
                    case "traducteur":
                        auteur.Role = AuteurRoleEnum.Traducteur;
                        break;
                    default:
                        auteur = null;
                        break;

                }
            }
            else
            {
                auteur = null;
            }


            return auteur;
        }
    }
}
