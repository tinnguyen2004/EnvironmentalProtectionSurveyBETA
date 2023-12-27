using EnvironmentalProtectionSurvey.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<Project2Context>();
builder.Services.AddSession();
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();
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

app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "usersRoute",
    pattern: "{controller=Users}/{action=Login}/{id?}");

app.MapControllerRoute(
    name: "adminRoute",
    pattern: "{controller=Admin}/{action=Index}/{id?}");

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "verifyEmail",
        pattern: "Users/VerifyEmail/{token?}",
        defaults: new { controller = "Users", action = "VerifyEmail" }
    );

    // Các route khác...
});

app.Run();
