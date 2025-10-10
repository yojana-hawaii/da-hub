using Infrastructure.dbcontext;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DirectoryContext>(
    options => options.UseSqlServer(
        //production - use prod database directly. Dont want to accidentally make development changes
        builder.Configuration.GetConnectionString("DaHubDev") //development
        )
    );


// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//to prepare the database and seed data. 
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    DirectoryInitializer.Initialize(serviceProvider: services, DeleteDatabase: false, UseMigrations: true, SeedSampleData: true);
    ExtraDirectoryMigration.CreateViewWithRawSqlScript(serviceProvider: services);
}

app.Run();
