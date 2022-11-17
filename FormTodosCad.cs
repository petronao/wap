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
    public partial class FormTodosCad : Form
    {
        public FormTodosCad()
        {
            InitializeComponent();
        }
        private string query(string texto)
        {
            string q = @"select
        to_char(t1.dt_cad_individual, 'DD/MM/YYYY') as dt_cad,  
        t1.no_cidadao as nome,		
		CASE 
			WHEN t2.co_dim_sexo = 1 THEN 'M'
			ELSE 'F'
		END sexo,		
		to_char(t2.dt_nascimento, 'DD/MM/YYYY') as dt_nasc,  
        date_part('year',age(t2.dt_nascimento)) as idade,
		t2.nu_cns as FATcns,
		t2.nu_cpf_cidadao as FATcpf,
		t3.nu_cns as cns,
		t3.nu_cpf_cidadao as cpf,
		/*t2.co_dim_unidade_saude as unidade_saude,    */
		t99.nu_cnes as cnes,
		t2.nu_micro_area as mc,
		/*t1.st_fora_area as fa,*/		
        CASE WHEN t2.st_hipertensao_arterial = '1' THEN 'has' ELSE '0' END AS has,		
        CASE WHEN t2.st_diabete = '1' THEN 'dm' ELSE '0' END AS dm,
        t2.st_fumante as tab,
        t2.st_alcool as alc,
		t2.st_gestante as ges,
		t3.co_dim_unidade_saude_vinc as unidade,
		(select kk.no_equipe from tb_dim_equipe kk where t3.co_dim_equipe_vinc = kk.co_seq_dim_equipe )as equipe_vinc,
        (select tp.no_profissional from tb_dim_profissional tp where t2.co_dim_profissional = tp.co_seq_dim_profissional) as acs,
		t3.st_faleceu,
		t2.co_fat_cidadao_pec
        from 
        tb_cds_cad_individual t1 
		left join tb_fat_cad_individual t2 on t1.co_unico_ficha = t2.nu_uuid_ficha
		left join tb_fat_cidadao_pec t3 on t2.co_fat_cidadao_pec = t3.co_seq_fat_cidadao_pec
		left join tb_dim_unidade_saude t99 on t2.co_dim_unidade_saude = t99.co_seq_dim_unidade_saude
		where
		(t1.st_versao_atual = 1) 
        AND (t1.st_ficha_inativa = 0)         
        AND (t1.st_fora_area = 0)
		AND (t2.co_dim_tipo_saida_cadastro = 3) " + texto;
            return q;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string condicao = "AND (t2.st_hipertensao_arterial = 1 AND t2.st_diabete = 0) ";
            if (dataGridView1.Rows.Count > 0) { dataGridView1.DataSource = null; dataGridView1.Rows.Clear(); dataGridView1.Refresh(); }
            Conexao c = new Conexao();
            var dt = c.Get_DataTable(query(condicao));
            dataGridView1.DataSource = dt;
            label1.Text = "" + dataGridView1.Rows.Count;

            this.dataGridView1.Columns["fatcns"].Visible = false;
            this.dataGridView1.Columns["fatcpf"].Visible = false;
            this.dataGridView1.Columns["unidade"].Visible = false;
            this.dataGridView1.Columns["ges"].Visible = false;
            this.dataGridView1.Columns["st_faleceu"].Visible = false;
            this.dataGridView1.Columns["co_fat_cidadao_pec"].Visible = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string condicao = "AND (t2.st_hipertensao_arterial = 0 AND t2.st_diabete = 1) ";
            if (dataGridView1.Rows.Count > 0) { dataGridView1.DataSource = null; dataGridView1.Rows.Clear(); dataGridView1.Refresh(); }
            Conexao c = new Conexao();
            var dt = c.Get_DataTable(query(condicao));
            dataGridView1.DataSource = dt;
            label1.Text = "" + dataGridView1.Rows.Count;
            this.dataGridView1.Columns["fatcns"].Visible = false;
            this.dataGridView1.Columns["fatcpf"].Visible = false;
            this.dataGridView1.Columns["unidade"].Visible = false;
            this.dataGridView1.Columns["ges"].Visible = false;
            this.dataGridView1.Columns["st_faleceu"].Visible = false;
            this.dataGridView1.Columns["co_fat_cidadao_pec"].Visible = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string condicao = "AND (t2.st_hipertensao_arterial = 1 AND t2.st_diabete = 1) ";
            if (dataGridView1.Rows.Count > 0) { dataGridView1.DataSource = null; dataGridView1.Rows.Clear(); dataGridView1.Refresh(); }
            Conexao c = new Conexao();
            var dt = c.Get_DataTable(query(condicao));
            dataGridView1.DataSource = dt;
            label1.Text = "" + dataGridView1.Rows.Count;
            this.dataGridView1.Columns["fatcns"].Visible = false;
            this.dataGridView1.Columns["fatcpf"].Visible = false;
            this.dataGridView1.Columns["unidade"].Visible = false;
            this.dataGridView1.Columns["ges"].Visible = false;
            this.dataGridView1.Columns["st_faleceu"].Visible = false;
            this.dataGridView1.Columns["co_fat_cidadao_pec"].Visible = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string condicao = "AND (t2.st_gestante = 1) ";
            if (dataGridView1.Rows.Count > 0) { dataGridView1.DataSource = null; dataGridView1.Rows.Clear(); dataGridView1.Refresh(); }
            Conexao c = new Conexao();
            var dt = c.Get_DataTable(query(condicao));
            dataGridView1.DataSource = dt;
            label1.Text = "" + dataGridView1.Rows.Count;
            this.dataGridView1.Columns["fatcns"].Visible = false;
            this.dataGridView1.Columns["fatcpf"].Visible = false;
            this.dataGridView1.Columns["unidade"].Visible = false;            
            this.dataGridView1.Columns["st_faleceu"].Visible = false;
            this.dataGridView1.Columns["co_fat_cidadao_pec"].Visible = false;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string query = @"select
        t2.no_cidadao,
        t1.nu_cns,
        t1.co_cidadao,
        t1.co_cidadao_master,
        t1.co_cidadao_unificado,
        t1.nu_cpf
        from
        tb_cidadao_grupo t1
        left join tb_cidadao t2
        on t1.co_cidadao = t2.co_seq_cidadao
        order by t2.no_cidadao";
        }
    }
}
