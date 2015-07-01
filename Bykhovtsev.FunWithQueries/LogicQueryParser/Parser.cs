using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace LogicQueryParser
{
    static public class Parser
    {
        private const char _startsWithChar = '^';

        private const char _endsWithChar = '$';

        private static Type[] _stringMethodsTypeArray = new Type[] { typeof(string), typeof(bool), typeof(System.Globalization.CultureInfo) }; 

        private static bool IsLogicalOperator(this char value)
        {
            switch (value)
            {
                case LogicalOperatorChar.AND:
                case LogicalOperatorChar.OR:
                case LogicalOperatorChar.NOT:
                    return true;
                default:
                    return false;
            }
        }

        private static int GetPriority(this char value)
        {
            switch (value)
            {
                case LogicalOperatorChar.AND:
                    return 1;
                case LogicalOperatorChar.OR:
                    return 0;
                case LogicalOperatorChar.NOT:
                    return 2;
                default:
                    throw new ArgumentException("GetPriroity could be applied only to logical characters");
            }
        }

        private static Expression GetStringOperator<T> (this string value, Expression prop)
        {
            try
            {
                bool startsWith = false;
                bool endsWith = false;

                if (value[0] == _startsWithChar)
                {
                    startsWith = true;
                }
                if (value[value.Length - 1] == _endsWithChar)
                {
                    endsWith = true;
                }
                if (startsWith && endsWith)
                {
                    return Expression.Equal(prop, Expression.Constant(value.Substring(1, value.Length - 2)));
                }
                if (startsWith)
                {
                    return Expression.Call(
                        prop,
                        (typeof(string)).GetMethod("StartsWith", _stringMethodsTypeArray),
                        Expression.Constant(value.Substring(1, value.Length - 1)),
                        Expression.Constant(true),
                        Expression.Constant(System.Globalization.CultureInfo.CurrentUICulture)
                    );
                }
                if (endsWith)
                {
                    return Expression.Call(
                        prop,
                        (typeof(string)).GetMethod("EndsWith", _stringMethodsTypeArray),
                        Expression.Constant(value.Substring(0, value.Length - 1)),
                        Expression.Constant(true),
                        Expression.Constant(System.Globalization.CultureInfo.CurrentUICulture)
                    );
                }

                return Expression.Call(
                        prop,
                        (typeof(string)).GetMethod("Contains", new Type[]{typeof(string)}),
                        Expression.Constant(value)
                    );
            }
            catch (Exception ex)
            {
                throw new Exception("Can not construct Expression by input string", ex);
            }
        }

        public static List<string> ConvertToRpn (char[] rawQuery)
        {
            List<string> result = new List<string>();
            Stack<char> operatorStack = new Stack<char>();
            StringBuilder curStr = new StringBuilder();

            foreach (char c in rawQuery)
            {
                if (c.IsLogicalOperator())
                {
                    if (curStr.Length > 0)
                    {
                        result.Add(curStr.ToString());
                        curStr.Clear();
                    }

                    while (operatorStack.Any() 
                        && operatorStack.Peek() != '(' 
                        && operatorStack.Peek().GetPriority() > c.GetPriority())
                    {
                        result.Add(operatorStack.Pop().ToString());
                    }

                    operatorStack.Push(c);
                }
                else if (c == '(')
                {
                    operatorStack.Push(c);
                }
                else if (c == ')')
                {
                    result.Add(curStr.ToString());
                    curStr.Clear();
                    try
                    {
                        char op = operatorStack.Pop();
                        while (op != '(')
                        {
                            result.Add(op.ToString());
                            op = operatorStack.Pop();
                        }
                    }
                    catch (InvalidOperationException ex)
                    {
                        throw new ArgumentException("Looks like query was invalid. Check braces", ex);
                    }
                }
                else
                {
                    curStr.Append(c);
                }
            }
            if (curStr.Length > 0)
            {
                result.Add(curStr.ToString());
                curStr.Clear();
            }
            while (operatorStack.Any())
            {
                if (operatorStack.Peek() == '(')
                    throw new ArgumentException("Looks like query was invalid. Check braces");

                result.Add(operatorStack.Pop().ToString());
            }

            return result;
        }

        public static Expression Parse (string[] rpnQuery)
        {
            try
            {
                Stack<Expression> stack = new Stack<Expression>();

                int i = 0;
                while (i < rpnQuery.Length )
                {
                    if (rpnQuery[i].Length == 1 && rpnQuery[i][0].IsLogicalOperator())
                    {
                        if (rpnQuery[i][0] == LogicalOperatorChar.NOT)
                        {
                            stack.Push(Expression.Not(stack.Pop()));
                        }
                        else if (rpnQuery[i][0] == LogicalOperatorChar.AND)
                        {
                            var right = stack.Pop();
                            stack.Push(Expression.AndAlso(stack.Pop(),right));
                        }
                        else if (rpnQuery[i][0] == LogicalOperatorChar.OR)
                        {
                            var right = stack.Pop();
                            stack.Push(Expression.OrElse(stack.Pop(), right));
                        }
                    }
                    else
                    {
                        stack.Push(Expression.Constant(Convert.ToBoolean(rpnQuery[i])));
                    }

                    ++i;
                }

                return stack.Peek();
            }
            catch (Exception ex)
            {
                throw new Exception("Can not parse rpnQuery", ex);
            }

            throw new NotImplementedException();
        }

        public static Expression Parse (string rawQuery)
        {
            return Parse(ConvertToRpn(rawQuery.ToCharArray()).ToArray());
        }

        public static Expression Parse<T> (string rawQuery, Expression<Func<T,string>> prop)
        {
            var mex = prop.Body as MemberExpression;
            if (mex == null) throw new ArgumentException("There should be class member in prop");

            var pi = mex.Member as PropertyInfo;
            if (pi == null) throw new ArgumentException("There should be property in prop");

            var pe = prop.Body;

            var rpnQuery = ConvertToRpn(rawQuery.ToCharArray());

            try
            {
                Stack<Expression> stack = new Stack<Expression>();

                int i = 0;
                while (i < rpnQuery.Count)
                {
                    if (rpnQuery[i].Length == 1 && rpnQuery[i][0].IsLogicalOperator())
                    {
                        if (rpnQuery[i][0] == LogicalOperatorChar.NOT)
                        {
                            stack.Push(Expression.Not(stack.Pop()));
                        }
                        else if (rpnQuery[i][0] == LogicalOperatorChar.AND)
                        {
                            var right = stack.Pop();
                            stack.Push(Expression.AndAlso(stack.Pop(), right));
                        }
                        else if (rpnQuery[i][0] == LogicalOperatorChar.OR)
                        {
                            var right = stack.Pop();
                            stack.Push(Expression.OrElse(stack.Pop(), right));
                        }
                    }
                    else
                    {
                        stack.Push(rpnQuery[i].GetStringOperator<T>(pe));
                    }

                    ++i;
                }

                return stack.Peek();
            }
            catch (Exception ex)
            {
                throw new Exception("Can not parse query", ex);
            }

            throw new NotImplementedException();
        }
    }
}
