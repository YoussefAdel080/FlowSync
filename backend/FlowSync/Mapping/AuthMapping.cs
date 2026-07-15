using FlowSync.Application.Models;
using FlowSync.Contracts.Requests;
using System.Runtime.CompilerServices;

namespace FlowSync.Mapping
{
    public static class AuthMapping
    {
        public static User MapToUser(this RegisterRequest request) {
            return new User {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Password = request.Password
            };
        }
    }
}
