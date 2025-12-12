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

namespace DotPulsar.Tests;

using Xunit.Abstractions;

public static class TestOutputHelperExtensions
{
    public static void Log(this ITestOutputHelper helper, string message)
        => helper.WriteLine($"{DateTime.UtcNow:HH:mm:ss}: {message}");

    public static void Log(this ITestOutputHelper helper, ExceptionContext context)
        => helper.Log($"PulsarClient got an exception: {context.Exception}");

    public static void Log(this ITestOutputHelper helper, ConsumerStateChanged stateChange)
        => helper.Log($"The consumer for topic '{stateChange.Consumer.Topic}' changed state to '{stateChange.ConsumerState}'");

    public static void Log(this ITestOutputHelper helper, ProducerStateChanged stateChange)
        => helper.Log($"The producer for topic '{stateChange.Producer.Topic}' changed state to '{stateChange.ProducerState}'");

    public static void Log(this ITestOutputHelper helper, ReaderStateChanged stateChange)
        => helper.Log($"The reader for topic '{stateChange.Reader.Topic}' changed state to '{stateChange.ReaderState}'");
}
