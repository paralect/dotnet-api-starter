﻿using System.Threading.Tasks;
using Common.DAL;
using Common.DAL.Documents;

namespace Common.Services.Interfaces
{
    public interface IDocumentService<TDocument, in TFilter>
        where TDocument : BaseDocument
        where TFilter : BaseFilter
    {
        Task<TDocument> FindByIdAsync(string id);
        Task<TDocument> FindOneAsync(TFilter filter);
    }
}
