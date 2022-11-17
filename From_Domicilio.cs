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
    public partial class From_Domicilio : Form
    {
        Dictionary<string,Domicilio> Lista_Domicilios = new Dictionary<string, Domicilio>();
        public From_Domicilio()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string query = @"select
t3.co_unico_domicilio as unico,
to_char(t3.dt_cad_domiciliar, 'DD/MM/YYYY') as dt,
t3.no_bairro , 
(select k.ds_tipo_logradouro from tb_dim_tipo_logradouro k where t2.co_dim_tipo_logradouro = k.co_seq_dim_tipo_logradouro) t,
t3.no_logradouro,
t3.nu_domicilio,
CASE WHEN t3.st_versao_atual = '1' THEN 'atual' END AS atual,
CASE WHEN t3.st_envio = '1' THEN 'envio' END AS envio,
CASE WHEN t5.st_mudou = '1' THEN 'mudou' END AS mudou,
t5.nu_cns_responsavel,
t5.nu_cpf_responsavel,
t2.co_dim_unidade_saude as ubs,
t2.nu_micro_area,
(select tp.no_profissional from tb_dim_profissional tp where t2.co_dim_profissional = tp.co_seq_dim_profissional) as prof
from tb_cds_cad_domiciliar t3
left join tb_fat_cad_domiciliar t2 on t2.nu_uuid_ficha = t3.co_unico_ficha
left join tb_fat_cad_dom_familia t5 on t2.co_seq_fat_cad_domiciliar = t5.co_fat_cad_domiciliar
WHERE t3.st_versao_atual = '1'
order by t3.co_unico_domicilio asc, t3.dt_cad_domiciliar desc";

            Lista_Domicilios.Clear();
            if (dataGridView1.Rows.Count > 0)
            {
                dataGridView1.DataSource = null; dataGridView1.Refresh(); dataGridView1.Rows.Clear();
            }
            Conexao c = new Conexao();
            var dt= c.Get_DataTable(query);
                dataGridView1.DataSource =dt;
                label1.Text = "" + dataGridView1.Rows.Count;

        }


        private string QUERYBUGGGGGGGGGGGG()
        {
            // estas duas opção ainda não sei a função delas...
            // t8.st_responsavel_ainda_reside MESMO = 0  existe o cadastro sim. estranho
	        //t8.st_domicilio_ativo mesmo = 0 existe o cadastro sim..

            // usando string no datagrid não o sort não funciona a data...
            string texto = @"select
        /*to_char(t3.dt_cad_domiciliar, 'DD/MM/YYYY') as dt,*/
    t3.dt_cad_domiciliar as dt,
    (select k.ds_tipo_logradouro from tb_dim_tipo_logradouro k where t2.co_dim_tipo_logradouro = k.co_seq_dim_tipo_logradouro) t,
    t3.no_bairro,
    t3.no_logradouro,
    t3.nu_domicilio,
    t3.nu_micro_area,    
    t8.co_cds_domicilio,
    CASE WHEN t3.st_versao_atual = '1' THEN 'atual' END AS atual,
    CASE WHEN t3.st_envio = '1' THEN 'envio' END AS envio,
	t8.nu_prontuario_familiar as prontuario,
	t8.nu_cpf_cns_responsavel,
    t8.st_responsavel_cadastrado,
    t8.st_responsavel_declarado,
	t8.st_responsavel_ainda_reside,
	t8.st_domicilio_ativo
    from tb_cds_cad_domiciliar t3
    left join tb_fat_cad_domiciliar t2 on t2.nu_uuid_ficha = t3.co_unico_ficha
        left join tb_cds_domicilio t4 on t3.co_unico_domicilio = t4.co_unico_domicilio
        left join tb_familia t8 on t4.co_seq_cds_domicilio = t8.co_cds_domicilio
        where (t3.st_versao_atual = 1)
            order by t3.dt_cad_domiciliar desc";
            return texto;

        }

        private string query_teste()
        {
            string text = @"select
t3.co_unico_domicilio as unico,
to_char(t3.dt_cad_domiciliar, 'DD/MM/YYYY') as dt,
t3.no_bairro as bairro, 
(select k.ds_tipo_logradouro from tb_dim_tipo_logradouro k where t2.co_dim_tipo_logradouro = k.co_seq_dim_tipo_logradouro) t,
t3.no_logradouro as rua,
t3.nu_domicilio,
CASE WHEN t3.st_versao_atual = '1' THEN 'atual' END AS atual,
CASE WHEN t3.st_ficha = 1 THEN 'envio' END AS envio,
CASE WHEN t5.st_mudou = 1 THEN 'mudou' END AS mudou,
t5.nu_cns_responsavel as cns,
t5.nu_cpf_responsavel as cpf,
t2.co_dim_unidade_saude as ubs,
t2.nu_micro_area as mc,
(select tp.no_profissional from tb_dim_profissional tp where t2.co_dim_profissional = tp.co_seq_dim_profissional) as prof
from tb_cds_cad_domiciliar t3
left
join tb_fat_cad_domiciliar t2 on t2.nu_uuid_ficha = t3.co_unico_ficha
left
join tb_fat_cad_dom_familia t5 on t2.co_seq_fat_cad_domiciliar = t5.co_fat_cad_domiciliar
order by t3.co_unico_domicilio asc, t3.dt_cad_domiciliar asc";
            return text;

        }
        private string query_todas_fichas_Versao()
        {
            string text = @"select
t3.co_unico_domicilio as unico,
to_char(t3.dt_cad_domiciliar, 'DD/MM/YYYY') as dt,
t3.no_bairro as bairro, 
(select k.ds_tipo_logradouro from tb_dim_tipo_logradouro k where t2.co_dim_tipo_logradouro = k.co_seq_dim_tipo_logradouro) t,
t3.no_logradouro as rua,
t3.nu_domicilio,
CASE WHEN t3.st_versao_atual = '1' THEN 'atual' END AS atual,
CASE WHEN t3.st_ficha = 1 THEN 'envio' END AS envio,
CASE WHEN t5.st_mudou = 1 THEN 'mudou' END AS mudou,
t5.nu_cns_responsavel as cns,
t5.nu_cpf_responsavel as cpf,
t2.co_dim_unidade_saude as ubs,
t2.nu_micro_area as mc,
(select tp.no_profissional from tb_dim_profissional tp where t2.co_dim_profissional = tp.co_seq_dim_profissional) as prof
from tb_cds_cad_domiciliar t3
left join tb_fat_cad_domiciliar t2 on t2.nu_uuid_ficha = t3.co_unico_ficha
left join tb_fat_cad_dom_familia t5 on t2.co_seq_fat_cad_domiciliar = t5.co_fat_cad_domiciliar
order by t3.co_unico_domicilio asc, t3.dt_cad_domiciliar desc";
            return text;

        }
        private void button2_Click(object sender, EventArgs e)
        {
            
            if (dataGridView1.Rows.Count > 0) { dataGridView1.DataSource = null; dataGridView1.Refresh(); dataGridView1.Rows.Clear(); }
            if (dataGridView2.Rows.Count > 0)
            {
                dataGridView2.DataSource = null; dataGridView2.Refresh(); dataGridView2.Rows.Clear();
            }
            if(Lista_Domicilios.Count > 0) Lista_Domicilios.Clear();

            Conexao c = new Conexao();            
            var dt = c.Get_DataTable(query_todas_fichas_Versao());             
            Geral.WriteToCsvFile(dt, "todas_FCD.csv");
            dataGridView1.DataSource = dt; label1.Text = "" + dataGridView1.RowCount;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //procura cns ou cpf
            dataGridView1.ClearSelection();// limpa a seleção
            String searchValue = textBox1.Text;
            int rowIndex = -1;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells[9].Value.ToString().Equals(searchValue))
                {
                    rowIndex = row.Index;
                    break;
                }
            }
            dataGridView1.Rows[rowIndex].Selected = true;
            textBox1.Text = "";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            /// TEMOS QUE USAR APENAS OS CADASTROS ATUAIS... QUE SÃO OS PROCESSADOS TABELA FAT
            /// para evitar bugggggss.. 
            /// 
            string query = @"select
