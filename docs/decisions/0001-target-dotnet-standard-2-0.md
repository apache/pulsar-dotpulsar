# Target .NET Standard 2.0

## Context and Problem Statement

Targeting multiple frameworks might increase the maintenance cost and therefore require careful consideration.
The added maintenance cost of a target, if any, must be justifiable considering the expected user base increase that the target brings.

Maintenance costs are typically related to the challenge of only using the lowest common denominator regarding APIs and dependencies or writing target-specific code.
Usually, the latter is chosen to gain the advantages of e.g. speed and code readability that the new APIs and dependencies offer.

## Decision Drivers

* We want to target as many of the supported .NET versions as possible
* We want to ensure that the code base is readable, evolvable, and performant

## Considered Options

* Target .NET Standard 2.0
* Target supported .NET Frameworks
* Don't target .NET Standard and .NET Framework

## Decision Outcome

Chosen option: "Target .NET Standard 2.0", because we can support a lot of "legacy" software with little effort.

Targeting .NET Framework(s) directly will require developers and the build and deployment pipelines to use Windows-only versions of .NET.
Therefore .NET Framework should be targeted indirectly by targeting .NET Standard instead.
The latest (and final) version of .NET Standard is 2.1 and it's not supported by any versions of .NET Framework. However, .NET Standard 2.0 is supported by .NET Framework 4.6.2 and up. 
As of this writing (16th of November 2023), the latest version of .NET Framework is version 4.8.1, which was released on the 9th of August 2022.
Microsoft has set an end date for support for .NET Framework 4.6.2 on the 12th of January 2027 and there is currently no end date for .NET Framework 4.8.1.
We can therefore expect .NET Framework to live for many years to come. 

## Pros and Cons of the Options

### Target .NET Standard 2.0

* Good, because it is supported by .NET Framework 4.6.2 and up
* Bad, because some NuGet packages have removed support for it
* Bad, because some target-specific code must be written to support it

### Target .NET Framework

* Good, because it has a huge user base
* Good, because a lot of versions are still supported by Microsoft
* Bad, because it is Windows only
* Bad, because some NuGet packages have removed support for it
* Bad, because some target-specific code must be written to support it

### Don't target .NET Standard and .NET Framework

* Good, because the developers can use the latest APIs and dependencies
* Bad, because we are losing a huge user base