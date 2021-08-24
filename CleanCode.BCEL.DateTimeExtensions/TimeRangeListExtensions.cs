
namespace CleanCode.BCEL.DateTimeExtensions
{
    using System.Collections.Generic;
    public static class TimeRangeListExtensions
    {
        public static List<TimeRange> Intersection(this List<TimeRange> baseRanges, TimeRange target)
        {
            // init variable
            List<TimeRange> ranges = new List<TimeRange>();
            // loop through baseRanges
            for (int i = 0; i < baseRanges.Count; i++)
            {
                var range = baseRanges[i];
                TimeRange result = range.GetOverlapRange(target);
                if (result != null) ranges.Add(result);
            }

            return ranges;
        }

        public static bool AnyOverlap(this List<TimeRange> baseRanges, TimeRange target)
        {
            foreach (var baseRange in baseRanges)
            {
                if (baseRange.IsOverLap(target)) return true;
            }
            return false;
        }

        public static bool AllOverlap(this List<TimeRange> baseRanges, TimeRange target)
        {
            //bool all = true;
            foreach (var baseRange in baseRanges)
            {
                if (!baseRange.IsOverLap(target)) return false;
            }
            return true;
        }
    }
}
