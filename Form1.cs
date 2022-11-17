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
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
            
            maskedTextBox2.Text = DateTime.Today.ToString();
            maskedTextBox1.Text = DateTime.Now.AddDays(-30).ToString();
           //maskedTextBox3.Text = abc.ToString();
        }

        private void cadastrosCDSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CadastrosCDS f = new CadastrosCDS();
            f.ShowDialog();
            f.Close();
        }

        private void rEMOVEACETOSEXCELToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ACENTUACAO f = new ACENTUACAO();
            f.ShowDialog();
            f.Close();
        }

        private void domiciliosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            From_Domicilio f = new From_Domicilio();
            this.Hide();
            f.ShowDialog();
            f.Close();
            this.Show();
        }

        private void queryManualToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form_QueryManual f = new Form_QueryManual();
            this.Hide();
            f.ShowDialog();
            f.Close();
            this.Show();
        }

        private void TODOSCADASTROSCDSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormTodosCad f = new FormTodosCad();
            this.Hide();
            f.ShowDialog();
            f.Close();
            this.Show();
        }

        private void cadastroCDSFullToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CadastrosCDScompletos f = new CadastrosCDScompletos();
            this.Hide();
            f.ShowDialog();
            f.Close();
            this.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FormGestantes f = new FormGestantes();
            this.Hide();
            f.ShowDialog();
            f.Close();
            this.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string id_profissional = textBox1.Text;
            DateTime dt1 = DateTime.Parse(maskedTextBox1.Text);
            DateTime dt2 = DateTime.Parse(maskedTextBox2.Text);
            string datainicial = dt1.ToString("yyyy/MM/dd").Replace("/", "");
            string datafinal = dt2.ToString("yyyy/MM/dd").Replace("/", "");         
            
            string condicao = @"tf.co_dim_tempo >= '" + datainicial + "' AND tf.co_dim_tempo <= '" + datafinal + "'";           
            condicao += " order by tf.co_dim_tempo";
            Busca_Atendimentos_Individuais(condicao);
        }
        private void Busca_Atendimentos_Individuais(string condicao)
        {
            try
            {
                string query = @"select
tf.co_dim_tempo as dt_atendimento,
to_char(to_date(tf.co_dim_tempo::text, 'YYYYMMDD'),'dd/mm/yyyy') as dia,
date_part('month',to_date(tf.co_dim_tempo::text, 'YYYYMMDD')) as mes,
cp.no_cidadao as nome,
to_char(tf.dt_nascimento, 'DD/MM/YYYY') as dt_nasc,
date_part('year',age(tf.dt_nascimento)) as idade,
tf.nu_cns as cns,
tf.nu_cpf_cidadao as cpf,
CASE WHEN tf.nu_cns ISNULL THEN tf.nu_cpf_cidadao ELSE tf.nu_cns END cns_cpf,
CASE 
WHEN tf.co_dim_tipo_ficha = 9 THEN 'PEC'
WHEN tf.co_dim_tipo_ficha = 4 THEN 'CDS'
ELSE 'OUTRO'
END as ficha,
CASE 
WHEN tf.co_dim_turno = 2 THEN 'dia'
WHEN tf.co_dim_turno = 3 THEN 'tarde'
ELSE 'noite'
END as turno,
tf.co_dim_profissional_2,
(select tp.no_profissional from tb_dim_profissional tp where tf.co_dim_profissional_1 = tp.co_seq_dim_profissional) as profissional,
(select cbo.no_cbo from tb_dim_cbo cbo where tf.co_dim_cbo_1 = cbo.co_seq_dim_cbo) as cbo,
/*tf.co_dim_unidade_saude_1 as unidade,*/
(select kk.nu_cnes from tb_dim_unidade_saude kk where tf.co_dim_unidade_saude_1 = kk.co_seq_dim_unidade_saude )as unidade,
/*cp.co_dim_unidade_saude_vinc as cunidade,*/
/*cp.co_dim_equipe_vinc as cequipe,*/
/*tf.co_dim_equipe_1 as equipe,*/
(select kk.no_equipe from tb_dim_equipe kk where tf.co_dim_equipe_1 = kk.co_seq_dim_equipe )as equipe,
tf.ds_filtro_cids as cids,
tf.ds_filtro_ciaps as ciaps,
tf.ds_filtro_proced_solicitados as ps,
tf.ds_filtro_proced_avaliados as pa,
tf.co_dim_tempo_dum as dum,
CASE WHEN tf.nu_idade_gestacional_semanas IS NULL THEN '0' ELSE tf.nu_idade_gestacional_semanas END ig,
tf.co_fat_cidadao_pec
FROM tb_fat_atendimento_individual tf
left join tb_fat_cidadao_pec cp on tf.co_fat_cidadao_pec = cp.co_seq_fat_cidadao_pec
where " + condicao;
                var lista_de_atendimentos = new List<Atendimento>();
                Conexao c = new Conexao();
                var dt = c.Get_DataTable(query);
                if (dt == null) { return; }                
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    var a = new Atendimento();
                    a.dia = row["dia"].ToString();
                    DateTime x = DateTime.Parse(a.dia);
                    var culture = new System.Globalization.CultureInfo("pt-br");
                    var day = culture.DateTimeFormat.GetDayName(x.DayOfWeek);
                    a.diaw = day;
                    a.nome = row["nome"].ToString();
                    a.dt_nascimento = row["dt_nasc"].ToString();
                    a.idade = row["idade"].ToString();
                    a.Mes = row["mes"].ToString();
                    a.cns = row["cns"].ToString()+".";
                    a.cpf = row["cpf"].ToString() + ".";
                    a.ficha = row["ficha"].ToString();
                    a.turno = row["turno"].ToString();
                    a.profissional = row["profissional"].ToString();
                    a.cbo = row["cbo"].ToString();
                    a.unidade = row["unidade"].ToString();
                    a.equipe = row["equipe"].ToString();
                    //a.cequipe = row["cequipe"].ToString();
                    //a.cunidade = row["cunidade"].ToString();
                    a.cids = row["cids"].ToString();//.Replace("|", " ");
                    a.ciaps = row["ciaps"].ToString();//.Replace("|", " ");
                    a.ps = row["ps"].ToString();//.Replace("|", " ");
                    a.pa = row["pa"].ToString();//.Replace("|", " ");
                    a.dum = row["dum"].ToString().Replace("30001231", "0");
                    a.ig = row["ig"].ToString();
                    a.co_fat_cidadao_pec = row["co_fat_cidadao_pec"].ToString();
                    a.acs = "";

                    lista_de_atendimentos.Add(a);
                }

                Geral.ToDataTable(lista_de_atendimentos,"Atendimentos");
                MessageBox.Show("Salvo com sucesso. " + lista_de_atendimentos.Count);
            }
            catch (Exception ex)
            { MessageBox.Show(ex.ToString()); }
        }
        private void profissionaisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var f = new FormRisco();
            this.Hide();
            f.ShowDialog();
            f.Close();
            this.Show();
        }

        private void escalaDeTrabalhoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var f = new Historico();
            this.Hide();
            f.ShowDialog();
            f.Close();
            this.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var f = new FormProfissionais();
            this.Hide();
            f.ShowDialog();
            f.Close();
            this.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {           
            CadastrosCDS c = new CadastrosCDS();
            c.Cadastros_CDS(" ");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if((maskedTextBox1.Text.Length == 10) && (maskedTextBox1.Text.Length == 10))
            {
                string dt1 = Geral.Get_DateMaskReverse(maskedTextBox1.Text);
                string dt2 = Geral.Get_DateMaskReverse(maskedTextBox2.Text);
                Vacinacao v = new Vacinacao();
                string complemento = "Where v.co_dim_tempo >= '"+dt1+ "' AND v.co_dim_tempo <= " + dt2 + "";// AND im.nu_identificador = '42'";
                v.BuscaSimples(complemento);              
            }
            
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {

                var c = new Conexao();
                var dt = c.Get_DataTable(textBox1.Text);
                if (dt == null) { return; }                
                Geral.WriteToCsvFile(dt, "busca_manual");
            }
            catch(Exception ex) { MessageBox.Show(ex.ToString()); }
        }

        private void button8_Click(object sender, EventArgs e)
        {

            try
            {
                string dt1 = Geral.Get_DateMaskReverse(maskedTextBox1.Text);
            string dt2 = Geral.Get_DateMaskReverse(maskedTextBox2.Text);
            string complemento = "Where t63.co_dim_tempo >= '" + dt1 + "' AND t63.co_dim_tempo <= " + dt2 + "";
string query = @"select
t63.co_dim_tempo as dt_atendimento,
t55.ds_turno as turno,
to_char(to_date(t63.co_dim_tempo::text, 'YYYYMMDD'),'dd/mm/yyyy') as dia,
date_part('month',to_date(t63.co_dim_tempo::text, 'YYYYMMDD')) as mes,
to_char(t63.dt_nascimento, 'DD/MM/YYYY') as dt_nasc,
date_part('year',age(t63.dt_nascimento)) as idade,
t63.nu_uuid_ficha,
/*t63.co_dim_municipio,*/
t72.no_profissional,
t99.nu_cnes as cnes,
t99.no_unidade_saude as unidade,
t63.co_dim_sexo,
t63.nu_micro_area,
t63.nu_cns,
t63.nu_cpf_cidadao,
t63.co_fat_cidadao_pec,
t63.st_mot_vis_cad_att
from tb_fat_visita_domiciliar t63
left join tb_dim_turno t55 on t63.co_dim_turno = t55.co_seq_dim_turno
left join tb_dim_unidade_saude t99 on t63.co_dim_unidade_saude = t99.co_seq_dim_unidade_saude
left join tb_dim_profissional t72 on t63.co_dim_profissional = t72.co_seq_dim_profissional " + complemento;

            query += " Order by t63.co_dim_tempo";

                var c = new Conexao();
                var dt = c.Get_DataTable(query);
                if (dt == null) { return; }
                Geral.WriteToCsvFile(dt, "VisitaDomiciliar");
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            try
            {
                string query = @"sELECT column_name FROM information_schema.columns where table_name = '"+ textBox1.Text +"'";
                textBox1.Text = "";
                List<string> lista = new List<string>();
                var c = new Conexao();
                var dt = c.Get_DataTable(query);
                if (dt == null) { return; }
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    string abc = row["column_name"].ToString().Replace("\"", "");
                    lista.Add(abc);
                }

                for (int x = 0; x < lista.Count; x++)
                {
                    textBox1.Text += "t987." + lista[x] + "," + Environment.NewLine;

                }
                for (int x = 0; x < lista.Count; x++)
                {
                    textBox1.Text += "CASE WHEN t987." + lista[x] + " IS NULL THEN '0' ELSE t987." + lista[x] + " END," + Environment.NewLine;
                    //CASE WHEN t39.st_prat_saude_prt_corp_atv_fis IS NULL THEN '0' ELSE t39.st_prat_saude_prt_corp_atv_fis,
                }
                for (int x = 0; x < lista.Count; x++)
                {
                    textBox1.Text += "CASE WHEN t987." + lista[x] + " = '1' THEN '1' ELSE '' END "+ lista[x] + "," + Environment.NewLine;
                    
                }
                for (int x = 0; x < lista.Count; x++)
                {
                    textBox1.Text +="public string " + lista[x] +" {get;set;}" + Environment.NewLine;

                }
                for (int x = 0; x < lista.Count; x++)
                {
                    textBox1.Text += "tws." + lista[x] + " = row[\""+ lista[x]+ "\"].ToString();"+ Environment.NewLine;

                }
                for (int x = 0; x < lista.Count; x++)
                {
                    textBox1.Text += "public int " + lista[x] + " {get;set;}" + Environment.NewLine;

                }

                for (int x = 0; x < lista.Count; x++)
                {
                    textBox1.Text += "h..Contains(\"1\")) {" + lista[x] + " {get;set;}" + Environment.NewLine;
                   // if (h.Tipo_Atividade.Contains("Mobiliza")) { mobilizacao++; return; }
                }

            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }

        }

        private void button10_Click(object sender, EventArgs e)
        {
            var f = new FormAtividadesColetivas();
            this.Hide();
            f.ShowDialog();
            f.Close();
            this.Show();
        }

        private void atividadeColetivaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DateTime dt1 = DateTime.Parse(maskedTextBox1.Text);
            DateTime dt2 = DateTime.Parse(maskedTextBox2.Text);
            string datainicial = dt1.ToString("yyyy/MM/dd").Replace("/", "");
            string datafinal = dt2.ToString("yyyy/MM/dd").Replace("/", "");
            var a = new AtividadesColetivas();
            a.Workteste(datainicial, datafinal);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            DateTime dt1 = DateTime.Parse(maskedTextBox1.Text);
            DateTime dt2 = DateTime.Parse(maskedTextBox2.Text);
            string datainicial = dt1.ToString("yyyy/MM/dd").Replace("/", "");
            string datafinal = dt2.ToString("yyyy/MM/dd").Replace("/", "");
            ProcedimentosIndividualizados p = new ProcedimentosIndividualizados();
            string complemento = " Where tf.co_dim_tempo >= '" + datainicial + "' AND tf.co_dim_tempo <= " + datafinal + " ";
            p.BuscaSimples(complemento);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            DateTime dt1 = DateTime.Parse(maskedTextBox1.Text);
            DateTime dt2 = DateTime.Parse(maskedTextBox2.Text);
            string datainicial = dt1.ToString("yyyy/MM/dd").Replace("/", "");
            string datafinal = dt2.ToString("yyyy/MM/dd").Replace("/", "");
            Odonto o = new Odonto();
            string complemento = " Where tf.co_dim_tempo >= '" + datainicial + "' AND tf.co_dim_tempo <= " + datafinal + " ";
            o.BuscaSimples(complemento);
        }

        private void vacinacaoI5ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string dt1 = Geral.Get_DateMaskReverse(maskedTextBox1.Text);
            string dt2 = Geral.Get_DateMaskReverse(maskedTextBox2.Text);
            Vacinacao v = new Vacinacao();
            string complemento = "Where v.co_dim_tempo >= '" + dt1 + "' AND v.co_dim_tempo <= " + dt2 + "";// AND im.nu_identificador = '42'";
            v.Busca(complemento);
        }
    }
}
