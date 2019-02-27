using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;


namespace WebKassaAPI
{
    public class JsonHelper
    {
        public static string ParseAuthorize(string src)
        {
            var pars = src.Split(new[] { "\"Token\":\"" }, StringSplitOptions.RemoveEmptyEntries);
            if (pars.Length < 2 )
                return null;

            var ind = pars[1].IndexOf('\"');
            var token = pars[1].Substring(0, ind);

            return token;
        }

        public static CheckFromWeb ParseGood(string src)
        {
            CheckFromWeb result = null;

            if (!src.StartsWith("{") || !src.EndsWith("}"))
                return result;

            src = src.Substring(1, src.Length - 2);

            if (Regex.IsMatch(src, "\\bErrors\\b"))
            {
                var lines = src.Split(new[] { "\"Errors\":" }, StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length <= 1 || !lines[0].StartsWith("\"Data\":{") || !lines[0].EndsWith("},"))
                    return result;

                src = lines[0].Substring(7, lines[0].Length - 8);
            }


            if (!src.StartsWith("{") || !src.EndsWith("}"))
                return null;
            src = src.Substring(1, src.Count() - 2);

            result = new CheckFromWeb();
            //result.Cashbox = new Cashbox();

            var subObject = src.Split(new[] { "{" }, StringSplitOptions.RemoveEmptyEntries);
            if (subObject.Length == 2)
            {
                src = subObject[0];
                subObject = subObject[1].Split(new[] { "}" }, StringSplitOptions.RemoveEmptyEntries);
                if (subObject.Length == 2)
                {
                    src += "sub," + subObject[1];
                }
            }

            var parameters = src.Split(new[] { ",\"" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var pair in parameters)
            {
                var values = pair.Split(new[] { "\":" }, StringSplitOptions.RemoveEmptyEntries);
                if (values.Count() != 2)
                    continue;

                var nm = values[0];
                if (nm.StartsWith("\""))
                    nm = nm.Substring(1, nm.Count() - 1);
                if (nm.EndsWith("\""))
                    nm = nm.Substring(0, nm.Count() - 1);

                var val = values[1];
                if (val.StartsWith("\""))
                    val = val.Substring(1, val.Count() - 1);
                if (val.EndsWith("\""))
                    val = val.Substring(0, val.Count() - 1);


                switch (nm)
                {
                    case "CheckNumber":
                        if (result != null)
                            result.CheckNumber = val == "null" ? null : val;
                        break;
                    case "DateTime":
                        if (result != null)
                            result.DateTime = val == "null" ? null : val;
                        break;
                    case "OfflineMode":
                        if (result != null)
                            result.OfflineMode = bool.Parse(val);
                        break;
                    case "CashboxUniqueNumber":
                        if (result != null)
                            result.CashboxUniqueNumber = bool.Parse(val);
                        break;
                    case "CashboxOfflineMode":
                        if (result != null)
                            result.CashboxOfflineMode = bool.Parse(val);
                        break;
                    case "StartOfflineMode":
                        if (result != null)
                            result.StartOfflineMode = val == "null" ? null : val;
                        break;
                    case "Cashbox":
                        if (result != null & val == "sub,")
                        {
                            result.Cashbox = ParseCashbox("{" + subObject[0] + "}");
                        }
                        break;
                    case "CheckOrderNumber":
                        if (result != null)
                            result.CheckOrderNumber = int.Parse(val);
                        break;
                    case "ShiftNumber":
                        if (result != null)
                            result.ShiftNumber = int.Parse(val);
                        break;
                    case "EmployeeName":
                        if (result != null)
                            result.EmployeeName = val == "null" ? null : val;
                        break;
                }
            }
            return result;
        }
        public static Cashbox ParseCashbox(string src)
        {
            Cashbox result = null;

            if (!src.StartsWith("{") || !src.EndsWith("}"))
                return result;

            src = src.Substring(1, src.Length - 2);

            result = new Cashbox();

            var parameters = src.Split(new[] { ",\"" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var pair in parameters)
            {
                var values = pair.Split(new[] { "\":" }, StringSplitOptions.RemoveEmptyEntries);
                if (values.Count() != 2)
                    continue;

                var nm = values[0];
                if (nm.StartsWith("\""))
                    nm = nm.Substring(1, nm.Count() - 1);
                if (nm.EndsWith("\""))
                    nm = nm.Substring(0, nm.Count() - 1);

                var val = values[1];
                if (val.StartsWith("\""))
                    val = val.Substring(1, val.Count() - 1);
                if (val.EndsWith("\""))
                    val = val.Substring(0, val.Count() - 1);


                switch (nm)
                {
                    case "UniqueNumber":
                        if (result != null)
                            result.UniqueNumber = val == "null" ? null : val;
                        break;
                    case "RegistrationNumber":
                        if (result != null)
                            result.RegistrationNumber = val == "null" ? null : val;
                        break;
                    case "IdentityNumber":
                        if (result != null)
                            result.IdentityNumber = val == "null" ? null : val;
                        break;
                }
            }
            return result;
        }
        public static ZReportFromWeb ParseZReport(string src)
        {
            ZReportFromWeb result = null;

            if (!src.StartsWith("{") || !src.EndsWith("}"))
                return result;

            src = src.Substring(1, src.Length - 2);

            if (Regex.IsMatch(src, "\\bErrors\\b"))
            {
                var lines = src.Split(new[] { "\"Errors\":" }, StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length <= 1 || !lines[0].StartsWith("\"Data\":{") || !lines[0].EndsWith("},"))
                    return result;

                src = lines[0].Substring(7, lines[0].Length - 8);
            } else {
                src = src.Substring(7, src.Length - 8);
            }

            if (!src.StartsWith("{") || !src.EndsWith("}"))
                return null;
            src = src.Substring(1, src.Count() - 2);

            result = new ZReportFromWeb();
            //result.Cashbox = new Cashbox();

            var subObject = src.Split(new[] { "{" , "}" }, StringSplitOptions.RemoveEmptyEntries);
            string[] subs = new string[6];
            int subIndex = 0;
            src = "";
            for (int i = 1; i < subObject.Length; ++i)
            {
                src += subObject[i-1] + "sub";
                if (subObject[i].EndsWith("[")) {
                    var ssubCnt = 0;
                    do
                    {
                        var ssubDel = ssubCnt > 1 ? "," : "";
                        subs[subIndex] += ssubCnt == 0 ?
                            subObject[i] :
                            ssubDel + "{" +  subObject[i] + "}";
                        ++ssubCnt;
                        ++i;
                    } while (!subObject[i].StartsWith("]"));
                }
                subs[subIndex] += subObject[i];
                ++subIndex;
                ++i;
            }

            var parameters = src.Split(new[] { ",\"" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var pair in parameters)
            {
                var values = pair.Split(new[] { "\":" }, StringSplitOptions.RemoveEmptyEntries);
                if (values.Count() != 2)
                    continue;

                var nm = values[0];
                if (nm.StartsWith("\""))
                    nm = nm.Substring(1, nm.Count() - 1);
                if (nm.EndsWith("\""))
                    nm = nm.Substring(0, nm.Count() - 1);

                var val = values[1];
                if (val.StartsWith("\""))
                    val = val.Substring(1, val.Count() - 1);
                if (val.EndsWith("\""))
                    val = val.Substring(0, val.Count() - 1);


                switch (nm)
                {
                    case "ReportNumber":
                        if (result != null)
                            result.ReportNumber = int.Parse(val);
                        break;
                    case "TaxPayerName":
                        if (result != null)
                            result.TaxPayerName = val == "null" ? null : val;
                        break;
                    case "TaxPayerIN":
                        if (result != null)
                            result.TaxPayerIN = val == "null" ? null : val;
                        break;
                    case "TaxPayerVAT":
                        if (result != null)
                            result.TaxPayerVAT = bool.Parse(val);
                        break;
                    case "TaxPayerVATSerial":
                        if (result != null)
                            result.TaxPayerVATSerial = val == "null" ? null : val;
                        break;
                    case "TaxPayerVATNumber":
                        if (result != null)
                            result.TaxPayerVATNumber = val == "null" ? null : val;
                        break;
                    case "CashboxSN":
                        if (result != null)
                        {
                            result.CashboxSN = val == "null" ? null : val;
                        }
                        break;
                    case "CashboxIN":
                        if (result != null)
                            result.CashboxIN = val == "null" ? null : val;
                        break;
                    case "StartON":
                        if (result != null)
                            result.StartON = val == "null" ? null : val;
                        break;
                    case "ReportON":
                        if (result != null)
                            result.ReportON = val == "null" ? null : val;
                        break;
                    case "CloseON":
                        if (result != null)
                            result.CloseON = val == "null" ? null : val;
                        break;
                    case "CashierCode":
                        if (result != null)
                            result.CashierCode = int.Parse(val);
                        break;
                    case "ShiftNumber":
                        if (result != null)
                            result.ShiftNumber = int.Parse(val);
                        break;
                    case "DocumentCount":
                        if (result != null)
                            result.DocumentCount = int.Parse(val);
                        break;
                    case "PutMoneySum":
                        if (result != null)
                            result.PutMoneySum = decimal.Parse(val.Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator));
                        break;
                    case "TakeMoneySum":
                        if (result != null)
                            result.TakeMoneySum = decimal.Parse(val.Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator));
                        break;
                    case "ControlSum":
                        if (result != null)
                            result.ControlSum = val == "null" ? null : val;
                        break;
                    case "OfflineMode":
                        if (result != null)
                            result.OfflineMode = bool.Parse(val);
                        break;
                    case "SumInCashbox":
                        if (result != null)
                            result.SumInCashbox = decimal.Parse(val.Replace('.',','));
                        break;
                    case "Sell":
                        if (result != null & val == "sub")
                        {
                            result.Sell = ParseOper("{" + subs[0] + "}");
                        }
                        break;
                    case "Buy":
                        if (result != null & val == "sub")
                        {
                            result.Buy = ParseOper("{" + subs[1] + "}");
                        }
                        break;
                    case "ReturnSell":
                        if (result != null & val == "sub")
                        {
                            result.ReturnSell = ParseOper("{" + subs[2] + "}");
                        }
                        break;
                    case "ReturnBuy":
                        if (result != null & val == "sub")
                        {
                            result.ReturnBuy = ParseOper("{" + subs[3] + "}");
                        }
                        break;
                    case "StartNonNullable":
                        if (result != null & val == "sub")
                        {
                            result.StartNonNullable = ParseNonNullable("{" + subs[4] + "}");
                        }
                        break;
                    case "EndNonNullable":
                        if (result != null & val == "sub")
                        {
                            result.EndNonNullable = ParseNonNullable("{" + subs[5] + "}");
                        }
                        break;
                }
            }
            return result;
        }
        public static Oper ParseOper(string src)
        {
            Oper result = null;

            if (!src.StartsWith("{") || !src.EndsWith("}"))
                return result;

            src = src.Substring(1, src.Length - 2);

            result = new Oper();
            result.PaymentBytTypesApiModel = new List<Payment>();
            var payment = new Payment();

            var parameters = src.Split(new[] { ",\"" , "[{", "}]" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var pair in parameters)
            {
                var values = pair.Split(new[] { "\":" }, StringSplitOptions.RemoveEmptyEntries);
                if (values.Count() != 2)
                    continue;

                var nm = values[0];
                if (nm.StartsWith("\""))
                    nm = nm.Substring(1, nm.Count() - 1);
                if (nm.EndsWith("\""))
                    nm = nm.Substring(0, nm.Count() - 1);

                var val = values[1];
                if (val.StartsWith("\""))
                    val = val.Substring(1, val.Count() - 1);
                if (val.EndsWith("\""))
                    val = val.Substring(0, val.Count() - 1);


                switch (nm)
                {
                    case "PaymentBytTypesApiModel":
                        //if (result != null)
                        //    result.PaymentBytTypesApiModel = null;
                        break;
                    case "Sum":
                        if (result != null)
                            payment.Sum = decimal.Parse(val.Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator));
                        break;
                    case "Type":
                        if (result != null)
                            payment.Type = int.Parse(val);
                        break;
                    case "Discount":
                        if (result != null)
                            result.Discount = decimal.Parse(val.Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator));
                        break;
                    case "Markup":
                        if (result != null)
                            result.Markup = decimal.Parse(val.Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator));
                        break;
                    case "Taken":
                        if (result != null)
                            result.Taken = decimal.Parse(val.Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator));
                        break;
                    case "Change":
                        if (result != null)
                            result.Change = decimal.Parse(val.Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator));
                        break;
                    case "Count":
                        if (result != null)
                            result.Count = int.Parse(val);
                        break;
                    case "VAT":
                        if (result != null)
                            result.VAT = decimal.Parse(val.Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator));
                        break;
                }
            }

            if (payment.Sum != 0 || payment.Type != 0)
            {
                result.PaymentBytTypesApiModel.Add(payment);
            }
            return result;
        }
        public static NonNullable ParseNonNullable(string src)
        {
            NonNullable result = null;

            if (!src.StartsWith("{") || !src.EndsWith("}"))
                return result;

            src = src.Substring(1, src.Length - 2);

            result = new NonNullable();

            var parameters = src.Split(new[] { ",\"", "[{", "}]" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var pair in parameters)
            {
                var values = pair.Split(new[] { "\":" }, StringSplitOptions.RemoveEmptyEntries);
                if (values.Count() != 2)
                    continue;

                var nm = values[0];
                if (nm.StartsWith("\""))
                    nm = nm.Substring(1, nm.Count() - 1);
                if (nm.EndsWith("\""))
                    nm = nm.Substring(0, nm.Count() - 1);

                var val = values[1];
                if (val.StartsWith("\""))
                    val = val.Substring(1, val.Count() - 1);
                if (val.EndsWith("\""))
                    val = val.Substring(0, val.Count() - 1);


                switch (nm)
                {
                    case "Sell":
                        if (result != null)
                            result.Sell = decimal.Parse(val.Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator));
                        break;
                    case "Buy":
                        if (result != null)
                            result.Buy = decimal.Parse(val.Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator));
                        break;
                    case "ReturnSell":
                        if (result != null)
                            result.ReturnSell = decimal.Parse(val.Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator));
                        break;
                    case "ReturnBuy":
                        if (result != null)
                            result.ReturnBuy = decimal.Parse(val.Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator));
                        break;
                }
            }

            return result;
        }
        public static List<Error> ParseResponseErrors(string src)
        {
            if (!src.StartsWith("{") || !src.EndsWith("}"))
                return new List<Error> { new Error { ErrorCode = -1, ErrorDescription = "Неверный формат ответа!" } };

            src = src.Substring(1, src.Length - 2);

            if (!Regex.IsMatch(src, "\\bErrors\\b") || Regex.IsMatch(src, "\"Errors\":null"))
                return null;

            var errline = src.Split(new[] { "\"Errors\":" }, StringSplitOptions.RemoveEmptyEntries);
            if (!errline[errline.Length - 1].StartsWith("[") || !errline[errline.Length - 1].EndsWith("]"))
                return new List<Error> { new Error { ErrorCode = -1, ErrorDescription = "Неверный формат ответа!" } };

            var errs = errline[errline.Length - 1].Substring(2, errline[errline.Length - 1].Length - 4).Split(new[] { "},{" }, StringSplitOptions.RemoveEmptyEntries);
            var res = new List<Error>();
            foreach (var err in errs)
            {
                var errObj = new Error();
                var pars = err.Split(new[] { ",\"" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var par in pars)
                {
                    var pair = par.Split(new[] { "\":" }, StringSplitOptions.RemoveEmptyEntries);
                    if (pair[0].EndsWith("\"Code"))
                    {
                        errObj.ErrorCode = int.Parse(pair[1]);
                    }
                    else if (pair[0].EndsWith("Text"))
                    {
                        errObj.ErrorDescription = pair[1];
                    }
                }
                if (errObj.ErrorCode != 0)
                {
                    res.Add(errObj);
                }
            }
            if (res.Count <= 0)
                return null;
            else
                return res;
        }
    }

}
