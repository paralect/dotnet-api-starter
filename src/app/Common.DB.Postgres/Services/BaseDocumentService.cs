using System;
using System.Threading.Tasks;
using Common.DB.Postgres.DAL.Documents;
using Common.DB.Postgres.DAL.Interfaces;
using Common.Services;
using LinqToDB;

namespace Common.DB.Postgres.Services
{
    public class BaseDocumentService<TDocument> : IDocumentService<TDocument>
         where TDocument : BasePostgresEntity
    {
        private readonly IRepository<TDocument> _repository;

        public BaseDocumentService(IRepository<TDocument> repository)
        {
            _repository = repository;
        }

        public Task<TDocument> FindByIdAsync(string id)
        {
            return _repository.GetQuery().FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
