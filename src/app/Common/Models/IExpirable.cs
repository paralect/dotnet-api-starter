using System;

namespace Common.Models
{
    public interface IExpirable
    {
        public DateTime ExpireAt { get; set; }
    }
}
