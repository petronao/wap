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
    public partial class Historico : Form
    {

        public Historico()
        {
            InitializeComponent();
        }

        private string Query(string condicao)
        {
            string texto = @"select
--tf.co_dim_tempo as dt_atendimento,
to_char(to_date(tf.co_dim_tempo::text, 'YYYYMMDD'),'dd/mm/yyyy') as dia,
--cp.no_cidadao as nome,
--to_char(tf.dt_nascimento, 'DD/MM/YYYY') as dt_nasc,
/*date_part('year',age(tf.dt_nascimento)) as idade,*/
/*tf.nu_cns as cns,
--tf.nu_cpf_cidadao as cpf,*/
--CASE WHEN tf.nu_cns ISNULL THEN tf.nu_cpf_cidadao ELSE tf.nu_cns END cns_cpf,
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
tf.co_dim_profissional_2 as p2,
(select tp.no_profissional from tb_dim_profissional tp where tf.co_dim_profissional_1 = tp.co_seq_dim_profissional) as p1,
(select substring(cbo.no_cbo,0,10) from tb_dim_cbo cbo where tf.co_dim_cbo_1 = cbo.co_seq_dim_cbo) as cbo,
/*tf.co_dim_unidade_saude_1 as unidade,*/
(select kk.nu_cnes from tb_dim_unidade_saude kk where tf.co_dim_unidade_saude_1 = kk.co_seq_dim_unidade_saude )as unidade,
/*cp.co_dim_unidade_saude_vinc as cunidade,*/
/*cp.co_dim_equipe_vinc as cequipe,*/
/*tf.co_dim_equipe_1 as equipe,*/
(select kk.no_equipe from tb_dim_equipe kk where tf.co_dim_equipe_1 = kk.co_seq_dim_equipe )as equipe
--tf.ds_filtro_cids as cids,
--tf.ds_filtro_ciaps as ciaps,
--tf.ds_filtro_proced_solicitados as ps,
--tf.ds_filtro_proced_avaliados as pa,
--tf.co_dim_tempo_dum as dum,
--CASE WHEN tf.nu_idade_gestacional_semanas IS NULL THEN '0' ELSE tf.nu_idade_gestacional_semanas END ig,
--tf.co_fat_cidadao_pec
FROM tb_fat_atendimento_individual tf
left join tb_fat_cidadao_pec cp on tf.co_fat_cidadao_pec = cp.co_seq_fat_cidadao_pec
where " + condicao;

            return texto;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Busca_Historico();
        }

        private void Busca_Historico()
        {
            try
            {
                string fat = textBox2.Text.Trim();
                string cns = textBox3.Text.Trim();

                string condicao = "";

                if (fat.Length > 0)
                {
                    condicao = @"tf.co_fat_cidadao_pec ='" + fat + "'";

                }
                if (cns.Length == 15)
                {
                    // condicao = @"tf.nu_cns LIKE '%" + cns + "%'";
                    condicao = @"tf.nu_cns = '" + cns + "'";
                }

                //condicao += " order by tf.co_dim_tempo desc limit 10";
                Conexao c = new Conexao();
                var dt = c.Get_DataTable(Query(condicao));
                if (dt == null) { return; }
                dataGridView2.DataSource = dt;
                Busca_cadastro(condicao);
            }
            catch (Exception ex)
            { MessageBox.Show(ex.ToString()); }


        }
        private void Busca_cadastro(string condicao)
        {
            try
            {
                string texto = @"select
to_char(t1.dt_cad_individual, 'DD/MM/YYYY') as dt_cad,
substring(t1.no_cidadao,0,15) as nome,
/*t3.nu_cns as cns,*/
CASE WHEN t2.nu_cns = '0' THEN t2.nu_cpf_cidadao ELSE t2.nu_cns END cns_cpf,
CASE WHEN t2.st_hipertensao_arterial = 1 THEN 'HAS' ELSE '' END has,
CASE WHEN t2.st_diabete = 1 THEN 'DM' ELSE '' END dm,
CASE WHEN t2.st_fumante = 1 THEN 'TAB' ELSE '' END tab,
CASE WHEN t2.st_alcool = 1 THEN 'ALC' ELSE '' END alc,
CASE WHEN t2.st_gestante = 1 THEN 'GES' ELSE '' END ges,
t99.nu_cnes as cnes,
CASE WHEN t2.co_dim_tipo_saida_cadastro = 1  THEN 'OBITO' WHEN t2.co_dim_tipo_saida_cadastro = 2  THEN 'MUDOU' ELSE '' END saida,
t2.nu_micro_area as mc,
(select tp.no_profissional from tb_dim_profissional tp where t2.co_dim_profissional = tp.co_seq_dim_profissional) as acs
from
tb_fat_cad_individual t2 
left join tb_cds_cad_individual t1 on t1.co_unico_ficha = t2.nu_uuid_ficha
left join tb_fat_cidadao_pec t3 on t2.co_fat_cidadao_pec = t3.co_seq_fat_cidadao_pec
left join tb_dim_unidade_saude t99 on t2.co_dim_unidade_saude = t99.co_seq_dim_unidade_saude
left join tb_dim_equipe t98 on t2.co_dim_equipe = t98.co_seq_dim_equipe
where ";
                texto += condicao;

                Conexao c = new Conexao();
                var dt = c.Get_DataTable(texto);
                if (dt == null) { return; }
                dataGridView3.DataSource = dt;
            }
            catch (Exception ex)
            { MessageBox.Show(ex.ToString()); }

        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {

            textBox3.Text = "";
            if (e.KeyCode == Keys.Enter)
            {
                Busca_Historico();
            }
        }

        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            textBox2.Text = "";
            if (e.KeyCode == Keys.Enter)
            {
                Busca_Historico();
            }
        }


        //buscar dados do paciente...
        private void button1_Click(object sender, EventArgs e)
        {
            string cnscpf = textBox4.Text.Replace(".", "").Replace("-", "").Trim();
            string complemento = "";
            if (cnscpf.Length == 15)
            {
                complemento = @" AND (t3.nu_cns ='" + cnscpf + "')";
            }
            else if (cnscpf.Length == 11)
            {
                complemento = @" AND (t3.nu_cpf_cidadao ='" + cnscpf + "')";
            }

            else { }

            Inicio(complemento);
        }
        private void Inicio(string complemento)
        {
            string texto = Geral.BuscaCadastrosCDS();
            texto += " Where (t1.st_versao_atual = 1) AND (t1.st_ficha_inativa = 0)";
            texto += complemento;
            var c = new Conexao();
            var dt = c.Get_DataTable(texto);
            if (dt == null) { return; }

            foreach (System.Data.DataRow dr in dt.Rows)
            {

                textBox1.Text += dr["dt_cad"].ToString() + Environment.NewLine;
                textBox1.Text += dr["nome"].ToString().ToUpper() + Environment.NewLine;
                textBox1.Text += dr["sexo"].ToString() + Environment.NewLine;
                textBox1.Text += dr["dt_nasc"].ToString() + Environment.NewLine;
                textBox1.Text += dr["idade"].ToString() + Environment.NewLine;
                textBox1.Text += "CDS CNS "+ dr["cns"].ToString() + Environment.NewLine;
                textBox1.Text += "FAT CNS " + dr["fatcns"].ToString() + Environment.NewLine;
                textBox1.Text += "CPF "+ dr["fatcpf"].ToString() + Environment.NewLine;
                textBox1.Text +="CDS ATUAL " + dr["cns_cpf"].ToString() + Environment.NewLine;
                textBox1.Text += dr["cnes"].ToString() + Environment.NewLine;
                textBox1.Text += dr["mc"].ToString() + Environment.NewLine;
                textBox1.Text += dr["acs"].ToString() + Environment.NewLine;
                textBox1.Text += dr["co_fat_cidadao_pec"].ToString() + Environment.NewLine;
                textBox1.Text += dr["responsavel"].ToString() + Environment.NewLine;
                textBox1.Text += dr["cpf_cns_responsavel"].ToString() + Environment.NewLine;
                textBox1.Text += dr["saida"].ToString() + Environment.NewLine;
            }
        }
    }
}
