// using System.IO;
// using System.Text.Json;
// using System.Text.Json.Serialization;
// using Microsoft.Azure.Cosmos;

// namespace KC.Persistence.Common.Serializers
// {

//     public class CustomJsonCosmosSerializer : CosmosSerializer
//     {
//         private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
//         {
//             DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
//         };

//         public override T FromStream<T>(Stream stream)
//         {
//             using (stream)
//             {
//                 if (typeof(Stream).IsAssignableFrom(typeof(T)))
//                 {
//                     return (T)(object)stream;
//                 }
//                 return JsonSerializer.Deserialize<T>(stream, _jsonSerializerOptions)!;
//             }
//         }

//         public override Stream ToStream<T>(T input)
//         {
//             MemoryStream memoryStream = new MemoryStream();
//             using (Utf8JsonWriter streamWriter = new Utf8JsonWriter(memoryStream))
//             {
//                 JsonSerializer.Serialize(streamWriter, input, _jsonSerializerOptions);
//                 streamWriter.Flush();
//             }

//             memoryStream.Position = 0L;
//             return memoryStream;
//         }
//     }
// }
