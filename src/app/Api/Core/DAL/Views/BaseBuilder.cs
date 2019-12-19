namespace Api.Core.DAL.Views
{
    public abstract class BaseBuilder<T>
    {
        protected T data;

        protected BaseBuilder()
        {

        }

        public T Build()
        {
            return data;
        }
    }
}
