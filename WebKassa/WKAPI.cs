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
    public enum CheckType {
        Покупка = 0,
        Возврат_покупки,
        Продажа,
        Возврат_продажи
    }
    public enum TaxType
    {
        Без_НДС = 0,
        С_НДС = 100
    }
    public enum RoundType
    {
        Без_округления = 0,
        Округление_итога,
        Округление_позиции
    }
    public enum PaymentType
    {
        Наличные = 0,
        Банковская_карта,
        Оплата_в_кредит,
        Оплата_тарой
    }


    public class Error
    {
        public string ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
    }

    public class CheckForSale
    {
        public string Token;
        public string CashboxUniqueNumber;
        public CheckType OperationType;
        public PositionForSale[] Positions;
        public PaymentForSale[] Payments;
        public decimal? Change;
        public RoundType RoundType;
        public string ExternalCheckNumber;
        public string CustomerEmail;
    }

    public class PositionForSale// : Good
    {
        public decimal Count { get; set; }
        public decimal Price { get; set; }
        public decimal Tax { get; set; }
        public TaxType TaxType { get; set; }
        public string PositionName { get; set; }
        public decimal Discount { get; set; }
        public decimal Markup { get; set; }
        public string SectionCode { get; set; }
        public bool IsStorno { get; set; }
        public bool MarkupDeleted { get; set; }
        public bool DiscountDeleted { get; set; }
    }

    public class PaymentForSale// : Osnovan
    {
        public decimal Sum { get; set; }
        public PaymentType PaymentType { get; set; }
    }

    public class CheckFromWeb
    {
        public string CheckNumber;
        public string DateTime;
        public bool OfflineMode;
        public bool CashboxUniqueNumber;
        public Cashbox Cashbox;
        public int CheckOrderNumber;
        public int ShiftNumber;
        public string EmployeeName;
    }

    public class Cashbox// : Osnovan
    {
        public string UnickueNumber;
        public string RegidtrationNumber;
        public string IdentityNumber;
    }

    class WKAPI
    {
        public static string url = "https://devkkm.webkassa.kz/api";
        public static string id_kassa = "SWK00030990";

        private static string authorize =
"{" + Environment.NewLine +
"  \"Login\": \"{0}\"," + Environment.NewLine +
"  \"Password\": \"{1}\"" + Environment.NewLine +
"}" + Environment.NewLine;

        public static string Authorize(string Login, string Pass)
        {
            var req = authorize.Replace("{0}", Login);
            req = req.Replace("{1}", Pass);
            var authorize_Raw = POST("Authorize", req);
            var err = JsonHelper.ParseResponseErrors(authorize_Raw);
            if (err != null)
            {
                LogError("Authorize ERROR: " + string.Join("; ", err.Select(e=>"code: [" + e.ErrorCode + "] Descr: " + e.ErrorDescription)), "JsonHelper.ParseResponseErrors");
                return null;
            }
                        
            return JsonHelper.ParseAuthorize(authorize_Raw);//JsonHelper.ParseGoodPrepare(good_Raw, kind);
        }

        public static bool SetOrder(CheckForSale itemsToSale)
        {
            try
            {
                var req = new JavaScriptSerializer().Serialize(itemsToSale);
                var order_Raw = POST("Check", req);

                var err = JsonHelper.ParseResponseErrors(order_Raw);
                var res = JsonHelper.ParseGood(order_Raw);
                if (err != null)
                {
                    LogError("Authorize ERROR: " + string.Join("; ", err.Select(e => "code: [" + e.ErrorCode + "] Descr: " + e.ErrorDescription)), "JsonHelper.ParseResponseErrors");
                    return false;
                }



                return JsonHelper.ParseGood(order_Raw) == null;//JsonHelper.ParseGoodPrepare(good_Raw, kind);
            }
            catch (Exception ex)
            {
                LogError("SetOrder ERROR: " + ex.ToString(), ex.StackTrace);
                return false;
            }
        }

        private static string POST(string method, string req_S)
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
