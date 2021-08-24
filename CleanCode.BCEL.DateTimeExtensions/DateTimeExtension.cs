namespace CleanCode.BCEL.DateTimeExtensions
{
    using System;
    public static class DateTimeExtension
    {
        /// <summary>
        /// return earlier datetime
        /// </summary>
        /// <param name="dateTime1">one datetime</param>
        /// <param name="dateTime2">another datetime</param>
        /// <returns></returns>
        public static DateTime Min(this DateTime dateTime1, DateTime dateTime2)
        {
            if (dateTime1 <= dateTime2)
                return dateTime1;
            else
                return dateTime2;
        }

        /// <summary>
        /// return later datetime
        /// </summary>
        /// <param name="dateTime1">one datetime</param>
        /// <param name="dateTime2">another datetime</param>
        /// <returns></returns>
        public static DateTime Max(this DateTime dateTime1, DateTime dateTime2)
        {
            if (dateTime1 >= dateTime2)
                return dateTime1;
            else
                return dateTime2;
        }
    }
}
