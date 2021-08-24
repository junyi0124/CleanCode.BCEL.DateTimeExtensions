
namespace CleanCode.BCEL.DateTimeExtensions
{
    using System;
    public class TimeRange
    {
        // 时间段的开始和结束时间，开始时间早于结束时间
        DateTime _start, _end;
        // 时间段的分钟数和秒数
        int _minutes;
        int _seconds;

        public TimeRange(string startAt, string endAt)
        {
            if (DateTime.TryParse(startAt, out _start) && DateTime.TryParse(endAt, out _end))
            {
                checkStartEnd();
            }
        }

        public TimeRange(DateTime start, DateTime end)
        {
            _start = start; _end = end;
            checkStartEnd();
        }

        public TimeRange(long startUnixTime, long endUnixTime)
        {
            DateTimeOffset dateTimeOffSet = DateTimeOffset.FromUnixTimeSeconds(startUnixTime);
            _start = dateTimeOffSet.DateTime;
            dateTimeOffSet = DateTimeOffset.FromUnixTimeSeconds(endUnixTime);
            _end = dateTimeOffSet.DateTime;
            checkStartEnd();
        }

        /// <summary>
        /// 检查开始时间和结束时间的业务逻辑，如果开始时间晚于结束时间，则会抛出异常
        /// </summary>
        void checkStartEnd()
        {
            if (_start > _end) throw new ArgumentException("start date is later then end date.");
            var span = _end - _start;
            _minutes = (int)span.TotalMinutes;
            _seconds = span.Seconds;
        }

        public DateTime Start
        {
            get { return _start; }
        }

        public DateTime End
        {
            get { return _end; }
            set
            {
                if (_end == DateTime.MinValue)
                {
                    _end = value;
                }
                else
                {
                    throw new ArgumentException("End Time ALREADY setted!");
                }
            }
        }

        public int Minutes { get { return _minutes; } }
        public int Seconds { get { return _seconds; } }

        public static TimeRange StartWith(DateTime start)
        {
            return new TimeRange(start.ToShortDateString() + " " + start.ToShortTimeString(), null);
        }

        /*                   8:00             12:00
        ----------------------|-------A-------|----------------|-------conditions-----|---result---|
        1   |----B1----|                                        "07:07:07", "07:59:27" false
        2          |----B2----|                                 "07:25:07", "08:00:00"  true
        3              |----B3----|                             "07:55:07", "08:10:03"  true
        4              |----------B4----------|                 "07:55:07", "12:00:00"  true
        5              |----------------B5----------------|     "07:55:07", "12:30:00"  true
        6                     |----B6----|                      "08:00:00", "11:50:00"  true
        ----------------------|-------A-------|----------------|-------conditions-----|---result---|
        7                     |------B7-------|                 "08:00:00", "12:00:00"  true
        8                     |----------B8-----------|         "08:00:00", "13:33:00"  true
        9                        |----B9----|                   "09:10:00", "11:00:00"  true
        10                       |----B10-----|                 "09:20:00", "12:00:00"  true
        11                       |--------B11--------|          "09:20:00", "12:00:00"  true
        12                                    |----B12----|     "12:00:00", "14:00:00"  true
        13                                       |----B13----|  "12:20:00", "14:20:00" false
        ----------------------|-------A-------|----------------|-------conditions-----|---result---|
                             8:00             12:00
        */
        /// <summary>

        /// </summary>
        /// <param name="timeRange"></param>
        /// <returns></returns>
        public bool IsOverLap(TimeRange timeRange)
        {
            return Start < timeRange.End
                && End > timeRange.Start;
        }

        /// <summary>
        /// 计算两个时间段的交集部分
        /*                  8:00             12:00
        ----------------------|-------A-------|----------------|-------conditions-----|---result---|
        1   |----B1----|                                        "07:07:07", "07:59:27" no
        2          |----B2----|                                 "07:25:07", "08:00:00" no
        3              |----B3----|                             "07:55:07", "08:10:03" 10m 3s
        4              |----------B4----------|                 "07:55:07", "12:00:00" 4h = 240m
        5              |----------------B5----------------|     "07:55:07", "12:30:00" 4h = 240m
        6                     |----B6----|                      "08:00:00", "11:50:00" 3h 50m = 230m
        ----------------------|-------A-------|----------------|-------conditions-----|---result---|
        7                     |------B7-------|                 "08:00:00", "12:00:00" 4h = 240m
        8                     |----------B8-----------|         "08:00:00", "13:33:00" 4h = 240m
        9                        |----B9----|                   "09:10:00", "11:00:00" 110m
        10                       |----B10-----|                 "09:20:00", "12:00:00" 160m
        11                       |--------B11--------|          "09:20:00", "14:00:00" 2h 40m =160m
        12                                    |----B12----|     "12:00:00", "14:00:00" no
        13                                       |----B13----|  "12:20:00", "14:20:00" no
        ----------------------|-------A-------|----------------|-------conditions-----|---result---|
                             8:00             12:00  */
        /// </summary>
        /// <param name="timeRange"></param>
        /// <returns></returns>
        public TimeRange GetOverlapRange(TimeRange targetTime)
        {
            if (!IsOverLap(targetTime)) return null;

            //var start = Start.Min(targetTime.Start);
            //var end = Start.Max(targetTime.End);
            //var gap = (end - start);
            //return gap.Minutes;
            if (targetTime.Start >= Start)
            {
                if (End >= targetTime.End)
                    return new TimeRange(targetTime.Start, targetTime.End);
                else
                    return new TimeRange(targetTime.Start, End);
            }
            else// if (targetTime.Start < Start)
            {
                if (targetTime.End >= End)
                    return new TimeRange(Start, End);
                else
                    return new TimeRange(Start, targetTime.End);
            }
        }
    }
}