t3.co_unico_domicilio as unico,
/*to_char(t3.dt_cad_domiciliar, 'DD/MM/YYYY')*/ t3.dt_cad_domiciliar as dt,
t3.no_bairro , 
(select k.ds_tipo_logradouro from tb_dim_tipo_logradouro k where t2.co_dim_tipo_logradouro = k.co_seq_dim_tipo_logradouro) t,
t3.no_logradouro,
t3.nu_domicilio,
CASE WHEN t3.st_versao_atual = '1' THEN 'atual' END AS atual,
CASE WHEN t3.st_ficha = '1' THEN 'envio' END AS envio,
CASE WHEN t5.st_mudou = '1' THEN 'mudou' END AS mudou,
t5.nu_cns_responsavel,
t5.nu_cpf_responsavel,
t2.co_dim_unidade_saude as ubs,
t2.nu_micro_area,
(select tp.no_profissional from tb_dim_profissional tp where t2.co_dim_profissional = tp.co_seq_dim_profissional) as prof,
t2.co_seq_fat_cad_domiciliar as t2cod,
t5.co_fat_cad_domiciliar as t5cod
from tb_fat_cad_domiciliar t2
left join tb_cds_cad_domiciliar t3 on t2.nu_uuid_ficha = t3.co_unico_ficha
left join tb_fat_cad_dom_familia t5 on t2.co_seq_fat_cad_domiciliar = t5.co_fat_cad_domiciliar
WHERE t3.st_versao_atual = '1'
order by t3.co_unico_domicilio asc, t3.dt_cad_domiciliar desc";
            
            if (dataGridView1.Rows.Count > 0)
            {
                dataGridView1.DataSource = null; dataGridView1.Refresh(); dataGridView1.Rows.Clear();
            }
            Conexao c = new Conexao();
            var dt = c.Get_DataTable(query);
            dataGridView1.DataSource = dt;
            label1.Text = "" + dataGridView1.Rows.Count;
        }

        
        private void button5_Click_1(object sender, EventArgs e)
        {
            List<Domicilio> lista_domicilios = new List<Domicilio>();
            if (dataGridView2.Rows.Count > 0)
            {
                dataGridView2.DataSource = null; dataGridView2.Refresh(); dataGridView2.Rows.Clear();
            }
            Conexao c = new Conexao();
            var dt = c.Get_DataTable(query_teste());
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Domicilio d = new Domicilio();
                    d.co_unico_domicilio = dr["unico"].ToString();
                    d.dia = dr["dt"].ToString();
                    string endereco = dr["bairro"].ToString();
                    endereco += " ";
                    endereco += dr["rua"].ToString();
                    endereco += " ";
                    endereco += dr["nu_domicilio"].ToString();
                    d.endereco = endereco;
                    d.atual = dr["atual"].ToString();
                    string cns = dr["cns"].ToString();
                    string cpf = dr["cpf"].ToString();
                    if (cns.Length == 15) { d.cns_cpf = cns; }
                    else if (cpf.Length == 11) { d.cns_cpf = cpf; }
                    else { d.cns_cpf = "0"; }
                    // d.enviou = dr["envio"].ToString();
                    d.mudou = dr["mudou"].ToString();
                    d.ubs = dr["ubs"].ToString();
                    d.mc = dr["mc"].ToString();
                    d.profissional = dr["prof"].ToString();

                   // if(d.cns_cpf == "209976091460003") 
                   // { 
                   // }
                    if (d.mudou == "mudou")
                    {
                    Found:
                        for (int x = 0; x < lista_domicilios.Count; x++)
                        {
                            if (lista_domicilios[x].co_unico_domicilio == d.co_unico_domicilio)
                            {
                                lista_domicilios[x].endereco = d.endereco; // atualizar o endreço
                                lista_domicilios[x].dia = d.dia;// dia
                            }
                            if ((lista_domicilios[x].co_unico_domicilio == d.co_unico_domicilio)
                                && (lista_domicilios[x].cns_cpf == d.cns_cpf)
                                && (lista_domicilios[x].ubs == d.ubs))
                            {
                                lista_domicilios.Remove(lista_domicilios[x]);
                                goto Found;
                            }
                        }
                    }
                    else
                    {
                        lista_domicilios.Add(d);
                    }
                    
                }
            }
            else { MessageBox.Show("Nada ou erro!"); }

            dataGridView2.DataSource = lista_domicilios;

        }
        private void nada()
        {
            Dictionary<string, box> listabox = new Dictionary<string, box>();
            if (dataGridView2.Rows.Count > 0)
            {
                dataGridView2.DataSource = null; dataGridView2.Refresh(); dataGridView2.Rows.Clear();
            }
            Conexao c = new Conexao();
            var dt = c.Get_DataTable(query_teste());
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Chefe d = new Chefe();
                    string co_unico_domicilio = dr["unico"].ToString();
                    d.data = dr["dt"].ToString();
                    string endereco = dr["bairro"].ToString();
                    endereco += " ";
                    endereco += dr["rua"].ToString();
                    endereco += " ";
                    endereco += dr["nu_domicilio"].ToString();
                    // d.atual = dr["atual"].ToString();
                    string cns = dr["cns"].ToString();
                    string cpf = dr["cpf"].ToString();
                    if (cns.Length == 15) { d.cns_cpf = cns; }
                    else if (cpf.Length == 11) { d.cns_cpf = cpf; }
                    else { d.cns_cpf = "0"; }
                    // d.enviou = dr["envio"].ToString();
                    d.mudou = dr["mudou"].ToString();
                    d.ubs = dr["ubs"].ToString();
                    d.mc = dr["mc"].ToString();
                    //d.profissional = dr["prof"].ToString();


                    if (listabox.ContainsKey(co_unico_domicilio))
                    {
                        //atualiza os dados do domicilio
                        listabox[co_unico_domicilio].data = d.data;
                        listabox[co_unico_domicilio].endereco = endereco;
                        listabox[co_unico_domicilio].atualizacoes++;
                        listabox[co_unico_domicilio].adicionarChefe(d); // check

                    }
                    else
                    {
                        box b = new box();
                        b.data = d.data;
                        b.endereco = endereco;
                        b.atualizacoes = 1;
                        listabox.Add(co_unico_domicilio, b);
                        listabox[co_unico_domicilio].adicionarChefe(d);
                    }


                }
            }
            else { MessageBox.Show("Nada ou erro!"); }

            dataGridView2.DataSource = listabox.Values.ToList();

        }
    }

    public class box
    {
        public string data { get; set; }
        public string endereco { get; set; }          
        public int atualizacoes { get; set; }

        public List<Chefe> lista_Chefes = new List<Chefe>();
        public int total_fichas { get { return lista_Chefes.Count(); } }

        public void adicionarChefe(Chefe t)
        {
            if(t.cns_cpf =="0")
            {
            // nao adiciona
            }
            else if(t.mudou =="mudou")
            {
                if (lista_Chefes.Count > 0)
                {
                    for (int x = 0; x < lista_Chefes.Count; x++)
                    {
                        if ((lista_Chefes[x].cns_cpf == t.cns_cpf) && (lista_Chefes[x].mc == t.mc) && (lista_Chefes[x].ubs == t.ubs))
                        {
                            lista_Chefes.Remove(lista_Chefes[x]);
                        }
                    }
                }
            }
            else 
            {
                if (lista_Chefes.Count > 0)
                {
                    for (int x = 0; x < lista_Chefes.Count; x++)
                    {
                        if ((lista_Chefes[x].cns_cpf == t.cns_cpf) && (lista_Chefes[x].mc == t.mc) && (lista_Chefes[x].ubs == t.ubs))
                        {
                            lista_Chefes[x].data = t.data; // atualizar só a data
                            break;
                        }
                        
                    }

                  
                        lista_Chefes.Add(t);
                    
                }
                else
                {
                    lista_Chefes.Add(t);

                }
            }
        }
    }
    public class Chefe
    {        
        public string data { get; set; }
        public string cns_cpf { get; set; }
        public string ubs { get; set; }
        public string mudou { get; set; }
        public string mc { get; set; }
    }
    public class Domicilio
    {        
        public string dia { get; set; }
        //public string co_unico_ficha { get; set; }
        public string co_unico_domicilio { get; set; }       
        public string endereco { get; set; }
        public string cns_cpf { get; set; }        
        public string prontuario { get; set; }
        public string enviou { get; set; }
        public string mc { get; set; }
        public string atual { get; set; }
        public string profissional { get; set; }
        public string mudou { get; set; }
        public string ubs { get; set; }
        public int total_chefes { get { return lista_chefes.Count(); } }

        public List<Responsavel> lista_chefes = new List<Responsavel>();

        public void adicionar_chefe(string ubs, string cns_cpf,string mc,string mudou)
        {
            if (cns_cpf == "0")
            { return; }
            else
            {
                if (mudou == "mudou")
                {
                    foreach (Responsavel g in lista_chefes)
                    {
                        if ((g.cns_cpf == cns_cpf) && (g.ubs == ubs) && (g.mc == mc))
                        {
                            lista_chefes.Remove(g);
                            return;
                        }

                    }
                }

                Responsavel r = new Responsavel();
                r.ubs = ubs;
                r.cns_cpf = cns_cpf;
                r.mc = mc;
                lista_chefes.Add(r);
            }

        }
    }

    public class Responsavel
    {        
        public string ubs { get; set; }
        public string cns_cpf { get; set; }
        public string mc { get; set; }
    }
}
