using Infrastructure.dbcontext;
using Microsoft.EntityFrameworkCore;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DirectoryContext>(
    options => options.UseSqlServer(
        builder.Configuration.GetConnectionString("DaHub")
        )
    );

//adfs
builder.Services.AddAuthentication(options =>
{
    // check if user has authentication cookie
    // if they dont then default is open id connect
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
    .AddOpenIdConnect(options =>
    {
        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

        //okta
        options.Authority = builder.Configuration["Okta:Domain"];
        options.ClientId = builder.Configuration["Okta:ClientId"];
        options.ClientSecret = builder.Configuration["Okta:ClientSecret"];

        //adfs
        //options.MetadataAddress = builder.Configuration["federation:address"];
        //options.ClientId = builder.Configuration["federation:clientId"];

        options.RequireHttpsMetadata = true;
        options.ResponseType = OpenIdConnectResponseType.Code; //authorization code grant type
        options.GetClaimsFromUserInfoEndpoint = true;
        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.SaveTokens = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = "name",
            RoleClaimType = "groups",
            ValidateIssuer = true
        };

        //options.SignInScheme = "Cookies";
        //// options.ResponseMode = OpenIdConnectResponseMode.FormPost;
        //options.UsePkce = false;
    })
    .AddCookie(options =>
    {
        options.AccessDeniedPath = "/";
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        "CanAccessAdminArea",
        policyBuilder => policyBuilder.RequireClaim(ClaimTypes.Role, "dahub-admin")
        );
});

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
    pattern: "{controller=Employee}/{action=Index}/{id?}");

//to prepare the database and seed data. 
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    DirectoryInitializer.Initialize(serviceProvider: services, DeleteDatabase: false, UseMigrations: false, SeedSampleData: false);
}

app.Run();
