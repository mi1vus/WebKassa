using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebKassaAPI;

namespace WebKassa
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            //var Author = UTAPI.Authorize("SWK00030990", "Kassa123");
            var Author = WKAPI.Authorize("chetverikov.v@agzs.kz", "Kassa123");

//            var str =
//"{\"Data\":{\"TaxPayerName\":\"ИП Есеркенов Г. К.\",\"TaxPayerIN\":\"761201350079\",\"TaxPayerVAT\":true,\"TaxPayerVATSeria\":\"62005\",\"TaxPayerVATNumber\":\"1234567\",\"Image\":null,\"ExternalReportId\":null,\"ReportNumber\":2,\"CashboxSN\":\"SWK00030990\",\"CashboxIN\":2801,\"CashboxRN\":\"091100030990\",\"StartOn\":\"22.02.2019 19:43:47\",\"ReportOn\":\"25.02.2019 19:34:15\",\"CloseOn\":\"25.02.2019 19:34:15\",\"CashierCode\":4,\"ShiftNumber\":10,\"DocumentCount\":2,\"PutMoneySum\":0.0,\"TakeMoneySum\":82.2,\"ControlSum\":4340,\"OfflineMode\":false,\"CashboxOfflineMode\":false,\"StartOfflineMode\":null,\"SumInCashbox\":0.0,\"Sell\":{\"PaymentsByTypesApiModel\":[{\"Sum\":82.2,\"Type\":0}],\"Discount\":0.0,\"Markup\":0.0,\"Taken\":82.2000000000,\"Change\":0.0,\"Count\":1,\"TotalCount\":16,\"VAT\":4.89},\"Buy\":{\"PaymentsByTypesApiModel\":[],\"Discount\":0.0,\"Markup\":0.0,\"Taken\":0.0,\"Change\":0.0,\"Count\":0,\"TotalCount\":0,\"VAT\":0.0},\"ReturnSell\":{\"PaymentsByTypesApiModel\":[],\"Discount\":0.0,\"Markup\":0.0,\"Taken\":0.0,\"Change\":0.0,\"Count\":0,\"TotalCount\":1,\"VAT\":0.0},\"ReturnBuy\":{\"PaymentsByTypesApiModel\":[],\"Discount\":0.0,\"Markup\":0.0,\"Taken\":0.0,\"Change\":0.0,\"Count\":0,\"TotalCount\":0,\"VAT\":0.0},\"EndNonNullable\":{\"Sell\":66149.4,\"Buy\":0.0,\"ReturnSell\":1650.0,\"ReturnBuy\":0.0},\"StartNonNullable\":{\"Sell\":66067.2,\"Buy\":0.0,\"ReturnSell\":1650.0,\"ReturnBuy\":0.0}},\"Errors\":null}";
//            var err = JsonHelper.ParseZReport(str);
            var x = WKAPI.XReport(Author, WKAPI.id_kassa);

            List<PositionForSale> positions = new List<PositionForSale> {
                new PositionForSale {
                    Count = 2,
                    Price = 22.8M,
                    Tax = 4.89M,
                    TaxType = TaxType.С_НДС,
                    PositionName = "Фрукты",
                    Discount = 0,
                    Markup = 0,
                    SectionCode = "1",
                    IsStorno = false,
                    MarkupDeleted = false,
                    DiscountDeleted = false,
                },
                new PositionForSale {
                    Count = 3,
                    Price = 12.2M,
                    Tax = 0,
                    TaxType = TaxType.Без_НДС,
                    PositionName = "АИ-92",
                    Discount = 0,
                    Markup = 0,
                    SectionCode = "1",
                    IsStorno = false,
                    MarkupDeleted = false,
                    DiscountDeleted = false,
                }
            };

            List<PaymentForSale> payments = new List<PaymentForSale> {
                new PaymentForSale {
                    Sum = 22.8M*2 + 12.2M * 3,//Math.Round( 22.8M*2 + 12.2M * 3, MidpointRounding.AwayFromZero),
                    PaymentType = PaymentType.Наличные
                }
            };
            
            var Check = new CheckForSale
            {
                Token = Author,
                CashboxUniqueNumber = WKAPI.id_kassa,
                OperationType = CheckType.Продажа,
                Positions = positions.ToArray(),
                Payments = payments.ToArray(),
                Change = null,
                RoundType = RoundType.Без_округления,
                ExternalCheckNumber = "171",
                CustomerEmail = "milvus@e1.ru"
            };

            var sale = WKAPI.SetOrder(Check);
        }
    }
}
