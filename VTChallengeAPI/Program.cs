using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NugetVTChallenge.Interfaces;
using VTChallenge.Services;
using VTChallengeAPI.Data;
using VTChallengeAPI.Helpers;
using VTChallengeAPI.Repositories;

var builder = WebApplication.CreateBuilder(args);
// IMPORTS PARA EL TOKEN ====================================================================================
builder.Services.AddSingleton<HelperOAuthToken>();
HelperOAuthToken helper = new HelperOAuthToken(builder.Configuration);
builder.Services.AddAuthentication(helper.GetAuthenticationOptions()).AddJwtBearer(helper.GetJwtOptions());


// Add services to the container.
string connectionString = builder.Configuration.GetConnectionString("SqlAzure");

builder.Services.AddSingleton<HelperCryptography>();
builder.Services.AddTransient<HelperUserToken>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<HttpClient>();
builder.Services.AddTransient<IVtChallenge, RepositoryVtChallenge>();
builder.Services.AddTransient<IServiceValorant, ServiceValorant>();
builder.Services.AddDbContext<VTChallengeContext>(options =>
    options.UseSqlServer(connectionString)
);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    options.SwaggerDoc("v1", new OpenApiInfo {
        Title = "API VTCHALLENGE",
        Description = "API para acceder de forma de directa al contenido de VTCHALLENGE",
        Version = "v1",

    });

});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(options => {
    options.SwaggerEndpoint(url: "/swagger/v1/swagger.json", name: "API CRUD MUSIC V1");
    options.RoutePrefix = "";
});

if (app.Environment.IsDevelopment()) { }

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
