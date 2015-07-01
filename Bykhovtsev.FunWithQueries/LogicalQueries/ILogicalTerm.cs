using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace LogicalQueries
{
    public interface ILogicalTerm
    {
        string GetValue();
        //Expression Expression { get; }
    }
}
