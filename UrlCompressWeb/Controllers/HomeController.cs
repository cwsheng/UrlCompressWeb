using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UrlCompressWeb.Models;

namespace UrlCompressWeb.Controllers
{
    public class HomeController : Controller
    {

        private Dictionary<string, string> keyValuesUrls = new Dictionary<string, string>();

        public IActionResult Index()
        {
            return View();
        }

        private string CompressWebRrl(string longUrl)
        {
            string[] arr = ShortUrl(longUrl);
            if (arr != null && arr.Length > 0)
            {
                foreach (var item in arr)
                {
                    keyValuesUrls[item] = longUrl;
                }
                return arr[0];
            }
            return string.Empty;
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public static string[] ShortUrl(string url)
        {
            //可以自定义生成MD5加密字符传前的混合KEY
            string key = "Freemud";
            //要使用生成URL的字符
            string[] chars = new string[]
            {
                "a", "b", "c", "d", "e", "f", "g", "h",
                "i", "j", "k", "l", "m", "n", "o", "p",
                "q", "r", "s", "t", "u", "v", "w", "x",
                "y", "z", "0", "1", "2", "3", "4", "5",
                "6", "7", "8", "9", "A", "B", "C", "D",
                "E", "F", "G", "H", "I", "J", "K", "L",
                "M", "N", "O", "P", "Q", "R", "S", "T",
                "U", "V", "W", "X", "Y", "Z"
            };
            //对传入网址进行MD5加密
            string hex = string.Empty;//System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(key + url, "md5");

            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.UTF8.GetBytes(key + url));
                hex = BitConverter.ToString(result);
                hex = hex.Replace("-", "");
            }
            string[] resUrl = new string[4];
            for (int i = 0; i < 4; i++)
            {
                //把加密字符按照8位一组16进制与0x3FFFFFFF进行位与运算
                int hexint = 0x3FFFFFFF & Convert.ToInt32("0x" + hex.Substring(i * 8, 8), 16);
                string outChars = string.Empty;
                for (int j = 0; j < 6; j++)
                {
                    //把得到的值与0x0000003D进行位与运算，取得字符数组chars索引
                    int index = 0x0000003D & hexint;
                    //把取得的字符相加
                    outChars += chars[index];
                    //每次循环按位右移5位
                    hexint = hexint >> 5;
                }
                //把字符串存入对应索引的输出数组
                resUrl[i] = outChars;
            }
            return resUrl;
        }

        [HttpPost]
        public IActionResult CompressUrl(string longUrl)
        {
            string host = HttpContext.Request.Host.Value;
            string url = host + "/"+ CompressWebRrl(longUrl);
            return Json(url);
        }
    }
}
