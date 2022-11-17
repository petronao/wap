using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp1
{
    public class Conexao
    {
        string serverName = "";                                          //localhost
        string port = "";                                                            //porta default
        string userName = "";                                               //nome do administrador
        string password = "";                                             //senha do administrador
        string databaseName = "";                                  //nome do banco de dados
        public string cs;
        NpgsqlConnection conexao = null;
        public Conexao()
        {                        
            string[] lines = System.IO.File.ReadAllLines("config.txt");
            serverName = lines[0].Replace("ip:", "");
            port = lines[1].Replace("port:", "");
            userName = lines[2].Replace("user:", "");
            password = lines[3].Replace("psw:", "");
            databaseName = lines[4].Replace("db:", "");
           cs = String.Format("Server={0};Port={1};User Id={2};Password={3};Database={4};", serverName, port, userName, password, databaseName);
        }

        public DataTable Get_DataTable(string query)
        {
            try
            {
                var dt = new DataTable();
                using (conexao = new NpgsqlConnection(cs))
                {
                    conexao.Open();
                    if (conexao.State == System.Data.ConnectionState.Open)
                    {
                        string sql = query;
                        using (NpgsqlDataAdapter Adpt = new NpgsqlDataAdapter(sql, conexao))
                        {
                            Adpt.Fill(dt);
                        }
                        return dt;
                    }
                    else
                        return null;
                }

            }
            catch (Exception ex)
            {
                return null;
            }

            finally
            {
                if (conexao.State == ConnectionState.Open) { conexao.Dispose(); conexao.Close(); }
            }
        }

        public NpgsqlDataReader Get_DataReader(string query)
        {
            try
            {
                var conn = new NpgsqlConnection(cs);
                conn.Open();
                NpgsqlCommand command = new NpgsqlCommand
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = query
                };
                NpgsqlDataReader dr = command.ExecuteReader();
                if (conn.State == ConnectionState.Open) { conn.Dispose(); conn.Close(); }
                return dr;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public NpgsqlDataReader Get_DataReader2(string query)
        {
            try
            {
                var conn = new NpgsqlConnection(cs);
                conn.Open();
                NpgsqlCommand command = new NpgsqlCommand
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = query
                };
                NpgsqlDataReader dr = command.ExecuteReader();
                //if (conn.State == ConnectionState.Open) { conn.Dispose(); conn.Close(); }
                return dr;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
