using System;
using System.Text.RegularExpressions;

namespace ElectricityConsumerApp.Service
{
    internal static class ValidationService
    {
        public static bool IsOnlyNumerics(string value)
        {
            string pattern = @"^[0-9 ]+$";
            return Regex.IsMatch(value, pattern);
        }

        public static bool IsOnlyLetters(string value)
        {
            string pattern = @"^[a-zA-Zа-яА-Я]+$";
            return Regex.IsMatch(value, pattern);
        }

        public static bool IsPastDate(DateTime value)
        {
            return DateTime.Compare(value, DateTime.Now) < 0;
        }
    }
}
