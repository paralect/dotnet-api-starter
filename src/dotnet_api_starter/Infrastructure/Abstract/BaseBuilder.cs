using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_api_starter.Infrastructure.Abstract
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
