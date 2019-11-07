using System;

namespace dCForm.Util
{
    public static class DateTimeExtensions
    {
        /// <summary>
        ///     Calculates what fiscal quarter relative to a startDate
        /// </summary>
        /// <param name="Q1StartDate">
        ///     custom start relative calculation are made from; the U.S. fiscal start day of Sep 10 if not
        ///     specified
        /// </param>
        /// <param name="DateOfInterest">Date of interest the quarter is needed; today if not specified</param>
        /// <returns>1 - 4</returns>
        private static short CalcFiscalQuarter(DateTime? Q1StartDate = null, DateTime? DateOfInterest = null)
        {
            if (DateOfInterest == null)
                DateOfInterest = DateTime.Now;

            if (Q1StartDate == null)
                Q1StartDate = new DateTime(1976, 10, 1);

            Q1StartDate = new DateTime(DateOfInterest.Value.Year, Q1StartDate.Value.Month, Q1StartDate.Value.Day);
            if (Q1StartDate > DateOfInterest.Value)
                Q1StartDate = Q1StartDate.Value.AddYears(-1);

            return (short) Math.Ceiling((decimal) (DateOfInterest.Value - Q1StartDate.Value).Days/(365/4));
        }

        /// <summary>
        ///     Gets what fiscal quarter relative to a startDate
        /// </summary>
        /// <param name="DateOfInterest">Date of interest the quarter is needed</param>
        /// <param name="Q1StartDate">
        ///     custom start relative calculation are made from; the U.S. fiscal start day of Sep 10 if not
        ///     specified
        /// </param>
        /// <returns></returns>
        public static int GetFiscalQuarter(this DateTime DateOfInterest, DateTime? Q1StartDate = null)
        {
            return CalcFiscalQuarter(Q1StartDate, DateOfInterest);
        }
    }
}