using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Finally_project.Data;
//using MailKit.Configuration;
using Microsoft.Extensions.Options;
using Finally_project.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<Finally_projectContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Finally_projectContext") ?? throw new InvalidOperationException("Connection string 'Finally_projectContext' not found.")));

builder.Services.Configure<MailSettings>(builder.Configuration.GetSection(nameof(MailSettings)));
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseSession();

app.UseHttpsRedirection();
app.UseStaticFiles();


app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=users}/{action=Index}/{id?}");

app.Run();
