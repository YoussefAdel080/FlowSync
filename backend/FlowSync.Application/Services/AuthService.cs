using Azure.Core;
using FlowSync.Application.Models;
using FlowSync.Application.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace FlowSync.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IValidator<User> _validator;

        public AuthService(IAuthRepository authRepository, IValidator<User> validator, IPasswordHasher<User> passwordHasher) {
            _authRepository = authRepository;
            _validator = validator;
            _passwordHasher = passwordHasher;
        }

        public async Task<bool> Register(User user, CancellationToken token)
        {
            await _validator.ValidateAndThrowAsync(user);

            user.Password = _passwordHasher.HashPassword(user, user.Password);

            return await _authRepository.Register(user, token);
        }
    }
}
