using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add database context using SQL Server
//builder.Services.AddDbContext<HealthcareSystem.Infrastructure.Persistence.ApplicationDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add database context using SQL Server with transient fault resiliency
builder.Services.AddDbContext<HealthcareSystem.Infrastructure.Persistence.ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptionsAction: sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        }));

// Add services to the container.

// Registering the generic repository using AddScoped 
// (a new instance is created per HTTP request)
builder.Services.AddScoped(typeof(HealthcareSystem.Application.Interfaces.IGenericRepository<>),
                           typeof(HealthcareSystem.Infrastructure.Repositories.GenericRepository<>));

// Registering the password hasher as a Singleton because it has no state
builder.Services.AddSingleton<HealthcareSystem.Application.Interfaces.IPasswordHasher,
                              HealthcareSystem.Infrastructure.Security.PasswordHasher>();

// Registering the JWT Provider service
builder.Services.AddScoped<HealthcareSystem.Application.Interfaces.IJwtProvider,
                           HealthcareSystem.Infrastructure.Security.JwtProvider>();

// Configuring Authentication and JWT Bearer details
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!))
        };
    });

// Configuring Authorization (RBAC)
builder.Services.AddAuthorization();

// Registering the Business Logic Services
builder.Services.AddScoped<HealthcareSystem.Application.Interfaces.IAuthService,
                           HealthcareSystem.Application.Services.AuthService>();

builder.Services.AddScoped<HealthcareSystem.Application.Interfaces.IPatientService,
                           HealthcareSystem.Application.Services.PatientService>();

builder.Services.AddScoped<HealthcareSystem.Application.Interfaces.IClinicalRecordService,
                           HealthcareSystem.Application.Services.ClinicalRecordService>();

builder.Services.AddScoped<HealthcareSystem.Application.Interfaces.IAppointmentService,
                           HealthcareSystem.Application.Services.AppointmentService>();

builder.Services.AddControllers();

// Register HTTP Context Accessor and Current User Service for global auditing
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<HealthcareSystem.Application.Interfaces.ICurrentUserService,
                           HealthcareSystem.Api.Services.CurrentUserService>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

// Configure OpenAPI to support global JWT Bearer authentication in the UI
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        var bearerScheme = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
        };

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes.Add("Bearer", bearerScheme);
        document.SecurityRequirements.Add(new OpenApiSecurityRequirement { [bearerScheme] = Array.Empty<string>() });

        return Task.CompletedTask;
    });
});

// Configure Swagger/OpenAPI standard documentation
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

builder.Services.AddRateLimiter(options =>
{
    // Applies a global rate limit: max 100 requests per minute per client
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(1)
            }));
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

var app = builder.Build();

app.UseMiddleware<HealthcareSystem.Api.Middlewares.ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Enable Swagger UI middleware in development mode
    //app.UseSwagger();
    //app.UseSwaggerUI();

    // 2. Replace UseSwagger and UseSwaggerUI with these:

}
// Move these OUTSIDE the if condition so they work on Azure Production
app.MapOpenApi();
app.MapScalarApiReference(); // Modern UI accessible at /scalar/v1


// Apply pending migrations automatically on application startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider
        .GetRequiredService<HealthcareSystem.Infrastructure.Persistence.ApplicationDbContext>();
    dbContext.Database.Migrate();
}


app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseRateLimiter();

app.MapControllers();

app.Run();
