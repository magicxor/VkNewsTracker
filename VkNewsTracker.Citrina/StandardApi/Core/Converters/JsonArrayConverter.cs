﻿using System;
using Citrina.StandardApi.Models;
using Newtonsoft.Json;

namespace Citrina.StandardApi.Core.Converters
{
    internal class JsonArrayConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((JsonArray)value).JsonValue);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(JsonArray);
        }
    }
}
