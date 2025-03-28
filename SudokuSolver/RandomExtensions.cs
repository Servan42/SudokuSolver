using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    public static class RandomExtensions
    {
        public static string CharNumString(this Random r, int length)
        {
            var result = new StringBuilder();
            for (var i = 0; i < length; i++)
            {
                result.Append(r.CharNum());
            }
            return result.ToString();
        }

        public static char CharNum(this Random r)
        {
            return (char)r.Next('1', '9' + 1);
        }

        public static bool NextBool(this Random r)
        {
            return r.Next(0, 2) == 0;
        }
    }
}
