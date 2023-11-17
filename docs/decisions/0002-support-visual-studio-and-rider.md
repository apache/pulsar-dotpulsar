# Support Visual Studio and Rider

## Context and Problem Statement

There are multiple IDEs to choose from when coding C#. The three most popular choices are Visual Studio, Visual Studio Code, and Rider.
Some IDEs will add certain files and folders and have their own opinion on when something is a warning and how code should be (auto)formatted.
If multiple IDEs are supported, then it must be ensured that the IDEs agree on warnings and formatting.
This can be achieved by carefully constructing a .editorconfig and *.DotSettings file that aligns formatting and warning behavior.

## Decision Drivers

* We want developers to be able to use their favorite IDE
* We want to ensure that different IDEs don't conflict, require unreasonable restructuring, or disagree on code formatting and warnings

## Considered Options

* Support Visual Studio
* Support Visual Studio Code
* Support Rider

## Decision Outcome

Chosen options: "Support Visual Studio" and "Support Rider", because the .NET developer base is divided between Visual Studio and Rider and we can support both.

Visual Studio Code is not a full-featured IDE and doesn't work with the code base out of the box.
It is, currently, deemed to be too much (and unreasonable) work for too small a developer base to consider adding support for Visual Studio Code.

## Validation

Open the solution and run a solution-wide "Analyze and Code Cleanup" in Visual Studio or "Reformat and Cleanup" in Rider.
Visual Studio and Rider should agree on the end result.

## Pros and Cons of the Options

### Support Visual Studio

* Good, because it has a huge user base
* Bad, because it is Windows only

### Support Visual Studio Code

* Good, because it is cross-platform
* Bad, because it doesn't work with the code base out of the box

### Support Rider

* Good, because it has a huge user base and is cross-platform
* Bad, because it is not free and requires a license