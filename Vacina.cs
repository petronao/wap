using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinFormsApp1
{
 /*   "25"	"dT"	"Dupla adulto"
"9"	"HB"	"Hepatite B"
"24"	"SCR"	"Tríplice viral"
"23"	"IGRH"	"Imunoglobulina anti rábica"
"67"	"HPV Quadri"	"HPV Quadrivalente"
"57"	"dTpa adulto"	"Tríplice bacteriana acelular (adulto) - dTpa"
"34"	"VARC"	"Varicela (atenuada)"
"42"	"Penta"	"DTP / HB / Hib"
"22"	"VIP"	"Poliomielite inativada"
"26"	"Pncc10V"	"Pneumocócica 10V"
"45"	"VRH"	"Vacina rotavírus humano"
"28"	"VOP"	"Poliomielite oral (Bivalente)"
"55"	"HAped"	"Hepatite A Pediátrica"
"33"	"FLU3V"	"Influenza Trivalente"
"14"	"FA"	"Febre amarela"
"41"	"Men Conj C"	"Meningocócica conjugada C"
"35"	"HA"	"Hepatite A (CRIE)"
"37"	"Vero"	"Raiva em cultivo celular Vero"
"47"	"DTPa"	"Tríplice acelular infantil"
"46"	"DTP"	"Tríplice bacteriana"
"21"	"Pncc23V"	"Pneumocócica 23V"
"74"	"MenACWY"	"Meningocócica ACWY"
"15"	"BCG"	"BCG"
"17"	"Hib"	"Haemophilus tipo b"
"59"	"Pncc13V"	"Pneumocócica 13V"
/*
Código 42 DTP/HB/Hib (Pentavalente)
Código 09 hepatite B; (HB)
Código 17 haemophilos tipo b (Hib);
Código 22 poliomielite inativada (VIP);
Código 29 pentavalente acelular (DTPa / Hib / Pólio Inativa);
Código 39 tetravalente (DTP + Hib);
Código 42 Pentavalente celular (DTP/HB/Hib);
Código 43 hexavalente (DTPa+ HB+ Hib +VIP);
Código 46 tríplice bacteriana (DTP). */

    public class Vacinacao
    {
        readonly List<Vacina> lista_Vacinas;        
        readonly Dictionary<string,Pessoa> Lista_Pessoas;
        readonly Dictionary<string, Gerenciar> Lista_G;

        public Vacinacao()
            {
            lista_Vacinas = new List<Vacina>();            
            Lista_Pessoas = new Dictionary<string,Pessoa>();
            Lista_G = new Dictionary<string, Gerenciar>();
        }
        public void BuscaSimples(string complemento)
        {         
            var c = new Conexao();
            var dt = c.Get_DataTable(BuscaFichasVacinas(complemento));
            if (dt == null) { return; }
            Geral.WriteToCsvFile(dt, "vacinacao");
        }
        public void BuscaPessoas()
        {
            try
            {
                var c = new Conexao();
                var dt = c.Get_DataTable(Geral.BuscaCadastrosCDS());
                if (dt == null) { return; }
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
                    h.dias = dr["dias"].ToString();
                    h.cns= dr["cns"].ToString();
                    h.fatcns = dr["fatcns"].ToString();
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
                    if(h.idade < 3)
                    {
                        if (Lista_G.ContainsKey(h.co_fat_cidadao_pec)) { }
                    else {
                        Gerenciar g = new Gerenciar();
                        g.pessoa = h;
                        Lista_G.Add(h.co_fat_cidadao_pec, g);                        
                        }                    
                    }
                }
            }
            catch (Exception ex) { }
        }
        public void Busca(string complemento)
        {
            BuscaPessoas();
            // BuscaFichasVacinas(complemento);
            BuscaFichasVacinasIndicador5(complemento);
            Processamento();
            
        }
        string BuscaFichasVacinas(string complemento)
        {           
                string texto = @"select
/*v.co_dim_tempo as Dt_atendimento,*/
to_char(to_date(v.co_dim_tempo::text, 'YYYYMMDD'),'dd/mm/yyyy') as Dt_atendimento,
date_part('month',to_date(v.co_dim_tempo::text, 'YYYYMMDD')) as mes,
CASE 
WHEN v.co_dim_tipo_ficha = 9 THEN 'PEC'
WHEN v.co_dim_tipo_ficha = 4 THEN 'CDS'
WHEN v.co_dim_tipo_ficha = 15 THEN 'CDS'
ELSE 'OUTRO'
END as Ficha,
CASE 
WHEN v.co_dim_turno = 2 THEN 'dia'
WHEN v.co_dim_turno = 3 THEN 'tarde'
ELSE 'noite'
END as Turno,
tb_dim_profissional.no_profissional as Profissional,
tb_dim_cbo.no_cbo as Cbo,
tb_dim_equipe.nu_ine as Ine,
tb_dim_equipe.no_equipe as Equipe,
tb_dim_unidade_saude.nu_cnes as Cnes,
tb_dim_unidade_saude.no_unidade_saude as Unidade,
tb_fat_cidadao_pec.no_cidadao as Nome,
CASE WHEN v.nu_cpf_cidadao IS NULL THEN '0' ELSE v.nu_cpf_cidadao END Cpf,
v.nu_cns as Cns,
to_char(v.dt_nascimento, 'DD/MM/YYYY') as Dt_nascimento,
date_part('year',age(v.dt_nascimento)) as Idade,
CASE WHEN v.co_dim_sexo = 1 THEN 'M' ELSE 'F' END Sexo,
CASE WHEN v.st_gestante = 1 THEN 'Sim' ELSE 'Nao' END Ges,
v.ds_filtro_imunobiologico as Imunobiologico,
v.ds_filtro_dose_imunobiologico as Dose,
v.co_fat_cidadao_pec as Co_fat_cidadao_pec
from tb_fat_vacinacao v
left join tb_dim_profissional ON tb_dim_profissional.co_seq_dim_profissional = v.co_dim_profissional
left join tb_fat_cidadao_pec ON v.co_fat_cidadao_pec = tb_fat_cidadao_pec.co_seq_fat_cidadao_pec
LEFT JOIN tb_dim_equipe ON tb_dim_equipe.co_seq_dim_equipe = v.co_dim_equipe
LEFT JOIN tb_dim_unidade_saude ON tb_dim_unidade_saude.co_seq_dim_unidade_saude = v.co_dim_unidade_saude
LEFT JOIN tb_dim_cbo ON v.co_dim_cbo = tb_dim_cbo.co_seq_dim_cbo " + complemento +" order by v.co_dim_tempo";
            return texto;
        }

        void BuscaFichasVacinasIndicador5(string complemento)
        {
            try
            {
                string query = @"select
v.nu_uuid_ficha as nid,
to_char(to_date(v.co_dim_tempo::text, 'YYYYMMDD'),'dd/mm/yyyy') as Dt_atendimento,
CASE 
WHEN v.co_dim_tipo_ficha = 9 THEN 'PEC'
WHEN v.co_dim_tipo_ficha = 4 THEN 'CDS'
WHEN v.co_dim_tipo_ficha = 15 THEN 'CDS'
ELSE 'OUTRO'
END as Ficha,
CASE 
WHEN v.co_dim_turno = 2 THEN 'dia'
WHEN v.co_dim_turno = 3 THEN 'tarde'
ELSE 'noite'
END as Turno,
tb_dim_profissional.no_profissional as Profissional,
tb_dim_cbo.no_cbo as Cbo,
tb_dim_equipe.nu_ine as Ine,
tb_dim_equipe.no_equipe as Equipe,
tb_dim_unidade_saude.nu_cnes as Cnes,
tb_dim_unidade_saude.no_unidade_saude as Unidade,
tb_fat_cidadao_pec.no_cidadao as Nome,
CASE WHEN v.nu_cpf_cidadao IS NULL THEN '0' ELSE v.nu_cpf_cidadao END Cpf,
v.nu_cns as Cns,
to_char(v.dt_nascimento, 'DD/MM/YYYY') as Dt_nascimento,
date_part('year',age(v.dt_nascimento)) as Idade,
CASE WHEN v.co_dim_sexo = 1 THEN 'M' ELSE 'F' END Sexo,
CASE WHEN v.st_gestante = 1 THEN 'Sim' ELSE 'Nao' END Ges,
v.ds_filtro_imunobiologico as Imunobiologico,
v.ds_filtro_dose_imunobiologico as Dose,
v.co_fat_cidadao_pec as Co_fat_cidadao_pec
from tb_fat_vacinacao v
left join tb_dim_profissional ON tb_dim_profissional.co_seq_dim_profissional = v.co_dim_profissional
left join tb_fat_cidadao_pec ON v.co_fat_cidadao_pec = tb_fat_cidadao_pec.co_seq_fat_cidadao_pec
LEFT JOIN tb_dim_equipe ON tb_dim_equipe.co_seq_dim_equipe = v.co_dim_equipe
LEFT JOIN tb_dim_unidade_saude ON tb_dim_unidade_saude.co_seq_dim_unidade_saude = v.co_dim_unidade_saude
LEFT JOIN tb_dim_cbo ON v.co_dim_cbo = tb_dim_cbo.co_seq_dim_cbo WHERE v.ds_filtro_imunobiologico like '%|22|%' or v.ds_filtro_imunobiologico like '%|42|%' order by v.co_dim_tempo";

                var c = new Conexao();                
                var dt = c.Get_DataTable(query);
                if (dt == null) { return; }
                Geral.WriteToCsvFile(dt, "vacinacao_geral");                
                foreach (System.Data.DataRow dr in dt.Rows)
                {
                    var h = new Vacina();
                    h.Dt_atendimento = dr["Dt_atendimento"].ToString();
                    string dataControle = dr["Dt_atendimento"].ToString();
                    h.Ficha = dr["Ficha"].ToString();
                    h.Turno = dr["Turno"].ToString();
                    h.Profissional = dr["Profissional"].ToString();
                    h.Cbo = dr["Cbo"].ToString();
                    h.Ine = dr["Ine"].ToString();
                    h.Equipe = dr["Equipe"].ToString();
                    h.Cnes = dr["Cnes"].ToString();
                    h.Unidade = dr["Unidade"].ToString();
                    h.Nome = dr["Nome"].ToString().ToUpper();
                    h.Cns = dr["Cns"].ToString();
                    h.Cpf = dr["Cpf"].ToString();
                    h.Dt_nascimento = dr["Dt_nascimento"].ToString();
                    h.Idade = Convert.ToByte(dr["Idade"].ToString());
                    h.Sexo = dr["Sexo"].ToString();
                    h.Ges = dr["Ges"].ToString();
                    h.Imunobiologico = dr["Imunobiologico"].ToString();
                    h.Dose = dr["Dose"].ToString();
                    h.Co_fat_cidadao_pec = dr["Co_fat_cidadao_pec"].ToString();
                    xadiciona(h);
                   
                }
            }
            catch (Exception ex) { }
        }

        private void xadiciona(Vacina h)
        {           

            if (Lista_G.ContainsKey(h.Co_fat_cidadao_pec))
            {
                Lista_G[h.Co_fat_cidadao_pec].Vacina(h);
            }
            else
            {
                var p = new Pessoa();
                p.nome = h.Nome +" ##NFCI#";
                p.nascimento = h.Dt_nascimento;
                p.idade = h.Idade;
                p.cns = h.Cns;
                p.sexo = h.Sexo;
                p.co_fat_cidadao_pec = h.Co_fat_cidadao_pec;
                Gerenciar g = new Gerenciar();
                g.pessoa = p;
                Lista_G.Add(p.co_fat_cidadao_pec, g);                
                Lista_G[h.Co_fat_cidadao_pec].Vacina(h);
            }
        }
        void Processamento()
        {
            //var tmp = Lista_Pessoas.Values.Where(x => x.idade < 3).ToList();
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string arquivo = "vacinacao_3dose.csv";
            string path = System.IO.Path.Combine(desktop, arquivo);
            using System.IO.StreamWriter sw = new System.IO.StreamWriter(path);
            string collum_name = "num;nome;cns;cnscpf;dt_nasc;idad;meses;dias;sx;dt_vip;dt_pole;codfat;saida;";
            sw.WriteLine(collum_name);
            int x = 0;
            foreach (var r in Lista_G.Values)
            {               
                string b = "";
                b += x + ";";
                b += r.pessoa.nome + ";";              
                b += r.pessoa.cns + ".;";
                b += r.pessoa.cns_cpf + ".;";
                b += r.pessoa.nascimento + ";";
                b += r.pessoa.idade + ";";
                b += r.pessoa.meses + ";";
                b += r.pessoa.dias + ";";
                b += r.pessoa.sexo + ";";
               
                b += r.vip + ";";
                b += r.pole + ";";
                b += r.pessoa.co_fat_cidadao_pec + ";";
                b += r.pessoa.saida + ";";
                b += r.resumo + "\n ;";
                sw.WriteLine(b);
                x++;
            }
            sw.Close();      
    }

   
       
    }
    public class Vacina
    {        
        internal string Dt_atendimento { get; set; }
        internal string Ficha { get; set; }
        internal string Turno { get; set; }
        public string Profissional { get; set; }
        public string Cbo { get; set; }
        public string Ine { get; set; }
        public string Equipe { get; set; }
        public string Cnes { get; set; }
        public string Unidade { get; set; }
        public string Nome { get; set; }
        public string Cpf { get; set; }
        public string Cns { get; set; }
        
        public string Dt_nascimento { get; set; }
        public byte Idade { get; set; }
        
        public string Sexo { get; set; }

        public string Ges { get; set; }
        public string Imunobiologico { get; set; }
        public string Dose { get; set; }
        public string Co_fat_cidadao_pec { get; set; }
       // public string nu_identificador { get; set; }
        //public string co_dim_dose_imunobiologico { get; set; }
        

    }


    class Gerenciar
    {
        public Pessoa pessoa;        
        public string vip;        
        public string pole;
        public string resumo;


        public void Vacina(Vacina v)
        {
            string dia = v.Dt_atendimento;   //01/05/2021
            string[] tmp = v.Imunobiologico.Split('|');  //  0,0,42,22,15,
            string[] tmpd = v.Dose.Split('|'); //2,1,3,2,1,
            int x = 0;
            foreach(string a in tmp)
            {
                if ((tmp[x] == "42") && (tmpd[x] == "3"))
                { pole += dia; }
                if ((tmp[x] == "22") && (tmpd[x] == "3"))
                { vip += dia; }
                
                x++;
            }
            resumo += "Dt "+ v.Dt_atendimento + " Imuno "+ v.Imunobiologico + " Dose " + v.Dose +"# ";
        }
    }
        
}
