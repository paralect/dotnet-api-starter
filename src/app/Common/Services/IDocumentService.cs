using System;
using System.Threading.Tasks;
using Common.Models;

namespace Common.Services
{
    public interface IDocumentService<TDocument>
        where TDocument : IEntity
    {
        Task<TDocument?> FindByIdAsync(Guid id);
    }
}
