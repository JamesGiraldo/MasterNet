using System.Net;
using MasterNet.Application.Accounts;
using MasterNet.Application.Accounts.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static MasterNet.Application.Accounts.Auth.AuthCommand;

namespace MasterNet.WebApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{

    private readonly ISender _sender;

    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("login")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<Profile>> Login(
        [FromBody] AuthRequest request,
        CancellationToken cancellationToken
    )
    {
        var command = new AuthCommandRequet(request);
        var resultado = await _sender.Send(command, cancellationToken);
        return resultado.IsSuccess ? Ok(resultado.Data) : BadRequest(resultado.Error);
    }

}