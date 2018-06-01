using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCR_Tool
{
    class Api_Auth_Model
    {
        public string API_KEY;
        public string SECRET_KEY;
    }

    static class Api_Auth
    {
        public static Api_Auth_Model apiAuthConfig=GetJsonConfig();

        public static Api_Auth_Model GetJsonConfig()
        {
            string dataDir = AppDomain.CurrentDomain.BaseDirectory;
            string fullpath = dataDir + "Config//Api_Auth.json";
            using (var sr = new StreamReader(fullpath))
            {
                var s = sr.ReadToEnd();
                var b = JsonConvert.DeserializeObject<Api_Auth_Model>(s);
                return b;
            }
        }

    }
}
