using System;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Common.Dal;

internal static class MongoCollectionExtensions
{
    public static async Task<Page<TDocument>> AggregateByPage<TDocument>(
        this IMongoCollection<TDocument> collection,
        FilterDefinition<TDocument> filterQuery,
        SortDefinition<TDocument> sortQuery,
        int pageIndex,
        int pageSize)
    {
        var count = await collection.CountDocumentsAsync(filterQuery);
        var data = await collection.Aggregate(
            PipelineDefinition<TDocument, TDocument>.Create(new[]
            {
                    PipelineStageDefinitionBuilder.Match(filterQuery),
                    PipelineStageDefinitionBuilder.Sort(sortQuery),
                    PipelineStageDefinitionBuilder.Skip<TDocument>((pageIndex - 1) * pageSize),
                    PipelineStageDefinitionBuilder.Limit<TDocument>(pageSize),
            }),
            new AggregateOptions
            {
                Collation = Constants.DefaultCollation
            }).ToListAsync();

        var totalPages = (int)Math.Ceiling((decimal)count / pageSize);

        var page = new Page<TDocument>
        {
            TotalPages = totalPages,
            Count = count,
            Items = data
        };

        return page;
    }
}
