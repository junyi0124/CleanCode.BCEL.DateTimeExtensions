using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CleanCode.BCEL.DateTimeExtensions.Test
{
    [TestClass]
    public class TimeRangeUnitTest
    {
        [TestMethod]
        public void TimeRangeCanBeCreatedByDate()
        {
            // arrange
            // act
            var rt1 = new TimeRange(DateTime.Parse("2021-7-1 07:07:07"), DateTime.Now);

            // asert
            Assert.IsNotNull(rt1);
            Assert.IsTrue(rt1.Minutes > 100);
            Assert.IsTrue(rt1.Seconds > 0);

        }

        [TestMethod]
        public void TimeRangeCanBeCreatedByDateTimeString()
        {
            // arrange
            var rt1 = new TimeRange("2021-7-1 07:07:07", "2021-7-1 08:17:09");
            var rt2 = new TimeRange("2021-7-1 07:07:07", "2021-7-1 08:17:09");
            // act
            // asert
            Assert.IsNotNull(rt1);
            Assert.IsTrue(rt1.Minutes == 70);
            Assert.IsTrue(rt1.Seconds == 2);
            var ex = Assert.ThrowsException<ArgumentException>(() => rt2.End = DateTime.Now);
            Assert.IsNotNull(ex);
        }


        [TestMethod]
        public void TimeRangeCanBeCreatedByStartTimeString()
        {
            // arrange
            // act
            var rt1 = new TimeRange("2021-7-1 07:07:07", "");
            var rt2 = new TimeRange("2021-7-1 07:07:07", null);
            var rt3 = TimeRange.StartWith(DateTime.Parse("2021-7-1 07:07:07"));


            // asert
            Assert.IsNotNull(rt1);
            Assert.IsNotNull(rt2);
            Assert.IsNotNull(rt3);

            // re-act
            var date = DateTime.Parse("2021-7-1 07:07:08");
            rt1.End = date;
            rt2.End = date;
            rt3.End = date;

            // re-asert
            Assert.AreEqual(rt1.End, date);
            Assert.AreEqual(rt2.End, date);
            Assert.AreEqual(rt3.End, date);
        }

        /*                  8:00             12:00
        ----------------------|-------A-------|----------------------
        1   |----B1----|                                        "07:07:07", "07:59:27"
        2          |----B2----|                                 "07:25:07", "08:00:00"
        3              |----B3----|                             "07:55:07", "08:10:03"
        4              |----------B4----------|                 "07:55:07", "12:00:00"
        5              |----------------B5----------------|     "07:55:07", "12:30:00"
        6                     |----B6----|                      "08:00:00", "11:50:00"
        ----------------------|-------A-------|----------------------
        7                     |------B7-------|                 "08:00:00", "12:00:00"
        8                     |----------B8-----------|         "08:00:00", "13:33:00"
        9                        |----B9----|                   "09:10:00", "11:00:00"
        10                       |----B10-----|                 "09:20:00", "12:00:00"
        11                       |--------B11--------|          "09:20:00", "12:00:00"
        12                                    |----B12----|     "12:00:00", "14:00:00"
        13                                       |----B13----|  "12:20:00", "14:20:00"
        ----------------------|-------A-------|----------------------
                             8:00             12:00
        */
        [TestMethod]
        public void TimeRangeIsOverlap()
        {
            // arrange
            var morning = new TimeRange("08:00:00", "12:00:00");
            var afternoon = new TimeRange("13:00:00", "17:00:00");
            // act
            var rt1 = new TimeRange("07:07:07", "07:59:27");
            var rt2 = new TimeRange("07:25:07", "08:00:00");
            var rt3 = new TimeRange("07:55:07", "08:10:03");
            var rt4 = new TimeRange("07:55:07", "12:00:00");
            var rt5 = new TimeRange("07:55:07", "12:30:00");
            var rt6 = new TimeRange("08:00:00", "11:50:00");
            var rt7 = new TimeRange("08:00:00", "12:00:00");
            var rt8 = new TimeRange("08:00:00", "13:33:00");
            var rt9 = new TimeRange("09:10:00", "11:00:00");
            var rt10 = new TimeRange("09:20:00", "12:00:00");
            var rt11 = new TimeRange("09:20:00", "12:00:00");
            var rt12 = new TimeRange("12:00:00", "14:00:00");
            var rt13 = new TimeRange("12:20:00", "14:20:00");
            // asert
            Assert.IsFalse(morning.IsOverLap(rt1));
            Assert.IsFalse(morning.IsOverLap(rt2));
            Assert.IsTrue(morning.IsOverLap(rt3));
            Assert.IsTrue(morning.IsOverLap(rt4));
            Assert.IsTrue(morning.IsOverLap(rt5));
            Assert.IsTrue(morning.IsOverLap(rt6));
            Assert.IsTrue(morning.IsOverLap(rt7));
            Assert.IsTrue(morning.IsOverLap(rt8));
            Assert.IsTrue(morning.IsOverLap(rt9));
            Assert.IsTrue(morning.IsOverLap(rt10));
            Assert.IsTrue(morning.IsOverLap(rt11));
            Assert.IsFalse(morning.IsOverLap(rt12));
            Assert.IsFalse(morning.IsOverLap(rt13));
        }
        /*                  8:00             12:00
        ----------------------|-------A-------|----------------------
        1   |----B1----|                                        "07:07:07", "07:59:27" no
        2          |----B2----|                                 "07:25:07", "08:00:00" no
        3              |----B3----|                             "07:55:07", "08:10:03" 10m 3s
        4              |----------B4----------|                 "07:55:07", "12:00:00" 4h = 240m
        5              |----------------B5----------------|     "07:55:07", "12:30:00" 4h = 240m
        6                     |----B6----|                      "08:00:00", "11:50:00" 3h 50m = 230m
        ----------------------|-------A-------|----------------------
        7                     |------B7-------|                 "08:00:00", "12:00:00" 4h = 240m
        8                     |----------B8-----------|         "08:00:00", "13:33:00" 4h = 240m
        9                        |----B9----|                   "09:10:00", "11:00:00" 110m
        10                       |----B10-----|                 "09:20:00", "12:00:00" 160m
        11                       |--------B11--------|          "09:20:00", "14:00:00" 2h 40m =160m
        12                                    |----B12----|     "12:00:00", "14:00:00" no
        13                                       |----B13----|  "12:20:00", "14:20:00" no
        ----------------------|-------A-------|----------------------
                             8:00             12:00
        */
        [TestMethod]
        public void TimeRangeOverlapGetMinutes()
        {
            // arrange
            var morning = new TimeRange("08:00:00", "12:00:00");
            var rt1 = new TimeRange("07:07:07", "07:59:27");
            var rt2 = new TimeRange("07:25:07", "08:00:00");
            var rt3 = new TimeRange("07:55:07", "08:10:03");
            var rt4 = new TimeRange("07:55:07", "12:00:00");
            var rt5 = new TimeRange("07:55:07", "12:30:00");
            var rt6 = new TimeRange("08:00:00", "11:50:00");
            var rt7 = new TimeRange("08:00:00", "12:00:00");
            var rt8 = new TimeRange("08:00:00", "13:33:00");
            var rt9 = new TimeRange("09:10:00", "11:00:00");
            var rt10 = new TimeRange("09:20:00", "12:00:00");
            var rt11 = new TimeRange("09:20:00", "12:00:00");
            var rt12 = new TimeRange("12:00:00", "14:00:00");
            var rt13 = new TimeRange("12:20:00", "14:20:00");
            // act
            // asert
            Assert.IsTrue(morning.GetOverlapRange(rt1) == null);
            Assert.IsTrue(morning.GetOverlapRange(rt2) == null);
            Assert.IsTrue(morning.GetOverlapRange(rt3).Minutes == 10);
            Assert.IsTrue(morning.GetOverlapRange(rt4).Minutes == 240);
            Assert.IsTrue(morning.GetOverlapRange(rt5).Minutes == 240);
            Assert.IsTrue(morning.GetOverlapRange(rt6).Minutes == 230);
            Assert.IsTrue(morning.GetOverlapRange(rt7).Minutes == 240);
            Assert.IsTrue(morning.GetOverlapRange(rt8).Minutes == 240);
            Assert.IsTrue(morning.GetOverlapRange(rt9).Minutes == 110);
            Assert.IsTrue(morning.GetOverlapRange(rt10).Minutes == 160);
            Assert.IsTrue(morning.GetOverlapRange(rt11).Minutes == 160);
            Assert.IsTrue(morning.GetOverlapRange(rt12) == null);
            Assert.IsTrue(morning.GetOverlapRange(rt13) == null);
        }


        /*                  8:00             12:00
        ----------------------|-------A-------|----------------------
        1   |----B1----|                                        "07:07:07", "07:59:27" no
        2          |----B2----|                                 "07:25:07", "08:00:00" no
        3              |----B3----|                             "07:55:07", "08:10:03" 10m 3s
        4              |----------B4----------|                 "07:55:07", "12:00:00" 4h = 240m
        5              |----------------B5----------------|     "07:55:07", "12:30:00" 4h = 240m
        6                     |----B6----|                      "08:00:00", "11:50:00" 3h 50m = 230m
        ----------------------|-------A-------|----------------------
        7                     |------B7-------|                 "08:00:00", "12:00:00" 4h = 240m
        8                     |----------B8-----------|         "08:00:00", "13:33:00" 4h = 240m
        9                        |----B9----|                   "09:10:00", "11:00:00" 110m
        10                       |----B10-----|                 "09:20:00", "12:00:00" 160m
        11                       |--------B11--------|          "09:20:00", "14:00:00" 2h 40m= 160m
        12         |-------------------------------B12----|     "12:00:00", "14:00:00" 4h+1h = 300m
        13           |--------------------------------B13----|  "12:20:00", "14:20:00" 4h+1h20m = 320m
        ----------------------|-------A-------|----------------------
                             8:00             12:00
        */
        [TestMethod]
        public void TimeRangeOverlapGetMinutes2()
        {
            // arrange
            var morning = new TimeRange("08:00:00", "12:00:00");
            var afternoon = new TimeRange("13:00:00", "17:00:00");
            var WorkTimeRange = new List<TimeRange> { morning, afternoon };

            var rt1 = new TimeRange("07:07:07", "07:59:27");
            var rt2 = new TimeRange("07:25:07", "08:00:00");
            var rt3 = new TimeRange("07:55:07", "08:10:03");
            var rt4 = new TimeRange("07:55:07", "12:00:00");
            var rt5 = new TimeRange("07:55:07", "12:30:00");
            var rt6 = new TimeRange("08:00:00", "11:50:00");
            var rt7 = new TimeRange("08:00:00", "12:00:00");
            var rt8 = new TimeRange("08:00:00", "13:33:00");
            var rt9 = new TimeRange("09:10:00", "11:00:00");
            var rt10 = new TimeRange("09:20:00", "12:00:00");
            var rt11 = new TimeRange("09:20:00", "12:00:00");
            var rt12 = new TimeRange("07:00:00", "14:00:00");
            var rt13 = new TimeRange("07:20:00", "14:20:00");
            // act
            // asert
            Assert.IsTrue(WorkTimeRange.Intersection(rt1).Count == 0);

            Assert.IsTrue(WorkTimeRange.Intersection(rt2).Count == 0);

            var list3 = WorkTimeRange.Intersection(rt3);
            Assert.IsNotNull(list3);
            Assert.IsTrue(list3.Count == 1);
            Assert.IsTrue(list3.Select(x => x.Minutes).Sum() == 10);

            var list4 = WorkTimeRange.Intersection(rt4);
            Assert.IsNotNull(list4);
            Assert.IsTrue(list4.Count == 1);
            Assert.IsTrue(list4.Select(x => x.Minutes).Sum() == 240);

            var list5 = WorkTimeRange.Intersection(rt5);
            Assert.IsNotNull(list5);
            Assert.IsTrue(list5.Count == 1);
            Assert.IsTrue(list5.Select(x => x.Minutes).Sum() == 240);

            var list6 = WorkTimeRange.Intersection(rt6);
            Assert.IsNotNull(list6);
            Assert.IsTrue(list6.Count == 1);
            Assert.IsTrue(list6.Select(x => x.Minutes).Sum() == 230);

            var list7 = WorkTimeRange.Intersection(rt7);
            Assert.IsNotNull(list7);
            Assert.IsTrue(list7.Count == 1);
            Assert.IsTrue(list7.Select(x => x.Minutes).Sum() == 240);

            var list8 = WorkTimeRange.Intersection(rt8);
            Assert.IsNotNull(list8);
            Assert.IsTrue(list8.Count == 2);
            Assert.IsTrue(list8.Select(x => x.Minutes).Sum() == 273);

            var list9 = WorkTimeRange.Intersection(rt9);
            Assert.IsNotNull(list9);
            Assert.IsTrue(list9.Count == 1);
            Assert.IsTrue(list9.Select(x => x.Minutes).Sum() == 110);

            var list10 = WorkTimeRange.Intersection(rt10);
            Assert.IsNotNull(list10);
            Assert.IsTrue(list10.Count == 1);
            Assert.IsTrue(list10.Select(x => x.Minutes).Sum() == 160);

            var list11 = WorkTimeRange.Intersection(rt11);
            Assert.IsNotNull(list11);
            Assert.IsTrue(list11.Count == 1);
            Assert.IsTrue(list11.Select(x => x.Minutes).Sum() == 160);

            var list12 = WorkTimeRange.Intersection(rt12);
            Assert.IsNotNull(list12);
            Assert.IsTrue(list12.Count == 2);
            Assert.IsTrue(list12.Select(x => x.Minutes).Sum() == 300);

            var list13 = WorkTimeRange.Intersection(rt13);
            Assert.IsNotNull(list13);
            Assert.IsTrue(list13.Count == 2);
            Assert.IsTrue(list13.Select(x => x.Minutes).Sum() == 320);
        }
    }

}
