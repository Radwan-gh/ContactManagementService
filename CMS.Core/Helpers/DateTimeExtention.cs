using System;
using System.Globalization;

namespace CMS.Core.Helpers
{
    public static class DateTimeExtention
    {
        public static string _DateFormat => "dd-MM-yyyy";
        public static bool ValidateDate(this string date)
        {
            return DateTime.TryParseExact(date,
                _DateFormat,
                new CultureInfo("en"),
                DateTimeStyles.AdjustToUniversal, out DateTime _);
        }
    }
}
