using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DDay.iCal;
using DDay.iCal.Serialization.iCalendar;

namespace DDayUtilities
{
    class Program
    {
        static void Main(string[] args)
        {
            TestDefaultSerialization();
            Console.WriteLine();
            TestDDayCalendarWriter();
        }

        public static void TestDefaultSerialization()
        {
            Console.WriteLine("Serializing using default DDay serializer");

            DateTime startTime = DateTime.Now;

            using (var iCal = new iCalendar())
            {
                iCal.AddLocalTimeZone();
                iCal.AddProperty("X-WR-CALNAME", "CalendarName");

                using (var writer = new FileStream("test.ical", FileMode.OpenOrCreate))
                {
                    for (int count = 0; count < 100000; count++)
                    {
                        var evnt = new Event { Summary = "Event " + count };
                        iCal.Events.Add(evnt);
                    }
                    var serializer = new iCalendarSerializer();
                    serializer.Serialize(iCal, writer, Encoding.UTF8);
                }
            }
            Console.WriteLine("Done: " + (DateTime.Now - startTime));
        }

        public static void TestDDayCalendarWriter()
        {
            Console.WriteLine("Serializing using DDayCalendarWriter");

            DateTime startTime = DateTime.Now;

            using (var iCal = new iCalendar())
            {
                iCal.AddLocalTimeZone();
                iCal.AddProperty("X-WR-CALNAME", "CalendarName");

                using (var stream = new StreamWriter("test2.ical"))
                {
                    var iCalwriter = new DDayCalendarWriter(iCal, stream);
                    for (int count = 0; count < 100000; count++)
                    {
                        var evnt = new Event { Summary = "Event " + count };
                        iCalwriter.Write(evnt);
                    }
                    iCalwriter.Close();
                }
            }
            Console.WriteLine("Done: " + (DateTime.Now - startTime));
        }

    }
}
