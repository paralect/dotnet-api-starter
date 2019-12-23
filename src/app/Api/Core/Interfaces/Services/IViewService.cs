using System.Threading.Tasks;
using Api.Core.DAL;
using Api.Core.DAL.Views;

namespace Api.Core.Interfaces.Services
{
    public interface IViewService<TView, in TFilter>
        where TView : BaseView
        where TFilter : BaseFilter
    {
        Task<TView> FindByIdAsync(string id);
        Task<TView> FindOneAsync(TFilter filter);
    }
}
