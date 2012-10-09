using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DDay.iCal;
using DDay.iCal.Serialization;
using DDay.iCal.Serialization.iCalendar;

namespace DDayUtilities
{
    public class DDayCalendarWriter : iCalendarSerializer
    {
        private readonly iCalendar _ical;
        private readonly TextWriter _writer;

        private bool _initialized;
        private ISerializerFactory _serializerFactory;

        public DDayCalendarWriter(iCalendar iCal, TextWriter writer)
        {
            if (iCal == null)
            {
                throw new ArgumentNullException("iCal");
            }

            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            _ical = iCal;
            _writer = writer;
        }

        public void Write(ICalendarObject calendarObject)
        {
            Initialize();
            WriteObject(calendarObject);
        }

        private void WriteObject(ICalendarObject calendarObject)
        {
            // Get a serializer for each child object.
            var serializer = _serializerFactory.Build(
                calendarObject.GetType(),
                SerializationContext) as IStringSerializer;

            if (serializer != null)
            {
                _writer.Write(serializer.SerializeToString(calendarObject));
            }
        }

        private void Initialize()
        {
            if (_initialized)
            {
                return;
            }

            _initialized = true;

            // Ensure VERSION and PRODUCTID are both set,
            // as they are required by RFC5545.
            var copy = _ical.Copy<iCalendar>();
            if (string.IsNullOrEmpty(_ical.Version))
                copy.Version = CalendarVersions.v2_0;
            if (string.IsNullOrEmpty(copy.ProductID))
                copy.ProductID = CalendarProductIDs.Default;

            _writer.Write(TextUtil.WrapLines("BEGIN:" + _ical.Name.ToUpper()));

            // Get a serializer factory
            _serializerFactory = GetService<ISerializerFactory>();

            // Sort the calendar properties in alphabetical order before
            // serializing them!
            var properties = new List<ICalendarProperty>(_ical.Properties);
            properties.Sort(PropertySorter);

            foreach (ICalendarProperty calendarProperty in properties)
            {
                var stringSerializer = _serializerFactory.Build(
                    calendarProperty.GetType(),
                    SerializationContext) as IStringSerializer;

                if (stringSerializer != null)
                {
                    _writer.Write(stringSerializer.SerializeToString(calendarProperty));
                }
            }

            foreach (ICalendarObject child in _ical.Children)
            {
                WriteObject(child);
            }
        }

        public void Close()
        {
            _writer.Write(TextUtil.WrapLines("END:" + _ical.Name.ToUpper()));
        }
    }
}
