using Code_Judge.Domain.Entities;
using Code_Judge.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWebUIServices();

var app = builder.Build();
var a =OnStartedUp(app.Services);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();

    // Initialise and seed database
    using (var scope = app.Services.CreateScope())
    {
        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
        await initialiser.InitialiseAsync();
        await initialiser.SeedAsync();
    }
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHealthChecks("/health");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSwaggerUi3(settings =>
{
    settings.Path = "/api";
    settings.DocumentPath = "/api/specification.json";
    settings.OAuth2Client = new NSwag.AspNetCore.OAuth2ClientSettings
    {
        ClientId = "swagger",
        ClientSecret = "a",
        UsePkceWithAuthorizationCodeGrant = true
    };
});

app.UseRouting();

app.UseAuthentication();
app.UseIdentityServer();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapRazorPages();

app.MapFallbackToFile("index.html");
 
app.Run();

 static async Task OnStartedUp(IServiceProvider services)
{
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var existingUser = await userManager.FindByNameAsync("administrator@local");
    if (existingUser is not null)
    {
        return;
    }

    var user = new ApplicationUser
    {
        UserName = "administrator@local", Email = "administrator@local", EmailConfirmed = true
    };
    var result = await userManager.CreateAsync(user, "Kaito@1412");
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    await roleManager.CreateAsync(new IdentityRole("Administrator"));
    await userManager.AddToRoleAsync(user, "Administrator");
}