using FluentValidation;
using MasterNet.Application.Core;
using MasterNet.Application.Interfaces;
using MasterNet.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MasterNet.Application.Accounts.Auth;

public class AuthCommand
{
    public record AuthCommandRequet(AuthRequest authRequest)
    : IRequest<Result<Profile>>;

    internal class AuthCommandHandler : IRequestHandler<AuthCommandRequet, Result<Profile>>
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;

        public AuthCommandHandler(UserManager<User> userManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public async Task<Result<Profile>> Handle(
            AuthCommandRequet request,
            CancellationToken cancellationToken
        )
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == request.authRequest.Email);
            if (user == null)
            {
                return Result<Profile>.Failure("User not found");
            }

            var result = await _userManager.CheckPasswordAsync(user, request.authRequest.Password);
            if (!result)
            {
                return Result<Profile>.Failure("Invalid password");
            }

            var profile = new Profile
            {
                Id = Guid.Parse(user.Id),
                FullName = $"{user.Name} {user.LastName}",
                Email = user.Email,
                UserName = user.UserName,
                Token = await _tokenService.CreateToken(user),
            };

            return Result<Profile>.Success(profile);
        }
    }

    public class AuthCommandRequestValidator : AbstractValidator<AuthCommandRequet>
    {
        public AuthCommandRequestValidator()
        {
            RuleFor(x => x.authRequest).SetValidator(new AuthValidator());
        }
    }


}