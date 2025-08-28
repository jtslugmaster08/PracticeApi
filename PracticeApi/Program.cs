using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using PracticeApi.Extensions;
using PracticeApi.Helpers;
using PracticeModel.Entities;
using PracticeRepository.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DataContext>();
builder.Services.AddIdentity<BaseUser, IdentityRole>(o =>
{
    o.User.RequireUniqueEmail = true;
}).AddRoles<IdentityRole>().AddEntityFrameworkStores<DataContext>().AddTokenProvider<DataProtectorTokenProvider<BaseUser>>(TokenOptions.DefaultProvider);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddBearerToken(options =>
{
    options.BearerTokenExpiration = TimeSpan.FromMinutes(10);
}).AddCookie().AddJwtBearer(cfg =>
{
    cfg.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Tokens:Issuer"],
        ValidAudience = builder.Configuration["Tokens:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Tokens:Key"]))
    };
});



builder.Services.AddControllers();
builder.Services.AddRepositories()
    .AddServices()
    .ConfigureCors()
    .ConfigureForms();

builder.Services.AddAutoMapper(typeof(MappingProfile));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (builder.Environment.EnvironmentName == "Debug")
{
    builder.Configuration.AddJsonFile("appsettings.json");
}
else
{
    builder.Configuration.AddJsonFile("appsettings.json",optional:true,reloadOnChange:true)
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json",optional:true,reloadOnChange:true);
}

    var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
