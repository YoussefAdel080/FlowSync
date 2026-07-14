using FlowSync.Application.Configuration;
using FlowSync.Application.Contexts;
using FlowSync.Application.Models;
using FlowSync.Application.Repositories;
using FlowSync.Application.Services;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace FlowSync.Application
{
    public static class ApplicationServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<ITokenRepository, TokenRepository>();
            services.AddScoped<IEmailVerificationRepository, EmailVerificationRepository>();
            services.AddScoped<IPasswordResetRepository, PasswordResetRepository>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IEmailVerificationService, EmailVerificationService>();
            services.AddScoped<IPasswordResetService, PasswordResetService>();
            services.AddValidatorsFromAssemblyContaining<IApplicationMarker>(ServiceLifetime.Scoped);
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

            services.Configure<SmtpOptions>(
                configuration.GetSection("Smtp"));
            return services;
        }

        public static IServiceCollection AddDbContext(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")));

            return services;
        }
    }
}
