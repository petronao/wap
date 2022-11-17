using System;
using System.Collections.Generic;
using System.Text;

namespace WinFormsApp1
{
    public class Odonto
    {
        public void BuscaSimples(string complemento)
        {
            var c = new Conexao();
            var dt = c.Get_DataTable(BuscaFichas(complemento));
            if (dt == null) { return; }
            Geral.WriteToCsvFile(dt, "Odontologia");

        }

        string BuscaFichas(string complemento)
        {
            string texto = @"select
--tf.co_dim_tempo as dt_atendimento,
to_char(to_date(tf.co_dim_tempo::text, 'YYYYMMDD'),'dd/mm/yyyy') as Dt_atendimento,
date_part('month',to_date(tf.co_dim_tempo::text, 'YYYYMMDD')) as mes,
tb_fat_cidadao_pec.no_cidadao as Nome,
/*co_fat_cidadao_pec,*/
(select aa.ds_filtro from tb_dim_tipo_ficha aa where tf.co_dim_tipo_ficha = aa.co_seq_dim_tipo_ficha) as ficha,
(select tp.no_profissional from tb_dim_profissional tp where tf.co_dim_profissional_1 = tp.co_seq_dim_profissional) as profissional,
(select cbo.no_cbo from tb_dim_cbo cbo where tf.co_dim_cbo_1 = cbo.co_seq_dim_cbo) as cbo,
tf.co_dim_unidade_saude_1,
tb_dim_equipe.no_equipe,
tt.no_equipe,
tf.nu_cns as cns,
tf.st_gestante
from
tb_fat_atendimento_odonto tf
left join tb_fat_cidadao_pec ON tf.co_fat_cidadao_pec = tb_fat_cidadao_pec.co_seq_fat_cidadao_pec
LEFT JOIN tb_dim_equipe on tf.co_dim_equipe_1 = tb_dim_equipe.co_seq_dim_equipe
LEFT JOIN tb_dim_equipe tt on tf.co_dim_equipe_2 = tt.co_seq_dim_equipe" + complemento + "ORDER BY tf.co_dim_tempo";
            return texto;

        }
    }
}
