using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicalQueries
{
    class StringValue : Attribute
    {
        public string Value { get; set; }
        public StringValue (string descr)
        {
            this.Value = descr;
        }
    }
}
