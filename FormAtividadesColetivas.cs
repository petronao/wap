using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class FormAtividadesColetivas : Form
    {
        public FormAtividadesColetivas()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DateTime dt1 = DateTime.Parse(maskedTextBox1.Text);
            DateTime dt2 = DateTime.Parse(maskedTextBox2.Text);
            string datainicial = dt1.ToString("yyyy/MM/dd").Replace("/", "");
            string datafinal = dt2.ToString("yyyy/MM/dd").Replace("/", "");
            AtividadesColetivas ac = new AtividadesColetivas();
            
        }


    }
}
