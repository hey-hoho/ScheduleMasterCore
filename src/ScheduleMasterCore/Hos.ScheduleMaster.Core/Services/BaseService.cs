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
        public IUnitOfWork _unitOfWork;

        [Autowired]
        protected RepositoryFactory _repositoryFactory;//=> new RepositoryFactory(_unitOfWork);

        public BaseService()
        {
            AutowiredServiceProvider.Autowired(this);
        }

        //public BaseService(IUnitOfWork unitOfWork)
        //{
        //    this._unitOfWork = unitOfWork;
        //}

        protected ApiResponseMessage ServiceResult(ResultStatus status, string msg = "", object data = null)
        {
            return new ApiResponseMessage(status, msg, data);
        }
    }

}
