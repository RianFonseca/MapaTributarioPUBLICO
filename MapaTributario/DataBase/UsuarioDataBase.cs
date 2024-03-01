using System.Data.SqlClient;
using MapaTributario.AutorizacaoEAutenticacao;

namespace teste0.DataBase
{
    public class UsuarioDataBase
    {
        private readonly string connectionString;

        public UsuarioDataBase(IConfiguration configuration)
        {
            var sqlServerConnectionString = Environment.GetEnvironmentVariable("SQLServer");

            if (string.IsNullOrEmpty(sqlServerConnectionString))
            {
                Console.WriteLine("Erro: A string de conexão 'SQLServer' está vazia ou nula.");
                throw new InvalidOperationException("Falha ao recuperar o 'SQLServer' das variáveis ​​de ambiente.");
            }

            this.connectionString = sqlServerConnectionString;
        }

        public Usuario GetUsuario(string login, string senha)
        {
            string queryString = "SELECT * FROM dbo.login_lefisc WHERE login = @login AND senha = @senha";
            
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                try
                {
                    connection.Open();
                    
                    using (SqlCommand command = new SqlCommand(queryString, connection))
                    {
                        command.Parameters.AddWithValue("@login", login);
                        command.Parameters.AddWithValue("@senha", senha);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Usuario()
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    Login = reader["Login"].ToString(),
                                    Senha = reader["Senha"].ToString()
                                };
                            }
                        }
                    }
                }
                catch (SqlException sqlEx)
                {
                    Console.WriteLine("SQL Error: " + sqlEx.Message);
                  
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }

                return null;
            }
        }
    }
}
