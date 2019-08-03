using DynamicPlugins.Core.Contracts;
using DynamicPlugins.Core.Helpers;

namespace DynamicPlugins.Core.DomainModel
{
    public abstract class BaseMigration : IMigration
    {
        private Version _version = null;
        private DbHelper _dbHelper = null;

        public BaseMigration(DbHelper dbHelper, Version version)
        {
            this._version = version;
            this._dbHelper = dbHelper;
        }

        public Version Version
        {
            get
            {
                return _version;
            }
        }

        protected void SQL(string sql)
        {
            _dbHelper.ExecuteNonQuery(sql);
        }

        public abstract void Down();

        public abstract void Up();
    }
}
