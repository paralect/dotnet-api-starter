namespace Api.Core.Abstract
{
    public interface IAuthService
    {
        string CreateAuthToken(string id);
    }
}
