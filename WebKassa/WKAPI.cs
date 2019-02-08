using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace WebKassaAPI
{
    public class Error
    {
        public string ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
    }

    class WKAPI
    {
        public static string url = "https://devkkm.webkassa.kz/api";

        private static string authorize =
"{" + Environment.NewLine +
"  \"Login\": \"{0}\"," + Environment.NewLine +
"  \"Password\": \"{1}\"" + Environment.NewLine +
"}" + Environment.NewLine;

        public static string Authorize(string Login, string Pass)
        {
            var req = authorize.Replace("{0}", Login);
            req = req.Replace("{1}", Pass);
            var authorize_Raw = POST("Authorize", req, 1/*request_code*/);
            //++request_code;
            //SaveCodes();
            int kind = -1;
            //if (!good_Raw.Contains("Kind"))
            //{
            //    var goods = GetGoodsList();
            //    kind = goods.First(t => t.Item == item).Kind;
            //}
            var err = JsonHelper.ParseResponseErrors(authorize_Raw);
            if (err != null)
            {
                LogError("Authorize ERROR: " + string.Join("; ", err.Select(e=>"code: [" + e.ErrorCode + "] Descr: " + e.ErrorDescription)), "JsonHelper.ParseResponseErrors");
                return null;
            }
                        
            return JsonHelper.ParseAuthorize(authorize_Raw);//JsonHelper.ParseGoodPrepare(good_Raw, kind);
        }

        private static string POST(string method, string req_S, int id)
        {
            var boundary = "------------------------" + DateTime.Now.Ticks;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + $"/{method}");
            request.Method = WebRequestMethods.Http.Post;
            request.ContentType = "application/json";
            try
            {
                using (Stream dataStream = request.GetRequestStream())
                {
                    var reqWriter = new StreamWriter(dataStream);
                    reqWriter.Write(req_S);
                    reqWriter.Flush();
                }

                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                    return reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                WebResponse errorResponse = ex.Response;
                using (Stream responseStream = errorResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.GetEncoding("utf-8"));
                    String errorText = reader.ReadToEnd();
                    LogError("WebException ERROR: " + errorText, ex.StackTrace);
                }
                throw;
            }
        }

        private static string LogError(string msg, string path)
        {
            bool res = false;
            try
            {
                if (!System.IO.Directory.Exists("Logs/"))
                    System.IO.Directory.CreateDirectory("Logs/");

                var fpath = "Logs/" + DateTime.Today.ToString("dd_MM_yyyy") + ".txt";
                var text = string.Format(
    @"[{0}] - {1}" + Environment.NewLine + "+++ [{2}]" + Environment.NewLine, DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), msg, path);
                System.IO.File.AppendAllText(fpath, text);
                res = true;
            }
            catch { }

            return res.ToString();
        }
    }
}
