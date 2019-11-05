using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hos.ScheduleMaster.Core.Repository
{
    public interface IUnitOfWork
    {
        int Commit();

        Task<int> CommitAsync();
    }
}
