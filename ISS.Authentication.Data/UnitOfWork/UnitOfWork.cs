using System;
using ISS.Authentication.Data.Interfaces.DbContext;

namespace ISS.Authentication.Data.UnitOfWork
{
    public class UnitOfWork : Interfaces.UnitOfWork.IUnitOfWork
    {
        public IDbContext DbContext { get; set; }

        public UnitOfWork(IDbContext context)
        {
            if (context == null)
                throw new ArgumentNullException("connectionString");

            this.DbContext = context;
        }

        public void Dispose()
        {

        }

        public void BeginWork()
        {
            DbContext.BeginTransaction();
        }

        public void CommitWork()
        {
            DbContext.CommitTransaction();
        }

        public void RollbackWork()
        {
            DbContext.RollbackTransaction();
        }

        public Interfaces.Repositories.IUserRepository UserStore
        {
            get;
            set;
        }

        public Interfaces.Repositories.IRoleRepository RoleStore
        {
            get;
            set;
        }

        public Interfaces.Repositories.IClaimRepository ClaimStore
        {
            get;
            set;
        }

        public Interfaces.Repositories.IClientRepository ClientStore
        {
            get;
            set;
        }

        public Interfaces.Repositories.IRefreshTokenRepository RefreshTokenStore
        {
            get;
            set;
        }

        public Interfaces.Repositories.IUserSessionRepository UserSessionStore
        {
            get;
            set;
        }

        public Interfaces.Repositories.IPasswordResetTokenRepository PasswordResetTokenStore
        {
            get;
            set;
        }
    }
}
