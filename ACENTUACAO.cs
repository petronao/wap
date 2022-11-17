using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class ACENTUACAO : Form
    {
        public ACENTUACAO()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //string s = Clipboard.GetText();
            IDataObject dataInClipboard = Clipboard.GetDataObject();
        }
    }
}
