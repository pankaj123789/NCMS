using System;

namespace Ncms.Contracts.Models.Common
{
    public static class ExtentionHelper
    {
        public static string ToLongDateWithoutWeekDayString(this DateTime source)
        {
            return source.ToString("d MMMM yyyy");
        }
    }
}
