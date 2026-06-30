using System.Text;
using AppointmentManagement.Application.Configurations;
using AppointmentManagement.Application.DTOs.Session;
using AppointmentManagement.Application.Interfaces;
using AppointmentManagement.Application.Interfaces.Repositories;
using AppointmentManagement.Application.Interfaces.Services;
using AppointmentManagement.Application.Validators.Session;
using AppointmentManagement.Infrastructure;
using AppointmentManagement.Infrastructure.Data;
using AppointmentManagement.Infrastructure.Repositories;
using AppointmentManagement.Infrastructure.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy("Dev", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

var jwtKey = builder.Configuration["Jwt:Key"]!;
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();

// Validators
builder.Services.AddScoped<IValidator<CreateSessionDto>, CreateSessionValidator>();
builder.Services.AddScoped<IValidator<UpdateSessionDto>, UpdateSessionValidator>();
builder.Services.AddScoped<IValidator<UpdateSessionGroupDto>, UpdateSessionGroupValidator>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
// Services — todos no Infrastructure agora
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<IEncryptionService, EncryptionService>();
builder.Services.AddScoped<IInvoiceIssuanceService, InvoiceIssuanceService>();
builder.Services.AddScoped<IInvoiceIssuanceRepository, InvoiceIssuanceRepository>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddHttpClient<ICepService, CepService>();
builder.Services.AddHttpClient<IFocusNFeService, FocusNFeService>();
builder.Services.Configure<FocusNFeSettings>(
    builder.Configuration.GetSection("FocusNFe"));
// Repositories
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<ISessionRepository, SessionRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "Appointment API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Bearer {seu_token}"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("Dev");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();