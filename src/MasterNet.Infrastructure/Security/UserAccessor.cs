using System.Security.Claims;
using MasterNet.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace MasterNet.Infrastructure.Security;

public class UserAccessor : IUserAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetUserName()
    {
        return _httpContextAccessor
            .HttpContext!.User.FindFirstValue(ClaimTypes.Name)!;
    }
}