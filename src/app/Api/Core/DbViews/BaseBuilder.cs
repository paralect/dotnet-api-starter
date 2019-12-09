namespace Api.Core.DbViews
{
    public abstract class BaseBuilder<T>
    {
        protected T data;

        public BaseBuilder()
        {

        }

        public T Build()
        {
            return data;
        }
    }
}
