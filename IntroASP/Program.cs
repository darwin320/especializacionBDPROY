using IntroASP.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddDbContext<NetcoreContext>(options =>
{
    options.UseMySql(builder.Configuration.GetConnectionString("PubContext"), new MySqlServerVersion(new Version(10, 4, 27)));
});

builder.Services.AddDbContext<ProjectDbContext>(options =>
{
    options.UseMySql(builder.Configuration.GetConnectionString("PubContext"), new MySqlServerVersion(new Version(10, 4, 27)));
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
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
