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
    public partial class FormProfissionais : Form
    {
        public FormProfissionais()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Conexao c = new Conexao();
                var dt = c.Get_DataTable(Busca());
                dataGridView1.DataSource = dt;
                Geral.WriteToCsvFile(dt, "funcionarios.csv");
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }
        private string Busca()
        {

            string textoorig = @"select count(distinct profission0_.co_seq_prof) as col_0_0_ 
from tb_prof profission0_ 
left outer join (tb_lotacao lotacao1_ inner join tb_ator_papel lotacao1_1_ 
on lotacao1_.co_ator_papel=lotacao1_1_.co_seq_ator_papel) 
on (profission0_.co_seq_prof=lotacao1_.co_prof) 
where (lotacao1_1_.st_ativo= 1 or lotacao1_.co_ator_papel is null) 
and (profission0_.no_profissional_filtro like '%valdivino%' escape '!')";
            string query = @"
            select
distinct t01.no_profissional,
t01.nu_cns,
t01.nu_cpf,
to_char(tt.dt_registro, 'DD/MM/YYYY') as dt_nasc,
tt.nu_mes
from tb_prof t01
left join tb_fat_cidadao_pec fp on t01.nu_cns = fp.nu_cns
/*left join tb_fat_cidadao_pec fp on t01.nu_cpf = fp.nu_cpf_cidadao*/
left join tb_dim_tempo tt on  fp.co_dim_tempo_nascimento = tt.co_seq_dim_tempo
left outer join (tb_lotacao lotacao1_ inner join tb_ator_papel lotacao1_1_ 
on lotacao1_.co_ator_papel=lotacao1_1_.co_seq_ator_papel) 
on (t01.co_seq_prof=lotacao1_.co_prof) 
where (lotacao1_1_.st_ativo= 1 or lotacao1_.co_ator_papel is null)
order by tt.nu_mes, dt_nasc";


            string texto = @"
            select
distinct t01.no_profissional,
t01.nu_cns,
t01.nu_cpf,
to_char(tt.dt_registro, 'DD/MM/YYYY') as dt_nasc,
tt.nu_mes
from tb_prof t01
left join tb_fat_cidadao_pec fp on t01.nu_cns = fp.nu_cns
left join tb_dim_tempo tt on  fp.co_dim_tempo_nascimento = tt.co_seq_dim_tempo
left outer join (tb_lotacao lotacao1_ inner join tb_ator_papel lotacao1_1_ 
on lotacao1_.co_ator_papel=lotacao1_1_.co_seq_ator_papel) 
on (t01.co_seq_prof=lotacao1_.co_prof)
order by tt.nu_mes, dt_nasc";
            return texto;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                string query = @"
            select
distinct t01.no_profissional,
t01.nu_cns,
to_char(fp.dt_nascimento,'DD/MM/YYYY') as dt_nasc,
to_char(fp.dt_nascimento, 'DD')::integer as dia,
to_char(fp.dt_nascimento, 'MM')::integer as mes
from tb_prof t01
left join tb_cidadao fp on t01.nu_cns = fp.nu_cns
order by mes , dia";

                Conexao c = new Conexao();
                var dt = c.Get_DataTable(query);
                dataGridView1.DataSource = dt;
                Geral.WriteToCsvFile(dt, "funcionarios.csv");
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }
    }
}
