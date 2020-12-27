using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolCat.Core.Contracts
{
    public interface IDbConnectionFactory
    {
        IDbConnection GetConnection();
    }
}
