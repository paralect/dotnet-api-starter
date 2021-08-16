﻿namespace Common.DB.Mongo.DAL.Documents
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
