using System.Text;
using Coravel;
using MedicalBackend.Database;
using MedicalBackend.Entities;
using MedicalBackend.Hub;
using MedicalBackend.Services;
using MedicalBackend.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddControllersWithViews().AddNewtonsoftJson();

services.AddCors(options =>
{
    options.AddPolicy("MedicalCorsPolicy", builder =>
    {
        builder
            .WithOrigins("http://localhost:4200", "https://ambitious-field-0cda3541e.5.azurestaticapps.net")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            .SetIsOriginAllowed(hosts => true);
    });
});

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
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;

    options.SignIn.RequireConfirmedEmail = false;
    options.ClaimsIdentity.UserIdClaimType = "id";
});

services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
services.AddScheduler();
services.AddTransient<SendSmsService>();
services.AddSwaggerGen();
services.AddSignalR();

services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("Default")));

services.AddCoreSpecifications();

var app = builder.Build();

app.Services.UseScheduler(scheduler =>
{
    scheduler.Schedule<SendSmsService>()
        .EverySeconds(59)
        .PreventOverlapping(nameof(SendSmsService));
});

app.UseStaticFiles();

app.UseRouting();
app.UseCors("MedicalCorsPolicy");
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Medical.Api v1"));

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        "default",
        "admin/{controller=Home}/{action=Index}/{id?}");
    endpoints.MapHub<MessageHub>("/appointment");
});

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var roles = new[] { "User", "Doctor", "Admin" };
    foreach (var role in roles)
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));

    var emailAdmin = configuration.GetValue<string>("Admin:Email");
    var passwordAdmin = configuration.GetValue<string>("Admin:Password");

    var user = new ApplicationUser
    {
        UserName = emailAdmin,
        Email = emailAdmin,
        NormalizedUserName = emailAdmin?.ToUpper(),
        NormalizedEmail = emailAdmin?.ToUpper(),
        FirstName = "Admin",
        LastName = "01",
        EmailConfirmed = true,
        Created = DateTime.Now.ToUniversalTime(),
        Updated = DateTime.Now.ToUniversalTime()
    };

    var context = scope.ServiceProvider.GetService<ApplicationDbContext>();

    if (!context.Users.Any(u => u.UserName == user.UserName))
    {
        var password = new PasswordHasher<ApplicationUser>();
        var hashed = password.HashPassword(user, passwordAdmin);
        user.PasswordHash = hashed;

        var userStore = new UserStore<ApplicationUser>(context);
        await userStore.CreateAsync(user);

        var _userManager = scope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
        var userToAddRole = await _userManager.FindByEmailAsync(user.Email);
        await _userManager.AddToRoleAsync(userToAddRole, "Admin");

        await context.SaveChangesAsync();
    }
}


AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

app.Run();