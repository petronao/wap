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
    public partial class CadastrosCDScompletos : Form
    {

        List<Pessoa> lista_cadastros;
        public CadastrosCDScompletos()
        {
            InitializeComponent();
            lista_cadastros = new List<Pessoa>();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count > 0) { dataGridView1.DataSource = null; dataGridView1.Rows.Clear(); dataGridView1.Refresh(); }
            if (lista_cadastros.Count > 0) lista_cadastros.Clear();
            Conexao c = new Conexao();
            var drx = c.Get_DataTable(Geral.query10xxxxxxxx());
            if (drx == null) { MessageBox.Show("tabela null"); return; }
            foreach (DataRow dr in drx.Rows)            
                {
                Pessoa h = new Pessoa();
                h.data_cadastro = dr["dt_cad"].ToString();
                string nome = dr["nome"].ToString().ToUpper();
                h.nome = Geral.removerAcentos(nome);
                h.sexo = dr["sexo"].ToString();
                h.nascimento = dr["dt_nasc"].ToString();
                h.idade = Convert.ToByte(dr["idade"].ToString());
               // h.fatcns = dr["fatcns"].ToString();
                //h.fatcpf = dr["fatcpf"].ToString();
               // h.cns = dr["cns"].ToString();
               // h.cpf = dr["cpf"].ToString();
                h.cnes = dr["cnes"].ToString();
                h.mc = dr["mc"].ToString();
                //h.FA = dr["fa"].ToString();
                h.has = dr["has"].ToString();
                h.dm = dr["dm"].ToString();
                h.tab = dr["tab"].ToString();
                h.alc = dr["alc"].ToString();
                h.ges = dr["ges"].ToString();
               // h.unidade = dr["unidade"].ToString();
                h.equipe_vinc = dr["equipe_vinc"].ToString();
                h.acs = dr["acs"].ToString();
              //  h.faleceu = dr["st_faleceu"].ToString();
                h.co_fat_cidadao_pec = dr["co_fat_cidadao_pec"].ToString();
                h.co_dim_raca_cor = dr["co_dim_raca_cor"].ToString();
               // h.nu_cns_responsavel = dr["nu_cns_responsavel"].ToString();
               // h.nu_cpf_responsavel = dr["nu_cpf_responsavel"].ToString();
                h.responsavel = dr["responsavel"].ToString();
                
                h.st_deficiencia = dr["deficiencia"].ToString();
                
                    
                    h.auditiva = dr["auditiva"].ToString();
                    h.intelectual= dr["intelectual"].ToString();
                    h.outra= dr["outra"].ToString();
                    h.visual = dr["visual"].ToString();
                    h.fisica = dr["fisica"].ToString();                   
                
                h.nu_celular_cidadao = dr["nu_celular_cidadao"].ToString();
                lista_cadastros.Add(h);
            }

            dataGridView1.DataSource = lista_cadastros;
            label1.Text = "Total " + dataGridView1.Rows.Count;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count > 0) { dataGridView1.DataSource = null; dataGridView1.Rows.Clear(); dataGridView1.Refresh(); }
            string query = @"select    
t3.co_fat_cad_individual,
t3.nu_uuid_ficha,
t3.nu_cns,
t3.nu_cpf_cidadao,
t3.st_ficha_inativa,
t6.nu_area,t6.nu_micro_area,
t6.dt_atualizado,
t6.st_registro_cadsus,
t6.nu_cpf,t6.nu_cns,
t6.no_cidadao,
t6.ds_cep,
t6.ds_logradouro,
t6.nu_numero,
t6.no_bairro,
t6.st_ativo_para_exibicao          
from 
tb_fat_cidadao t3
left join
tb_cidadao t6
on t3.nu_uuid_ficha = t6.co_unico_cidadao
WHERE (t3.st_ficha_inativa = 0)";
        }
    }
}
