/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace DotPulsar
{
    /// <summary>
    /// The supported schema types for messages.
    /// </summary>
    public enum SchemaType : byte
    {
        /// <summary>
        /// No schema.
        /// </summary>
        None = 0,

        /// <summary>
        /// UTF-8 schema.
        /// </summary>
        String = 1,

        /// <summary>
        /// JSON schema.
        /// </summary>
        Json = 2,

        /// <summary>
        /// Protobuf schema.
        /// </summary>
        Protobuf = 3,

        /// <summary>
        /// Avro schema.
        /// </summary>
        Avro = 4,

        /// <summary>
        /// Boolean schema.
        /// </summary>
        Boolean = 5,

        /// <summary>
        /// 8-byte integer schema.
        /// </summary>
        Int8 = 6,

        /// <summary>
        /// 16-byte integer schema.
        /// </summary>
        Int16 = 7,

        /// <summary>
        ///32-byte integer schema.
        /// </summary>
        Int32 = 8,

        /// <summary>
        /// 64-byte integer schema.
        /// </summary>
        Int64 = 9,

        /// <summary>
        /// Float schema.
        /// </summary>
        Float = 10,

        /// <summary>
        /// Double schema.
        /// </summary>
        Double = 11,

        /// <summary>
        /// Date schema.
        /// </summary>
        Date = 12,

        /// <summary>
        /// Time schema.
        /// </summary>
        Time = 13,

        /// <summary>
        /// Timestamp schema.
        /// </summary>
        Timestamp = 14,

        /// <summary>
        /// KeyValue schema.
        /// </summary>
        KeyValue = 15,

        /// <summary>
        /// Instant schema.
        /// </summary>
        Instant = 16,

        /// <summary>
        /// Local date schema.
        /// </summary>
        LocalDate = 17,

        /// <summary>
        /// Local time schema.
        /// </summary>
        LocalTime = 18,

        /// <summary>
        /// Local data time schema.
        /// </summary>
        LocalDateTime = 19,

        /// <summary>
        /// Protobuf native schema.
        /// </summary>
        ProtobufNative = 20
    }
}
