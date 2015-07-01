using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicalQueries
{
    public static class LogicalOperatorDescriptor
    {
        public static string GetCustomDescription(object objEnum)
        {
            var fi = objEnum.GetType().GetField(objEnum.ToString());
            var attributes = (StringValue[])fi.GetCustomAttributes(typeof(StringValue), false);
            return (attributes.Length > 0) ? attributes[0].Value : objEnum.ToString();
        }

        public static string Description(this Enum value)
        {
            return GetCustomDescription(value);
        }
    }
}
