using Hos.ScheduleMaster.Core.Repository;
using System;
using System.Collections.Generic;
using System.Text;
using Hos.ScheduleMaster.Core.Interface;
using Hos.ScheduleMaster.Core.Models;

namespace Hos.ScheduleMaster.Core.Services
{
    public abstract class BaseService
    {
        [Autowired]
        public IUnitOfWork _unitOfWork { get; set; }

        [Autowired]
        public RepositoryFactory _repositoryFactory { get; set; }//=> new RepositoryFactory(_unitOfWork);

        public BaseService()
        {

        }

        protected ServiceResponseMessage ServiceResult(ResultStatus status, string msg = "", object data = null)
        {
            return new ServiceResponseMessage(status, msg, data);
        }
    }

}
