using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace WinFormsApp1
{
	public partial class FormGestantes : Form
	{
		List<Gestante> lista_gestantes_cds = new List<Gestante>();
		List<atendimentos> lista_atendimentos = new List<atendimentos>();
		Dictionary<string, Gestante> lista_gestantes = new Dictionary<string, Gestante>();
		Dictionary<string, string> list02 = new Dictionary<string, string>();
		Dictionary<string, Gestante> list99 = new Dictionary<string, Gestante>();

		public FormGestantes()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			Busca_Gestantes_CDS();
		}


		private void Busca_Gestantes_CDS()
		{
			string query = @"select        
        t1.dt_cad_individual as dt_cad,
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
		CASE WHEN t2.st_gestante = 1 THEN 'ATIVA' ELSE '' END ges,
		t3.co_dim_unidade_saude_vinc as unidade,
		(select kk.no_equipe from tb_dim_equipe kk where t3.co_dim_equipe_vinc = kk.co_seq_dim_equipe )as equipe_vinc,
        (select tp.no_profissional from tb_dim_profissional tp where t2.co_dim_profissional = tp.co_seq_dim_profissional) as acs,
		t2.co_fat_cidadao_pec
        from 
        tb_fat_cad_individual t2
		left join tb_cds_cad_individual t1 on t1.co_unico_ficha = t2.nu_uuid_ficha
		left join tb_fat_cidadao_pec t3 on t2.co_fat_cidadao_pec = t3.co_seq_fat_cidadao_pec
		left join tb_dim_unidade_saude t99 on t2.co_dim_unidade_saude = t99.co_seq_dim_unidade_saude
		where
		(t1.st_versao_atual = 1)        
        AND (t1.st_ficha_inativa = 0)         
        AND (t1.st_fora_area = 0)
		AND (t2.co_dim_tipo_saida_cadastro = 3) AND t2.st_gestante = 1 ORDER BY equipe_vinc , nome";

			if (lista_gestantes_cds.Count > 0) lista_gestantes_cds.Clear(); label1.Text = "";
			if (dataGridView1.Rows.Count > 0) { dataGridView1.DataSource = null; dataGridView1.Rows.Clear(); dataGridView1.Refresh(); }
			Conexao c = new Conexao();
			
			var dt = c.Get_DataTable(query);
			if (dt == null) return;
			foreach (DataRow row in dt.Rows)
			{
				var g = new Gestante();
				g.nome = row["nome"].ToString();
				g.nascimento = row["dt_nasc"].ToString();
				g.cns = row["cns"].ToString();
				g.cpf = row["cpf"].ToString();
				g.ges = row["ges"].ToString();
				g.codigo_fat = row["co_fat_cidadao_pec"].ToString();
				g.acs = row["acs"].ToString();
				g.equipe_vinc = row["equipe_vinc"].ToString();
				lista_gestantes_cds.Add(g);

				if (lista_gestantes.ContainsKey(g.codigo_fat))
				{
					lista_gestantes[g.codigo_fat].ges = g.ges;
				}
				else
				{
					lista_gestantes.Add(g.codigo_fat, g);
				}
			}

			dataGridView1.DataSource = lista_gestantes_cds;
			label1.Text = "" + lista_gestantes_cds.Count;

		}


		// ESTA BUSCA É PARA IDENTIFICAR QUEM TEVE ATENDIMENTO PRÉ NATAL... DURANTE O PERIODO.. JÁ QUE PELO CDS SÓ CONSEGUIMOS PUXAR AS QUE ESTÃO MARCADAS COMO GESTANTE.
		// DEPOIS TEM QUE FAZER UMA BUSCA DE TODOS OS ATENDIMENTOS E PROCEDIMENTOS DE CADA GESTANTE  ENCONTRADA...
		private void Identificar_Gestantes(string dtinicial, string dtfinal)
		{
			if (lista_gestantes.Count > 0) lista_gestantes.Clear();

			string query = @"select
(select cc.no_cidadao from tb_fat_cidadao_pec cc where tf.co_fat_cidadao_pec = cc.co_seq_fat_cidadao_pec) as nome,
CASE WHEN tf.co_fat_cidadao_pec IS NULL THEN '0' ELSE tf.co_fat_cidadao_pec END co_fat_cidadao_pec,
tf.nu_cns as cns,
tf.nu_cpf_cidadao as cpf,
tf.dt_nascimento as dt_nasc
from
tb_fat_atendimento_individual tf
where (tf.co_dim_tempo >= '" + dtinicial + "' AND tf.co_dim_tempo <= '" + dtfinal + "')";
			query += @" AND (
			tf.ds_filtro_ciaps LIKE ANY (
				array[
					'%|W03|%',
					'%|W05|%',
					'%|W29|%',
					'%|W71|%',
					'%|W78|%',
					'%|W79|%',
					'%|W80|%',
					'%|W81|%',
					'%|W84|%',
					'%|W85|%',
					--'%|W72|%',
					--'%|W73|%',
					--'%|W75|%',
					--'%|W76|%',
					'%|ABP001|%'
				]
			) OR
			tf.ds_filtro_cids LIKE ANY (
				array[
					'%|O11|%',
					'%|O120|%',
					'%|O121|%',
					'%|O122|%',
					'%|O13|%',
					'%|O140|%',
					'%|O141|%',
					'%|O149|%',
					'%|O150|%',
					'%|O151|%',
					'%|O159|%',
					'%|O16|%',
					'%|O200|%',
					'%|O208|%',
					'%|O209|%',
					'%|O210|%',
					'%|O211|%',
					'%|O212|%',
					'%|O218|%',
					'%|O219|%',
					'%|O220|%',
					'%|O221|%',
					'%|O222|%',
					'%|O223|%',
					'%|O224|%',
					'%|O225|%',
					'%|O228|%',
					'%|O229|%',
					'%|O230|%',
					'%|O231|%',
					'%|O232|%',
					'%|O233|%',
					'%|O234|%',
					'%|O235|%',
					'%|O239|%',
					'%|O299|%',
					'%|O300|%',
					'%|O301|%',
					'%|O302|%',
					'%|O308|%',
					'%|O309|%',
					'%|O311|%',
					'%|O312|%',
					'%|O318|%',
					'%|O320|%',
					'%|O321|%',
					'%|O322|%',
					'%|O323|%',
					'%|O324|%',
					'%|O325|%',
					'%|O326|%',
					'%|O328|%',
					'%|O329|%',
					'%|O330|%',
					'%|O331|%',
					'%|O332|%',
					'%|O333|%',
					'%|O334|%',
					'%|O335|%',
					'%|O336|%',
					'%|O337|%',
					'%|O338|%',
					'%|O752|%',
					'%|O753|%',
					'%|O990|%',
					'%|O991|%',
					'%|O992|%',
					'%|O993|%',
					'%|O994|%',
					'%|O240|%',
					'%|O241|%',
					'%|O242|%',
					'%|O243|%',
					'%|O244|%',
					'%|O249|%',
					'%|O25|%',
					'%|O260|%',
					'%|O261|%',
					'%|O263|%',
					'%|O264|%',
					'%|O265|%',
					'%|O268|%',
					'%|O269|%',
					'%|O280|%',
					'%|O281|%',
					'%|O282|%',
					'%|O283|%',
					'%|O284|%',
					'%|O285|%',
					'%|O288|%',
					'%|O289|%',
					'%|O290|%',
					'%|O291|%',
					'%|O292|%',
					'%|O293|%',
					'%|O294|%',
					'%|O295|%',
					'%|O296|%',
					'%|O298|%',
					'%|O009|%',
					'%|O339|%',
					'%|O340|%',
					'%|O341|%',
					'%|O342|%',
					'%|O343|%',
					'%|O344|%',
					'%|O345|%',
					'%|O346|%',
					'%|O347|%',
					'%|O348|%',
					'%|O349|%',
					'%|O350|%',
					'%|O351|%',
					'%|O352|%',
					'%|O353|%',
					'%|O354|%',
					'%|O355|%',
					'%|O356|%',
					'%|O357|%',
					'%|O358|%',
					'%|O359|%',
					'%|O360|%',
					'%|O361|%',
					'%|O362|%',
					'%|O363|%',
					'%|O365|%',
					'%|O366|%',
					'%|O367|%',
					'%|O368|%',
					'%|O369|%',
					'%|O40|%',
					'%|O410|%',
					'%|O411|%',
					'%|O418|%',
					'%|O419|%',
					'%|O430|%',
					'%|O431|%',
					'%|O438|%',
					'%|O439|%',
					'%|O440|%',
					'%|O441|%',
					'%|O460|%',
					'%|O468|%',
					'%|O469|%',
					'%|O470|%',
					'%|O471|%',
					'%|O479|%',
					'%|O48|%',
					'%|O995|%',
					'%|O996|%',
					'%|O997|%',
					'%|Z640|%',
					'%|O00|%',
					'%|O10|%',
					'%|O12|%',
					'%|O14|%',
					'%|O15|%',
					'%|O20|%',
					'%|O21|%',
					'%|O22|%',
					'%|O23|%',
					'%|O24|%',
					'%|O26|%',
					'%|O28|%',
					'%|O29|%',
					'%|O30|%',
					'%|O31|%',
					'%|O32|%',
					'%|O33|%',
					'%|O34|%',
					'%|O35|%',
					'%|O36|%',
					'%|O41|%',
					'%|O43|%',
					'%|O44|%',
					'%|O46|%',
					'%|O47|%',
					'%|O98|%',
					'%|Z34|%',
					'%|Z35|%',
					'%|Z36|%',
					'%|Z321|%',
					'%|Z33|%',
					'%|Z340|%',
					'%|Z340|%',
					'%|Z348|%',
					'%|Z349|%',
					'%|Z350|%',
					'%|Z351|%',
					'%|Z352|%',
					'%|Z353|%',
					'%|Z354|%',
					'%|Z357|%',
					'%|Z358|%',
					--'%|Z356|%',
					'%|Z359|%'
				]
			)
		)	
		
ORDER BY tf.co_dim_tempo";

			var c = new Conexao();
			var dt = c.Get_DataTable(query);
			foreach (DataRow row in dt.Rows)
			{
				var g = new Gestante();
				g.nome = row["nome"].ToString();
				g.nascimento = row["dt_nasc"].ToString().Substring(0, 10);
				g.cns = row["cns"].ToString();
				g.cpf = row["cpf"].ToString();
				g.codigo_fat = row["co_fat_cidadao_pec"].ToString();

				if (lista_gestantes.ContainsKey(g.codigo_fat))
				{
				}
				else
				{
					if (g.codigo_fat == "0") // atendimentos sem cns ou cpf são 0
					{
					}
					else
					{
						lista_gestantes.Add(g.codigo_fat, g);
					}
				}
			}
		}
		private string xxxxxxxxxxxxxxxxx(string s1, string s2)
		{
			string texto = @"select
tf.co_dim_tempo as dt_atendimento,
(select cc.no_cidadao from tb_fat_cidadao_pec cc where tf.co_fat_cidadao_pec = cc.co_seq_fat_cidadao_pec) as nome,
CASE WHEN tf.co_fat_cidadao_pec IS NULL THEN '0'
			ELSE tf.co_fat_cidadao_pec END
			co_fat_cidadao_pec,
(select aa.ds_filtro from tb_dim_tipo_ficha aa where tf.co_dim_tipo_ficha = aa.co_seq_dim_tipo_ficha) as ficha,
(select tp.no_profissional from tb_dim_profissional tp where tf.co_dim_profissional_1 = tp.co_seq_dim_profissional) as profissional,
(select cbo.no_cbo from tb_dim_cbo cbo where tf.co_dim_cbo_1 = cbo.co_seq_dim_cbo) as cbo,
tf.co_dim_unidade_saude_1,
tf.nu_cns as cns,
tf.nu_cpf_cidadao as cpf,
tf.dt_nascimento,
tf.ds_filtro_cids as cids,
tf.ds_filtro_ciaps as ciaps,
tf.ds_filtro_proced_solicitados as ps,
tf.ds_filtro_proced_avaliados as pa,
tf.st_vacinacao_em_dia,
tf.co_dim_tempo_dum as dum,
tf.nu_idade_gestacional_semanas,
tf.nu_gestas_previas,
tf.nu_partos
from
tb_fat_atendimento_individual tf
where (tf.co_dim_tempo >= '" + s1 + "' AND tf.co_dim_tempo <= '" + s2 + "')";
			string texto1 = @" AND (
			tf.ds_filtro_ciaps LIKE ANY (
				array[
					'%|W03|%',
					'%|W05|%',
					'%|W29|%',
					'%|W71|%',
					'%|W78|%',
					'%|W79|%',
					'%|W80|%',
					'%|W81|%',
					'%|W84|%',
					'%|W85|%',
					--'%|W72|%',
					--'%|W73|%',
					--'%|W75|%',
					--'%|W76|%',
					'%|ABP001|%'
				]
			) OR
			tf.ds_filtro_cids LIKE ANY (
				array[
					'%|O11|%',
					'%|O120|%',
					'%|O121|%',
					'%|O122|%',
					'%|O13|%',
					'%|O140|%',
					'%|O141|%',
					'%|O149|%',
					'%|O150|%',
					'%|O151|%',
					'%|O159|%',
					'%|O16|%',
					'%|O200|%',
					'%|O208|%',
					'%|O209|%',
					'%|O210|%',
					'%|O211|%',
					'%|O212|%',
					'%|O218|%',
					'%|O219|%',
					'%|O220|%',
					'%|O221|%',
					'%|O222|%',
					'%|O223|%',
					'%|O224|%',
					'%|O225|%',
					'%|O228|%',
					'%|O229|%',
					'%|O230|%',
					'%|O231|%',
					'%|O232|%',
					'%|O233|%',
					'%|O234|%',
					'%|O235|%',
					'%|O239|%',
					'%|O299|%',
					'%|O300|%',
					'%|O301|%',
					'%|O302|%',
					'%|O308|%',
					'%|O309|%',
					'%|O311|%',
					'%|O312|%',
					'%|O318|%',
					'%|O320|%',
					'%|O321|%',
					'%|O322|%',
					'%|O323|%',
					'%|O324|%',
					'%|O325|%',
					'%|O326|%',
					'%|O328|%',
					'%|O329|%',
					'%|O330|%',
					'%|O331|%',
					'%|O332|%',
					'%|O333|%',
					'%|O334|%',
					'%|O335|%',
					'%|O336|%',
					'%|O337|%',
					'%|O338|%',
					'%|O752|%',
					'%|O753|%',
					'%|O990|%',
					'%|O991|%',
					'%|O992|%',
					'%|O993|%',
					'%|O994|%',
					'%|O240|%',
					'%|O241|%',
					'%|O242|%',
					'%|O243|%',
					'%|O244|%',
					'%|O249|%',
					'%|O25|%',
					'%|O260|%',
					'%|O261|%',
					'%|O263|%',
					'%|O264|%',
					'%|O265|%',
					'%|O268|%',
					'%|O269|%',
					'%|O280|%',
					'%|O281|%',
					'%|O282|%',
					'%|O283|%',
					'%|O284|%',
					'%|O285|%',
					'%|O288|%',
					'%|O289|%',
					'%|O290|%',
					'%|O291|%',
					'%|O292|%',
					'%|O293|%',
					'%|O294|%',
					'%|O295|%',
					'%|O296|%',
					'%|O298|%',
					'%|O009|%',
					'%|O339|%',
					'%|O340|%',
					'%|O341|%',
					'%|O342|%',
					'%|O343|%',
					'%|O344|%',
					'%|O345|%',
					'%|O346|%',
					'%|O347|%',
					'%|O348|%',
					'%|O349|%',
					'%|O350|%',
					'%|O351|%',
					'%|O352|%',
					'%|O353|%',
					'%|O354|%',
					'%|O355|%',
					'%|O356|%',
					'%|O357|%',
					'%|O358|%',
					'%|O359|%',
					'%|O360|%',
					'%|O361|%',
					'%|O362|%',
					'%|O363|%',
					'%|O365|%',
					'%|O366|%',
					'%|O367|%',
					'%|O368|%',
					'%|O369|%',
					'%|O40|%',
					'%|O410|%',
					'%|O411|%',
					'%|O418|%',
					'%|O419|%',
					'%|O430|%',
					'%|O431|%',
					'%|O438|%',
					'%|O439|%',
					'%|O440|%',
					'%|O441|%',
					'%|O460|%',
					'%|O468|%',
					'%|O469|%',
					'%|O470|%',
					'%|O471|%',
					'%|O479|%',
					'%|O48|%',
					'%|O995|%',
					'%|O996|%',
					'%|O997|%',
					'%|Z640|%',
					'%|O00|%',
					'%|O10|%',
					'%|O12|%',
					'%|O14|%',
					'%|O15|%',
					'%|O20|%',
					'%|O21|%',
					'%|O22|%',
					'%|O23|%',
					'%|O24|%',
					'%|O26|%',
					'%|O28|%',
					'%|O29|%',
					'%|O30|%',
					'%|O31|%',
					'%|O32|%',
					'%|O33|%',
					'%|O34|%',
					'%|O35|%',
					'%|O36|%',
					'%|O41|%',
					'%|O43|%',
					'%|O44|%',
					'%|O46|%',
					'%|O47|%',
					'%|O98|%',
					'%|Z34|%',
					'%|Z35|%',
					'%|Z36|%',
					'%|Z321|%',
					'%|Z33|%',
					'%|Z340|%',
					'%|Z340|%',
					'%|Z348|%',
					'%|Z349|%',
					'%|Z350|%',
					'%|Z351|%',
					'%|Z352|%',
					'%|Z353|%',
					'%|Z354|%',
					'%|Z357|%',
					'%|Z358|%',
					--'%|Z356|%',
					'%|Z359|%'
				]
			)
		)	
		
