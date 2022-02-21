using Common.DAL.Interfaces;
using MongoDB.Bson;

namespace Common.DAL
{
    public class IdGenerator : IIdGenerator
    {
        public string Generate()
        {
            return ObjectId.GenerateNewId().ToString();
        }
    }
}
