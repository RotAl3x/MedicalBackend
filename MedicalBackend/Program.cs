using System.Text;
using MedicalBackend.Database;
using MedicalBackend.Entities;
using MedicalBackend.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddControllersWithViews().AddNewtonsoftJson();
services.AddRazorPages();

services.AddCors(options =>
{
    options.AddPolicy("MedicalCorsPolicy", builder =>
    {
        builder
            .WithOrigins("*")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

//services.AddAutoMapper(typeof(MappingProfile));

services.AddDefaultIdentity<ApplicationUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

services.AddAuthentication()
    .AddJwtBearer(x =>
    {
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey =
                new SymmetricSecurityKey(
                    Encoding.ASCII.GetBytes(configuration.GetSection("JWT:Secret").Value)),
            ValidateIssuer = true,
            ValidIssuer = configuration.GetSection("JWT:Issuer").Value,
            ValidateAudience = false,
            RequireExpirationTime = false,
            ValidateLifetime = true
        };
    });

services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 3;

    options.SignIn.RequireConfirmedEmail = true;
    options.ClaimsIdentity.UserIdClaimType = "id";

});

services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
services.AddSwaggerGen();

var connectionString = configuration.GetConnectionString("Default");
services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));

services.AddCoreSpecifications();

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Medical.Api v1"));

app.UseStaticFiles();

app.UseRouting();
app.UseCors("MedicalCorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        "default",
        "admin/{controller=Home}/{action=Index}/{id?}");
    endpoints.MapRazorPages();
});


AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

app.Run();