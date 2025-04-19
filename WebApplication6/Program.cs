using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WebApplication6;
using WebApplication6.Models;
using WebApplication6.Repository;
using WebApplication6.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyCorsPolicy", builder => builder
        .WithOrigins("http://localhost:3000", "http://localhost:3002", "http://localhost:3001", "http://localhost:3003", "http://localhost:3004", "http://localhost:3005", "http://localhost:3006") // Frontend origin
        .AllowAnyMethod() // Allow all HTTP methods
        .AllowCredentials() // Allow credentials (cookies)
        .WithHeaders("Accept", "Content-Type", "Origin", "X-My-Header", "Authorization") // Include Authorization header
    );
});


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "OR_APIServer2", Version = "v1" });

    // Add JWT Bearer security scheme to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Configure CORS globally
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyCorsPolicy", corsPolicyBuilder => corsPolicyBuilder
        .WithOrigins("http://localhost:3000", "http://localhost:3001", "http://localhost:3002", "http://localhost:3004", "http://localhost:3005")
        .AllowAnyMethod()
        .AllowCredentials()
        .AllowAnyHeader() // Simplify to allow any header
    );
});

// Configure EF Core DbContext
builder.Services.AddDbContext<Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection")));

// Add repositories and services to DI container
builder.Services.AddScoped<IProperty, PropertyRepository>();
builder.Services.AddScoped<IRegistrationRepository, RegistrationRepository>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<ILeaseRepository, LeaseRepository>();
builder.Services.AddScoped<ILeaseService, LeaseService>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPaymentService, PaymentServices>();
builder.Services.AddScoped<IMaintainanceRepository, MaintainanceRepository>();
builder.Services.AddScoped<IMaintainanceService, MaintainanceService>();
builder.Services.AddScoped<IHistoryRepository, HistoryRepository>();
builder.Services.AddScoped<IAuth>(provider => new Auth(
    "This is my first secret Test Key for authentication, test it and use it when needed",
    provider.GetRequiredService<Context>()
));

// Configure Authentication
var key = Encoding.ASCII.GetBytes("This is my first secret Test Key for authentication, test it and use it when needed");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwtOptions =>
{
    jwtOptions.RequireHttpsMetadata = false;
    jwtOptions.SaveToken = true;
    jwtOptions.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = false,
        ValidateAudience = false,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("MyCorsPolicy"); // Apply CORS globally
app.UseAuthentication(); // Authentication middleware
app.UseAuthorization(); // Authorization middleware
app.MapControllers(); // Map controller endpoints
app.Run();
