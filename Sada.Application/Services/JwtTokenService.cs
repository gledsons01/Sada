using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Sada.Api.Entity.Model.Response;

namespace Sada.Application.Services;

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings _settings;

    public JwtTokenService(IOptions<JwtSettings> settings)
    {
        _settings = settings.Value;
    }

    public TokenModelResponse PreencherTokens(UsuarioModelResponse usuario)
    {
        var agora = DateTime.UtcNow;
        var tokenExpiraEm = agora.AddMinutes(Math.Max(_settings.AccessTokenMinutes, 1));
        var refreshTokenExpiraEm = agora.AddDays(Math.Max(_settings.RefreshTokenDays, 1));

        usuario.Token = GerarToken(usuario, agora, tokenExpiraEm);
        usuario.TokenExpiraEm = tokenExpiraEm;
        usuario.RefreshToken = GerarRefreshToken();
        usuario.RefreshTokenExpiraEm = refreshTokenExpiraEm;

        return new TokenModelResponse
        {

            Token = usuario.Token,
            IdUsuarioToken = usuario.IdUsuario,
            TokenExpiraEm = usuario.TokenExpiraEm,
            RefreshToken = usuario.RefreshToken,
            RefreshTokenExpiraEm = usuario.RefreshTokenExpiraEm
        };
    }

    private string GerarToken(UsuarioModelResponse usuario, DateTime emitidoEm, DateTime expiraEm)
    {
        if (string.IsNullOrWhiteSpace(_settings.Key))
        {
            throw new InvalidOperationException("A chave JWT nao foi configurada.");
        }

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.IdUsuario.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString())
        };

        if (!string.IsNullOrWhiteSpace(usuario.Login))
        {
            claims.Add(new Claim(ClaimTypes.Name, usuario.Login));
        }

        if (!string.IsNullOrWhiteSpace(usuario.EMail))
        {
            claims.Add(new Claim(ClaimTypes.Email, usuario.EMail));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            notBefore: emitidoEm,
            expires: expiraEm,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GerarRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }
}
