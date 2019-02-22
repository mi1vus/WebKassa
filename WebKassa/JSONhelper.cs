using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;


namespace WebKassaAPI
{
    class JsonHelper
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
        /*
                public static List<Good> ParseGoods(string src)
                {
                    var begInd = src.IndexOf('[', 0);
                    var endInd = src.IndexOf(']', 0);

                    //            if (!src.StartsWith(
                    //"{\r\n  \"GoodsList\":[") || !src.EndsWith("]\r\n  }"))
                    //                return null;

                    src = src.Substring(begInd + 7, endInd - begInd - 11);
                    src = src.Replace("\r\n      ", "").Replace("\r\n    ", "").Replace("\r\n  ", "");

                    var result = new List<Good>();

                    var objs = src.Split(new[] { "},{" }, StringSplitOptions.RemoveEmptyEntries);
                    var count = 0;
                    foreach (var g in objs)
                    {
                        try
                        {
                            if (count > 0 && count < objs.Count() - 1)
                                result.Add(ParseGood("{" + g + "}"));
                            else if (count == objs.Count() - 1)
                                result.Add(ParseGood("{" + g));
                            else
                                result.Add(ParseGood(g + "}"));
                            ++count;
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    return result;
                }
                public static Good ParseGoodPrepare(string src, int kind = -1)
                {
                    src = src.Replace("\r\n      ", "").Replace("\r\n    ", "").Replace("\r\n  ", "");
                    src = src.Substring(12, src.Length - 12 - 3);

                    if (kind >= 0)
                        src = src.Insert(1, "\"Kind\":" + kind + ",");

                    return ParseGood(src);
                }


                public static List<Osnovan> ParsegetOsnovans(string src)
                {
                    var begInd = src.IndexOf('[', 0);
                    var endInd = src.IndexOf(']', 0);

                    //            if (!src.StartsWith(
                    //"{\r\n  \"GoodsList\":[") || !src.EndsWith("]\r\n  }"))
                    //                return null;

                    src = src.Substring(begInd + 7, endInd - begInd - 11);
                    src = src.Replace("\r\n      ", "").Replace("\r\n    ", "").Replace("\r\n  ", "");

                    var result = new List<Osnovan>();

                    var objs = src.Split(new[] { "},{" }, StringSplitOptions.RemoveEmptyEntries);
                    var count = 0;
                    foreach (var g in objs)
                    {
                        try
                        {
                            if (count > 0 && count < objs.Count() - 1)
                                result.Add(ParseOsnovan("{" + g + "}"));
                            else if (count == objs.Count() - 1)
                                result.Add(ParseOsnovan("{" + g));
                            else
                                result.Add(ParseOsnovan(g + "}"));
                            ++count;
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    return result;
                }
                private static Osnovan ParseOsnovan(string src)
                {
                    Osnovan result = new Osnovan();

                    if (!src.StartsWith("{") || !src.EndsWith("}"))
                        return null;
                    src = src.Substring(1, src.Count() - 2);

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
                            case "OsnovanId":
                                if (result != null)
                                    result.OsnovanId = int.Parse(val);
                                break;
                            case "Name":
                                if (result != null)
                                    result.Name = val == "null" ? null : val;
                                break;
                            case "ShortName":
                                if (result != null)
                                    result.ShortName = val == "null" ? null : val;
                                break;
                            case "NoMoneyInReports":
                                if (result != null)
                                    result.NoMoneyInReports = bool.Parse(val);
                                break;
                            case "ZeroAmountsInCheque":
                                if (result != null)
                                    result.ZeroAmountsInCheque = bool.Parse(val);
                                break;
                            case "PriceInCheque":
                                if (result != null)
                                    result.PriceInCheque = bool.Parse(val);
                                break;
                            case "IsDefault":
                                if (result != null)
                                    result.IsDefault = bool.Parse(val);
                                break;
                            case "IsDisallowed":
                                if (result != null)
                                    result.IsDisallowed = bool.Parse(val);
                                break;
                            case "IsHidden":
                                if (result != null)
                                    result.IsHidden = bool.Parse(val);
                                break;
                            case "ForGoodsAndServices":
                                if (result != null)
                                    result.ForGoodsAndServices = bool.Parse(val);
                                break;
                            case "ForFuels":
                                if (result != null)
                                    result.ForFuels = bool.Parse(val);
                                break;
                            case "DisallowPrepayMode":
                                if (result != null)
                                    result.DisallowPrepayMode = bool.Parse(val);
                                break;
                            case "DisallowPostpayMode":
                                if (result != null)
                                    result.DisallowPostpayMode = bool.Parse(val);
                                break;
                            case "PrintOsnovanName":
                                if (result != null)
                                    result.PrintOsnovanName = bool.Parse(val);
                                break;
                            case "FuelReturnsToTank":
                                if (result != null)
                                    result.FuelReturnsToTank = bool.Parse(val);
                                break;
                            case "MaxLitersPreset":
                                if (result != null)
                                    result.MaxLitersPreset = int.Parse(val);
                                break;
                            case "MaxMoneyPreset":
                                if (result != null)
                                    result.MaxMoneyPreset = int.Parse(val);
                                break;
                            case "DisallowMovePreset":
                                if (result != null)
                                    result.DisallowMovePreset = bool.Parse(val);
                                break;

                        }
                    }
                    return result;
                }
        */
        public static List<Error> ParseResponseErrors(string src)
        {
            if (!src.StartsWith("{") || !src.EndsWith("}"))
                return new List<Error> { new Error { ErrorCode = "null", ErrorDescription = "Неверный формат ответа!" } };

            src = src.Substring(1, src.Length - 2);

            if (!Regex.IsMatch(src, "\\bErrors\\b") || Regex.IsMatch(src, "\"Errors\":null"))
                return null;

            var errline = src.Split(new[] { "\"Errors\":" }, StringSplitOptions.RemoveEmptyEntries);
            if (errline.Length <= 1 || !errline[errline.Length - 1].StartsWith("[") || !errline[errline.Length - 1].EndsWith("]"))
                return new List<Error> { new Error { ErrorCode = "null", ErrorDescription = "Неверный формат ответа!" } };

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
                        errObj.ErrorCode = pair[1];
                    }
                    else if (pair[0].EndsWith("Text"))
                    {
                        errObj.ErrorDescription = pair[1];
                    }
                }
                if (!string.IsNullOrWhiteSpace(errObj.ErrorCode) && !string.IsNullOrWhiteSpace(errObj.ErrorDescription) &&
                    errObj.ErrorCode != "0")
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
