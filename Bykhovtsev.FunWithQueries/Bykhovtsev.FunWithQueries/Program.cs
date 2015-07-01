using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LogicQueryParser;

namespace Bykhovtsev.FunWithQueries
{
    class User
    {
        public string Name { get; set; }
    }


    class Program
    {
        static void Main(string[] args)
        {

            Expression exp = LogicQueryParser.Parser.Parse<User>("(^va|sya$&!^du)|^petya$|!go", u => u.Name);

            Console.WriteLine(exp.ToString());
        }
    }
}
