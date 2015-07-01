using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicalQueries
{
    internal class UnaryOperation : ILogicalTerm, ILogicalOperation
    {
        LogicalOperator _operator = LogicalOperator.NOT;
        ILogicalTerm _internalTerm;

        public LogicalOperator Operator
        {
            get { return _operator; }
        }

        public ILogicalTerm InternalTerm 
        {
            get { return _internalTerm; } 
        }

        public string GetValue()
        {
            return string.Format("{0},{1}", Operator.Description(), _internalTerm.GetValue());
        }

        public UnaryOperation (ILogicalTerm term)
        {
            _internalTerm = term;
        }

    }
}
