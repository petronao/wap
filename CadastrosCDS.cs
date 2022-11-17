using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class CadastrosCDS : Form
    {        
        string conexaoString;
        NpgsqlConnection conn; 
        Dictionary<string,Pessoa> Lista_Cadastros_individuais = new Dictionary<string, Pessoa>();
        Dictionary<string, Filtro_1> Filtro_Situacao;
        public CadastrosCDS()
        {
            InitializeComponent();
            var conexao = new Conexao();
            conexaoString = conexao.cs;          
        }   
        

        private void Get_Familias(string area, string cnes)
        {
            string ine = "0";           

            if (cnes == "2186306")
            {                
                ine = "0000270563";
            }
            else if (cnes == "2186314")
            {                
                ine = "0000270571";
            }

            else
            { return; }



            string query = @"select 
tb_fat_cad_domiciliar.nu_micro_area,
tb_fat_familia_territorio.nu_prontuario,
tb_fat_cidadao_pec.nu_cns,
tb_fat_cidadao_pec.nu_cpf_cidadao,
tb_fat_cidadao_pec.no_cidadao,
tb_cds_cad_domiciliar.no_bairro, 
tb_cds_cad_domiciliar.no_logradouro, 
tb_dim_tipo_logradouro.ds_tipo_logradouro,
tb_fat_cidadao_pec.nu_telefone_celular, 
tb_fat_cidadao_pec.co_dim_tempo_nascimento,
tb_fat_cidadao_pec.co_dim_sexo, 
tbDimUnidadeSaudeVinculada.nu_cnes, 
tbDimUnidadeSaudeVinculada.no_unidade_saude, 
tbDimEquipeVinculada.nu_ine,
tbDimEquipeVinculada.no_equipe,
tb_fat_cidadao_pec.co_seq_fat_cidadao_pec
    from tb_fat_cad_domiciliar tb_fat_cad_domiciliar
    left join tb_cds_cad_domiciliar tb_cds_cad_domiciliar
    on tb_fat_cad_domiciliar.nu_uuid_ficha = tb_cds_cad_domiciliar.co_unico_ficha
    left join tb_fat_familia_territorio tb_fat_familia_territorio on tb_fat_familia_territorio.co_fat_cad_domiciliar = tb_fat_cad_domiciliar.co_seq_fat_cad_domiciliar
    and tb_fat_familia_territorio.st_familia_consistente = 1
    left join tb_dim_equipe tb_dim_equipe
    on tb_dim_equipe.co_seq_dim_equipe = tb_fat_cad_domiciliar.co_dim_equipe
    left join tb_dim_unidade_saude tb_dim_unidade_saude
    on tb_dim_unidade_saude.co_seq_dim_unidade_saude = tb_fat_cad_domiciliar.co_dim_unidade_saude
    left join tb_dim_tipo_imovel tb_dim_tipo_imovel
    on tb_dim_tipo_imovel.co_seq_dim_tipo_imovel = tb_fat_cad_domiciliar.co_dim_tipo_imovel
    left join tb_dim_tipo_logradouro tb_dim_tipo_logradouro
    on tb_dim_tipo_logradouro.co_seq_dim_tipo_logradouro = tb_fat_cad_domiciliar.co_dim_tipo_logradouro
    left join tb_fat_cidadao_territorio tb_fat_cidadao_territorio
    on tb_fat_familia_territorio.co_seq_fat_familia_territorio = tb_fat_cidadao_territorio.co_fat_familia_territorio
    and tb_fat_cidadao_territorio.st_cidadao_consistente = 1
    left join tb_fat_cidadao_pec tb_fat_cidadao_pec
    on tb_fat_cidadao_territorio.co_fat_cidadao_pec = tb_fat_cidadao_pec.co_seq_fat_cidadao_pec
    left join tb_dim_unidade_saude tbDimUnidadeSaudeVinculada
    on tbDimUnidadeSaudeVinculada.co_seq_dim_unidade_saude = tb_fat_cidadao_pec.co_dim_unidade_saude_vinc
    left join tb_dim_equipe tbDimEquipeVinculada
    on tbDimEquipeVinculada.co_seq_dim_equipe = tb_fat_cidadao_pec.co_dim_equipe_vinc
    left join tb_fat_cidadao_territorio tbFatCidadaoTerritorioResponsv
    on tb_fat_cidadao_territorio.co_fat_ciddo_terrtrio_resp = tbFatCidadaoTerritorioResponsv.co_seq_fat_cidadao_territorio
    left join tb_fat_cidadao_pec tbFatCidadaoPecResponsv
    on tbFatCidadaoTerritorioResponsv.co_fat_cidadao_pec = tbFatCidadaoPecResponsv.co_seq_fat_cidadao_pec
    where tb_fat_cad_domiciliar.co_dim_tempo_validade = '30001231' and tb_fat_cad_domiciliar.st_recusa_cadastro = 0 And tb_dim_tipo_imovel.nu_identificador = '1'
    And tb_dim_unidade_saude.st_registro_valido = 1  and tb_dim_equipe.st_registro_valido = 1 ";
            query += "And tb_dim_unidade_saude.nu_cnes = '" + cnes + "' ";
            query += "And tb_dim_equipe.nu_ine = '" + ine + "' ";
            query += "And tb_fat_cad_domiciliar.nu_micro_area = '" + area + "' ";
            query += "order by tb_fat_familia_territorio.nu_prontuario asc, tb_fat_cidadao_territorio.st_responsavel desc";


            try
            {               
                conn = new NpgsqlConnection(conexaoString);
                conn.Open();
                NpgsqlCommand command = new NpgsqlCommand();
                command.Connection = conn;
                command.CommandType = CommandType.Text;
                command.CommandText = query;
                NpgsqlDataReader dr = command.ExecuteReader();

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        Pessoa h = new Pessoa();
                        h.mc = dr[0].ToString();
                        string prontuario = dr[1].ToString();                       
                        switch(prontuario)
                        {
                            case "": prontuario = "999"; break;
                            case "01": prontuario = "1"; break;
                            case "02": prontuario = "2"; break;
                            case "03": prontuario = "3"; break;
                            case "04": prontuario = "4"; break;
                            case "05": prontuario = "5"; break;
                            case "06": prontuario = "6"; break;
                            case "07": prontuario = "7"; break;
                            case "08": prontuario = "8"; break;
                            case "09": prontuario = "9"; break;                   

                        }

                       // h.prontuario = Convert.ToInt32(prontuario); 
                        h.prontuario = prontuario;

                        // h.cns = dr[2].ToString();
                        // h.cpf = dr[3].ToString();                        
                        // string nome = dr[4].ToString();
                        //h.nome = Geral.removerAcentos(nome);
                        h.Bairro = dr[5].ToString();
                        h.log = dr[6].ToString();
                        h.t_log= dr[7].ToString();
                        h.tel = dr[8].ToString();
                        h.nascimento = dr[9].ToString();
                        h.sexo= dr[10].ToString();
                        h.cnes = dr[11].ToString();
                        // h.unidade= dr[12].ToString();
                        //h.ine = dr[13].ToString();
                        // h.equip = dr[14].ToString();
                        h.co_fat_cidadao_pec = dr["co_seq_fat_cidadao_pec"].ToString(); 
                        //
                        // if(Lista_1.ContainsKey(codigo))
                        // {
                        //     Lista_1.Remove(codigo);
                        // }
                        // 
                        
                    }
                }
                conn.Dispose();
                conn.Close();                
               // lista_familias.Sort((x, y) => x.pront.CompareTo(y.pront));
               // if (lista_familias.Count > 0) { dataGridView2.DataSource = lista_familias; }

            }//try
            catch (Exception ex) { if (conn.State == ConnectionState.Open) { conn.Close(); } MessageBox.Show(ex.ToString()); }
        }
        private void executar(string texto)
        {
            try
            { 
                var conn = new NpgsqlConnection(conexaoString);
                conn.Open();
                var command = new NpgsqlCommand();
                command.Connection = conn;
                command.CommandType = CommandType.Text;
                command.CommandText = texto;

                NpgsqlDataReader dr = command.ExecuteReader();                
                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        Pessoa h = new Pessoa();
                        h.data_cadastro = dr["dt_cad"].ToString();                        
                        string nome = dr["nome"].ToString().ToUpper();
                        h.nome = Geral.removerAcentos(nome);
                        h.sexo = dr["sexo"].ToString();
                        h.nascimento = dr["dt_nasc"].ToString();
                        h.idade = Convert.ToByte(dr["idade"].ToString());
                        string cns = dr["fatcns"].ToString().Trim(); // mesmo zero ele tem espaços
                       string cpf = dr["fatcpf"].ToString().Trim();
                        if (cpf.Length == 11) { h.cns_cpf= cpf; }
                        if (cns.Length == 15) { h.cns_cpf = cns; }
                        h.cns = dr["cns"].ToString();
                        h.cpf = dr["cpf"].ToString();                        
                        h.cnes = dr["cnes"].ToString();
                        h.mc = dr["mc"].ToString();
                       // h.FA = dr["fa"].ToString();
                        h.has = dr["has"].ToString();
                        h.dm = dr["dm"].ToString();
                        h.tab = dr["tab"].ToString();
                        h.alc = dr["alc"].ToString();
                        h.ges = dr["ges"].ToString();
                       // h.unidade = dr["unidade"].ToString();
                        h.equipe_vinc = dr["equipe_vinc"].ToString();
                        h.acs = dr["acs"].ToString();
                       // h.faleceu = dr["st_faleceu"].ToString();
                        h.co_fat_cidadao_pec = dr["co_fat_cidadao_pec"].ToString();
                        h.responsavel = dr["responsavel"].ToString();
                        string nu_cpf_responsavel = dr["nu_cpf_responsavel"].ToString();                       
                        string nu_cns_responsavel = dr["nu_cns_responsavel"].ToString();
                        if (nu_cpf_responsavel.Length == 11) { h.cpf_cns_responsavel = nu_cpf_responsavel; }
                        if (nu_cns_responsavel.Length == 15) { h.cpf_cns_responsavel = nu_cns_responsavel; }

                        if (h.responsavel == "s")
                        {
                            h.co_fat_cidadao_pec_responsvl = h.co_fat_cidadao_pec; // chefe/responsável
                        }
                        else
                        {
                            h.co_fat_cidadao_pec_responsvl = dr["co_fat_cidadao_pec_responsvl"].ToString();
                        }



                        //if (Lista_1.ContainsKey(h.co_fat_cidadao_pec))
                        //{ 
                        //    //dataGridView4.Rows.Add(h.nome, h.cns_cpf, h.co_fat_cidadao_pec);
                        //}
                        //else
                        //{
                        //    lista_pessoas.Add(h); Lista_1.Add(h.co_fat_cidadao_pec, h.nome);
                        //}
                    }                
                }

                //for (int i = 0; i < 30; i++)
                //{
                //    string collumns = i.ToString();
                //    dataGridView1.Columns.Add(collumns, collumns);

                //}

                //for (int t =0; t < lista_pessoas.Count; t++)
                //{

                //    dataGridView1.Rows.Add(lista_pessoas[t].data_cadastro, lista_pessoas[t].nome, lista_pessoas[t].sexo, lista_pessoas[t].nascimento, lista_pessoas[t].idade,
                //              lista_pessoas[t].fatcns, lista_pessoas[t].fatcpf, lista_pessoas[t].cns, lista_pessoas[t].cpf, lista_pessoas[t].cnes, lista_pessoas[t].mc, 
                //              lista_pessoas[t].has, lista_pessoas[t].dm, lista_pessoas[t].tab, lista_pessoas[t].alc,
                //              lista_pessoas[t].ges, lista_pessoas[t].equipe_vinc, lista_pessoas[t].acs, lista_pessoas[t].faleceu, lista_pessoas[t].co_fat_cidadao_pec);

                //}
                conn.Dispose();   conn.Close();
               // BindingList<Pessoa> view = new BindingList<Pessoa>(lista_pessoas);
               // dataGridView1.DataSource = view;
               // dataGridView1.DataSource = lista_pessoas;  // não da para ordenar isso é ruim pois tem que jogar no excel ou criar order by              
                
            }
            catch (Exception ex)
            {
                if (conn.State == ConnectionState.Open) { conn.Close(); }
               
            }
        }

         private IEnumerable<string> Get()
        {

            var result = new List<string>();
            using (var conn = new NpgsqlConnection())
            {
                conn.ConnectionString = "Server=localhost;Port=5433;User Id=postgres;Password=esus;Database=esus;";
                conn.Open();

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText =
                        "SELECT * FROM tb_cidadao LIMIT 10";
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(reader.GetString(2));
                        }
                    }
                }
            }

            return result;



        }
        private string Query(string texto)
        {
            string q = @"select
to_char(t1.dt_cad_individual, 'DD/MM/YYYY') as dt_cad,
CASE WHEN t2.st_responsavel_familiar = 1 THEN 'sim' ELSE 'nao' END responsavel,
t1.no_cidadao as nome,
to_char(t2.dt_nascimento, 'DD/MM/YYYY') as dt_nasc,
date_part('year',age(t2.dt_nascimento)) as idade,
date_part('month', age(t2.dt_nascimento)) as meses,
CASE
    WHEN t2.co_dim_sexo = 1 THEN 'M'
    ELSE 'F'
END sexo,
CASE WHEN t2.nu_cns = '0' THEN t2.nu_cpf_cidadao ELSE t2.nu_cns END cns_cpf,
t3.nu_cns as cns,
t3.nu_cpf_cidadao as cpf,
CASE WHEN t2.st_hipertensao_arterial = 1 THEN 'has' ELSE '' END has,
CASE WHEN t2.st_diabete = 1 THEN 'dm' ELSE '' END dm,
CASE WHEN t2.st_fumante = 1 THEN 'tab' ELSE '' END tab,
CASE WHEN t2.st_alcool = 1 THEN 'alc' ELSE '' END alc,
CASE WHEN t2.st_gestante = 1 THEN 'ges' ELSE '' END ges,
/*t2.co_dim_unidade_saude as unidade,*/
t99.nu_cnes as cnes,
t99.no_unidade_saude as unidade,
/*t2.co_dim_equipe as equipe,*/
t98.nu_ine as ine,
t98.no_equipe as equipe,
CASE WHEN t2.co_dim_tipo_saida_cadastro = 1  THEN 'OBITO' WHEN t2.co_dim_tipo_saida_cadastro = 2  THEN 'MUDOU' ELSE 'RESIDE' END saida,
t2.nu_micro_area as mc,
(select tp.no_profissional from tb_dim_profissional tp where t2.co_dim_profissional = tp.co_seq_dim_profissional) as acs,
(select kk.no_equipe from tb_dim_equipe kk where t3.co_dim_equipe_vinc = kk.co_seq_dim_equipe )as equipe_vinc,
CASE WHEN t2.nu_cns_responsavel ISNULL THEN t2.nu_cpf_responsavel ELSE t2.nu_cns_responsavel END cpf_cns_responsavel,
CASE WHEN t2.co_fat_cidadao_pec_responsvl ISNULL THEN t2.co_fat_cidadao_pec ELSE t2.co_fat_cidadao_pec_responsvl END co_fat_cidadao_pec_responsvl,
t2.co_fat_cidadao_pec
from
tb_cds_cad_individual t1
left join tb_fat_cad_individual t2 on t1.co_unico_ficha = t2.nu_uuid_ficha
left join tb_fat_cidadao_pec t3 on t2.co_fat_cidadao_pec = t3.co_seq_fat_cidadao_pec
left join tb_dim_unidade_saude t99 on t2.co_dim_unidade_saude = t99.co_seq_dim_unidade_saude
left join tb_dim_equipe t98 on t2.co_dim_equipe = t98.co_seq_dim_equipe
where
(t1.st_versao_atual = 1) AND (t1.st_ficha_inativa = 0)" + texto + "ORDER BY co_fat_cidadao_pec_responsvl, responsavel desc";
            return q;
        }
        private void button1_Click(object sender, EventArgs e)
        {
           
        }        
        public void Cadastros_CDS(string texto)
        {
            try
            {
            var list_filtro = new Dictionary<string, Filtro_1>();
            var List_tmp = new Dictionary<string, Pessoa>();

                Conexao c = new Conexao();
                //"(t1.st_versao_atual = 1) AND(t1.st_ficha_inativa = 0) ORDER BY co_fat_cidadao_pec_responsvl, responsavel desc")
                var dt = c.Get_DataTable(Geral.BuscaCadastrosCDS());
                if (dt == null) { return; }
                Geral.WriteToCsvFile(dt,"fci");
                foreach (System.Data.DataRow dr in dt.Rows)
                {
                    var h = new Pessoa();
                    h.data_cadastro = dr["dt_cad"].ToString();
                    string nome = dr["nome"].ToString().ToUpper();
                    h.nome = Geral.removerAcentos(nome);
                    h.sexo = dr["sexo"].ToString();
                    h.nascimento = dr["dt_nasc"].ToString();
                    h.idade = Convert.ToByte(dr["idade"].ToString());
                    h.meses = dr["meses"].ToString();
                    h.cns = dr["cns"].ToString();
                    h.cpf = dr["cpf"].ToString();
                    h.cns_cpf = dr["cns_cpf"].ToString();
                    h.cnes = dr["cnes"].ToString();
                    h.unidade = dr["unidade"].ToString();
                    h.ine = dr["ine"].ToString();
                    h.equipe = dr["equipe"].ToString();
                    h.mc = dr["mc"].ToString();                   
                    h.has = dr["has"].ToString();
                    h.dm = dr["dm"].ToString();
                    h.tab = dr["tab"].ToString();
                    h.alc = dr["alc"].ToString();
                    h.ges = dr["ges"].ToString();
                    h.equipe_vinc = dr["equipe_vinc"].ToString();
                    h.acs = dr["acs"].ToString();
                    h.co_fat_cidadao_pec = dr["co_fat_cidadao_pec"].ToString();
                    h.responsavel = dr["responsavel"].ToString();
                    h.cpf_cns_responsavel = dr["cpf_cns_responsavel"].ToString();
                    h.co_fat_cidadao_pec_responsvl = dr["co_fat_cidadao_pec_responsvl"].ToString();
                    h.saida = dr["saida"].ToString();



                    //AND t2.co_dim_tipo_saida_cadastro = 3
                    if (h.saida =="RESIDE")
                    {
                        if (list_filtro.ContainsKey(h.unidade))
                        {
                            list_filtro[h.unidade].Analize(h);
                        }
                        else
                        {
                            Filtro_1 f = new Filtro_1();
                            list_filtro.Add(h.unidade, f);
                            list_filtro[h.unidade].Analize(h);
                        }
                    }
                   

                    string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    string arquivo = "b.csv";
                    string path = System.IO.Path.Combine(desktop, arquivo);
                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(path))
                    {
                        string collum_name = "Unidade;Cnes;Cadastros;Masculino;Feminino;DM;HAS;ALC;TAB;GES";
                        sw.WriteLine(collum_name);

                        foreach (var r in list_filtro.Values)
                        {
                            string b = "";
                            b += r.unidade + ";";
                            b += r.cnes + ";";
                            b += r.cadastros + ";";
                            b += r.masculino + ";";
                            b += r.feminino + ";";                            
                            b += r.diabetico + ";";
                            b += r.hipertenso + ";";
                            b += r.fazUsoBebida + ";";
                            b += r.fumante +";";
                            b += r.gestante+ ";";
                            sw.WriteLine(b);
                        }

                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();
                        sw.WriteLine();

                        sw.WriteLine(collum_name);
                        sw.WriteLine(collum_name);
                        sw.WriteLine(collum_name);
                        sw.Close();
                    }



                    if (List_tmp.ContainsKey(h.co_fat_cidadao_pec)) { }
                    else { List_tmp.Add(h.co_fat_cidadao_pec, h); }
                }



            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }

        private void button2_Click(object sender, EventArgs e)
        {           
            //dataGridView1.Columns["has"].Width = 30;
            //dataGridView1.Columns["dm"].Width = 30;
            //dataGridView1.Columns["tab"].Width = 30;
            //dataGridView1.Columns["alc"].Width = 30;
            //dataGridView1.Columns["sexo"].Width = 30;
            //dataGridView1.Columns["idade"].Width = 30;
            //dataGridView1.Columns["chefe"].Width = 30;
            //dataGridView1.Columns["mc"].Width = 30;
            //dataGridView1.Columns["ges"].Width = 30;
            //dataGridView1.Columns["cnes"].Width = 30;
            //dataGridView1.Columns["co_fat_cidadao_pec_responsvl"].Width = 30;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (dataGridView5.Rows.Count > 0) { dataGridView5.DataSource = null; dataGridView5.Rows.Clear(); dataGridView5.Refresh(); }

            var list = new List<TB_CIDADAO>();
            using (var conn = new NpgsqlConnection())
            {
                conn.ConnectionString = conexaoString;
                conn.Open();
                string sort = "WHERE tb.st_ativo_para_exibicao = 1 ORDER BY tb.no_cidadao";
                string query = @"SELECT  
                        tb.dt_atualizado, 
                        tb.co_seq_cidadao, 
                        tb.nu_micro_area,
                        tb.st_registro_cadsus, 
                        tb.dt_atualizado_cadsus,
                        tb.st_ativo,
                        tb.st_ativo_para_exibicao,
                        tb.st_unificado,
                        tb.nu_cns,
                        tb.nu_cpf,
                        tb.no_cidadao,
                        tb.ds_cep,
                        tb.no_bairro,
                        tb.nu_numero,
                        tb.ds_logradouro 
                        FROM tb_cidadao tb " + sort;
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = query;
                        
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var tb = new TB_CIDADAO();

                            tb.dt_atualizado = dr[0].ToString().Substring(0,10);
                            tb.co_seq_cidadao = dr[1].ToString();
                            tb.nu_micro_area = dr[2].ToString();
                            tb.st_registro_cadsus = dr[3].ToString();
                            tb.dt_atualizado_cadsus = dr[4].ToString();
                            tb.st_ativo = dr[5].ToString(); // cadastro inativo pelo usuario mas ele é visivel na pesquisa
                            tb.st_ativo_para_exibicao = dr[6].ToString(); // cadastro inativo não é visivel pela pesquisa (cadastros unificados._)
                            tb.st_unificado = dr[7].ToString();
                            tb.nu_cns = dr[8].ToString();
                            tb.nu_cpf = dr[9].ToString();
                            tb.no_cidadao = dr[10].ToString();
                            tb.ds_cep = dr[11].ToString();
                            tb.no_bairro = dr[12].ToString();
                            tb.nu_numero = dr[13].ToString();
                            tb.ds_logradouro = dr[14].ToString();
                            list.Add(tb);
                        }
                    }
                }
            }
            dataGridView5.DataSource = list;
        }

        private void button4_Click(object sender, EventArgs e)
        {
           
        }

        private void todosCadastrosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // só não vai pegar as inativas...
            string texto = "";                      
            Cadastros_CDS(texto);
            
        }

        private void cidadaoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {            
                Conexao c = new Conexao();
                var dt = c.Get_DataTable(QueryX(""));
                if (dt == null) { return; }
                Geral.WriteToCsvFile(dt, "cidadao.csv");
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }

        private void criancas0A2AnosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.Rows.Count > 0) { dataGridView1.DataSource = null; dataGridView1.Rows.Clear(); dataGridView1.Refresh(); }
                var list = new Dictionary<string,Pessoa>();
                var c = new Conexao();
                NpgsqlDataReader dr = c.Get_DataReader2(QueryX(""));
                if (dr == null) { return; }
                while (dr.Read())
                {
                    var h = new Pessoa();
                    //h.CDS = dr["CDS"].ToString();
                    h.data_cadastro = dr["dt_ultima_ficha"].ToString();
                    string nome = dr["nome"].ToString().ToUpper();
                    h.nome = Geral.removerAcentos(nome);
                    h.sexo = dr["sexo"].ToString();
                    h.nascimento = dr["nasc"].ToString();
                    h.idade = Convert.ToByte(dr["idade"].ToString());
                    
                    h.meses = dr["meses"].ToString();
                    h.cns = dr["cns"].ToString();
                    h.cpf = dr["cpf"].ToString();
                    //h.cns_cpf = dr["cns_cpf"].ToString();
                    //h.cnes = dr["cnes"].ToString();
                    //h.unidade = dr["unidade"].ToString();
                    //h.ine = dr["ine"].ToString();
                    //h.equipe = dr["equipe"].ToString();                   
                    //h.has = dr["has"].ToString();
                    //h.dm = dr["dm"].ToString();
                    //h.tab = dr["tab"].ToString();
                    //h.alc = dr["alc"].ToString();
                    //h.ges = dr["ges"].ToString();
                    //h.equipe_vinc = dr["equipe_vinc"].ToString();
                    h.mc = dr["mc"].ToString();
                    h.acs = dr["acs"].ToString();                    
                    h.saida = dr["saida"].ToString(); //co_dim_tipo_saida_cadastro
                    h.co_fat_cidadao_pec = dr["co_fat_cidadao_pec"].ToString();


                    int idade = Convert.ToInt32(h.idade);

                    if (list.ContainsKey(h.co_fat_cidadao_pec))
                    { //duplicadosssssss
                    }
                    else
                    {
                        if (idade <= 2)
                        {
                            list.Add(h.co_fat_cidadao_pec, h);
                        }
                    }
                }

                Vacinacao v = new Vacinacao();             
               
                dataGridView1.DataSource = list;
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }
        private string QueryX(string complemento)
        {
            string texto = @"select
tc.st_faleceu,
/*tc.co_seq_cidadao,
tc.co_unico_cidadao,*/
tc.dt_atualizado,
tc.dt_ultima_ficha,
case when tc.co_unico_ultima_ficha isnull THEN 'SEM CADASTRO CDS'else tc.co_unico_ultima_ficha end CDS,
tc.no_cidadao as nome,
tc.no_sexo as sexo,
tc.dt_nascimento as nasc,
date_part('year',age(tc.dt_nascimento)) as idade,
date_part('month',age(tc.dt_nascimento)) as meses,
/*date_part('day',age(tc.dt_nascimento)) as dd,*/
tc.nu_cpf as cpf, tc.nu_cns as cns,
tc.no_mae as mae, tc.no_pai as pai,
case when tc.st_unificado = 1 then 'unificado' else '' end unificado,
case when tc.st_ativo = 1 then 'ativo' else 'inativo' end ativo,
case when tc.st_registro_cadsus = 1 then 'sim' else 'nao' end adSus,
case when tc.st_faleceu = 1 then 'sim' else 'nao' end faleceu,
tc.ds_cep,
tc.ds_complemento,
tc.ds_logradouro,
tc.nu_numero,
tc.no_bairro,
t7.st_versao_atual,
t8.nu_micro_area as mc,
(select tp.no_profissional from tb_dim_profissional tp where t8.co_dim_profissional = tp.co_seq_dim_profissional) as acs,
CASE WHEN t8.co_dim_tipo_saida_cadastro = 1  THEN 'OBITO' WHEN t8.co_dim_tipo_saida_cadastro = 2  THEN 'MUDOU' ELSE '' END saida,
case when t8.co_fat_cidadao_pec isnull then t9.co_seq_fat_cidadao_pec else t8.co_fat_cidadao_pec end co_fat_cidadao_pec
from tb_cidadao tc
left join tb_cds_cad_individual t7 on tc.co_unico_ultima_ficha = t7.co_unico_ficha
left join tb_fat_cad_individual t8 on t7.co_unico_ficha = t8.nu_uuid_ficha
left join tb_fat_cidadao_pec t9 on tc.co_seq_cidadao = t9.co_cidadao
where tc.st_ativo_para_exibicao = 1 order by tc.dt_atualizado";
            return texto;
        }
    }
    public class Filtro_1
    {
        public string unidade { get; set; }
        public string cnes { get; set; }
        public int cadastros { get; set; }
        public int hipertenso { get; set; }
        public int diabetico { get; set; }
        public int fumante { get; set; }
        public int fazUsoBebida { get; set; }
        public int gestante { get; set; }
        public int masculino { get; set; }
        public int feminino { get; set; }

        public void Analize(Pessoa p)
        {           
            unidade = p.unidade;
            cnes = p.cnes;
            cadastros++;
            if (p.has == "has") { hipertenso++; }
            if (p.dm == "dm") { diabetico++; }
            if (p.tab == "tab") { fazUsoBebida++; }
            if (p.alc == "alc") { fumante++; }
            if (p.ges == "ges") { gestante++; }
            if (p.sexo == "M") { masculino++; }
            if (p.sexo == "F") { feminino++; }
        }
        
    }

    class TB_CIDADAO
    {
        public string dt_atualizado { get; set; }
        public string co_seq_cidadao { get; set; }
        public string nu_micro_area { get; set; }
        public string st_registro_cadsus { get; set; }
        public string dt_atualizado_cadsus { get; set; }
        public string st_ativo { get; set; }
        public string st_ativo_para_exibicao { get; set; }
        public string st_unificado { get; set; }
        public string nu_cns { get; set; }
        public string nu_cpf { get; set; }
        public string no_cidadao { get; set; }
        public string ds_cep { get; set; }
        public string no_bairro { get; set; }
        public string nu_numero { get; set; }
        public string ds_logradouro { get; set; }
       
    }


}
