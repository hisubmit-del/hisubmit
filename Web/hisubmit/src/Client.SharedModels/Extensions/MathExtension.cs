using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSubmit.Client.SharedModels.Extensions
{
    public static class MathExtension
    {
        public static int AbsoluteValue(this int number)
        {
            if (number > 0)
            {
                return number;
            }
            else
            {
                return (-(number));
            }
        }
    }
}
