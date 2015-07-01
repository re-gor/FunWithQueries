using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicalQueries
{
    class BinaryOperation : ILogicalOperation, ILogicalTerm
    {
        LogicalOperator _operator;
        ILogicalTerm _leftTerm;
        ILogicalTerm _rightTerm;

        public LogicalOperator Operator
        {
            get { return _operator; }
        }

        public ILogicalTerm LeftTerm 
        {
            get { return _leftTerm; } 
        }

        public ILogicalTerm RightTerm
        {
            get { return _rightTerm; }
        }

        public string GetValue()
        {
            return string.Format("{0}{1}{2}", _leftTerm.GetValue(), Operator.Description(), _rightTerm.GetValue());
        }

        public BinaryOperation (ILogicalTerm leftTerm, ILogicalTerm rightTerm, LogicalOperator operation)
        {
            _operator = operation;
            _leftTerm = leftTerm;
            _rightTerm = rightTerm;
        }
    }
}
