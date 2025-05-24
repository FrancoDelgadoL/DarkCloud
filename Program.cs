using Microsoft.EntityFrameworkCore;
using DarkCloud.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.Sqlite;

var builder = WebApplication.CreateBuilder(args);

// --- Fallback automático PostgreSQL -> SQLite ---
string? pgConn = builder.Configuration.GetConnectionString("DefaultConnection");
string? sqliteConn = builder.Configuration.GetConnectionString("SQLiteConnection");

// --- Soporte para Render: convertir DATABASE_URL a formato Npgsql ---
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
if (!string.IsNullOrEmpty(databaseUrl))
{
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':');
    var npgsqlConn =
        $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};Ssl Mode=Require;Trust Server Certificate=true";
    pgConn = npgsqlConn;
}

bool usePostgres = true;
try
{
    // Intentar abrir una conexión a PostgreSQL
    var npgsqlConn = new Npgsql.NpgsqlConnection(pgConn);
    npgsqlConn.Open();
    npgsqlConn.Close();
}
catch
{
    usePostgres = false;
}
if (usePostgres)
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(pgConn));
}
else
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite(sqliteConn));
}

// Configuración de Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseSession(); // Habilitar sesiones antes de Auth y Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Aplicar migraciones automáticamente al iniciar
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    // Log de diagnóstico avanzado para Render
    try
    {
        var migrationsPath = Path.Combine(AppContext.BaseDirectory, "Migrations");
        if (Directory.Exists(migrationsPath))
        {
            var files = Directory.GetFiles(migrationsPath);
            Console.WriteLine($"Archivos en Migrations ({migrationsPath}):");
            foreach (var file in files)
                Console.WriteLine(" - " + Path.GetFileName(file));
        }
        else
        {
            Console.WriteLine($"No se encontró la carpeta Migrations en {migrationsPath}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al listar archivos de migración: {ex.Message}");
    }
    // Diagnóstico: mostrar migraciones pendientes y aplicadas
    try
    {
        var pending = db.Database.GetPendingMigrations();
        Console.WriteLine("Migraciones pendientes detectadas por EF Core:");
        foreach (var m in pending)
            Console.WriteLine(" - " + m);
        var applied = db.Database.GetAppliedMigrations();
        Console.WriteLine("Migraciones ya aplicadas:");
        foreach (var m in applied)
            Console.WriteLine(" - " + m);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al obtener migraciones: {ex.Message}");
    }
    db.Database.Migrate();

    // Crear usuario administrador si no existe
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var adminEmail = "admin@darkcloud.com";
    var adminPassword = "Admin123!"; // Cambia esto por una contraseña segura
    var adminUser = userManager.FindByEmailAsync(adminEmail).GetAwaiter().GetResult();
    if (adminUser == null)
    {
        var user = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
        var result = userManager.CreateAsync(user, adminPassword).GetAwaiter().GetResult();
        if (result.Succeeded)
        {
            // Si tienes roles, asígnale el rol de administrador aquí
            // userManager.AddToRoleAsync(user, "Administrador").GetAwaiter().GetResult();
            Console.WriteLine("Usuario administrador creado correctamente.");
        }
        else
        {
            Console.WriteLine("Error al crear el usuario administrador: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}

app.Run();
