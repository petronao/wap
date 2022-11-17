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
    public partial class Form_QueryManual : Form
    {
        public Form_QueryManual()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {            
            label1.Text = "";
            if (dataGridView1.Rows.Count > 0) dataGridView1.DataSource = null; dataGridView1.Refresh();
            Conexao c = new Conexao();
            var dt = c.Get_DataTable(textBox1.Text);
            dataGridView1.DataSource = dt;
            label1.Text = "" + dataGridView1.Rows.Count;
        }
    }
}
