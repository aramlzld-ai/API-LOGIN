using System.Text;
using System.Security.Claims;

using System.Security.Cryptography;

using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using LoginApi;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http.HttpResults;
internal class Program
{ 
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var key = "ESTA_ES_UNA_CLAVE_SUPER_SECRETA_DE_32_CHARS"; // luego la movemos a appsettings

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(key)
                )
            };
        });

        builder.Services.AddAuthorization();

        builder.Services.AddScoped<UsuarioRepository>();
        builder.Services.AddScoped<SecurityTools>();

        builder.Services.AddCors( options =>
        {
            options.AddPolicy("ReglasCORS", policy =>
            {
                policy.WithOrigins("http://127.0.0.1:5500")
                .AllowAnyHeader()
                .AllowAnyMethod();
            });
        });

        var app = builder.Build();

        app.UseCors("ReglasCORS");
        app.UseAuthentication();
        app.UseAuthorization();


        app.MapGet("/", () => "API funcionando");

        app.MapPost("/login", (LoginRequest login, UsuarioRepository repository, SecurityTools tools) =>
        {
            if (string.IsNullOrEmpty(login.email) || string.IsNullOrEmpty(login.password))
            {
                return Results.BadRequest(new { error = "Datos incompletos" });
            }

            string hash = tools.HashearPassword(login.password);
            

            var resultado = repository.ValidarLogin(login.email, hash);

            if (resultado.EstadoLogin != EstadoLogin.Ok)
            {
                return resultado.EstadoLogin switch
                {

                    EstadoLogin.CredencialesIncorrectas =>
                        Results.BadRequest(new { error = "Credenciales incorrectas" }),

                    EstadoLogin.UsuarioInactivo =>
                        Results.BadRequest(new { error = "Usuario inactivo" }),

                    _ =>
                        Results.BadRequest(new { error = "Error de login" })
                };
            }

            var token = tools.GenerarToken(
                resultado.NombreUsuario,
                resultado.RolUsuario,
                key
                );

            
            return Results.Ok(new
            {
                token,
                usuario = resultado.NombreUsuario,
                rol = resultado.RolUsuario
            });
        });
        app.MapGet("/perfil", (ClaimsPrincipal user) =>
        {
            var usuario = user.FindFirst("usuario")?.Value;
            var rol = user.FindFirst(ClaimTypes.Role)?.Value;
            return Results.Ok(new
            {
                usuario,
                rol
            });
        })
        .RequireAuthorization();

        app.MapGet("/admin", () =>
        {
            return Results.Ok("Zona ADMIN");
        })
        .RequireAuthorization(policy => policy.RequireRole("Admin"));

        app.MapGet("/empleado", () =>
        {
            return Results.Ok("Zona Empleados");
        })
        .RequireAuthorization(policy =>
            policy.RequireRole("Admin", "Empleado"));

        app.MapGet("/invitado", () =>
        {
            return Results.Ok("Zona Invitados");
        }).RequireAuthorization(policy => 
            policy.RequireRole("Invitado"));
            
        app.MapPost("/registro", (SecurityTools seguridad, UsuarioRepository repositorio, RegisterRequest register) =>
        {
            
            if (string.IsNullOrWhiteSpace(register.Nombre) ||
                string.IsNullOrWhiteSpace(register.Email) ||
                string.IsNullOrWhiteSpace(register.Password))
            {
                return Results.BadRequest(new { error = "Todos los campos son obligatorios" });
            }
            if (repositorio.ExisteCorreo(register.Email))
            {
                return Results.BadRequest(new {error = "Correo ya registrado"});
            }
            var hash = seguridad.HashearPassword(register.Password);
            var resultado = repositorio.registrarUsuario(register.Nombre, register.Email, register.Edad, hash);
            if (resultado == false) { 
                return Results.BadRequest( new  { error = "Error al registrar usuario"});
            } else
            {
                return Results.Ok(new { msj = "Usuario Creado" });
            }
        });
        app.Run();
    }
}

record LoginRequest(string email, string password);
public record RegisterRequest(
    string Nombre,
    string Email,
    int Edad,
    string Password
);


