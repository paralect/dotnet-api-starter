using System.Threading.Tasks;
using Common.DalSql.Entities;

namespace Api.Services.Domain
{
    public interface ITokenService
    {
        Task<Token> FindByValueAsync(string value);
    }
}
