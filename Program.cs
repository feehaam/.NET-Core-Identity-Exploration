using IdentityExploration;
using IdentityExploration.Controllers.Auths;
using IdentityExploration.Models;
// 1. Identity imports. Installed packages: JwtBearer, Identity.EntityFrameworkCore
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<Token, JWT>();

// 2. Adding data context
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// 3. Adding Identity with IdentityUser and IdentityRole
// 4. As I extended IdentityUser by Employee so here istead of IdentityUser, I used Employee
builder.Services.AddIdentity<Employee, IdentityRole>()
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();

// 5. Identity password complexity
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
});

// 6. JWT bearer setup (further definitions are in appsettings.json)
var secretKey = builder.Configuration.GetSection("JwtSettings").GetValue<string>("SecretKey");
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
// 17. The default setup for JWT validation. All necessary values coming from appsettings.json, which was used 
// for generating hash code as well.
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = false,
            ValidIssuer = builder.Configuration.GetSection("JwtSettings").GetValue<string>("Issuer"),
            ValidAudience = builder.Configuration.GetSection("JwtSettings").GetValue<string>("Audience"),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        };
    });

var app = builder.Build();

// 7. Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 8. These two are must add for Auths
app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
