namespace LoginApi
{
    public enum EstadoLogin
    {
        Ok, 
        CredencialesIncorrectas,
        UsuarioInactivo
    }
    public class UsuarioLoginResult
    {
        public string NombreUsuario { get; set; }
        public string RolUsuario { get; set; }
        public EstadoLogin EstadoLogin { get; set; }
    }
}