ORDER BY tf.co_dim_tempo";
			return texto + texto1;
		}
		private void button2_Click(object sender, EventArgs e)
		{
			label2.Text = "0";
			if (dataGridView2.Rows.Count > 0) { dataGridView2.DataSource = null; dataGridView2.Rows.Clear(); dataGridView2.Refresh(); }

			DateTime t1 = DateTime.Parse(maskedTextBox1.Text);
			DateTime t2 = DateTime.Parse(maskedTextBox2.Text);
			string dt1 = t1.ToString("yyyy/MM/dd");
			string dt2 = t2.ToString("yyyy/MM/dd");
			// primeiramente busca quem foi atendido com os cids e ciaps de gestante...
			Identificar_Gestantes(dt1.Replace("/", ""), dt2.Replace("/", ""));

			Busca_Gestantes_CDS();

			

			busca_atendimentos(dt1.Replace("/", ""), dt2.Replace("/", ""));



			Processamento();



			label2.Text = "" + dataGridView2.Rows.Count;
			dataGridView2.Columns[0].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
			dataGridView2.Columns[1].DefaultCellStyle.WrapMode = DataGridViewTriState.True;

		}


		private void busca_atendimentos(string dtincial, string dtfinal)
		{
			if (lista_atendimentos.Count > 0) { lista_atendimentos.Clear(); }
			string query = @"select
tf.co_dim_tempo as dt_atendimento,
(select cc.no_cidadao from tb_fat_cidadao_pec cc where tf.co_fat_cidadao_pec = cc.co_seq_fat_cidadao_pec) as nome,
CASE WHEN tf.co_fat_cidadao_pec IS NULL THEN '0'
			ELSE tf.co_fat_cidadao_pec END
			co_fat_cidadao_pec,
(select aa.ds_filtro from tb_dim_tipo_ficha aa where tf.co_dim_tipo_ficha = aa.co_seq_dim_tipo_ficha) as ficha,
(select tp.no_profissional from tb_dim_profissional tp where tf.co_dim_profissional_1 = tp.co_seq_dim_profissional) as profissional,
(select cbo.no_cbo from tb_dim_cbo cbo where tf.co_dim_cbo_1 = cbo.co_seq_dim_cbo) as cbo,
tf.co_dim_unidade_saude_1 as unidade,
tf.nu_cns as cns,
tf.nu_cpf_cidadao as cpf,
tf.dt_nascimento,
tf.ds_filtro_cids as cids,
tf.ds_filtro_ciaps as ciaps,
tf.ds_filtro_proced_solicitados as ps,
tf.ds_filtro_proced_avaliados as pa,
tf.st_vacinacao_em_dia,
tf.co_dim_tempo_dum as dum,
tf.nu_idade_gestacional_semanas as ig,
tf.nu_gestas_previas,
tf.nu_partos
from
tb_fat_atendimento_individual tf
where (tf.co_dim_tempo >= '" + dtincial + "' AND tf.co_dim_tempo <= '" + dtfinal + "') ORDER BY tf.co_dim_tempo";

			var c = new Conexao();
			var dt = c.Get_DataTable(query);
			foreach (DataRow row in dt.Rows)
			{
				var a = new atendimentos();
				a.dt_atendimento = row["dt_atendimento"].ToString();
				a.tp_ficha = row["ficha"].ToString();
				a.profissional = row["profissional"].ToString();
				a.cbo = row["cbo"].ToString();
				a.co_dim_unidade_saude_1 = row["unidade"].ToString();
				a.cns = row["cns"].ToString();
				a.cpf = row["cpf"].ToString();
				a.cids = row["cids"].ToString();
				a.ciaps = row["ciaps"].ToString();
				a.solicitado = row["ps"].ToString();
				a.avaliado = row["pa"].ToString();
				a.co_fat_cidadao_pec = row["co_fat_cidadao_pec"].ToString();
				a.dum = row["dum"].ToString();
				a.ig = row["ig"].ToString();
				lista_atendimentos.Add(a);

				if (lista_gestantes.ContainsKey(a.co_fat_cidadao_pec))
				{
					lista_gestantes[a.co_fat_cidadao_pec].adiciona_atendimento(a);
				}
				else
				{
				}
			}

		}
		private void adicionaitem(string key, string value)
		{
			if (list02.ContainsKey(key))
			{
				list02[key] += Environment.NewLine + "" + value;
			}
			else
			{
				string b = value;
				list02.Add(key, b);
			}
		}

		private void adicionaPessoa(atendimentos a)
		{
			string key = a.co_fat_cidadao_pec;
			Gestante p = new Gestante();
			p.codigo_fat = key;

			if (list99.ContainsKey(key))
			{
				list99[key].adiciona_atendimento(a);

			}
			else
			{
				list99.Add(key, p);
				list99[key].adiciona_atendimento(a);
			}
		}

		private void Atualiza_Pessoa(Gestante a)
		{
			string key = a.codigo_fat;
			if (list99.ContainsKey(key))
			{
				list99[key].nome = a.nome;
				list99[key].ges = a.ges;
			}
			else
			{
				list99.Add(key, a);
			}
		}

		private void Processamento()
		{
			foreach (var v in lista_gestantes.Values)
			{
				string dados = "";
				dados += v.nome + Environment.NewLine;
				dados += v.nascimento + Environment.NewLine;
				dados += "cns: " + v.cns + Environment.NewLine;
				dados += "cpf: " + v.cpf + Environment.NewLine;
				dados += "acs: " + v.acs + Environment.NewLine;
				dados += "eqp: " + v.equipe_vinc + Environment.NewLine;
				dados += "Gestante ? : " + v.ges + Environment.NewLine;

				dataGridView2.Rows.Add(dados, v.atendimentos);

			}
		}

	}

	public class atendimentos
	{

		public string dt_atendimento { get; set; }
		public string nome { get; set; }

		public string dt_nascimento { get; set; }
		public string co_fat_cidadao_pec { get; set; }
		public string tp_ficha { get; set; }
		public string profissional { get; set; }
		public string cbo { get; set; }
		public string co_dim_unidade_saude_1 { get; set; }
		public string cns { get; set; }
		public string cpf { get; set; }

		public string cids { get; set; }
		public string ciaps { get; set; }

		public string solicitado { get; set; }
		public string avaliado { get; set; }

		public string dum { get; set; }
		public string ig { get; set; }

	}


	public class Gestante
	{
		public string nome { get; set; }
		public string cns { get; set; }
		public string cpf { get; set; }
		public string nascimento { get; set; }
		public string codigo_fat { get; set; }
		public string ges { get; set; }
		public string equipe_vinc { get; set; }
		public string acs { get; set; }
		public string atendimentos { get; set; }

		List<string> lista_atendimentos = new List<string>();

		public void adiciona_atendimento(atendimentos a)
		{
			string ano = a.dt_atendimento.Substring(0, 4);
			string mes = a.dt_atendimento.Substring(4, 2);
			string dia = a.dt_atendimento.Substring(6, 2);
			string data = dia + "/" + mes + "/" + ano;
			string cbo = a.cbo.Replace("DA ESTRATÉGIA DE SAÚDE DA FAMÍLIA", "ESF");
			string avaliado = "";
			if (a.avaliado.Contains("|ABEX019|")) { avaliado += "ABEX019"; }
			if (a.avaliado.Contains("|ABEX018|")) { avaliado += " ABEX018"; }
			string t = "";
			t += data + " ";
			t += cbo + " ";
			t += a.profissional + " ";
			t += a.cids + " ";
			t += a.ciaps + " ";
			//t += " S " + a.solicitado;
			if (avaliado.Length > 0) { t += " A " + avaliado; }		
			t += " Dum " + a.dum.Replace("30001231","");

			t += " IG " + a.ig;
			lista_atendimentos.Add(t);
			atendimentos += t + Environment.NewLine;

		}
	}
}
