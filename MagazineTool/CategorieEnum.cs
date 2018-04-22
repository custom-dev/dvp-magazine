using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Developpez.MagazineTool
{
    public enum CategorieEnum
    {
        Access                  = 19,
        Accueil                 = 1,
        Ajax                    = 85,        
        Alm                     = 42,
        Android                 = 95,
        Applications            = 88,
        Apache                  = 71,
        ASP                     = 10,
        ASPNet                  = 83,
        Aspose                  = 136,
        Assembleur              = 16,
        Autres                  = 21,
        BacASable               = 112,
        BigData                 = 126,
        Blogs                   = 28,
        BPM                     = 134,
        BSD                     = 69,
        BusinessIntelligence    = 61,
        C                       = 3,
        Cpp                     = 59,
        [Display(Name = "Cloud computing")]
        CloudComputing          = 103,
        Club                    = 24,
        CMS                     = 138,        
        Corba                   = 14,
        CRM                     = 92,
        CSharp                  = 81,
        CSS                     = 51,
        D                       = 123,
        Dart                    = 125,
        DB2                     = 36,
        DebuterAlgorithmique    = 47,
        Delphi                  = 2,
        Dev2D3DJeux             = 38,
        Dev4D                   = 39,
        DeveloppementWeb        = 8,
        Droit                   = 132,
        Eclipse                 = 55,
        EDI                     = 90,
        EmploiInformatique      = 48,
        ERP_PGI                 = 91,
        EtudesFormations        = 62,
        Etats                   = 15,
        Excel                   = 75,
        Firebird                = 73,
        FlashFlex               = 41,
        Formation               = 63,
        Fortran                 = 60,
        Go                      = 124,
        GreenIT                 = 104,
        GTKp                    = 46,
        Hardware                = 64,
        HPC                     = 139,
        HTML                    = 50,
        HumourInformatique      = 22,
        IBMBluemix              = 135,        
        InterBase               = 32,
        IOS                     = 101,
        IRC                     = 44,
        Java                    = 4,
        Javascript              = 45,
        JavaWeb                 = 97,
        Kylix                   = 6,
        LabView                 = 72,
        Langages                = 89,
        Laravel                 = 140,
        Latex                   = 49,
        Lazarus                 = 96,        
        LibertyBasic            = 52,
        LibreOpenSource         = 121,
        Linux                   = 18,
        Mac                     = 57,
        MatLab                  = 58,
        MegaOffice              = 130,
        Merise                  = 43,
        MicrosoftAzure          = 107,
        MicrosoftBizTalkServer  = 115,

        [Display(Name = "Microsoft .Net")]
        MicrosoftDotNet         = 20,
        MicrosoftOffice         = 54,
        MicrosoftProject        = 99,
        Mobiles                 = 94,
        MySQL                   = 31,
        NetBeans                = 66,
        NodeJS                  = 141,
        NoSQL                   = 114,
        ObjectiveC              = 117,
        OMNIS                   = 23,
        OpenOfficeLibreOffice   = 116,
        Oracle                  = 25,
        Outlook                 = 77,
        Pascal                  = 7,
        Perl                    = 29,
        PHP                     = 9,
        PostgreSQL              = 33,
        PowerPoint              = 78,
        Programmation           = 119,
        Projets                 = 98,
        PureBasic               = 67,
        PyQT                    = 102,
        Python                  = 27,
        Qt                      = 65,
        QtCreator               = 109,
        R                       = 100,
        RaspberryPi             = 131,
        Reseau                  = 70,
        RubyRails               = 53,
        Rust                    = 137,
        SAP                     = 106,
        SAS                     = 93,
        Scilab                  = 120,
        Securite                = 40,
        SGBDSQL                 = 13,
        SharePoint              = 79,
        SolutionEntreprises     = 86,
        Spring                  = 87,
        SQLServer               = 34,
        Stages                  = 122,
        Supervision             = 129,
        Sybase                  = 37,
        Symphony                = 133,
        Systemes                = 30,
        SystemesEmbarques       = 110,
        Swift                   = 128,
        Talend                  = 118,
        TV                      = 35,
        TypeScript              = 127,
        UML                     = 12,
        Unix                    = 68,
        VBA                     = 113,
        VBNet                   = 82,
        Virtualisation          = 108,
        VisualBasic6            = 5,
        VisualStudio            = 84,
        WebMarketing            = 80,
        WebSemantique           = 111,
        WinDev                  = 74,
        Windows                 = 17,
        WindowsPhone            = 105,
        Word                    = 76,
        XML                     = 11,
        XMLRAD                  = 26,
        ZendFramework           = 56
    }

    public static class CategorieExtension
    {
        public static CategorieEnum FromID(string sID)
        {
            int id = int.Parse(sID);
            return (CategorieEnum)id;
        }

        public static string ToLabel(this CategorieEnum value)
        {
            Type type = typeof(CategorieEnum);
            MemberInfo valueInfo = type.GetMember(value.ToString()).FirstOrDefault();
            DisplayAttribute attribute = valueInfo.GetCustomAttribute<DisplayAttribute>();
            
            if (attribute != null)
            {
                return attribute.Name;
            }
            else
            {
                return value.ToString();
            }
        }
    }

    
}
