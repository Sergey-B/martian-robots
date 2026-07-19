namespace Application.Abstractions.Authentication;

public interface ITokenProvider
{
    string GenerateRefreshToken();
}
