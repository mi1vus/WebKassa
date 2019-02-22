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
                ExternalCheckNumber = "123",
                CustomerEmail = "milvus@e1.ru"
            };

            var sale = WKAPI.SetOrder(Check);
        }
    }
}
