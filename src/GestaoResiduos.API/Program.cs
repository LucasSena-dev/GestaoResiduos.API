using GestaoResiduos.API.Data;
using GestaoResiduos.API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure DbContext with SQL Server
// Corrigindo o possível null reference
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        connectionString,
        sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()
    )
);

// Register services for dependency injection
builder.Services.AddScoped<IResidueService, ResidueService>();
builder.Services.AddScoped<ICollectionPointService, CollectionPointService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IScheduledCollectionService, ScheduledCollectionService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { 
        Title = "Gestão de Resíduos API", 
        Version = "v1",
        Description = "API para gerenciamento de resíduos e pontos de coleta"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

// Redirecionar a rota raiz para o Swagger
app.MapGet("/", () => Results.Redirect("/swagger"));

app.MapControllers();

app.Run();
