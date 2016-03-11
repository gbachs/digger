using System;

namespace Digger
{
    public static class SysTime
    {
        public static Func<DateTime> Now { get; set; }

        static SysTime()
        {
            Now = () => DateTime.UtcNow;
        }
    }
}