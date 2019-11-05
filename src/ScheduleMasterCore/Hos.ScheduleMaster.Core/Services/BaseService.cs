using Hos.ScheduleMaster.Core.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hos.ScheduleMaster.Core.Services
{
    public abstract class BaseService
    {
        public IUnitOfWork UnitOfWork { get; set; }

        protected RepositoryFactory _repositoryFactory => new RepositoryFactory(UnitOfWork);


    }
}
