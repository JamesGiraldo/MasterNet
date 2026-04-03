using MasterNet.Domain.Entities;

namespace MasterNet.Application.Interfaces;

public interface ITokenService
{
    Task<string> CreateToken(User user);
}