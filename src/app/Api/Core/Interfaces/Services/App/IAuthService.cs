namespace Api.Core.Interfaces.Services.App
{
    public interface IAuthService
    {
        string CreateAuthToken(string id);
    }
}
