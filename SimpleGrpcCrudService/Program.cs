using Microsoft.EntityFrameworkCore;
using SimpleGrpcCrudService.Core.BLL.RecordInterfaces;
using SimpleGrpcCrudService.Core.BLL.RecordRepository;
using SimpleGrpcCrudService.Core.DAL.Data;
using SimpleGrpcCrudService.Core.DAL.GAP.PersistenceInterfaces;
using SimpleGrpcCrudService.Core.DAL.GAP.Persistences;
using SimpleGrpcCrudService.Core.DAL.Models;
using SimpleGrpcCrudService.Services;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();

builder.Services.AddTransient<IStudentRecordPersistence, StudentRecordPersistence>();

builder.Services.AddSingleton<IStudentRecordRepository, StudentRecordRepository>();



builder.Services.AddDbContext<StudentContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("connectionString"), b => b.MigrationsAssembly("SimpleGrpcCrudService.Core"));
});


builder.Services.AddCors(corsOptions =>
{
    corsOptions.AddPolicy("cors", policy =>
    {
        policy.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin().WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding");
    });
});


var app = builder.Build();

app.UseCors();

// Configure the HTTP request pipeline.
app.MapGrpcService<StudentService>();


app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
