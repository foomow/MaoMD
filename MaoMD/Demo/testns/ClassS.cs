using System;
using System.Collections.Generic;
using System.Text;

namespace Demo.testns
{
    static class ClassS
    {
        /// <summary>
        /// sss
        /// </summary>
        /// <param name="str">sss</param>
        /// <param name="f_r">rrr</param>
        /// <param name="f_in">iii</param>
        /// <param name="f_out">ooo</param>
        public static void toIn(this string str, ref int f_r, in int f_in, out int f_out)
        {
            f_out = 22;
        }
    }
}
