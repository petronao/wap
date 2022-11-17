using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinFormsApp1
{
    public class AtividadesColetivas
    {
        Dictionary<string, Atividade> lista;
        List<string> listateste = new List<string>();
        string dt1;
        string dt2;
        public AtividadesColetivas()
        {  
        }
        public void Workteste(string DataInicial, string DataFinal)
        {
            try
            {
                dt1 = DataInicial;
                dt2 = DataFinal;
                lista = new Dictionary<string, Atividade>();
                Conexao c = new Conexao();
                var dt = c.Get_DataTable(ApenasTemas());
                if (dt == null) { return; }
                Geral.WriteToCsvFile(dt, "AtividadesColetivas");
            }
            catch (Exception ex) { return; }
        }
        public void Work()
        {
            try
            {
                lista = new Dictionary<string, Atividade>();              
                Conexao c = new Conexao();
                var dt = c.Get_DataTable(QueryBuscaAtividades());
                if (dt == null) { return; }
                Geral.WriteToCsvFile(dt,"AtividadesColetivas");
               foreach (System.Data.DataRow row in dt.Rows)
                {
                    var h = new atvdd();
                    h.Cod = row["Cod"].ToString();
                    h.Dt_atendimento = row["dt_atendimento"].ToString();
                    h.Mes = row["mes"].ToString();
                    h.Cnes = row["Cnes"].ToString();
                    h.Unidade = row["Unidade"].ToString();
                    h.Profissional = row["Profissional"].ToString();
                    h.Cbo = row["Cbo"].ToString();
                    h.Tipo_Atividade = row["Tipo_Atividade"].ToString();
                    h.st_tema_saude_comb_aedes_aegyp = row["st_tema_saude_comb_aedes_aegyp"].ToString();
                    h.st_tema_saude_agravos_negligen = row["st_tema_saude_agravos_negligen"].ToString();
                    h.st_tema_saude_aliment_saudavel = row["st_tema_saude_aliment_saudavel"].ToString();
                    h.st_tema_saude_pess_doenc_croni = row["st_tema_saude_pess_doenc_croni"].ToString();
                    h.st_tema_saude_cidad_dirt_human = row["st_tema_saude_cidad_dirt_human"].ToString();
                    h.st_tema_saude_dependen_quimica = row["st_tema_saude_dependen_quimica"].ToString();
                    h.st_tema_saude_envelhecimento = row["st_tema_saude_envelhecimento"].ToString();
                    h.st_tema_saude_pant_medic_fitot = row["st_tema_saude_pant_medic_fitot"].ToString();
                    h.st_tema_saude_preven_violencia = row["st_tema_saude_preven_violencia"].ToString();
                    h.st_tema_saude_saude_ambiental = row["st_tema_saude_saude_ambiental"].ToString();
                    h.st_tema_saude_saude_bucal = row["st_tema_saude_saude_bucal"].ToString();
                    h.st_tema_saude_saude_trabalhad = row["st_tema_saude_saude_trabalhad"].ToString();
                    h.st_tema_saude_saude_mental = row["st_tema_saude_saude_mental"].ToString();
                    h.st_tema_saude_saude_sex_repro = row["st_tema_saude_saude_sex_repro"].ToString();
                    h.st_tema_saude_seman_saud_esco = row["st_tema_saude_seman_saud_esco"].ToString();
                    h.st_tema_saude_outros = row["st_tema_saude_outros"].ToString();
                    h.st_prat_saude_antropometria = row["st_prat_saude_antropometria"].ToString();
                    h.st_prat_saude_aplic_topi_fluor = row["st_prat_saude_aplic_topi_fluor"].ToString();
                    h.st_prat_saude_desenv_linguagem = row["st_prat_saude_desenv_linguagem"].ToString();
                    h.st_prat_saude_escov_supervisio = row["st_prat_saude_escov_supervisio"].ToString();
                    h.st_prat_saude_prt_corp_atv_fis = row["st_prat_saude_prt_corp_atv_fis"].ToString();
                    h.st_prat_saude_pnct_1 = row["st_prat_saude_pnct_1"].ToString();
                    h.st_prat_saude_pnct_2 = row["st_prat_saude_pnct_2"].ToString();
                    h.st_prat_saude_pnct_3 = row["st_prat_saude_pnct_3"].ToString();
                    h.st_prat_saude_pnct_4 = row["st_prat_saude_pnct_4"].ToString();
                    h.st_prat_saude_saude_auditiva = row["st_prat_saude_saude_auditiva"].ToString();
                    h.st_prat_saude_saude_ocular = row["st_prat_saude_saude_ocular"].ToString();
                    h.st_prat_saude_situacao_vacinal = row["st_prat_saude_situacao_vacinal"].ToString();
                    h.st_prat_saude_outras = row["st_prat_saude_outras"].ToString();
                    h.st_prat_saude_outro_procedimen = row["st_prat_saude_outro_procedimen"].ToString();
                    h.co_dim_procedimento = row["co_dim_procedimento"].ToString();

                    string chave = h.Cnes +" " +h.Mes;
                    if(lista.ContainsKey(chave))
                    {
                        lista[chave].Gerenciar(h);
                    }
                    else
                    {
                        var novo = new Atividade();
                        novo.Gerenciar(h);
                        lista.Add(chave, novo);
                    }
                }

            }
            catch (Exception ex) { return;  }
        }

        private string QueryBuscaAtividades()
        {
            string query = @"SELECT
t37.co_seq_fat_atividade_coletiva as Cod,
to_char(to_date(t37.co_dim_tempo::text, 'YYYYMMDD'),'dd/mm/yyyy') as dt_atendimento,
date_part('month',to_date(t37.co_dim_tempo::text, 'YYYYMMDD')) as mes,
t99.nu_cnes as Cnes,
t99.no_unidade_saude as Unidade,
t72.no_profissional as Profissional,
tb_dim_cbo.no_cbo as Cbo,
t40.ds_tipo_atividade as Tipo_Atividade,
/*t37.ds_filtro_tema_reuniao,*/
t37.ds_filtro_tema_reuniao,
t37.ds_filtro_tema_para_saude,
t37.ds_filtro_public_alvo,
t37.ds_filtro_pratica_em_saude,
t37.co_dim_inep,
t38.co_inep_escola,
CASE WHEN t39.st_tema_saude_comb_aedes_aegyp = '1' THEN '1' ELSE '' END st_tema_saude_comb_aedes_aegyp,
CASE WHEN t39.st_tema_saude_agravos_negligen = '1' THEN '1' ELSE '' END st_tema_saude_agravos_negligen,
CASE WHEN t39.st_tema_saude_aliment_saudavel = '1' THEN '1' ELSE '' END st_tema_saude_aliment_saudavel,
CASE WHEN t39.st_tema_saude_pess_doenc_croni = '1' THEN '1' ELSE '' END st_tema_saude_pess_doenc_croni,
CASE WHEN t39.st_tema_saude_cidad_dirt_human = '1' THEN '1' ELSE '' END st_tema_saude_cidad_dirt_human,
CASE WHEN t39.st_tema_saude_dependen_quimica = '1' THEN '1' ELSE '' END st_tema_saude_dependen_quimica,
CASE WHEN t39.st_tema_saude_envelhecimento = '1' THEN '1' ELSE '' END st_tema_saude_envelhecimento,
CASE WHEN t39.st_tema_saude_pant_medic_fitot = '1' THEN '1' ELSE '' END st_tema_saude_pant_medic_fitot,
CASE WHEN t39.st_tema_saude_preven_violencia = '1' THEN '1' ELSE '' END st_tema_saude_preven_violencia,
CASE WHEN t39.st_tema_saude_saude_ambiental = '1' THEN '1' ELSE '' END st_tema_saude_saude_ambiental,
CASE WHEN t39.st_tema_saude_saude_bucal = '1' THEN '1' ELSE '' END st_tema_saude_saude_bucal,
CASE WHEN t39.st_tema_saude_saude_trabalhad = '1' THEN '1' ELSE '' END st_tema_saude_saude_trabalhad,
CASE WHEN t39.st_tema_saude_saude_mental = '1' THEN '1' ELSE '' END st_tema_saude_saude_mental,
CASE WHEN t39.st_tema_saude_saude_sex_repro = '1' THEN '1' ELSE '' END st_tema_saude_saude_sex_repro,
CASE WHEN t39.st_tema_saude_seman_saud_esco = '1' THEN '1' ELSE '' END st_tema_saude_seman_saud_esco,
CASE WHEN t39.st_tema_saude_outros = '1' THEN '1' ELSE '' END st_tema_saude_outros,
CASE WHEN t39.st_prat_saude_antropometria = '1' THEN '1' ELSE '' END st_prat_saude_antropometria,
CASE WHEN t39.st_prat_saude_aplic_topi_fluor = '1' THEN '1' ELSE '' END st_prat_saude_aplic_topi_fluor,
CASE WHEN t39.st_prat_saude_desenv_linguagem = '1' THEN '1' ELSE '' END st_prat_saude_desenv_linguagem,
CASE WHEN t39.st_prat_saude_escov_supervisio = '1' THEN '1' ELSE '' END st_prat_saude_escov_supervisio,
CASE WHEN t39.st_prat_saude_prt_corp_atv_fis = '1' THEN '1' ELSE '' END st_prat_saude_prt_corp_atv_fis,
CASE WHEN t39.st_prat_saude_pnct_1 = '1' THEN '1' ELSE '' END st_prat_saude_pnct_1,
CASE WHEN t39.st_prat_saude_pnct_2 = '1' THEN '1' ELSE '' END st_prat_saude_pnct_2,
CASE WHEN t39.st_prat_saude_pnct_3 = '1' THEN '1' ELSE '' END st_prat_saude_pnct_3,
CASE WHEN t39.st_prat_saude_pnct_4 = '1' THEN '1' ELSE '' END st_prat_saude_pnct_4,
CASE WHEN t39.st_prat_saude_saude_auditiva = '1' THEN '1' ELSE '' END st_prat_saude_saude_auditiva,
CASE WHEN t39.st_prat_saude_saude_ocular = '1' THEN '1' ELSE '' END st_prat_saude_saude_ocular,
CASE WHEN t39.st_prat_saude_situacao_vacinal = '1' THEN '1' ELSE '' END st_prat_saude_situacao_vacinal,
CASE WHEN t39.st_prat_saude_outras = '1' THEN '1' ELSE '' END st_prat_saude_outras,
CASE WHEN t39.st_prat_saude_outro_procedimen = '1' THEN '1' ELSE '' END st_prat_saude_outro_procedimen,
CASE WHEN t39.co_dim_procedimento = '1' THEN '1' ELSE '' END co_dim_procedimento
FROM tb_fat_atividade_coletiva t37
left join tb_cds_ficha_ativ_col t38 on t37.nu_uuid_ficha = t38.co_unico_ficha
left join tb_fat_atvdd_coletiva_ext t39 on t37.co_seq_fat_atividade_coletiva = t39.co_fat_atividade_coletiva
left join tb_dim_tipo_atividade t40 on t37.co_dim_tipo_atividade = t40.co_seq_dim_tipo_atividade
left join tb_dim_profissional t72 on t37.co_dim_profissional = t72.co_seq_dim_profissional
left join tb_dim_unidade_saude t99 on t37.co_dim_unidade_saude= t99.co_seq_dim_unidade_saude
LEFT JOIN tb_dim_cbo ON t37.co_dim_cbo = tb_dim_cbo.co_seq_dim_cbo";           
            query += @" WHERE t37.co_dim_tempo >= '" + dt1 + "' AND t37.co_dim_tempo <= '" + dt2 + "'";
            query += @" Order By t37.co_dim_tempo";

            return query;
        }

        private string ApenasTemas()
        {
            string query = @"Select
t37.co_seq_fat_atividade_coletiva as Cod,
to_char(to_date(t37.co_dim_tempo::text, 'YYYYMMDD'),'dd/mm/yyyy') as dt_atendimento,
date_part('month',to_date(t37.co_dim_tempo::text, 'YYYYMMDD')) as mes,
t99.nu_cnes as Cnes,
t99.no_unidade_saude as Unidade,
t72.no_profissional as Profissional,
tb_dim_cbo.no_cbo as Cbo,
t40.ds_tipo_atividade as Tipo_Atividade,
t37.co_dim_inep,
t38.co_inep_escola,
CASE WHEN t39.st_tema_saude_comb_aedes_aegyp = '1' THEN '1' ELSE '' END st_tema_saude_comb_aedes_aegyp,
CASE WHEN t39.st_tema_saude_agravos_negligen = '1' THEN '1' ELSE '' END st_tema_saude_agravos_negligen,
CASE WHEN t39.st_tema_saude_aliment_saudavel = '1' THEN '1' ELSE '' END st_tema_saude_aliment_saudavel,
CASE WHEN t39.st_tema_saude_pess_doenc_croni = '1' THEN '1' ELSE '' END st_tema_saude_pess_doenc_croni,
CASE WHEN t39.st_tema_saude_cidad_dirt_human = '1' THEN '1' ELSE '' END st_tema_saude_cidad_dirt_human,
CASE WHEN t39.st_tema_saude_dependen_quimica = '1' THEN '1' ELSE '' END st_tema_saude_dependen_quimica,
CASE WHEN t39.st_tema_saude_envelhecimento = '1' THEN '1' ELSE '' END st_tema_saude_envelhecimento,
CASE WHEN t39.st_tema_saude_pant_medic_fitot = '1' THEN '1' ELSE '' END st_tema_saude_pant_medic_fitot,
CASE WHEN t39.st_tema_saude_preven_violencia = '1' THEN '1' ELSE '' END st_tema_saude_preven_violencia,
CASE WHEN t39.st_tema_saude_saude_ambiental = '1' THEN '1' ELSE '' END st_tema_saude_saude_ambiental,
CASE WHEN t39.st_tema_saude_saude_bucal = '1' THEN '1' ELSE '' END st_tema_saude_saude_bucal,
CASE WHEN t39.st_tema_saude_saude_trabalhad = '1' THEN '1' ELSE '' END st_tema_saude_saude_trabalhad,
CASE WHEN t39.st_tema_saude_saude_mental = '1' THEN '1' ELSE '' END st_tema_saude_saude_mental,
CASE WHEN t39.st_tema_saude_saude_sex_repro = '1' THEN '1' ELSE '' END st_tema_saude_saude_sex_repro,
CASE WHEN t39.st_tema_saude_seman_saud_esco = '1' THEN '1' ELSE '' END st_tema_saude_seman_saud_esco,
CASE WHEN t39.st_tema_saude_outros = '1' THEN '1' ELSE '' END st_tema_saude_outros,
CASE WHEN t39.st_prat_saude_antropometria = '1' THEN '1' ELSE '' END st_prat_saude_antropometria,
CASE WHEN t39.st_prat_saude_aplic_topi_fluor = '1' THEN '1' ELSE '' END st_prat_saude_aplic_topi_fluor,
CASE WHEN t39.st_prat_saude_desenv_linguagem = '1' THEN '1' ELSE '' END st_prat_saude_desenv_linguagem,
CASE WHEN t39.st_prat_saude_escov_supervisio = '1' THEN '1' ELSE '' END st_prat_saude_escov_supervisio,
CASE WHEN t39.st_prat_saude_prt_corp_atv_fis = '1' THEN '1' ELSE '' END st_prat_saude_prt_corp_atv_fis,
CASE WHEN t39.st_prat_saude_pnct_1 = '1' THEN '1' ELSE '' END st_prat_saude_pnct_1,
CASE WHEN t39.st_prat_saude_pnct_2 = '1' THEN '1' ELSE '' END st_prat_saude_pnct_2,
CASE WHEN t39.st_prat_saude_pnct_3 = '1' THEN '1' ELSE '' END st_prat_saude_pnct_3,
CASE WHEN t39.st_prat_saude_pnct_4 = '1' THEN '1' ELSE '' END st_prat_saude_pnct_4,
CASE WHEN t39.st_prat_saude_saude_auditiva = '1' THEN '1' ELSE '' END st_prat_saude_saude_auditiva,
CASE WHEN t39.st_prat_saude_saude_ocular = '1' THEN '1' ELSE '' END st_prat_saude_saude_ocular,
CASE WHEN t39.st_prat_saude_situacao_vacinal = '1' THEN '1' ELSE '' END st_prat_saude_situacao_vacinal,
CASE WHEN t39.st_prat_saude_outras = '1' THEN '1' ELSE '' END st_prat_saude_outras,
CASE WHEN t39.st_prat_saude_outro_procedimen = '1' THEN '1' ELSE '' END st_prat_saude_outro_procedimen,
CASE WHEN t39.co_dim_procedimento = '1' THEN '1' ELSE '' END co_dim_procedimento
FROM tb_fat_atividade_coletiva t37
left join tb_cds_ficha_ativ_col t38 on t37.nu_uuid_ficha = t38.co_unico_ficha
left join tb_fat_atvdd_coletiva_ext t39 on t37.co_seq_fat_atividade_coletiva = t39.co_fat_atividade_coletiva
left join tb_dim_tipo_atividade t40 on t37.co_dim_tipo_atividade = t40.co_seq_dim_tipo_atividade
left join tb_dim_profissional t72 on t37.co_dim_profissional = t72.co_seq_dim_profissional
left join tb_dim_unidade_saude t99 on t37.co_dim_unidade_saude= t99.co_seq_dim_unidade_saude
LEFT JOIN tb_dim_cbo ON t37.co_dim_cbo = tb_dim_cbo.co_seq_dim_cbo";
            query += @" WHERE t37.co_dim_tempo >= '" + dt1 + "' AND t37.co_dim_tempo <= '" + dt2 + "'";
            query += @" Order By t37.co_dim_tempo";

            return query;
        }
    }

    public class Atividade
    {
        
        public List<atvdd> lista_atividades = new List<atvdd>();


        public int reuniao;
        public int educacao;
        public int avaliacao;
        public int grupo;
        public int mobilizacao;
        public int st_tema_saude_comb_aedes_aegyp { get; set; }
        public int st_tema_saude_agravos_negligen { get; set; }
        public int st_tema_saude_aliment_saudavel { get; set; }
        public int st_tema_saude_pess_doenc_croni { get; set; }
        public int st_tema_saude_cidad_dirt_human { get; set; }
        public int st_tema_saude_dependen_quimica { get; set; }
        public int st_tema_saude_envelhecimento { get; set; }
        public int st_tema_saude_pant_medic_fitot { get; set; }
        public int st_tema_saude_preven_violencia { get; set; }
        public int st_tema_saude_saude_ambiental { get; set; }
        public int st_tema_saude_saude_bucal { get; set; }
        public int st_tema_saude_saude_trabalhad { get; set; }
        public int st_tema_saude_saude_mental { get; set; }
        public int st_tema_saude_saude_sex_repro { get; set; }
        public int st_tema_saude_seman_saud_esco { get; set; }
        public int st_tema_saude_outros { get; set; }
        public int st_prat_saude_antropometria { get; set; }
        public int st_prat_saude_aplic_topi_fluor { get; set; }
        public int st_prat_saude_desenv_linguagem { get; set; }
        public int st_prat_saude_escov_supervisio { get; set; }
        public int st_prat_saude_prt_corp_atv_fis { get; set; }
        public int st_prat_saude_pnct_1 { get; set; }
        public int st_prat_saude_pnct_2 { get; set; }
        public int st_prat_saude_pnct_3 { get; set; }
        public int st_prat_saude_pnct_4 { get; set; }
        public int st_prat_saude_saude_auditiva { get; set; }
        public int st_prat_saude_saude_ocular { get; set; }
        public int st_prat_saude_situacao_vacinal { get; set; }
        public int st_prat_saude_outras { get; set; }
        public int st_prat_saude_outro_procedimen { get; set; }
        public int co_dim_procedimento { get; set; }
        public void Gerenciar(atvdd h)
        {
            lista_atividades.Add(h);
            if (h.Tipo_Atividade.Contains("Reuni")) { reuniao++; return; }
            if (h.Tipo_Atividade.Contains("Educa")) { educacao++; return; }
            if (h.Tipo_Atividade.Contains("grupo")) { grupo++; return; }
            if (h.Tipo_Atividade.Contains("Avalia")) { avaliacao++; return; }
            if (h.Tipo_Atividade.Contains("Mobiliza")) { mobilizacao++; return; }
        }        
    }

    public class atvdd
    {
        public string Cod { get; set; }
        public string Dt_atendimento { get; set; }
        public string Mes { get; set; }
        public string Turno { get; set; }
        public string Cnes { get; set; }
        public string Unidade { get; set; }
        public string Profissional { get; set; }
        public string Cbo { get; set; }
        public string Tipo_Atividade { get; set; }
        public string st_tema_saude_comb_aedes_aegyp { get; set; }
        public string st_tema_saude_agravos_negligen { get; set; }
        public string st_tema_saude_aliment_saudavel { get; set; }
        public string st_tema_saude_pess_doenc_croni { get; set; }
        public string st_tema_saude_cidad_dirt_human { get; set; }
        public string st_tema_saude_dependen_quimica { get; set; }
        public string st_tema_saude_envelhecimento { get; set; }
        public string st_tema_saude_pant_medic_fitot { get; set; }
        public string st_tema_saude_preven_violencia { get; set; }
        public string st_tema_saude_saude_ambiental { get; set; }
        public string st_tema_saude_saude_bucal { get; set; }
        public string st_tema_saude_saude_trabalhad { get; set; }
        public string st_tema_saude_saude_mental { get; set; }
        public string st_tema_saude_saude_sex_repro { get; set; }
        public string st_tema_saude_seman_saud_esco { get; set; }
        public string st_tema_saude_outros { get; set; }
        public string st_prat_saude_antropometria { get; set; }
        public string st_prat_saude_aplic_topi_fluor { get; set; }
        public string st_prat_saude_desenv_linguagem { get; set; }
        public string st_prat_saude_escov_supervisio { get; set; }
        public string st_prat_saude_prt_corp_atv_fis { get; set; }
        public string st_prat_saude_pnct_1 { get; set; }
        public string st_prat_saude_pnct_2 { get; set; }
        public string st_prat_saude_pnct_3 { get; set; }
        public string st_prat_saude_pnct_4 { get; set; }
        public string st_prat_saude_saude_auditiva { get; set; }
        public string st_prat_saude_saude_ocular { get; set; }
        public string st_prat_saude_situacao_vacinal { get; set; }
        public string st_prat_saude_outras { get; set; }
        public string st_prat_saude_outro_procedimen { get; set; }
        public string co_dim_procedimento { get; set; }

    }
}
