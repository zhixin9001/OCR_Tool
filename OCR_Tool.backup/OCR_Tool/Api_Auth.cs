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
            var dataDir = AppDomain.CurrentDomain.BaseDirectory;
            var fullpath = dataDir + "Configs//Api_Auth.json";
            using (var reader = new StreamReader(fullpath))
            {
                var content = reader.ReadToEnd();
                var obj = JsonConvert.DeserializeObject<Api_Auth_Model>(content);
                return obj;
            }
        }

    }
}
