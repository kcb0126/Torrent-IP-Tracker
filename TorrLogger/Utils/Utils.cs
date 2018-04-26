using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TorrLogger.Utils
{
    class Utils
    {
        public static string FileSizeExpression(double size)
        {
            string sizeUnit = "";
            if (size >= 1048576000)
            {
                size = size / (1024 * 1024 * 1024);
                sizeUnit = "GB";
            }
            else if (size >= 1024000)
            {
                size = size / (1024 * 1024);
                sizeUnit = "MB";
            }
            else if (size >= 1000)
            {
                size = size / 1024;
                sizeUnit = "KB";
            }
            else
            {
                sizeUnit = "bytes";
            }
            return string.Format("{0:0.##} {1}", size, sizeUnit);
        }

        private static string _myip = string.Empty;

        private static string MyIp() {
            if (_myip.Equals(string.Empty))
            {
                dynamic result = IspAndCountryFromIp(string.Empty);
                _myip = result.Query;
            }
            return _myip;
        }

        public static ExpandoObject IspAndCountryFromIp(string ip)
        {
            if (!ip.Equals(string.Empty))
            {
                if (ip.Equals(MyIp()))
                {
                    dynamic result1 = new ExpandoObject();
                    result1.Country = "Unknown";
                    result1.Isp = "Unknown";
                    return result1;
                }
            }

            var json = new WebClient().DownloadString("http://ip-api.com/json/" + ip);
            dynamic jsonObj;
            dynamic result = new ExpandoObject();
            try
            {
                jsonObj = JsonConvert.DeserializeObject(json);

                if (jsonObj.status != "success")
                {
                    result.Country = "Unknown";
                    result.Isp = "Unknown";
                    return result;
                }
                result.Country = jsonObj.country;
                result.Isp = jsonObj.isp;
                result.Query = jsonObj.query;
                return result;
            }
            catch
            {
                result.Country = "Unknown";
                result.Isp = "Unknown";
                return result;
            }
        }
    }
}
