using Microsoft.EntityFrameworkCore;
using DEBA.Models;
using Microsoft.AspNetCore.Routing;
using UserCredentialsModels;
// The first section is the coding to login to the server.
var builder = WebApplication.CreateBuilder(args);
IConfiguration configuration = builder.Configuration;
var server = configuration["DbServer"] ?? "localhost";
var user = configuration["DbUser"] ?? "SA";
var pwd = configuration["DbPwd"] ?? "C0mp2001!";     
var database = configuration["DB"] ?? "Master";


builder.Services.AddDbContext<UserCredentialsContext>(options =>
options.UseSqlServer($"Server={server};Initial Catalog={database};User ID={user};Password={pwd}; TrustServerCertificate = True;"));


// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
