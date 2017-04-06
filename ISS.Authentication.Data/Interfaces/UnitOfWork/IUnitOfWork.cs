using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISS.Authentication.Data.Interfaces.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        void BeginWork();
        void CommitWork();
        void RollbackWork();
    }
}
