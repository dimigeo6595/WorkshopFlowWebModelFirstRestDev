
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WorkshopFlow.Configuration;
using WorkshopFlow.Data;
using WorkshopFlow.Repositories;
using WorkshopFlow.Security;
using WorkshopFlow.Services;
using Serilog;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
namespace WorkshopFlow
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connString = builder.Configuration.GetConnectionString("DevConnection");

            builder.Services.AddDbContext<WorkshopFlowContext>(options =>
                options.UseSqlServer(connString));

            builder.Services.AddScoped<IUserService, UserService>();            
            builder.Services.AddScoped<IApplicationService, ApplicationService>();
            builder.Services.AddSingleton<IEncryptionUtil, EncryptionUtil>();

            builder.Services.AddRepositories();

            builder.Services.AddAutoMapper(cfg => cfg.AddProfile<Configuration.MapperConfig>());

            var jwtSettings = builder.Configuration.GetSection("Jwt");


            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                //options.IncludeErrorDetails = builder.Environment.IsDevelopment();  // χρήσιμο σε development, δείχνει αναλυτικά errors. Στο production βάζουμε false.
                // options.SaveToken = true; αποθηκεύει το token στο HttpContext ώστε να μπορούμε να το διαβάσουμε μετά με HttpContext.GetTokenAsync("access_token")
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],

                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],

                    ValidateLifetime = true,

                    ValidateIssuerSigningKey = true,

                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]!))
                };
            });


            builder.Services.AddControllers();
           

            var app = builder.Build();

            
            

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
