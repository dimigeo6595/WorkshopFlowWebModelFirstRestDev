
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using WorkshopFlow.Configuration;
using WorkshopFlow.Data;
using WorkshopFlow.Helpers;
using WorkshopFlow.Repositories;
using WorkshopFlow.Security;
using WorkshopFlow.Services;
namespace WorkshopFlow
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog((hostingContext, configuration) =>
            {
                configuration.ReadFrom.Configuration(hostingContext.Configuration);
            });

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


            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowClient", policy =>
                policy.WithOrigins(builder.Configuration["Cors:Origin"]!)
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });

            builder.Services.AddControllers().AddJsonOptions( options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Workshop Flow", Version = "v1" });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);

                // options.SupportNonNullableReferenceTypes(); // default true > .NET 6
                options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme,
                    new OpenApiSecurityScheme
                    {
                        Description = "JWT Authorization header using the Bearer scheme.",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.Http,
                        Scheme = JwtBearerDefaults.AuthenticationScheme,
                        BearerFormat = "JWT"
                    });
                options.OperationFilter<AuthorizeOperationFilter>();
            });


            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

            builder.Services.AddProblemDetails();

            builder.Services.AddAuthorization(options =>
            {
                // User policies
                options.AddPolicy("VIEW_USER", p => p.RequireClaim("capability", "VIEW_USER"));
                options.AddPolicy("VIEW_USERS", p => p.RequireClaim("capability", "VIEW_USERS"));
                options.AddPolicy("INSERT_USER", p => p.RequireClaim("capability", "INSERT_USER"));
                options.AddPolicy("EDIT_USER", p => p.RequireClaim("capability", "EDIT_USER"));
                options.AddPolicy("DELETE_USER", p => p.RequireClaim("capability", "DELETE_USER"));

                // Item policies
                options.AddPolicy("VIEW_ITEMS", p => p.RequireClaim("capability", "VIEW_ITEMS"));
                options.AddPolicy("INSERT_ITEM", p => p.RequireClaim("capability", "INSERT_ITEM"));
                options.AddPolicy("EDIT_ITEM", p => p.RequireClaim("capability", "EDIT_ITEM"));
                options.AddPolicy("DELETE_ITEM", p => p.RequireClaim("capability", "DELETE_ITEM"));

                // BOM policies
                options.AddPolicy("VIEW_BOM", p => p.RequireClaim("capability", "VIEW_BOM"));
                options.AddPolicy("EDIT_BOM", p => p.RequireClaim("capability", "EDIT_BOM"));

                // Routing policies
                options.AddPolicy("VIEW_ROUTING", p => p.RequireClaim("capability", "VIEW_ROUTING"));
                options.AddPolicy("EDIT_ROUTING", p => p.RequireClaim("capability", "EDIT_ROUTING"));

                // Machine policies
                options.AddPolicy("VIEW_MACHINES", p => p.RequireClaim("capability", "VIEW_MACHINES"));
                options.AddPolicy("EDIT_MACHINES", p => p.RequireClaim("capability", "EDIT_MACHINES"));

                // Work Order policies
                options.AddPolicy("VIEW_WORK_ORDERS", p => p.RequireClaim("capability", "VIEW_WORK_ORDERS"));
                options.AddPolicy("INSERT_WORK_ORDER", p => p.RequireClaim("capability", "INSERT_WORK_ORDER"));
                options.AddPolicy("EDIT_WORK_ORDER", p => p.RequireClaim("capability", "EDIT_WORK_ORDER"));
                options.AddPolicy("START_WORK_ORDER", p => p.RequireClaim("capability", "START_WORK_ORDER"));
                options.AddPolicy("COMPLETE_WORK_ORDER", p => p.RequireClaim("capability", "COMPLETE_WORK_ORDER"));
                options.AddPolicy("ASSIGN_WORK_ORDER", p => p.RequireClaim("capability", "ASSIGN_WORK_ORDER"));

                // Inventory policies
                options.AddPolicy("VIEW_INVENTORY", p => p.RequireClaim("capability", "VIEW_INVENTORY"));
                options.AddPolicy("ADJUST_INVENTORY", p => p.RequireClaim("capability", "ADJUST_INVENTORY"));
            });




            var app = builder.Build();




            
            app.UseExceptionHandler();

            if(app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            
            app.UseHttpsRedirection();

            app.UseCors("AllowClient");
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
