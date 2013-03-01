using Crosstalk.Common.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crosstalk.Common.Attributes
{
    public class PartialRepositoryAttribute : Attribute
    {
        private readonly Type _repositoryType;

        public PartialRepositoryAttribute(Type repositoryType)
        {
            if (!typeof(IPartialResolver<>).IsAssignableFrom(repositoryType))
            {
                throw new ArgumentOutOfRangeException("Repository is does not implement IPartialResolver", "repositoryType");
            }
        }

        public Type GetRepositoryType()
        {
            return this._repositoryType;
        }
    }
}
