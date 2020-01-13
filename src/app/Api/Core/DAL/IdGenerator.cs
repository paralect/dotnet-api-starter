using Api.Core.Interfaces.DAL;
using MongoDB.Bson;

namespace Api.Core.DAL
{
    public class IdGenerator : IIdGenerator
    {
        public string Generate()
        {
            return ObjectId.GenerateNewId().ToString();
        }
    }
}
