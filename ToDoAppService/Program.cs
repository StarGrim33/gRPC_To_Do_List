using ToDoAppService.Data;
using ToDoAppService.Services;

namespace ToDoAppService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDataAccessLayer(builder.Configuration);
            builder.Services.AddGrpc().AddJsonTranscoding();

            var app = builder.Build();

            app.MapGrpcService<GreeterService>();
            app.MapGrpcService<ToDoService>();

            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

            app.Run();
        }
    }
}