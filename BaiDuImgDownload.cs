using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quick.Framework.Tool.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace QuickRMS.Site.WebUI.Code
{
    public class BaiDuImgDownload
    {
        public static string DownloadImage(string keyWord)
        {
            //得到保存的路径  
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "\\files\\img\\baidu");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("http://image.baidu.com/search/acjson?tn=resultjson_com&ipn=rj&ct=201326592&is=&fp=result&queryWord=" + Uri.EscapeUriString(keyWord) + "&cl=2&lm=-1&ie=utf-8&oe=utf-8&adpicid=&st=-1&z=&ic=0&word=" + Uri.EscapeUriString(keyWord) + "&s=&se=&tab=&width=&height=&face=0&istype=2&qc=&nc=1&fr=&pn=30&rn=30&gsm=1e&1471422249875=");
            using (HttpWebResponse res = (HttpWebResponse)req.GetResponse())
            {
                if (res.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream stream = res.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            //分析图片路径 PS：获取第一张图片就够了
                            string json = reader.ReadToEnd();
                            dynamic objRoot = (dynamic)JsonConvert.DeserializeObject(json);
                            string objUrl = (string)objRoot.data[0].thumbURL;
                            req = (HttpWebRequest)WebRequest.Create(objUrl);
                            req.Referer = "http://www.baidu.com/";//欺骗网站服务器这是从百度图片发出的  
                            var resquest = (HttpWebResponse)req.GetResponse();
                            if (resquest.StatusCode == HttpStatusCode.OK)
                            {
                                path = Path.Combine(path, Path.GetFileName(objUrl));
                                using (Stream st = resquest.GetResponseStream())
                                using (Stream filestream = new FileStream(path, FileMode.Create))
                                {
                                    st.CopyTo(filestream);
                                }
                            }
                            else
                            {
                                throw new Exception("下载失败" + res.StatusCode);
                            }
                        }
                    }
                }
                else
                {
                    LogHelper.WriteLog("百度图片下载失败" + res.StatusCode);
                }
            }
            return path;
        }
    }
}