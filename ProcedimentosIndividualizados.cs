using System;
using System.Collections.Generic;
using System.Text;

namespace WinFormsApp1
{
    public class ProcedimentosIndividualizados
    {

        public void BuscaSimples(string complemento)
        {
            var c = new Conexao();
            var dt = c.Get_DataTable(BuscaFichas(complemento));
            if (dt == null) { return; }
            Geral.WriteToCsvFile(dt, "ProcedimentosIndividualizados");

        }

            string BuscaFichas(string complemento)
        {
            string texto = @"SELECT
tb_dim_cbo.no_cbo as cbo,
tb_dim_profissional.no_profissional,
tb_dim_equipe.no_equipe as equipe,
tb_dim_unidade_saude.nu_cnes as cnes,
tb_dim_unidade_saude.no_unidade_saude as unidade,
CASE WHEN tf.nu_cns IS NULL THEN '0' ELSE tf.nu_cns END cns,
CASE WHEN tf.nu_cpf_cidadao IS NULL THEN '0' ELSE tf.nu_cpf_cidadao END cpf,
tdtf.ds_tipo_ficha as ficha,
tb_dim_turno.ds_turno as turno,
to_char(to_date(tf.co_dim_tempo::text, 'YYYYMMDD'),'dd/mm/yyyy') as dia,
date_part('month',to_date(tf.co_dim_tempo::text, 'YYYYMMDD')) as mes,
tb_fat_cidadao_pec.no_cidadao,
tb_dim_sexo.sg_sexo as sexo,
to_char(tf.dt_nascimento, 'DD/MM/YYYY') as dt_nasc,
date_part('year',age(tf.dt_nascimento)) as idade,
tb_dim_procedimento.ds_proced as procedimento,
CASE WHEN tf.co_fat_cidadao_pec IS NULL THEN '0' ELSE tf.co_fat_cidadao_pec END co_fat_cidadao_pec
FROM
tb_fat_proced_atend_proced tf
LEFT JOIN tb_dim_tipo_ficha tdtf on tf.co_dim_tipo_ficha = tdtf.co_seq_dim_tipo_ficha
LEFT JOIN tb_dim_profissional ON tb_dim_profissional.co_seq_dim_profissional = tf.co_dim_profissional
LEFT JOIN tb_fat_cidadao_pec ON tf.co_fat_cidadao_pec = tb_fat_cidadao_pec.co_seq_fat_cidadao_pec
LEFT JOIN tb_dim_equipe ON tb_dim_equipe.co_seq_dim_equipe = tf.co_dim_equipe
LEFT JOIN tb_dim_unidade_saude ON tb_dim_unidade_saude.co_seq_dim_unidade_saude = tf.co_dim_unidade_saude
LEFT JOIN tb_dim_turno on tf.co_dim_turno = tb_dim_turno.co_seq_dim_turno
LEFT JOIN tb_dim_cbo ON tb_dim_cbo.co_seq_dim_cbo = tf.co_dim_cbo
LEFT JOIN tb_dim_procedimento ON tf.co_dim_procedimento = tb_dim_procedimento.co_seq_dim_procedimento
LEFT JOIN tb_dim_sexo ON tf.co_dim_sexo = tb_dim_sexo.co_seq_dim_sexo" + complemento + "ORDER BY tf.co_dim_tempo";
            return texto;

        }
       
    }
}
