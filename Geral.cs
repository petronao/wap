using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace WinFormsApp1
{
    public static class Geral
    {
        static string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        static string filePath = System.IO.Path.Combine(desktop, "PastaRelatorios");

        private static void CheckFolderExist()
        {
            if (Directory.Exists(filePath))
            {
                // Directory exits!
            }
            else
            {
                System.IO.Directory.CreateDirectory(filePath);
            }
        }

        public static string Get_DateMaskReverse(string DateStringtext)
        {
            DateTime t1 = DateTime.Parse(DateStringtext);
            string dt1 = t1.ToString("yyyy/MM/dd");
            return dt1.Replace("/", "");
        }
        public static DateTime Convert_dataString_data(string dt_dim)
        {
            if (dt_dim.Length == 8)
            {
                string ano = dt_dim.Substring(0, 4);
                string mes = dt_dim.Substring(4, 2);
                string dia = dt_dim.Substring(6, 2);
                return Convert.ToDateTime(dia + "/" + mes + "/" + ano);
            }
            else { return Convert.ToDateTime("31/12/3000"); }

        }
        public static string removerAcentos(string texto)
        {
            string comAcentos = "ÄÅÁÂÀÃäáâàãÉÊËÈéêëèÍÎÏÌíîïìÖÓÔÒÕöóôòõÜÚÛüúûùÇç";
            string semAcentos = "AAAAAAaaaaaEEEEeeeeIIIIiiiiOOOOOoooooUUUuuuuCc";

            for (int i = 0; i < comAcentos.Length; i++)
            {
                texto = texto.Replace(comAcentos[i].ToString(), semAcentos[i].ToString());
            }
            return texto;
        }
        public static void WriteToCsvFile(this System.Data.DataTable dataTable, string fileName)
        {
            // string datae = DateTime.Now.ToString("ddMMyyyyHHmmss");


            StringBuilder fileContent = new StringBuilder();

            foreach (var col in dataTable.Columns)
            {
                fileContent.Append(col.ToString() + ";");
            }

            fileContent.Append(System.Environment.NewLine);

            foreach (System.Data.DataRow dr in dataTable.Rows)
            {
                foreach (var c in dr.ItemArray)
                {
                    //string x = c.ToString()+",";
                    fileContent.Append(c.ToString() + ";");
                }

                fileContent.Append(System.Environment.NewLine);
            }
            CheckFolderExist();

            //System.IO.File.WriteAllText(filePath + "\\" + fileName+ "utf8.csv", fileContent.ToString(), Encoding.Unicode);
            // System.IO.File.WriteAllText(filePath + "\\" + fileName + "x.csv", fileContent.ToString());
            System.IO.File.WriteAllText(filePath + "\\" + fileName + ".csv", fileContent.ToString(), Encoding.GetEncoding("iso-8859-1"));
        }
        public static void ToDataTable<T>(this IList<T> data, string file_name)
        {
            PropertyDescriptorCollection properties =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            WriteToCsvFile(table, file_name);
        }
        public static void SaveFilecsv(List<string> dados, string file_name)
        {
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string arquivo = file_name + ".txt";
            string path = Path.Combine(desktop, arquivo);
            //File.AppendText(path)) continua escrevendo no final do arquivo..           
            File.WriteAllLines(arquivo, dados.Select(x => string.Join(";", x)));


            using StreamWriter sw = new StreamWriter(path);
            for (int x = 0; x < dados.Count; x++)
            {
                sw.WriteLine(dados[x]);
            }
            sw.Close();
        }
        static void queryGestanteRel()
        {

            string q = @"SELECT
t1.co_gestacao,
t1.dt_inicio_gestacao,
t1.dt_inicio_puerperio,
t1.dt_fim_puerperio,
t1.dt_fvd_ultimo,
t2.no_cidadao
FROM tb_fat_rel_op_gestante t1
left join tb_fat_cidadao_pec t2
on t1.co_fat_cidadao_pec = t2.co_seq_fat_cidadao_pec
ORDER BY t2.no_cidadao ASC, t1.co_gestacao ASC";

        }
        public static string query10xxxxxxxx()
        {
            string texto = @"select
        to_char(t1.dt_cad_individual, 'DD/MM/YYYY') as dt_cad,
       /* t1.dt_cad_individual as dt_cad,*/
        t1.no_cidadao as nome,		
		CASE WHEN t2.co_dim_sexo = 1 THEN 'M' ELSE 'F' END sexo,		
		to_char(t2.dt_nascimento, 'DD/MM/YYYY') as dt_nasc,  
        date_part('year',age(t2.dt_nascimento)) as idade,
		t2.nu_cns as FATcns,
		t2.nu_cpf_cidadao as FATcpf,
		t3.nu_cns as cns,
		t3.nu_cpf_cidadao as cpf,		
		t99.nu_cnes as cnes,
		t2.nu_micro_area as mc,
		t1.st_fora_area as fa,		
		t2.st_hipertensao_arterial as has,
		t2.st_diabete as dm,
        t2.st_fumante as tab,
        t2.st_alcool as alc,
		t2.st_gestante as ges,
		t3.co_dim_unidade_saude_vinc as unidade,
		(select kk.no_equipe from tb_dim_equipe kk where t3.co_dim_equipe_vinc = kk.co_seq_dim_equipe )as equipe_vinc,
        (select tp.no_profissional from tb_dim_profissional tp where t2.co_dim_profissional = tp.co_seq_dim_profissional) as acs,
		t3.st_faleceu,
		t2.co_fat_cidadao_pec,
        t2.co_dim_raca_cor,
        t2.nu_cns_responsavel,
        t2.nu_cpf_responsavel,
        t2.st_responsavel_familiar,        
CASE WHEN t2.st_deficiencia = 1 THEN 'Sim' ELSE '' END deficiencia,       
        CASE WHEN t2.st_defi_auditiva = 1 THEN 'auditiva' ELSE '' END auditiva,
CASE WHEN t2.st_defi_intelectual_cognitiva = 1 THEN 'intelectual' ELSE '' END intelectual,        
CASE WHEN t2.st_defi_outra = 1 THEN 'outra' ELSE '' END outra, 
CASE WHEN t2.st_defi_visual = 1 THEN 'visual' ELSE '' END visual,        
CASE WHEN t2.st_defi_fisica = 1 THEN 'fisica' ELSE '' END fisica,
        t1.nu_celular_cidadao
        from 
        tb_fat_cad_individual t2
		left join tb_cds_cad_individual t1 on t1.co_unico_ficha = t2.nu_uuid_ficha
		left join tb_fat_cidadao_pec t3 on t2.co_fat_cidadao_pec = t3.co_seq_fat_cidadao_pec
		left join tb_dim_unidade_saude t99 on t2.co_dim_unidade_saude = t99.co_seq_dim_unidade_saude
		where
		(t1.st_versao_atual = 1)        
        AND (t1.st_ficha_inativa = 0)         
        AND (t1.st_fora_area = 0)
		AND (t2.co_dim_tipo_saida_cadastro = 3)";
            return texto;
        }


        public static string BuscaCadastrosCDS()
        {
            string query = @"select
to_char(t1.dt_cad_individual, 'DD/MM/YYYY') as dt_cad,
CASE WHEN t2.st_responsavel_familiar = 1 THEN 'sim' ELSE 'nao' END responsavel,
t3.no_cidadao as nome,
to_char(t2.dt_nascimento, 'DD/MM/YYYY') as dt_nasc,
date_part('year',age(t2.dt_nascimento)) as idade,
date_part('month', age(t2.dt_nascimento)) as meses,
date_part('day', age(t2.dt_nascimento)) as dias,
CASE
    WHEN t2.co_dim_sexo = 1 THEN 'M'
    ELSE 'F'
END sexo,
CASE WHEN t2.nu_cns = '0' THEN t2.nu_cpf_cidadao ELSE t2.nu_cns END cns_cpf,
t2.nu_cns as cns,
t2.nu_cpf_cidadao as cpf,
t3.nu_cns as fatcns,
t3.nu_cpf_cidadao as fatcpf,
CASE WHEN t2.st_hipertensao_arterial = 1 THEN 'has' ELSE '' END has,
CASE WHEN t2.st_diabete = 1 THEN 'dm' ELSE '' END dm,
CASE WHEN t2.st_fumante = 1 THEN 'tab' ELSE '' END tab,
CASE WHEN t2.st_alcool = 1 THEN 'alc' ELSE '' END alc,
CASE WHEN t2.st_gestante = 1 THEN 'ges' ELSE '' END ges,
t99.nu_cnes as cnes,
t99.no_unidade_saude as unidade,
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
tb_fat_cad_individual t2
left join tb_cds_cad_individual t1 on t1.co_unico_ficha = t2.nu_uuid_ficha
left join tb_fat_cidadao_pec t3 on t2.co_fat_cidadao_pec = t3.co_seq_fat_cidadao_pec
left join tb_dim_unidade_saude t99 on t2.co_dim_unidade_saude = t99.co_seq_dim_unidade_saude
left join tb_dim_equipe t98 on t2.co_dim_equipe = t98.co_seq_dim_equipe";
//where ";
//query += condicoes;
            return query;
        }


    }
}
