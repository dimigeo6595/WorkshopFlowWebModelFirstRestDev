
using Microsoft.EntityFrameworkCore;
using WorkshopFlow.Data;

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

            // Add services to the container.

            builder.Services.AddControllers();
           

            var app = builder.Build();

            
            

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
