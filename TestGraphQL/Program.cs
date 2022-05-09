using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TestGraphQL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


var configuration = new ConfigurationBuilder();

configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsetting.Production.json", optional: true);

IConfigurationRoot config = configuration.Build();

builder.Services.AddControllers();

builder.Services
                .AddGraphQLServer()
                .AddAuthorization()
                .AddQueryType<Query>();

builder.Services.Configure<TokenSettings>(config.GetSection("TokenSettings")); ;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(option =>
               {
                   var tokenSettings = config.GetSection("TokenSettings").Get<TokenSettings>();
                   option.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidIssuer = tokenSettings.Issure,
                       ValidateIssuer = true,
                       ValidAudience = tokenSettings.Audience,
                       ValidateAudience = true,
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSettings.Key)),
                       ValidateIssuerSigningKey = true,
               };
               });


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseRouting();

app.UseWebSockets();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapGraphQL();

app.Run();
