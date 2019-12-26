using Api.Core.DAL.Views;
using Api.Core.Interfaces.DAL;

namespace Api.Core.DAL
{
    public class BaseRepositoryOptions<TView> : IRepositoryOptions<TView> where TView : BaseView
    {
        public virtual bool IsAddCreatedOnField => true;
        public virtual bool IsAddUpdatedOnField => true;
    }
}
