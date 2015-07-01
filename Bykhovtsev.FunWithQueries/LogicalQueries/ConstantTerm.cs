using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicalQueries
{
    class ConstantTerm : ILogicalTerm
    {
        private string _value;

        public string GetValue()
        {
            return _value;
        }

        public ConstantTerm(string value)
        {
            _value = value;
        }
    }
}
