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


        }
    }
}
