using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using SkyRoute.Api.Configuration;
using SkyRoute.Api.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SkyRouteDbContext>(options =>
{
    var cs = builder.Configuration.GetConnectionString("SkyRouteSql")
             ?? throw new InvalidOperationException("Connection string 'SkyRouteSql' not found.");
    options.UseSqlServer(cs);
});

builder.Services.AddSkyRouteApplication();

builder.Services.AddControllers().AddJsonOptions(o =>
{
    o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SkyRouteDbContext>();
    db.Database.Migrate();
}

app.UseCors();
app.MapControllers();

app.Run();
