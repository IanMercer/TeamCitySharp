using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamCitySharp
{
    public class TeamCityDateTimeConverter : DateTimeConverterBase
    {
        private const string DateFormat = "yyyyMMddTHHmmsszz00";

        public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.String)
            {
                throw new FormatException(String.Format("Unexpected token parsing date. Expected String, got {0}.", reader.TokenType));
            }

            var dateString = (string)reader.Value;

            // TODO: Handle both DateTime and DateTimeOffset
            //if (objectType == typeof(DateTime))
            //{
            //}

            DateTimeOffset value;
            if (!DateTimeOffset.TryParseExact(dateString, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out value))
            {
                return default(DateTimeOffset);
            }
            return value;
        }

        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            if (value is DateTime)
            {
                writer.WriteValue(((DateTime)value).ToString(DateFormat));
            }
            else if (value is DateTimeOffset)
            {
                writer.WriteValue(((DateTimeOffset)value).ToString(DateFormat));
            }
            else
            {
                throw new Exception("Expected date object value.");
            }
        }
    }
}﻿