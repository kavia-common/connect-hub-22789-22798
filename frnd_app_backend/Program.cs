using System.Reflection;
using System.Text;
using FrndAppBackend.Data;
using FrndAppBackend.Middleware;
using FrndAppBackend.Repositories;
using FrndAppBackend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Ocean Professional Theme colors for docs
const string OceanPrimary = "#2563EB";
const string OceanSecondary = "#F59E0B";
const string OceanError = "#EF4444";

// Bind JWT settings from configuration/environment
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

// EF Core using Sqlite (default demo). For production, update connection string via env.
var connString = builder.Configuration.GetConnectionString("Default") 
                 ?? "Data Source=frndapp.db";
builder.Services.AddDbContext<AppDbContext>(opts =>
{
    opts.UseSqlite(connString);
});

// Repos and Services DI
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IFriendRequestRepository, FriendRequestRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IFriendService, FriendService>();
builder.Services.AddScoped<IMessageService, MessageService>();

// Controllers + model validation
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        // Default automatic 400 with validation problems
    });

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
              .AllowCredentials()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Swagger (Swashbuckle) for API docs
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Connect Hub API",
        Version = "v1",
        Description = "A modern social networking API. Theme: Ocean Professional (blue & amber accents).",
        Contact = new OpenApiContact { Name = "Connect Hub", Url = new Uri("https://example.com") }
    });

    // JWT auth in Swagger
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter 'Bearer {your JWT token}'",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference { Id = JwtBearerDefaults.AuthenticationScheme, Type = ReferenceType.SecurityScheme }
    };
    c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });

    // XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
    }
});

// Authentication - JWT Bearer
var jwtSection = builder.Configuration.GetSection("Jwt");
var signingKey = jwtSection.GetValue<string>("SigningKey") ?? "CHANGE_ME_SUPER_SECRET_MIN32CHARS";
var issuer = jwtSection.GetValue<string>("Issuer") ?? "frnd-app";
var audience = jwtSection.GetValue<string>("Audience") ?? "frnd-app-clients";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
        ValidateIssuer = true,
        ValidIssuer = issuer,
        ValidateAudience = true,
        ValidAudience = audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromSeconds(30)
    };
});

var app = builder.Build();

// Database migration at startup (code-first)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Middleware
app.UseCors("AllowAll");
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

// Swagger with custom endpoint and small UI theming via custom CSS injection note
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Connect Hub API v1");
    c.DocumentTitle = "Connect Hub API Docs";
    // Use an injected stylesheet via headContent for subtle Ocean Professional touch.
    c.HeadContent = $@"<style>
        :root {{ --primary: {OceanPrimary}; --secondary: {OceanSecondary}; --error: {OceanError}; }}
        .topbar {{ background: linear-gradient(90deg, #2563EB1A, #f9fafb); }}
        .swagger-ui .topbar .download-url-wrapper .select-label select {{ border-color: var(--primary); }}
        .swagger-ui .btn.authorize, .swagger-ui .opblock .opblock-summary-method {{ background: var(--secondary) !important; }}
        .swagger-ui .models-control:focus, .swagger-ui .expand-operation:focus {{ outline-color: var(--primary); }}
    </style>";
    c.RoutePrefix = "docs";
});

// API endpoints
app.MapControllers();

// Health check endpoint
app.MapGet("/", () => Results.Json(new { message = "Healthy" }));

app.Run();