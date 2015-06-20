using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabyBrother.Base
{
    public static class TimeUtilities
    {
        public static bool IsValid(DateTimeOffset? time)
        {
            return time != null && IsValid(time.Value);
        }

        public static bool IsValid(DateTimeOffset time)
        {
            return time != DateTimeOffset.MinValue && time != new DateTimeOffset();
        }
    }
}
