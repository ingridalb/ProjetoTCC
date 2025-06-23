using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MaoSolidaria.Data;
using MaoSolidaria.Models;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Configuração do banco de dados e Identity com ApplicationUser
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;

    // Permitir senhas mais simples
    options.Password.RequireDigit = false;                // Não exige número
    options.Password.RequiredLength = 6;                  // Pelo menos 6 caracteres
    options.Password.RequireNonAlphanumeric = false;      // Não exige símbolo especial
    options.Password.RequireUppercase = false;            // Não exige letra maiúscula
    options.Password.RequireLowercase = false;            // Não exige letra minúscula
})
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages();

var port = Environment.GetEnvironmentVariable("PORT") ?? "5432";

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(int.Parse(port));
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

// Garante que o banco é criado
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.Run();
