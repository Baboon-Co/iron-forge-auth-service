using Api;
using Api.GrpcServices;
using Application;
using Infrastructure;
using Infrastructure.Databases;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add Layers
builder.Services.AddApi();
builder.Services.AddApplication();
builder.Services.AddInfrastructure();

// Application
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.MapGrpcService<AuthService>();
app.Run();