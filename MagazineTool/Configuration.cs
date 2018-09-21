using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Developpez.MagazineTool
{
    partial class ConfigurationType
    {
        private static ConfigurationType _singleton;

        static ConfigurationType()
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Developpez.MagazineTool.Configuration.xml"))
            using (StreamReader reader = new StreamReader(stream))
            {
                string xml = reader.ReadToEnd();
                _singleton = ConfigurationType.Deserialize(xml);
            }
        }

        public static ConfigurationType Singleton 
        {
            get {return _singleton;}
        }

        internal static LicenceType GetLicenceByType(int licType)
        {
            return _singleton.Licences.FirstOrDefault(x => x.ID == licType);
        }
    }
}
