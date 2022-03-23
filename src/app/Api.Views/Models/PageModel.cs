using System.Collections.Generic;

namespace Api.Views.Models;

public class PageModel<TModel>
{
    public int TotalPages { get; set; }
    public long Count { get; set; }
    public IEnumerable<TModel> Items { get; set; }
}
