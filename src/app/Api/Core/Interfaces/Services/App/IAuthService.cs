using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Core.DbViews.Token;

namespace Api.Core.Interfaces.Services.App
{
    public interface IAuthService
    {
        Task<List<Token>> SetTokens(string userId);
    }
}
