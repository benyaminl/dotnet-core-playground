using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using TodoApi.Models.Requests;
using TodoApi.Models.Responses;
using TodoApi.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace TodoApi.Controllers;

[ApiController]
[Route("auth")]
public class AuthenticationController : Controller 
{
    private readonly IConfiguration _conf;

    public AuthenticationController(IConfiguration configuration)
    {
        this._conf = configuration;
    }
    /// <summary>
    /// This is specific for .NET... Built in Roles, Policy, and Claims... 
    /// See more on the Program.cs
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("/login")]
    public IActionResult AuthAction([FromBody] LoginRequest request)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_conf.GetValue<string>("Authentication:Schemes:Bearer:IssuerSigningKey") ?? ""));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var jwtHeader = new JwtHeader(creds);

        var audiences = _conf.GetSection("Authentication:Schemes:Bearer:ValidAudiences").Get<string[]>();
        var issuer = _conf.GetValue<string>("Authentication:Schemes:Bearer:ValidIssuer") ?? "";

        var config = _conf.GetSection("Authentication");
        var claims = new[]
                {
                    new Claim("unique_name", request.user),
                    new Claim(JwtRegisteredClaimNames.Sub, request.user),
                    new Claim(JwtRegisteredClaimNames.Jti, request.user),
                    new Claim("scope", "RequireAdminRole"),
                    new Claim("role", "admin"),
                    new Claim(JwtRegisteredClaimNames.Nbf, DateTime.UtcNow.ToEpoch().ToString(), ClaimValueTypes.Integer),
                    new Claim(JwtRegisteredClaimNames.Exp, DateTime.UtcNow.AddDays(1).ToEpoch().ToString(), ClaimValueTypes.Integer),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToEpoch().ToString(), ClaimValueTypes.Integer),
                    new Claim(JwtRegisteredClaimNames.Iss, issuer)
                };

        var jwtBody = new JwtPayload(claims);
        
        jwtBody.Add("aud", audiences);

        // var token = new JwtSecurityToken(_conf.GetValue<string>("Authentication:Schemes:Bearer:ValidIssuer"),
        // jsonAud,
        // claims,
        // expires: DateTime.Now.AddMinutes(1),
        // signingCredentials: creds);

        var token = new JwtSecurityToken(jwtHeader, jwtBody);
        
        return Ok(new ApiResponse(){
            Data = new {token = new JwtSecurityTokenHandler().WriteToken(token), signkey = key, config = config}
        });
    }
}