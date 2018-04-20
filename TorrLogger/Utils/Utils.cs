﻿using Newtonsoft.Json;
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

        //public static string CountryFromIp(string ip)
        //{
        //    var json = new WebClient().DownloadString("http://ip-api.com/json/" + ip);
        //    dynamic obj = JsonConvert.DeserializeObject(json);
        //    string country = obj.country;

        //    return country;
        //}

        public static ExpandoObject IspAndCountryFromIp(string ip)
        {
            var json = new WebClient().DownloadString("http://ip-api.com/json/" + ip);
            dynamic jsonObj = JsonConvert.DeserializeObject(json);
            dynamic result = new ExpandoObject();
            result.Country = jsonObj.country;
            result.Isp = jsonObj.isp;
            return result;
        }
    }
}
