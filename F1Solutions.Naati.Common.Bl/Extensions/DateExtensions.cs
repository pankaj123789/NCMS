using System;


namespace F1Solutions.Naati.Common.Bl.Extensions
{
    public static class DateExtensions
    {
        public static DateTime GetLeapYearAdjustedEndDate(this DateTime startDate, int policyYears)
        {
            var policyDays = 365 * policyYears;
            return startDate.AddDays(policyDays);
        }
    }
}
