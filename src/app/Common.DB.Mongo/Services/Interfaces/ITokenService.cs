//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Common.DB.Mongo.DAL.Documents.Token;
//using Common.DB.Mongo.DAL.Repositories;

//namespace Common.DB.Mongo.Services.Interfaces
//{
//    public interface ITokenService : IDocumentService<Token, TokenFilter>
//    {
//        Task<List<Token>> CreateAuthTokensAsync(Guid userId);
//        Task<Token> FindAsync(string tokenValue);
//        Task DeleteUserTokensAsync(Guid userId);
//    }
//}