using KuberDemo.HealthChecks;
using Prometheus;

namespace KuberDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddHealthChecks()
                  .AddCheck<RandomHealthCheck>("Random check")
                  .ForwardToPrometheus(); 
            
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();
            app.MapControllers();

            app.UseHttpMetrics(); 
            app.UseMetricServer();
            app.MapMetrics();

            app.MapHealthChecks("/startup");
            app.MapHealthChecks("/live");
            app.MapHealthChecks("/ready");

            app.Run();
        }
    }
}