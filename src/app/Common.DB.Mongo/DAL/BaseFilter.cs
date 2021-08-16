using System;

namespace Common.DB.Mongo.DAL
{
    public class BaseFilter
    {
        public Guid? Id { get; set; }
        public bool IsEmptyFilterAllowed { get; set; }
    }
}
