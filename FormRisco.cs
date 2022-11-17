using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class FormRisco : Form
    {
        
        List<Pessoa> Lista_Cadastros_individuais;
        Dictionary<string,Domicilios> Lista_Cadastros_Domiciliares;
        public FormRisco()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Lista_Cadastros_individuais = new List<Pessoa>();
            Lista_Cadastros_Domiciliares = new Dictionary<string, Domicilios>();
            Busca_Cadastros_Domiciliares(textBox2.Text, textBox1.Text);
            Busca_Cadastros_Individuais(textBox2.Text, textBox1.Text);
           // Processamento();
        }      
        private void Busca_Cadastros_Individuais(string cnes, string area)
        {
            try
            {
                string query = @"select
to_char(t1.dt_cad_individual, 'DD/MM/YYYY') as dt_cad,
CASE WHEN t2.st_responsavel_familiar = 1 THEN 's' ELSE '' END responsavel,
t1.no_cidadao as nome,
to_char(t2.dt_nascimento, 'DD/MM/YYYY') as dt_nasc,
date_part('year',age(t2.dt_nascimento)) as idade,
CASE
    WHEN t2.co_dim_sexo = 1 THEN 'M'
    ELSE 'F'
END sexo,
CASE WHEN t2.nu_cns = '0' THEN t2.nu_cpf_cidadao ELSE t2.nu_cns END cns_cpf,
t3.nu_cns as cns,
t3.nu_cpf_cidadao as cpf,
CASE WHEN t2.st_hipertensao_arterial = 1 THEN 'HAS' ELSE '' END has,
CASE WHEN t2.st_diabete = 1 THEN 'DM' ELSE '' END dm,
CASE WHEN t2.st_fumante = 1 THEN 'TAB' ELSE '' END tab,
CASE WHEN t2.st_alcool = 1 THEN 'ALC' ELSE '' END alc,
CASE WHEN t2.st_gestante = 1 THEN 'GES' ELSE '' END ges,
CASE WHEN t2.st_defi_fisica = 1 THEN 'FISICA' ELSE '' END fisica,
CASE WHEN t2.st_defi_intelectual_cognitiva = 1 THEN 'MENTAL' ELSE '' END intelectual,
CASE WHEN t2.st_acamado = 1 THEN 'sim' ELSE 'nao' END acamado,
CASE WHEN t2.st_outra_droga = 1 THEN 'sim' ELSE 'nao' END outras_drogas,
CASE WHEN t2.co_dim_situacao_trabalho = 7 THEN 'sim' ELSE 'nao' END desempregado,
CASE WHEN t2.co_dim_tipo_escolaridade = 15 THEN 'nenhum' ELSE '' END escolaridade,
CASE WHEN t2.co_dim_tipo_condicao_peso = 1 THEN 'abaixo' ELSE '' END peso,
t99.nu_cnes as cnes,
t99.no_unidade_saude as unidade,
t98.nu_ine as ine,
t98.no_equipe as equipe,
CASE WHEN t2.co_dim_tipo_saida_cadastro = 1  THEN 'OBITO' WHEN t2.co_dim_tipo_saida_cadastro = 2  THEN 'MUDOU' ELSE '' END saida,
t2.nu_micro_area as mc,
t1.nu_celular_cidadao as tel,
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
left join tb_dim_equipe t98 on t2.co_dim_equipe = t98.co_seq_dim_equipe";

                query += @" WHERE (t1.st_versao_atual = 1) AND (t1.st_ficha_inativa = 0) AND (t2.co_dim_tipo_saida_cadastro = 3)";
                query += @" AND(t99.nu_cnes = '" + cnes + "') AND (t2.nu_micro_area = '" + area + "')";
                query += @" ORDER BY co_fat_cidadao_pec_responsvl, responsavel desc";

                Conexao c = new Conexao();
                var dt = c.Get_DataTable(query);
                if (dt == null) { return; }                
                foreach (System.Data.DataRow dr in dt.Rows)
                {
                    var h = new Pessoa();
                    h.prontuario = "";
                    h.data_cadastro = dr["dt_cad"].ToString();
                    string nome = dr["nome"].ToString().ToUpper();
                    h.nome = Geral.removerAcentos(nome);
                    h.sexo = dr["sexo"].ToString();
                    h.nascimento = dr["dt_nasc"].ToString();
                    h.idade = Convert.ToByte(dr["idade"].ToString());
                    int idad = Convert.ToInt32(h.idade);
                    if (idad > 70) h.aps_escala += 1;
                    if (idad == 0) h.aps_escala += 1; // CONVERTER PARA MESES E DIAS.....
                    h.cns = dr["cns"].ToString();
                    h.cpf = dr["cpf"].ToString();
                    h.cns_cpf = dr["cns_cpf"].ToString();
                    h.cnes = dr["cnes"].ToString();
                    h.unidade = dr["unidade"].ToString();
                    h.ine = dr["ine"].ToString();
                    h.equipe = dr["equipe"].ToString();
                    h.mc = dr["mc"].ToString();
                    h.has = dr["has"].ToString();
                    if (h.has == "sim") h.aps_escala += 1;
                    h.dm = dr["dm"].ToString();
                    if (h.dm == "sim") h.aps_escala += 1;
                    h.tab = dr["tab"].ToString();
                    h.alc = dr["alc"].ToString();
                    h.ges = dr["ges"].ToString();
                    h.equipe_vinc = dr["equipe_vinc"].ToString();
                    h.acs = dr["acs"].ToString();
                    h.tel = dr["tel"].ToString();
                    h.co_fat_cidadao_pec = dr["co_fat_cidadao_pec"].ToString();
                    h.responsavel = dr["responsavel"].ToString();
                    h.cpf_cns_responsavel = dr["cpf_cns_responsavel"].ToString();
                    h.co_fat_cidadao_pec_responsvl = dr["co_fat_cidadao_pec_responsvl"].ToString();
                    h.outras_drogas= dr["outras_drogas"].ToString();
                    if (h.outras_drogas == "sim") h.aps_escala += 2;
                    h.desempregado = dr["desempregado"].ToString();
                    if (h.desempregado == "sim") h.aps_escala += 2;
                    h.escolaridade = dr["escolaridade"].ToString();
                    if ((h.escolaridade == "nenhum") && (idad > 4)) h.aps_escala += 1;
                    h.peso = dr["peso"].ToString();
                    if (h.peso == "abaixo") h.aps_escala += 3;
                    h.fisica = dr["fisica"].ToString();
                    if (h.fisica == "sim") h.aps_escala += 3;
                    h.intelectual = dr["intelectual"].ToString();
                    if (h.intelectual == "sim") h.aps_escala += 3;
                    h.acamado = dr["acamado"].ToString();
                    if (h.acamado == "sim") h.aps_escala += 3;
                    h.endereco = "";

                    var domicilio = GetDadosDomiciliares(h.co_fat_cidadao_pec_responsvl);
                    if (domicilio != null)
                    {
                        h.endereco += domicilio.bairro;
                        h.endereco += " " + domicilio.logradouro;
                        h.endereco += " " + domicilio.numero;
                        h.prontuario = domicilio.nu_prontuario;
                    }

                    Lista_Cadastros_individuais.Add(h);                    
                }

                if (Lista_Cadastros_individuais.Count > 0)
                {
                    if (checkBox1.Checked)
                    {
                        Geral.ToDataTable(Lista_Cadastros_individuais, "FCI" + cnes + area);
                    }
                }
                label3.Text = "Total pessoas "+ Lista_Cadastros_individuais.Count;
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }
        private void Busca_Cadastros_Domiciliares(string cnes, string area)
        {
            try
            {
                string query = @"select
CASE WHEN t66.nu_prontuario ISNULL THEN '0' ELSE t66.nu_prontuario END nu_prontuario,
t3.co_unico_domicilio as co_unico,
CASE WHEN t5.co_fat_cad_domiciliar ISNULL THEN '0' ELSE t5.co_fat_cad_domiciliar END co_fat_cad_domiciliar,
to_char(t3.dt_cad_domiciliar, 'DD/MM/YYYY') as dt_cad,
t3.no_bairro as bairro, 
(select k.ds_tipo_logradouro from tb_dim_tipo_logradouro k where t2.co_dim_tipo_logradouro = k.co_seq_dim_tipo_logradouro) as tipo,
t3.no_logradouro as logradouro,
t3.nu_domicilio as numero,
t2.nu_micro_area as mc,
CASE WHEN t2.nu_comodo ISNULL THEN '0' ELSE t2.nu_comodo END comodos,
case when t3.st_versao_atual = 0 then '0' else 'ATUAL' end Atual,
case when t5.st_mudou = 1 then 'Mudou' else '0' end Mudou,
CASE WHEN t5.nu_cns_responsavel ISNULL THEN '0' ELSE t5.nu_cns_responsavel END nu_cns_responsavel,
CASE WHEN t5.nu_cpf_responsavel ISNULL THEN '0' ELSE t5.nu_cpf_responsavel END nu_cpf_responsavel,
t2.co_dim_tipo_destino_lixo as Destinolixo,
t2.co_dim_tipo_tratamento_agua as QualidadeAgua,
t2.co_dim_tipo_escoamento_sanitar as sanitario,
CASE WHEN t5.co_fat_cidadao_pec ISNULL THEN '0' ELSE t5.co_fat_cidadao_pec END co_fat_cidadao_pec,
t99.nu_cnes as cnes,
(select tp.no_profissional from tb_dim_profissional tp where t2.co_dim_profissional = tp.co_seq_dim_profissional) as prof
from tb_fat_cad_domiciliar t2
left join tb_cds_cad_domiciliar t3 on t2.nu_uuid_ficha = t3.co_unico_ficha
left join tb_fat_cad_dom_familia t5 on t2.co_seq_fat_cad_domiciliar = t5.co_fat_cad_domiciliar
left join tb_cds_domicilio_familia t66 on t3.co_seq_cds_cad_domiciliar = t66.co_cds_cad_domiciliar
left join tb_dim_unidade_saude t99 on t2.co_dim_unidade_saude = t99.co_seq_dim_unidade_saude";
                query += @" WHERE t3.st_versao_atual = 1 AND t3.st_fora_area = 0";
                query += @" AND(t99.nu_cnes = '" + cnes + "') AND (t2.nu_micro_area = '" + area + "')";
                query += @" ORDER BY t3.dt_cad_domiciliar DESC";

                if (Lista_Cadastros_Domiciliares.Count > 0) Lista_Cadastros_Domiciliares.Clear();
                var c = new Conexao();
                var dt = c.Get_DataTable(query);
                if (dt == null) { return; }
                
                foreach (System.Data.DataRow dr in dt.Rows)
                {
                    var d = new Domicilios();
                    d.nu_prontuario = dr["nu_prontuario"].ToString();
                    d.co_unico = dr["co_unico"].ToString();
                    d.dt_cad = dr["dt_cad"].ToString();
                    d.co_fat_cad_domiciliar = dr["co_fat_cad_domiciliar"].ToString();
                    d.bairro = dr["bairro"].ToString();
                    d.tipo = dr["tipo"].ToString();                    
                    d.logradouro = dr["logradouro"].ToString();
                    d.numero = dr["numero"].ToString();
                    d.mc = dr["mc"].ToString();
                    int xcomodos = Convert.ToInt32(dr["comodos"].ToString());
                    if(xcomodos == 0) { xcomodos = 1; } // tem que existir pelo menos 1 comodo nao zero.
                    d.comodos = xcomodos;
                    d.atual = dr["atual"].ToString();
                   d.mudou = dr["mudou"].ToString();
                    string cns = dr["nu_cns_responsavel"].ToString();
                    if(cns.Length == 15) { d.cnscpf = cns; }
                    else { d.cnscpf = dr["nu_cpf_responsavel"].ToString(); }
                    d.Destinolixo = dr["Destinolixo"].ToString();
                    d.QualidadeAgua = dr["QualidadeAgua"].ToString();
                    d.sanitario = dr["sanitario"].ToString();
                    d.co_fat_cidadao_pec = dr["co_fat_cidadao_pec"].ToString();
                    d.cnes = dr["cnes"].ToString();
                    d.prof = dr["prof"].ToString();

                    if(Lista_Cadastros_Domiciliares.ContainsKey(d.co_unico))
                    {
                        if(d.mudou == "Mudou") { }
                        else 
                        {
                            Lista_Cadastros_Domiciliares[d.co_unico].cnscpf = d.cnscpf;
                            Lista_Cadastros_Domiciliares[d.co_unico].co_fat_cidadao_pec = d.co_fat_cidadao_pec;
                            Lista_Cadastros_Domiciliares[d.co_unico].nu_prontuario = d.nu_prontuario;
                        }
                    }
                    else 
                    {
                        if (d.mudou == "Mudou")
                        { 
                            d.cnscpf = "0";
                            d.co_fat_cidadao_pec = "0";
                            d.nu_prontuario = "0";
                            
                        }
                        Lista_Cadastros_Domiciliares.Add(d.co_unico, d); 
                    }
                    
                }

                if (Lista_Cadastros_Domiciliares.Count > 0)
                {
                    if (checkBox2.Checked)
                    {
                        Geral.ToDataTable(Lista_Cadastros_Domiciliares.Values.ToList(), "FCD" + cnes + area);
                    }
                }
                label5.Text = "Total domicilios " + Lista_Cadastros_Domiciliares.Count;
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }

        }       

        private string teste(string cnes, string area, string ine)
        {

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
    left join tb_fat_familia_territorio tb_fat_familia_territorio
    on tb_fat_familia_territorio.co_fat_cad_domiciliar = tb_fat_cad_domiciliar.co_seq_fat_cad_domiciliar
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

            return query;
        }


        private void Processamento()
        {
           
            //var tmp = new List<Pessoa>();
            //var tmp2 = new List<Filtro>();


                //foreach (var i in Lista_Cadastros_individuais.ToList())
                //{
                //    foreach (var a in Lista_Cadastros_Domiciliares.Values)
                //    {
                //        if(a.co_fat_cidadao_pec == i.co_fat_cidadao_pec_responsvl)
                //        {
                //            i.endereco += Lista_Cadastros_Domiciliares[a.co_unico].bairro;
                //            i.endereco += " " + Lista_Cadastros_Domiciliares[a.co_unico].logradouro;
                //            i.endereco += " " + Lista_Cadastros_Domiciliares[a.co_unico].numero;

                //            Lista_Cadastros_Domiciliares[a.co_unico].Lista_Pessoas.Add(i);
                //            tmp.Add(i);

                //            var f = new Filtro();
                //            f.prontuario = a.nu_prontuario;
                //            f.responsavel = i.responsavel;
                //            f.nome = i.nome;
                //            f.nasc = i.nascimento;
                //            f.idade = i.idade;
                //            f.sexo = i.sexo;
                //            f.cns = i.cns;
                //            f.cpf = i.cpf;
                //            f.endereco = i.endereco;
                //            f.has = i.has;
                //            f.dm = i.dm;
                //            f.tab = i.tab;
                //            f.alc = i.alc;
                //            f.ges = i.ges;
                //            f.deficiente = i.st_deficiencia;
                //            f.outros = "";
                //            f.telefone = i.tel;
                //            tmp2.Add(f);



                //            Lista_Cadastros_individuais.Remove(i);// já que foi para o domicilio então remove da lista...
                //        }
                //        //           // Lista_Cadastros_Domiciliares[a.co_fat_cad_domiciliar].Lista_Pessoas.Add(i);

                //        //            //Lista_Cadastros_Domiciliares[a.co_fat_cad_domiciliar].aps_escala += i.aps_escala;



                //    }
                //}

                //Geral.ToDataTable(tmp2,"final");
        }
      private Domicilios GetDadosDomiciliares(string co_fat_responsavel)
        {
            foreach (var a in Lista_Cadastros_Domiciliares.Values)
            {
                if (a.co_fat_cidadao_pec == co_fat_responsavel)
                    return a;
                
            }
            return null;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            Lista_Cadastros_Domiciliares = new Dictionary<string, Domicilios>();
            Lista_Cadastros_individuais = new List<Pessoa>();
            
            string comp1 = " ";
            string comp2 = " ";
            if(!checkBox3.Checked)
            {
                comp1 = " AND t3.st_fora_area = 0 ";
                comp2 = " AND (t2.co_dim_tipo_saida_cadastro = 3) AND (t1.st_fora_area = 0) ";
            }
            Busca_Cadastros_Domiciliares_Rurais(comp1);
            Busca_Cadastros_Individuais_Rurais(comp2);
        }
        private void Busca_Cadastros_Domiciliares_Rurais(string comp)
        {
            try
            {
                string query = @"select
CASE WHEN t66.nu_prontuario ISNULL THEN '0' ELSE t66.nu_prontuario END nu_prontuario,
t3.co_unico_domicilio as co_unico,
CASE WHEN t5.co_fat_cad_domiciliar ISNULL THEN '0' ELSE t5.co_fat_cad_domiciliar END co_fat_cad_domiciliar,
to_char(t3.dt_cad_domiciliar, 'DD/MM/YYYY') as dt_cad,
t3.no_bairro as bairro, 
(select k.ds_tipo_logradouro from tb_dim_tipo_logradouro k where t2.co_dim_tipo_logradouro = k.co_seq_dim_tipo_logradouro) as tipo,
t3.no_logradouro as logradouro,
t3.nu_domicilio as numero,
t2.nu_micro_area as mc,
t3.st_fora_area as FA,
CASE WHEN t2.nu_comodo ISNULL THEN '0' ELSE t2.nu_comodo END comodos,
case when t3.st_versao_atual = 0 then '0' else 'ATUAL' end Atual,
case when t5.st_mudou = 1 then 'Mudou' else '0' end Mudou,
CASE WHEN t5.nu_cns_responsavel ISNULL THEN '0' ELSE t5.nu_cns_responsavel END nu_cns_responsavel,
CASE WHEN t5.nu_cpf_responsavel ISNULL THEN '0' ELSE t5.nu_cpf_responsavel END nu_cpf_responsavel,
td10.ds_tipo_domicilio,
td11.ds_tipo_imovel,
td1.ds_tipo_abastecimento_agua,
td2.ds_tipo_tratamento_agua,
td3.ds_tipo_escoamento_sanitario,
td4.ds_tipo_destino_lixo,
td5.ds_tipo_material_parede,
td6.ds_tipo_acesso_domicilio,
td8.ds_tipo_localizacao,
case when t2.st_disp_energia = 1 then 'Sim' else 'Nao' END energia,
td7.ds_tipo_renda_familiar,
CASE WHEN t5.co_fat_cidadao_pec ISNULL THEN '0' ELSE t5.co_fat_cidadao_pec END co_fat_cidadao_pec,
t99.nu_cnes as cnes,
t99.no_unidade_saude as unidade,
(select tp.no_profissional from tb_dim_profissional tp where t2.co_dim_profissional = tp.co_seq_dim_profissional) as prof
from tb_fat_cad_domiciliar t2
left join tb_cds_cad_domiciliar t3 on t2.nu_uuid_ficha = t3.co_unico_ficha
left join tb_fat_cad_dom_familia t5 on t2.co_seq_fat_cad_domiciliar = t5.co_fat_cad_domiciliar
left join tb_fat_familia_territorio t66 on t2.co_seq_fat_cad_domiciliar = t66.co_fat_cad_domiciliar
left join tb_dim_unidade_saude t99 on t2.co_dim_unidade_saude = t99.co_seq_dim_unidade_saude
left join tb_dim_tipo_abastecimento_agua td1 on t2.co_dim_tipo_tratamento_agua = td1.co_seq_dim_tipo_abastec_agua
left join tb_dim_tipo_tratamento_agua td2 on t2.co_dim_tipo_tratamento_agua = td2.co_seq_dim_tipo_tratament_agua
left join tb_dim_tipo_escoamento_sanitar td3 on t2.co_dim_tipo_escoamento_sanitar = td3.co_seq_dim_tipo_escoamento_snt
left join tb_dim_tipo_destino_lixo td4 on t2.co_dim_tipo_destino_lixo = td4.co_seq_dim_tipo_destino_lixo
left join tb_dim_tipo_material_parede td5 on t2.co_dim_tipo_material_parede = td5.co_seq_dim_tipo_material_pared
left join tb_dim_tipo_acesso_domicilio td6 on t2.co_dim_tipo_acesso_domicilio = td6.co_seq_dim_tipo_acesso_domicil
left join tb_dim_tipo_localizacao td8 on t2.co_dim_tipo_localizacao = td8.co_seq_dim_tipo_localizacao
left join tb_dim_tipo_renda_familiar td7 on t5.co_dim_tipo_renda_familiar = td7.co_seq_dim_tipo_renda_familiar
left join tb_dim_tipo_domicilio td10 on t2.co_dim_tipo_domicilio = td10.co_seq_dim_tipo_domicilio
left join tb_dim_tipo_imovel td11 on t2.co_dim_tipo_imovel = td11.co_seq_dim_tipo_imovel
WHERE t3.st_versao_atual = 1" + comp;// AND t3.st_fora_area = 0";
               //co_dim_tipo_localizacao = 3 (rural) de qualquer forma ´só puxa as rurais. =]  


                var c = new Conexao();
                var dt = c.Get_DataTable(query);
                if (dt == null) { return; }

                foreach (System.Data.DataRow dr in dt.Rows)
                {
                    var d = new Domicilios();
                    d.nu_prontuario = dr["nu_prontuario"].ToString();
                    d.co_unico = dr["co_unico"].ToString();
                    d.dt_cad = dr["dt_cad"].ToString();
                    d.co_fat_cad_domiciliar = dr["co_fat_cad_domiciliar"].ToString();
                    d.bairro = dr["bairro"].ToString();
                    d.tipo = dr["tipo"].ToString();
                    d.logradouro = dr["logradouro"].ToString();
                    d.numero = dr["numero"].ToString();
                    d.mc = dr["mc"].ToString();
                    d.FA= dr["FA"].ToString();
                    int xcomodos = Convert.ToInt32(dr["comodos"].ToString());
                    if (xcomodos == 0) { xcomodos = 1; } // tem que existir pelo menos 1 comodo nao zero.
                    d.comodos = xcomodos;
                    d.atual = dr["atual"].ToString();
                    d.mudou = dr["mudou"].ToString();
                    string cns = dr["nu_cns_responsavel"].ToString();
                    if (cns.Length == 15) { d.cnscpf = cns; }
                    else { d.cnscpf = dr["nu_cpf_responsavel"].ToString(); }                   

                    d.Tipo_Domicilio = dr["ds_tipo_domicilio"].ToString();
                    d.Tipo_Imovel = dr["ds_tipo_imovel"].ToString();
                    d.tipo_abastecimento_agua = dr["ds_tipo_abastecimento_agua"].ToString();
                    d.tipo_tratamento_agua = dr["ds_tipo_tratamento_agua"].ToString();
                    d.tipo_escoamento_sanitar = dr["ds_tipo_escoamento_sanitario"].ToString();
                    d.tipo_destino_lixo = dr["ds_tipo_destino_lixo"].ToString();
                    d.tipo_material_parede = dr["ds_tipo_material_parede"].ToString();
                    d.tipo_acesso_domicilio = dr["ds_tipo_acesso_domicilio"].ToString();
                    d.tipo_localizacao = dr["ds_tipo_localizacao"].ToString();
                    d.energia = dr["energia"].ToString();
                    d.renda_familiar = dr["ds_tipo_renda_familiar"].ToString();


                    d.co_fat_cidadao_pec = dr["co_fat_cidadao_pec"].ToString();
                    d.cnes = dr["cnes"].ToString();
                    d.Unidade = dr["unidade"].ToString();
                    d.prof = dr["prof"].ToString();


                    //if(d.co_unico == "2186306-c16a868a-c2f5-45a6-8db9-ae37a10c73a0")
                   // { }
                    if (Lista_Cadastros_Domiciliares.ContainsKey(d.co_unico))
                    {
                        if (d.mudou == "Mudou") { }
                        else
                        {
                            Lista_Cadastros_Domiciliares[d.co_unico].cnscpf = d.cnscpf;
                            Lista_Cadastros_Domiciliares[d.co_unico].co_fat_cidadao_pec = d.co_fat_cidadao_pec;
                            Lista_Cadastros_Domiciliares[d.co_unico].nu_prontuario = d.nu_prontuario;
                        }
                    }
                    else
                    {
                        if (d.mudou == "Mudou")
                        {
                            d.cnscpf = "0";
                            d.co_fat_cidadao_pec = "0";
                            d.nu_prontuario = "0";

                        }
                        Lista_Cadastros_Domiciliares.Add(d.co_unico, d);
                    }
                }

               Geral.ToDataTable(Lista_Cadastros_Domiciliares.Values.ToList(), "domiciliar");

            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }

        }
        private void Busca_Cadastros_Individuais_Rurais(string comp)
        {
            try
            {
                string query = @"select
to_char(t1.dt_cad_individual, 'DD/MM/YYYY') as dt_cad,
CASE WHEN t2.st_responsavel_familiar = 1 THEN 's' ELSE '' END responsavel,
t1.no_cidadao as nome,
to_char(t2.dt_nascimento, 'DD/MM/YYYY') as dt_nasc,
date_part('year',age(t2.dt_nascimento)) as idade,
CASE
    WHEN t2.co_dim_sexo = 1 THEN 'M'
    ELSE 'F'
END sexo,
CASE WHEN t2.nu_cns = '0' THEN t2.nu_cpf_cidadao ELSE t2.nu_cns END cns_cpf,
t3.nu_cns as cns,
t3.nu_cpf_cidadao as cpf,
CASE WHEN t2.st_hipertensao_arterial = 1 THEN 'HAS' ELSE '' END has,
CASE WHEN t2.st_diabete = 1 THEN 'DM' ELSE '' END dm,
CASE WHEN t2.st_fumante = 1 THEN 'TAB' ELSE '' END tab,
CASE WHEN t2.st_alcool = 1 THEN 'ALC' ELSE '' END alc,
CASE WHEN t2.st_gestante = 1 THEN 'GES' ELSE '' END ges,
CASE WHEN t2.st_deficiencia = 1 THEN 'sim' ELSE '' END st_deficiencia,
CASE WHEN t2.st_defi_fisica = 1 THEN 'FISICA' ELSE '' END fisica,
CASE WHEN t2.st_defi_intelectual_cognitiva = 1 THEN 'MENTAL' ELSE '' END intelectual,
CASE WHEN t2.st_acamado = 1 THEN 'sim' ELSE '' END acamado,
CASE WHEN t2.st_outra_droga = 1 THEN 'sim' ELSE 'nao' END outras_drogas,
CASE WHEN t2.co_dim_situacao_trabalho = 7 THEN 'sim' ELSE 'nao' END desempregado,
CASE WHEN t2.co_dim_tipo_escolaridade = 15 THEN 'nenhum' ELSE '' END escolaridade,
CASE WHEN t2.co_dim_tipo_condicao_peso = 1 THEN 'abaixo' ELSE '' END peso,
t99.nu_cnes as cnes,
t99.no_unidade_saude as unidade,
t98.nu_ine as ine,
t98.no_equipe as equipe,
CASE WHEN t2.co_dim_tipo_saida_cadastro = 1  THEN 'OBITO' WHEN t2.co_dim_tipo_saida_cadastro = 2  THEN 'MUDOU' ELSE '' END saida,
t2.nu_micro_area as mc,
t1.nu_celular_cidadao as tel,
(select tp.no_profissional from tb_dim_profissional tp where t2.co_dim_profissional = tp.co_seq_dim_profissional) as acs,
(select kk.no_equipe from tb_dim_equipe kk where t3.co_dim_equipe_vinc = kk.co_seq_dim_equipe )as equipe_vinc,
CASE WHEN t2.nu_cns_responsavel ISNULL THEN t2.nu_cpf_responsavel ELSE t2.nu_cns_responsavel END cpf_cns_responsavel,
CASE WHEN t2.co_fat_cidadao_pec_responsvl ISNULL THEN t2.co_fat_cidadao_pec ELSE t2.co_fat_cidadao_pec_responsvl END co_fat_cidadao_pec_responsvl,
t2.co_fat_cidadao_pec,
td30.ds_dim_tipo_escolaridade,
td31.ds_dim_situacao_trabalho,
CASE WHEN t2.st_doenca_cardiaca = 1 THEN 'sim' ELSE '' END doenca_cardiaca
from
tb_cds_cad_individual t1
left join tb_fat_cad_individual t2 on t1.co_unico_ficha = t2.nu_uuid_ficha
left join tb_fat_cidadao_pec t3 on t2.co_fat_cidadao_pec = t3.co_seq_fat_cidadao_pec
left join tb_dim_unidade_saude t99 on t2.co_dim_unidade_saude = t99.co_seq_dim_unidade_saude
left join tb_dim_equipe t98 on t2.co_dim_equipe = t98.co_seq_dim_equipe
left join tb_dim_tipo_escolaridade td30 on t2.co_dim_tipo_escolaridade = td30.co_seq_dim_tipo_escolaridade
left join tb_dim_situacao_trabalho td31 on t2.co_dim_situacao_trabalho = td31.co_seq_dim_situacao_trabalho 
WHERE (t1.st_versao_atual = 1) AND (t1.st_ficha_inativa = 0)" + comp +"ORDER BY co_fat_cidadao_pec_responsvl, responsavel desc";

                                Conexao c = new Conexao();
                var dt = c.Get_DataTable(query);
                if (dt == null) { return; }
                foreach (System.Data.DataRow dr in dt.Rows)
                {
                    var h = new Pessoa();
                    h.prontuario = "";
                    h.data_cadastro = dr["dt_cad"].ToString();
                    string nome = dr["nome"].ToString().ToUpper();
                    h.nome = Geral.removerAcentos(nome);
                    h.sexo = dr["sexo"].ToString();
                    h.nascimento = dr["dt_nasc"].ToString();
                    h.idade = Convert.ToByte(dr["idade"].ToString());
                    int idad = Convert.ToInt32(h.idade);
                    if (idad > 70) h.aps_escala += 1;
                    if (idad == 0) h.aps_escala += 1; // CONVERTER PARA MESES E DIAS.....
                    h.cns = dr["cns"].ToString();
                    h.cpf = dr["cpf"].ToString();
                    h.cns_cpf = dr["cns_cpf"].ToString();
                    h.cnes = dr["cnes"].ToString();
                    h.unidade = dr["unidade"].ToString();
                    h.ine = dr["ine"].ToString();
                    h.equipe = dr["equipe"].ToString();
                    h.mc = dr["mc"].ToString();
                    h.has = dr["has"].ToString();
                    if (h.has == "sim") h.aps_escala += 1;
                    h.dm = dr["dm"].ToString();
                    if (h.dm == "sim") h.aps_escala += 1;
                    h.tab = dr["tab"].ToString();
                    h.alc = dr["alc"].ToString();
                    h.ges = dr["ges"].ToString();
                    h.equipe_vinc = dr["equipe_vinc"].ToString();
                    h.acs = dr["acs"].ToString();
                    h.tel = dr["tel"].ToString();
                    h.co_fat_cidadao_pec = dr["co_fat_cidadao_pec"].ToString();
                    h.responsavel = dr["responsavel"].ToString();
                    h.cpf_cns_responsavel = dr["cpf_cns_responsavel"].ToString();
                    h.co_fat_cidadao_pec_responsvl = dr["co_fat_cidadao_pec_responsvl"].ToString();
                    h.outras_drogas = dr["outras_drogas"].ToString();
                    if (h.outras_drogas == "sim") h.aps_escala += 2;
                    h.desempregado = dr["desempregado"].ToString();
                    if (h.desempregado == "sim") h.aps_escala += 2;
                    h.escolaridade = dr["escolaridade"].ToString();
                    if ((h.escolaridade == "nenhum") && (idad > 4)) h.aps_escala += 1;
                    h.peso = dr["peso"].ToString();
                    if (h.peso == "abaixo") h.aps_escala += 3;
                    h.st_deficiencia = dr["st_deficiencia"].ToString();
                    h.fisica = dr["fisica"].ToString();
                    if (h.fisica == "sim") h.aps_escala += 3;
                    h.intelectual = dr["intelectual"].ToString();
                    if (h.intelectual == "sim") h.aps_escala += 3;
                    h.acamado = dr["acamado"].ToString();
                    if (h.acamado == "sim") h.aps_escala += 3;
                    h.endereco = "";
                    h.ds_dim_situacao_trabalho = dr["ds_dim_situacao_trabalho"].ToString();
                    h.ds_dim_tipo_escolaridade = dr["ds_dim_tipo_escolaridade"].ToString();
                    h.doenca_cardiaca = dr["doenca_cardiaca"].ToString();

                    var domicilio = GetDadosDomiciliares(h.co_fat_cidadao_pec_responsvl);

                    if (domicilio != null)
                    {
                        h.Bairro = domicilio.bairro;
                        h.endereco += domicilio.bairro;
                        h.endereco += " " + domicilio.logradouro;
                        h.endereco += " " + domicilio.numero;
                        h.prontuario = domicilio.nu_prontuario;
                        h.Acesso = domicilio.tipo_acesso_domicilio;
                        h.Local = domicilio.tipo_localizacao;
                    }

                    Lista_Cadastros_individuais.Add(h);
                }
               
                Geral.ToDataTable(Lista_Cadastros_individuais, "individual");
                
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }
    }

    public class Domicilios
    {
        public string nu_prontuario { get; set; }
        public string co_unico { get; set; }
        public string dt_cad { get; set; }
        public string bairro { get; set; }
        public string tipo { get; set; }
        public string logradouro { get; set; }
        public string numero { get; set; }
        public string mc { get; set; }
        public string FA { get; set; }
        public int Membros { get { return Lista_Pessoas.Count; } }
        public int comodos { get; set; }        
        public string atual;
        public string mudou;
        public string cnscpf { get; set; }
        public string co_fat_cad_domiciliar;
        public string co_fat_cidadao_pec { get; set; }
        public string Tipo_Domicilio { get; set; } // casa. apartamento..
        public string Tipo_Imovel { get; set; } // domicilio, comercio...
        public string Destinolixo { get; set; } // 4 = céu aberto
        public string QualidadeAgua { get; set; } // 6 = sem tratamento
        public string sanitario { get; set; } //5 = ceu aberto
        public string cnes { get; set; }
        public string Unidade { get; set; }
        public string prof { get; set; }

        public int aps_escala { get; set; }
        public double risco
        { 
            get
            {
                if (Membros == 0) { return 0; }
                double media = (double)Membros / (double)comodos;
                if(media > 1) { return 3; }
                else if(media == 1) { return 2; }
                else { return 1; }
            }        
        }
        public int saneamento
        {
            get
            {
                if ((Destinolixo == "4") || (QualidadeAgua == "6") || (sanitario == "5")) { return 3; }
                return 0;
            }
        }
        public double final
        {
            get
            {                
                return aps_escala + risco + saneamento;
            }
        }
        public List<Pessoa> Lista_Pessoas = new List<Pessoa>();



        public string tipo_abastecimento_agua { get; set; }
        public string tipo_tratamento_agua { get; set; }
        public string tipo_escoamento_sanitar { get; set; }
        public string tipo_destino_lixo { get; set; }
        public string tipo_material_parede { get; set; }
        public string tipo_acesso_domicilio { get; set; } // asfalto , chao batido
        public string tipo_localizacao { get; set; } // rural urbano
        public string energia { get; set; }
        public string renda_familiar { get; set; }
    }


    public class Filtro
    {
        public string responsavel { get; set; }
        public string prontuario { get; set; }
        public string nome { get; set; }
        public string nasc { get; set; }
        public string idade { get; set; }

        public string sexo { get; set; }

        public string cns { get; set; }


        public string cpf { get; set; }

        public string endereco { get; set; }

        public string has { get; set; }
        public string dm { get; set; }
        public string tab { get; set; }
        public string alc { get; set; }
        public string ges { get; set; }

        public string deficiente { get; set; }
        public string outros { get; set; }
        public string telefone { get; set; }

        

    }
}
