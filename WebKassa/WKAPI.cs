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
        public int ErrorCode { get; set; }
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
        public bool CashboxOfflineMode;
        public string StartOfflineMode;
        public bool CashboxUniqueNumber;
        public Cashbox Cashbox;
        public int CheckOrderNumber;
        public int ShiftNumber;
        public string EmployeeName;
    }

    public class Cashbox// : Osnovan
    {
        public string UniqueNumber;
        public string RegistrationNumber;
        public string IdentityNumber;
    }

    public class ZReportFromWeb
    {
        public int ReportNumber;
        public string TaxPayerName;
        public string TaxPayerIN;
        public bool TaxPayerVAT;
        public string TaxPayerVATSerial;
        public string TaxPayerVATNumber;
        public string CashboxSN;
        public string CashboxIN;
        public string StartON;
        public string ReportON;
        public string CloseON;
        public int CashierCode;
        public int ShiftNumber;
        public int DocumentCount;
        public decimal PutMoneySum;
        public decimal TakeMoneySum;
        public string ControlSum;
        public bool OfflineMode;
        public decimal SumInCashbox;
        public Oper Sell;
        public Oper Buy;
        public Oper ReturnSell;
        public Oper ReturnBuy;
        public NonNullable StartNonNullable;
        public NonNullable EndNonNullable;
    }

    public class Oper// : Osnovan
    {
        public List<Payment> PaymentBytTypesApiModel;
        public decimal Discount;
        public decimal Markup;
        public decimal Taken;
        public decimal Change;
        public int Count;
        public decimal VAT;
    }

    public class Payment// : Osnovan
    {
        public decimal Sum;
        public int Type;
    }

    public class NonNullable// : Osnovan
    {
        public decimal Sell;
        public decimal Buy;
        public decimal ReturnSell;
        public decimal ReturnBuy;
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

        private static string zReport =
"{" + Environment.NewLine +
"  \"Token\": \"{0}\"," + Environment.NewLine +
"  \"CashboxUniqueNumber\": \"{1}\"" + Environment.NewLine +
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

        public static CheckFromWeb SetOrder(CheckForSale itemsToSale)
        {
            try
            {
                var req = new JavaScriptSerializer().Serialize(itemsToSale);
                var order_Raw = POST("Check", req);

                var err = JsonHelper.ParseResponseErrors(order_Raw);
                //CheckFromWeb res = JsonHelper.ParseGood(order_Raw);
                if (err != null)
                {
                    LogError("Authorize ERROR: " + string.Join("; ", err.Select(e => "code: [" + e.ErrorCode + "] Descr: " + e.ErrorDescription)), "JsonHelper.ParseResponseErrors");
                    return null;
                }

                return JsonHelper.ParseGood(order_Raw);//JsonHelper.ParseGoodPrepare(good_Raw, kind);
            }
            catch (Exception ex)
            {
                LogError("SetOrder ERROR: " + ex.ToString(), ex.StackTrace);
                return null;
            }
        }

        public static ZReportFromWeb ZReport(string Token, string CashboxUniqueNumber)
        {
            var req = zReport.Replace("{0}", Token);
            req = req.Replace("{1}", CashboxUniqueNumber);
            var zreport_Raw = POST("ZReport", req);
            var err = JsonHelper.ParseResponseErrors(zreport_Raw);
            if (err != null)
            {
                LogError("Authorize ERROR: " + string.Join("; ", err.Select(e => "code: [" + e.ErrorCode + "] Descr: " + e.ErrorDescription)), "JsonHelper.ParseResponseErrors");
                return null;
            }

            return JsonHelper.ParseZReport(zreport_Raw);//JsonHelper.ParseGoodPrepare(good_Raw, kind);
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
