using System.Threading.Tasks;
using Api.Core.DAL;
using Api.Core.DAL.Views;
using Api.Core.Interfaces.DAL;
using Api.Core.Interfaces.Services;

namespace Api.Core.Services
{
    public class BaseViewService<TView, TFilter> : IViewService<TView, TFilter>
        where TView : BaseView
        where TFilter : BaseFilter, new()
    {
        private readonly IRepository<TView, TFilter> _repository;

        public BaseViewService(IRepository<TView, TFilter> repository)
        {
            _repository = repository;
        }

        public async Task<TView> FindByIdAsync(string id)
        {
            return await FindOneAsync(new TFilter {Id = id});
        }

        public async Task<TView> FindOneAsync(TFilter filter)
        {
            return await _repository.FindOneAsync(filter);
        }
    }
}
