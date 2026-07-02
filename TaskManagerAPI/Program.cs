using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TaskManagerAPI.Data;
using TaskManagerAPI.Data.Configurations.Identity;
using TaskManagerAPI.GlobalExceptionHandler;
using TaskManagerAPI.GlobalExceptionHandler.Exceptions.Server;
using TaskManagerAPI.Models.Identity;
using TaskManagerAPI.Options;
using TaskManagerAPI.Repositories;
using TaskManagerAPI.Services;
using TaskManagerAPI.Services.Identity;

namespace TaskManagerAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddDbContext<TaskManagerDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentityCore<TaskManagerUser>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequiredUniqueChars = 1;
                options.Password.RequiredLength = 6;
                options.SignIn.RequireConfirmedEmail = false;
            }
            ).AddRoles<IdentityRole>().AddEntityFrameworkStores<TaskManagerDbContext>().AddDefaultTokenProviders();

            builder.Services.AddValidatorsFromAssemblyContaining<Program>();
            builder.Services.AddFluentValidationAutoValidation();

            builder.Services.Configure<JWTOptions>(builder.Configuration.GetSection("JWT"));
            builder.Services.Configure<SeededEmailsOptions>(builder.Configuration.GetSection("SeededEmails"));
            builder.Services.Configure<SeededPasswordsOptions>(builder.Configuration.GetSection("SeededPasswords"));

            var jwtOptions = builder.Configuration.GetSection("JWT").Get<JWTOptions>() ?? throw new InvalidConfigurationException("JWT");

            if (string.IsNullOrWhiteSpace(jwtOptions.Key))
                throw new InvalidConfigurationException("JWT:Key");

            if (string.IsNullOrWhiteSpace(jwtOptions.Issuer))
                throw new InvalidConfigurationException("JWT:Issuer");

            if (string.IsNullOrWhiteSpace(jwtOptions.Audience))
                throw new InvalidConfigurationException("JWT:Audience");

            if (jwtOptions.TokenExpirationInHours <= 0)
                throw new InvalidConfigurationException("Jwt:TokenExpirationInHours");

            builder.Services.AddExceptionHandler<TaskManagerGlobalExceptionHandler>();
            builder.Services.AddProblemDetails();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtOptions.Issuer,

                ValidateAudience = true,
                ValidAudience = jwtOptions.Audience,

                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key))
            });

            builder.Services.AddAuthorization();

            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter a token in format: Bearer {token}"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            }
            );

            builder.Services.AddAutoMapper(cfg => { }, typeof(Program));

            //Identity
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IAccountService, AccountService>();

            //TaskItemStatus
            builder.Services.AddScoped<ITaskItemStatusRepository, TaskItemStatusRepository>();
            builder.Services.AddScoped<ITaskItemStatusService, TaskItemStatusService>();

            //TaskItem
            builder.Services.AddScoped<ITaskItemRepository, TaskItemRepository>();
            builder.Services.AddScoped<ITaskItemService, TaskItemService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                await RoleSeeder.SeedRolesAsync(services);
                await UserSeeder.SeedUsersAsync(services);
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseExceptionHandler();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            await app.RunAsync();
        }
    }
}