<a name="readme-top"></a>

<br />
<div align="center">
<h3 align="center">C# coding style guide</h3>
  <p align="center">
    DotPulsar style guide for C#
  </p>
</div>

<!-- TABLE OF CONTENTS -->

<!-- TOC start -->
- [Why we need a style guide](#why-we-need-a-style-guide)
- [The Solution](#the-solution)
    * [Philosophy behind the choices made in the EditorConfig and DotSettings](#philosophy)
- [Setup auto clean in IDEs](#setup-auto-clean-in-ides)
    * [Rider](#rider)
    * [Visual Studio with Resharper](#visual-studio-with-resharper)
    * [Visual Studio.](#visual-studio)
<!-- TOC end -->

<!-- TOC --><a name="why-we-need-a-style-guide"></a>
## Why we need a style guide

<a name="why-we-need-a-style-guide"></a>

* Create a consistent look to the code, so that developers can focus on content.
* Onboarding new developers become easier because the code is uniform and easier to understand.
* Reduction of development time by allowing code reviewers to focus on substance.

<!-- TOC --><a name="the-solution"></a>
## The Solution

Introducing the EditorConfig and DotSettings files!

The [EditorConfig] helps maintain consistent coding styles for multiple developers working on the same project across various editors and IDEs.
The EditorConfig is used to define coding styles, and because the file is human-readable it works nicely with version control systems.

DotSettings is a file used to set [Layer-based settings] for Rider and ReSharper.
This file contains the 'DotPulsar: Full Cleanup' cleanup profile and the setting for enabling auto clean up on save for Visual Studio with ReSharper.

<!-- TOC --><a name="philosophy"></a>
### Philosophy behind the choices made in the EditorConfig and DotSettings

The editorconfig default settings have been chosen to prioritize style compatibility between Jetbrains and Microsoft offerings.
Developers should still have the choice of what style overall makes sense for them in a given coding scenario.
Therefore the configs contain no strict rules on if a method should be an expression body or block body.
But it will enforce small things, like an empty space at the end of each cs file.

## Setup auto clean in IDEs

<!-- TOC --><a name="rider"></a>
### Rider

1. In the commit window, click the gear icon.
2. Make sure only the "Cleanup 'DotPulsar: Full Cleanup' profile" is checked. ("Check TODO" can be left on or off according to your preference)

![riderAutoCleanOnCommit](/docs/dev/assets/riderAutoCleanOnCommit.gif)

<!-- TOC --><a name="visual-studio-with-resharper"></a>
### Visual Studio with Resharper

1. Goto Extensions, then ReSharper and click Options...
2. Find Code Editing then Code Cleanup and click General
3. Check "Automatically run cleanup when saving a file (not supported for shared/linked files)"
4. Goto Profiles and make sure the profile 'DotPulsar: Full Cleanup' is set as the default
5. Save

![vs2022WithReSharper](/docs/dev/assets/vs2022WithReSharper.gif)

<!-- TOC --><a name="visual-studio"></a>
### Visual Studio.

To get Visual Studio to automatically enforce the code style on file save, do the following

1. Goto Tools and then Options...
2. Text Editor -> Code Cleanup
3. Check the box "Run Code Cleanup profile on Save"
4. click ok

![vs2022CleanOnSave](/docs/dev/assets/vs2022CleanOnSave.gif)

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- MARKDOWN LINKS & IMAGES -->

<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->

[EditorConfig]: https://editorconfig.org/
[Layer-based settings]: https://www.jetbrains.com/help/rider/Sharing_Configuration_Options.html
