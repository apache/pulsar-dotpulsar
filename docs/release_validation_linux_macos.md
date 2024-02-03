# Validating DotPulsar release on Linux And MacOS

## Prerequisites

### Install dotnet-sdk

#### MacOS

```
brew install --cask dotnet-sdk
```

#### Linux

You can find install [instructions here](https://learn.microsoft.com/en-us/dotnet/core/install/linux?WT.mc_id=dotnet-35129-website)

### Show dotnet-sdk version

```
dotnet --info
```

## Validating source release

Set exvironment variables
```shell
export DOTPULSAR_VERSION_RC=3.1.2-rc.1
export DOTPULSAR_VERSION=${DOTPULSAR_VERSION_RC%-rc.*}
```

Download files
```shell
wget https://dist.apache.org/repos/dist/dev/pulsar/pulsar-dotpulsar-${DOTPULSAR_VERSION_RC}/pulsar-dotpulsar-${DOTPULSAR_VERSION}-src.tar.gz{,.asc,.sha512}
```

Import GPG public keys
```shell
gpg --import <(curl -s https://downloads.apache.org/pulsar/KEYS)
```

Validate files
```shell
sha512sum -c *.sha512
gpg --verify-files *.asc
```

## Building source package

```shell
tar zxvf pulsar-dotpulsar-${DOTPULSAR_VERSION}-src.tar.gz
cd pulsar-dotpulsar-${DOTPULSAR_VERSION_RC}
dotnet build
```

## Validating Nuget package

Create a simple Pulsar app
```shell
dotnet new console -n PulsarApp
cd PulsarApp
dotnet add package DotPulsar --version "$DOTPULSAR_VERSION_RC"
cat >Program.cs <<EOF
using DotPulsar;
using DotPulsar.Extensions;

const string myTopic = "persistent://public/default/mytopic";

// connecting to pulsar://localhost:6650
await using var client = PulsarClient.Builder().Build();

// consume messages
await using var consumer = client.NewConsumer(Schema.String)
    .SubscriptionName("MySubscription")
    .Topic(myTopic)
    .InitialPosition(SubscriptionInitialPosition.Earliest)
    .Create();

// produce a message
await using var producer = client.NewProducer(Schema.String).Topic(myTopic).Create();
await producer.Send("Hello World");

var message = consumer.Receive().Result;
Console.WriteLine("Received: " + message.Value());
await consumer.Acknowledge(message);
Console.WriteLine("Acknowledged message");
EOF
dotnet build
docker run --name pulsar-standalone -d --rm -it -p 8080:8080 -p 6650:6650 apachepulsar/pulsar:3.0.2 /pulsar/bin/pulsar standalone -nss -nfw
dotnet run
docker stop pulsar-standalone
```