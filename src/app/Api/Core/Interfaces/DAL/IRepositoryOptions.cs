using Api.Core.DAL.Views;

namespace Api.Core.Interfaces.DAL
{
    public interface IRepositoryOptions<TView> where TView : BaseView
    {
        bool IsAddCreatedOnField { get; }
        bool IsAddUpdatedOnField { get; }
    }
}
