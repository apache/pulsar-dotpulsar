# Contribution to DotPulsar

You can contribute to DotPulsar with issues and PRs. Simply filing issues for problems you encounter is a great way to contribute. Contributing implementations is greatly appreciated.

## Reporting Issues

We always welcome bug reports, feature proposals and overall feedback. Here are a few tips on how you can make reporting your issue as effective as possible.

### Finding Existing Issues

Before filing a new issue, please search our [open issues](https://github.com/apache/pulsar-dotpulsar/issues) to check if it already exists.

If you do find an existing issue, please include your own feedback in the discussion. Do consider upvoting (👍 reaction) the original post, as this helps us prioritize popular issues in our backlog.

### Writing a Good Bug Report

Good bug reports make it easier for maintainers to verify and root cause the underlying problem. The better a bug report, the faster the problem will be resolved. Ideally, a bug report should contain the following information:

* A high-level description of the problem.
* A _minimal reproduction_, i.e. the smallest size of code/configuration required to reproduce the wrong behavior.
* A description of the _expected behavior_, contrasted with the _actual behavior_ observed.
* Information on the environment: OS/distro, CPU arch, SDK version, etc.
* Additional information, e.g. are there any known workarounds?

When ready to submit a bug report, please use the [Bug Report issue template](https://github.com/apache/pulsar-dotpulsar/issues/new?assignees=&labels=&template=01_bug_report.yml).

#### Why are Minimal Reproductions Important?

A reproduction lets maintainers verify the presence of a bug, and diagnose the issue using a debugger. A _minimal_ reproduction is the smallest possible console application demonstrating that bug. Minimal reproductions are generally preferable since they:

1. Focus debugging efforts on a simple code snippet,
2. Ensure that the problem is not caused by unrelated dependencies/configuration,
3. Avoid the need to share production codebases.

#### Are Minimal Reproductions Required?

In certain cases, creating a minimal reproduction might not be practical (e.g. due to nondeterministic factors, external dependencies). In such cases you would be asked to provide as much information as possible, for example by sharing a memory dump of the failing application. If maintainers are unable to root cause the problem, they might still close the issue as not actionable. While not required, minimal reproductions are strongly encouraged and will significantly improve the chances of your issue being prioritized and fixed by the maintainers.

#### How to Create a Minimal Reproduction

The best way to create a minimal reproduction is gradually removing code and dependencies from a reproducing app, until the problem no longer occurs. A good minimal reproduction:

* Excludes all unnecessary types, methods, code blocks, source files, nuget dependencies and project configurations.
* Contains documentation or code comments illustrating expected vs actual behavior.
* If possible, avoids performing any unneeded IO or system calls. For example, can the microservice based reproduction be converted to a plain old console app?

### DOs and DON'Ts

Please do:

* **DO** follow our [coding style](/docs/coding-style.md) (C# code-specific).
* **DO** include tests when adding new features. When fixing bugs, start with
  adding a test that highlights how the current behavior is broken.
* **DO** keep the discussions focused. When a new or related topic comes up
  it's often better to create new issue than to side track the discussion.

Please do not:

* **DON'T** surprise us with big pull requests. Instead, file an issue and start
  a discussion so we can agree on a direction before you invest a large amount
  of time.
* **DON'T** submit PRs that alter licensing related files or headers. If you believe there's a problem with them, file an issue and we'll be happy to discuss it.

### Suggested Workflow

We use and recommend the following workflow:

1. Create an issue for your work.
   - Reuse an existing issue on the topic, if there is one.
   - Get agreement from maintainers that your proposed change is a good one.
   - Clearly state that you are going to take on implementing it, if that's the case. You can request that the issue be assigned to you. Note: The issue filer and the implementer don't have to be the same person.
2. Create a personal fork of the repository on GitHub (if you don't already have one).
3. In your fork, create a branch off of main (`git checkout -b my_feature`).
   - Name the branch so that it clearly communicates your intentions, such as issue-123 or githubhandle-issue.
   - Branches are useful since they isolate your changes from incoming changes from upstream. They also enable you to create multiple PRs from the same fork.
4. Make and commit your changes to your branch.
   - Please follow our [Commit Messages](#commit-messages) guidance.
5. Add new tests corresponding to your change, if applicable.
6. Build the repository with your changes.
   - Make sure that the builds are clean.
   - Make sure that the tests are all passing, including your new tests.
7. Create a pull request (PR) against the apache/pulsar-dotpulsar repository's **master** branch.
   - State in the description what issue or improvement your change is addressing.
   - Check if all the Continuous Integration checks are passing.
8. Wait for feedback or approval of your changes from the maintainers.
   - Should you receive feedback. The maintainer will apply the label 'WIP' to indicate that you should incorporate the changes. 
9. If all looks good, and all checks are green, your PR will be merged.
   - The next official build will automatically include your change.
   - You can delete the branch you used for making the change.


If a PR or issue has no activity and thus gone stale, it will be closed after six months.

### Commit Messages

Please format commit messages as follows (based on [A Note About Git Commit Messages](http://tbaggery.com/2008/04/19/a-note-about-git-commit-messages.html)):

```
Summarize change in 50 characters or less

Provide more detail after the first line. Leave one blank line below the
summary and wrap all lines at 72 characters or less.

If the change fixes an issue, leave another blank line after the final
paragraph and indicate which issue is fixed in the specific format
below.

Fix #42
```

### File Headers

The following file header is the used for files in this repo. Please use it for new files.

```
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
```

- See [PulsarClient.cs](https://github.com/apache/pulsar-dotpulsar/blob/master/src/DotPulsar/PulsarClient.cs) for an example of the header in a C# file.
