using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PurchaseOrderManagementSystem.Data;
using Microsoft.AspNetCore.Identity;
using System.Text;

// This is the entry point of the ASP.NET Core application.
// It sets up the web host, configures services, and defines the HTTP request pipeline.

var builder = WebApplication.CreateBuilder(args);

// Configure logging:
// Clear any existing logging providers and add console and debug output.
// Set the minimum logging level to Information.
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Add services to the container (Dependency Injection).
// These services are available throughout the application.

// Adds support for MVC controllers and views.
builder.Services.AddControllersWithViews();
// Registers the JwtService for dependency injection. This service is responsible for generating JWT tokens.
builder.Services.AddScoped<PurchaseOrderManagementSystem.Services.JwtService>(); // Register JwtService
// Register DbSeeder
builder.Services.AddScoped<PurchaseOrderManagementSystem.Data.DbSeeder>();
builder.Services.AddTransient<PurchaseOrderManagementSystem.Services.IEmailSender, PurchaseOrderManagementSystem.Services.AuthMessageSender>();
// Provides access to HttpContext (e.g., for accessing cookies, session, etc.) from non-HTTP context classes.
builder.Services.AddHttpContextAccessor();
// Register HttpClient for dependency injection
builder.Services.AddHttpClient();

// Configure MySQL database connection using Entity Framework Core.
// The connection string is retrieved from the application's configuration (e.g., appsettings.json).
// It also specifies the MySQL server version and configures retry logic for transient failures.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 34)),
        mySqlOptions =>
        {
            mySqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5, // Number of times to retry on failure
                maxRetryDelay: TimeSpan.FromSeconds(10), // Delay between retries
                errorNumbersToAdd: null); // Specific error numbers to consider for retry
        }));

// Add Razor Pages services for building UI with Razor syntax.
builder.Services.AddRazorPages();

// Configure ASP.NET Core Identity for user management (authentication, authorization, roles).
// It uses ApplicationUser as the user model and IdentityRole as the role model.
// The Identity system will use ApplicationDbContext for storing identity-related data.
builder.Services.AddIdentity<PurchaseOrderManagementSystem.Models.ApplicationUser, Microsoft.AspNetCore.Identity.IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>() // Specifies the DbContext for Identity
    .AddDefaultTokenProviders(); // Adds default token providers for things like password reset tokens

// Configure authentication schemes.
builder.Services.AddAuthentication(options =>
{
    // Sets JWT Bearer as the default scheme for authentication.
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    // Sets JWT Bearer as the default scheme for challenging unauthenticated requests.
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    // Sets the default sign-in scheme, typically used by SignInManager.
    options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
})
// Adds JWT Bearer authentication.
.AddJwtBearer(options =>
{
    // Configure token validation parameters.
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, // Validate the server that created the token
        ValidateAudience = true, // Validate the recipient of the token is authorized to receive it
        ValidateLifetime = true, // Validate the token's expiration date
        ValidateIssuerSigningKey = true, // Validate the signing key of the issuer

        // Specifies valid issuer, audience, and the signing key for token validation.
        // Values are retrieved from configuration, with fallbacks if not found.
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"] ?? "your-app",
        ValidAudience = builder.Configuration["JwtSettings:Audience"] ?? "your-app-users",
        // The secret key used to sign the token, converted to bytes.
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"] ?? "thisisalongandverysecuresecretkeyforjwtauthentication"))
    };
    // Configure JWT Bearer events.
    options.Events = new JwtBearerEvents
    {
        // This event is triggered when a message is received, allowing custom token extraction.
        OnMessageReceived = context =>
        {
            // Tries to retrieve the JWT token from a cookie named "jwt_token".
            string? token = context.HttpContext.Request.Cookies["jwt_token"];
            if (!string.IsNullOrEmpty(token))
            {
                // If a token is found in the cookie, it's assigned to the context.Token.
                context.Token = token;
                Console.WriteLine($"OnMessageReceived: JWT cookie found. Token length: {token.Length}");
            }
            else
            {
                Console.WriteLine("OnMessageReceived: JWT cookie NOT found or empty.");
            }
            return Task.CompletedTask; // Indicates the asynchronous operation is complete.
        }
    };
});

/* =============================================================
 * Build the application.
 * This step finalizes the service configuration and prepares the application
 * to handle HTTP requests.
 */
var app = builder.Build();
/*
 * Application built successfully.
 * Further configurations for the HTTP request pipeline will follow.
 * ============================================================
 */


// Configure the HTTP request pipeline.
// This section defines the order in which middleware components process HTTP requests.

// Checks if the application is NOT in Development environment.
if (!app.Environment.IsDevelopment())
{
    // Adds a middleware to handle exceptions and re-execute the request with a new path.
    app.UseExceptionHandler("/Home/Error");
    // Adds middleware to enforce HTTP Strict Transport Security (HSTS).
    app.UseHsts();
}

// Redirects HTTP requests to HTTPS.
app.UseHttpsRedirection();
// Serves static files (HTML, CSS, JavaScript, images) from wwwroot.
app.UseStaticFiles();

// Adds routing capabilities to the application.
app.UseRouting();

// Adds authentication middleware to the pipeline. This must be before UseAuthorization.
app.UseAuthentication();
// Adds authorization middleware to the pipeline. This must be after UseAuthentication.
app.UseAuthorization();

// Configures the default route for MVC controllers.
// Requests will be routed based on /{controller}/{action}/{id?}.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Apply database migrations and seed data during application startup.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Starting database migration and seeding...");

        // Retrieve an instance of ApplicationDbContext from the service provider.
        var context = services.GetRequiredService<ApplicationDbContext>();
        // Applies any pending database migrations.
        logger.LogInformation("Applying database migrations...");
        context.Database.Migrate();
        logger.LogInformation("Database migrations completed successfully.");

        // Seed the database
        logger.LogInformation("Starting database seeding...");
        var seeder = services.GetRequiredService<DbSeeder>();
        await seeder.SeedAsync();
        logger.LogInformation("Database seeding completed successfully.");
    }
    catch (Exception ex)
    {
        // If an error occurs during migration or seeding, log the error.
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
        throw; // Rethrow to ensure the application doesn't start with an unseeded database
    }
}

// Runs the application, starting the web server and listening for incoming HTTP requests.
app.Run();
