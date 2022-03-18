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

namespace DotPulsar.Internal;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Threading;

public static class DotPulsarMeter
{
#pragma warning disable IDE0079
#pragma warning disable IDE0044
    private static int _numberOfClients;
    private static int _numberOfConnections;
    private static int _numberOfReaders;
    private static int _numberOfConsumers;
    private static int _numberOfProducers;
#pragma warning restore IDE0044
#pragma warning restore IDE0079
    private static readonly Histogram<double> _producerSendDuration;
    private static readonly Histogram<double> _consumerProcessDuration;

    static DotPulsarMeter()
    {
        Meter = new(Constants.ClientName, Constants.ClientVersion);
        _ = Meter.CreateObservableGauge("dotpulsar.client.count", GetNumberOfClients, "{clients}", "Number of clients");
        _ = Meter.CreateObservableGauge("dotpulsar.connection.count", GetNumberOfConnections, "{connections}", "Number of connections");
        _ = Meter.CreateObservableGauge("dotpulsar.reader.count", GetNumberOfReaders, "{readers}", "Number of readers");
        _ = Meter.CreateObservableGauge("dotpulsar.consumer.count", GetNumberOfConsumers, "{consumers}", "Number of consumers");
        _ = Meter.CreateObservableGauge("dotpulsar.producer.count", GetNumberOfProducers, "{producers}", "Number of producers");
        _producerSendDuration = Meter.CreateHistogram<double>("dotpulsar.producer.send.duration", "ms", "Measures the duration for sending a message");
        _consumerProcessDuration = Meter.CreateHistogram<double>("dotpulsar.consumer.process.duration", "ms", "Measures the duration for processing a message");
    }

    public static Meter Meter { get; }

    public static void ClientCreated() => Interlocked.Increment(ref _numberOfClients);
    public static void ClientDisposed() => Interlocked.Decrement(ref _numberOfClients);
    private static int GetNumberOfClients() => Volatile.Read(ref _numberOfClients);

    public static void ConnectionCreated() => Interlocked.Increment(ref _numberOfConnections);
    public static void ConnectionDisposed() => Interlocked.Decrement(ref _numberOfConnections);
    private static int GetNumberOfConnections() => Volatile.Read(ref _numberOfConnections);

    public static void ReaderCreated() => Interlocked.Increment(ref _numberOfReaders);
    public static void ReaderDisposed() => Interlocked.Decrement(ref _numberOfReaders);
    private static int GetNumberOfReaders() => Volatile.Read(ref _numberOfReaders);

    public static void ConsumerCreated() => Interlocked.Increment(ref _numberOfConsumers);
    public static void ConsumerDisposed() => Interlocked.Decrement(ref _numberOfConsumers);
    private static int GetNumberOfConsumers() => Volatile.Read(ref _numberOfConsumers);

    public static void ProducerCreated() => Interlocked.Increment(ref _numberOfProducers);
    public static void ProducerDisposed() => Interlocked.Decrement(ref _numberOfProducers);
    private static int GetNumberOfProducers() => Volatile.Read(ref _numberOfProducers);


    public static bool MessageSentEnabled => _producerSendDuration.Enabled;
    public static void MessageSent(long startTimestamp, KeyValuePair<string, object?>[] tags) =>
        _producerSendDuration.Record(GetMillisecondsTillNow(startTimestamp), tags);

    public static bool MessageProcessedEnabled => _consumerProcessDuration.Enabled;
    public static void MessageProcessed(long startTimestamp, KeyValuePair<string, object?>[] tags)
        => _consumerProcessDuration.Record(GetMillisecondsTillNow(startTimestamp), tags);

    private static double GetMillisecondsTillNow(long startTimestamp)
    {
        var timestampDelta = Stopwatch.GetTimestamp() - startTimestamp;
        var ticks = (long) (Constants.TimestampToTicks * timestampDelta);
        return new TimeSpan(ticks).TotalMilliseconds;
    }
}
