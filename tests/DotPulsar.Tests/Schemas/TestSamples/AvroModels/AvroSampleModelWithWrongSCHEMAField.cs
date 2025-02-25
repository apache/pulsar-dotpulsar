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

namespace DotPulsar.Tests.Schemas.TestSamples.AvroModels;

using Avro.Specific;
using System;

public class AvroSampleModelWithWrongSCHEMAField : ISpecificRecord
{
    public static string _SCHEMA = "WRONG!";
    public Avro.Schema Schema => throw new NotImplementedException();

    public object Get(int fieldPos)
    {
        throw new NotImplementedException();
    }

    public void Put(int fieldPos, object fieldValue)
    {
        throw new NotImplementedException();
    }
}
