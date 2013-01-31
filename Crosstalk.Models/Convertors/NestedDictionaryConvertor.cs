//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;

//namespace Crosstalk.Core.Models.Convertors
//{
//    class NestedDictionaryConvertor : JsonConverter
//    {
//        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
//        {
//            writer.WriteValue(JsonConvert.SerializeObject(value));
//        }

//        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
//        {
//            if (null == reader.Value)
//            {
//                return null;
//            }
//            var obj = new Dictionary<string, dynamic>();
//            var scratch = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(reader.Value.ToString());
//            foreach (var item in scratch)
//            {
//                if (item.Value is JArray)
//                {
//                    obj[item.Key] = (item.Value as JArray).Select(t => t.Value<object>()).ToArray();
//                } else if (item.Value is JObject)
//                {
//                    obj[item.Key] = (item.Value as JObject).ToObject<object>();
//                }
//            }
//            return obj;
//        }

//        public override bool CanConvert(Type objectType)
//        {
//            return objectType == typeof (Dictionary<string, dynamic>);
//        }
//    }
//}
