#if NETCOREAPP
using Umbraco.Cms.Infrastructure.Migrations;
#else
using Umbraco.Core.Migrations;
#endif

namespace uSupport.Migrations
{
    public abstract class uSupportMigrationBase : MigrationBase
    {
        public uSupportMigrationBase(IMigrationContext context) : base(context) { }

        protected abstract void DoMigrate();

#if NETCOREAPP
        protected override void Migrate() => DoMigrate();
#else
        public override void Migrate() => DoMigrate();
#endif
    }
}