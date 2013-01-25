using System;

namespace Crosstalk.Core.Exceptions
{
    public class ObjectNotFoundException : Exception
    {
        public ObjectNotFoundException(string message) : base(message) {}
    }

    public class ObjectNotFoundException<T> : ObjectNotFoundException
    {
        public ObjectNotFoundException(object id)
            : base(string.Format("No {0} with ID = {1}", typeof(T), id.ToString())) {}
    }
}