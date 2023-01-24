using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Internal.VisualStudio.Shell
{
    public class Validate2
    {
        public static String IsNotNullAndNotEmpty(String parameter, String name)
        {
            IsNotNull(parameter, name);
            if (String.IsNullOrEmpty(parameter))
            {
                throw new ArgumentException(String.Format("Input parameter '{0}' must not be null or empty", name));
            }
            return parameter;
        }

        public static T IsNotNull<T>(T parameter, String name)
        {
            if (parameter == null)
            {
                throw new ArgumentException(String.Format("Input parameter '{0}' must not be null", name));
            }
            return parameter;
        }

    }
}
