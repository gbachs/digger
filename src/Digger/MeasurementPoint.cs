using System;
using System.Collections.Generic;
using System.Linq;
using EnsureThat;

namespace Digger
{
    public class MeasurementPoint
    {
        private readonly Dictionary<string, string> _tags;
        private readonly Dictionary<string, ValueType> _valueTypeFields;
        private readonly Dictionary<string, string> _stringFields;

        public string Measurement { get; }
        public DateTime TimeStamp { get; private set; }
        public bool HasTags => _tags.Any();
        public bool HasFields => _valueTypeFields.Any();
        public IEnumerable<KeyValuePair<string, string>> Tags => _tags;
        public IEnumerable<KeyValuePair<string, ValueType>> ValueTypeFields => _valueTypeFields;
        public IEnumerable<KeyValuePair<string, string>> StringFields => _stringFields;

        public MeasurementPoint(string measurement)
        {
            Ensure.That(measurement, nameof(measurement)).IsNotNullOrWhiteSpace();

            _tags = new Dictionary<string, string>();
            _valueTypeFields = new Dictionary<string, ValueType>();
            _stringFields = new Dictionary<string, string>();

            Measurement = measurement;
            TimeStamp = SysTime.Now();
        }

        public MeasurementPoint AddTag(string name, string value)
        {
            Ensure.That(name, nameof(name)).IsNotNullOrWhiteSpace();

            _tags.Add(name, value);

            return this;
        }

        public MeasurementPoint AddField(string name, bool value)
        {
            return AddValueTypeField(name, value);
        }

        public MeasurementPoint AddField(string name, int value)
        {
            return AddValueTypeField(name, value);
        }

        public MeasurementPoint AddField(string name, long value)
        {
            return AddValueTypeField(name, value);
        }

        public MeasurementPoint AddField(string name, float value)
        {
            return AddValueTypeField(name, value);
        }

        public MeasurementPoint AddField(string name, double value)
        {
            return AddValueTypeField(name, value);
        }

        public MeasurementPoint AddField(string name, decimal value)
        {
            return AddValueTypeField(name, value);
        }

        public MeasurementPoint AddField(string name, string value)
        {
            Ensure.That(name, nameof(name)).IsNotNullOrWhiteSpace();

            _stringFields.Add(name, value);

            return this;
        }

        private MeasurementPoint AddValueTypeField(string name, ValueType value)
        {
            Ensure.That(name, nameof(name)).IsNotNullOrWhiteSpace();

            _valueTypeFields.Add(name, value);

            return this;
        }

        public MeasurementPoint AddTimeStamp(DateTime value)
        {
            TimeStamp = value;

            return this;
        }
    }
}