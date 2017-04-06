using System;
using System.Data;

namespace ISS.Authentication.Data.Interfaces.DbContext
{
    public interface IDbContext : IDisposable
    {

        IDbConnection OpenConnection();
        IDbConnection OpenConnection(IDbTransaction transaction);
        IDbTransaction CurrentTransaction { get; }
        string ConnectionString { get; set; }
        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();

    }
}
