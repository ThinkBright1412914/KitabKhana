

using KitabKhana.Data.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using KitabKhana.Data.Data;
using KitabKhana.Data.Repository;
using Microsoft.AspNetCore.Identity.UI.Services;
using KitabKhana.Utility;
using KitabKhana.ViewComponents;
using KitabKhana.Data.Seed;




var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<StripeSetup>(builder.Configuration.GetSection("Stripe"));

builder.Services.AddIdentity<IdentityUser , IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddScoped<iUnitOfWork , UnitOfWork>();
builder.Services.AddTransient<CartViewComponent>();
builder.Services.AddSingleton<IEmailSender, EmailSender>();
builder.Services.AddScoped<IDbInitialiser, DbInitialiser>();
builder.Services.AddScoped<ProductConverter>();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenined";
});
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{

    options.IdleTimeout = TimeSpan.FromMinutes(100);
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
app.UseStaticFiles();

app.UseRouting();

Stripe.StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<String>();
seedDatabase();
app.UseAuthentication();;

app.UseAuthorization();
app.UseSession();
app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();


void seedDatabase()
{
    using (var scope = app.Services.CreateScope())
    {
        var dbIntializer = scope.ServiceProvider.GetRequiredService<IDbInitialiser>();
        dbIntializer.Initialise();
    }
}