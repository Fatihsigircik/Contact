using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Common
{
    internal class Utility
    {
        public static string CheckNullOrEmpty(string value, string parameter)
        {
            return string.IsNullOrEmpty(value) ? throw new ArgumentNullException($"'{nameof(parameter)}' cannot be null or empty.", nameof(parameter)) : value;
        }

    }
}
