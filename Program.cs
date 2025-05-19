using Microsoft.EntityFrameworkCore;
using DarkCloud.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.Sqlite;

var builder = WebApplication.CreateBuilder(args);

// --- Fallback automático PostgreSQL -> SQLite ---
string? pgConn = builder.Configuration.GetConnectionString("DefaultConnection");
string? sqliteConn = builder.Configuration.GetConnectionString("SQLiteConnection");
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
    db.Database.Migrate();
}

app.Run();
