using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;

namespace LoginApi
{
    public class UsuarioRepository
    {
        string cadenaConexion =
                "Server=ARAM\\SQLEXPRESS;" +
                "Database=Tienda;" +
                "Trusted_Connection=True;" +
                "TrustServerCertificate=True;";
        public UsuarioLoginResult? ValidarLogin(string email, string passwordHash)
        {
            
            var resultados = new UsuarioLoginResult();
            using (SqlConnection connection = new SqlConnection(cadenaConexion))
            {
                
                string comando = "SELECT Usuarios.nombreUsuario, Usuarios.Activo, Roles.nombreRol FROM Usuarios INNER JOIN Roles ON Usuarios.Id_Rol = Roles.Id_Rol WHERE emailUsuario = @email AND PasswordHash = @hash";

                using (SqlCommand command = new SqlCommand(comando, connection))
                {

                    connection.Open();
                    command.Parameters.Add("@email", System.Data.SqlDbType.VarChar).Value = email;
                    command.Parameters.AddWithValue("@hash", passwordHash);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {


                        if (!reader.Read())
                        {
                            return new UsuarioLoginResult
                            {
                                EstadoLogin = EstadoLogin.CredencialesIncorrectas
                            };
                        }
                        else
                        {
                            bool activo = (bool)reader["Activo"];
                            if (!activo)
                            {
                                return new UsuarioLoginResult { 
                                    EstadoLogin = EstadoLogin.UsuarioInactivo
                                };
                            }

                            return new UsuarioLoginResult
                            {
                                EstadoLogin = EstadoLogin.Ok,
                                NombreUsuario = reader["nombreUsuario"].ToString(),
                                RolUsuario = reader["nombreRol"].ToString()

                            };


                        }
                    }
                }
            }
        }

        public bool ExisteCorreo(string correo)
        {
            string sql = "SELECT COUNT(1) FROM Usuarios WHERE emailUsuario = @email";
            using var connection = new SqlConnection(cadenaConexion);
            using var command = new SqlCommand(sql, connection);
            command.Parameters.Add("@email", System.Data.SqlDbType.NVarChar).Value = correo;
            connection.Open();
            int resultado = (int)command.ExecuteScalar();
            return resultado > 0;
        }

        public bool registrarUsuario(string usuario, string email, int edad, string password)
        {
            int rolID = 3;
            bool activo = true;

            string comando = "INSERT INTO Usuarios (nombreUsuario, edadUsuario, Id_Rol, Activo, PasswordHash, emailUsuario) VALUES (@nombre, @edad, @rolID, @activo, @hash, @email)";
            using var connection = new SqlConnection(cadenaConexion);
            using var command = new SqlCommand(comando, connection);

            command.Parameters.Add("@nombre", System.Data.SqlDbType.NVarChar).Value = usuario;
            command.Parameters.Add("@email", System.Data.SqlDbType.NVarChar).Value = email;
            command.Parameters.Add("@rolID", System.Data.SqlDbType.Int).Value = rolID;
            command.Parameters.Add("@edad", System.Data.SqlDbType.Int).Value = edad;
            command.Parameters.Add("@activo", System.Data.SqlDbType.Bit).Value = activo;
            command.Parameters.Add("@hash", System.Data.SqlDbType.NVarChar).Value = password;
            connection.Open();
            int efecto = command.ExecuteNonQuery();
            return efecto > 0;
            
        }
    }
}
