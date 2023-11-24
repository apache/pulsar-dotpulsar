<!--

    Licensed to the Apache Software Foundation (ASF) under one
    or more contributor license agreements.  See the NOTICE file
    distributed with this work for additional information
    regarding copyright ownership.  The ASF licenses this file
    to you under the Apache License, Version 2.0 (the
    "License"); you may not use this file except in compliance
    with the License.  You may obtain a copy of the License at

      http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing,
    software distributed under the License is distributed on an
    "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
    KIND, either express or implied.  See the License for the
    specific language governing permissions and limitations
    under the License.

-->

This guide illustrates how to perform a release for Apache DotPulsar.

In general, you need to perform the following steps:

1. Update the version and tag of a package.
2. Create a GitHub prerelease.
3. Download the source code.
4. Sign and stage the artifacts.
5. Run a vote.
6. Summary of vote.
7. Promote the release.
8. Create a GitHub release.
9. Announce the release.

#### Managing backporting of patches
When we need to create a backporting patch, we simply create a branch for the version that requires the backport.
We can easily locate a specific version using git tags. We do not create a branch until the actual need arises.

### Requirements
- [Creating GPG keys to sign release artifacts](https://pulsar.apache.org/contribute/create-gpg-keys/)

## Versioning
Bump up the version number as follows.

* Major version (e.g. 2.8.0 => 3.0.0)
    * Changes that break backward compatibility
* Minor version (e.g. 2.8.0 => 2.9.0)
    * Backward compatible new features
* Patch version (e.g. 2.8.0 => 2.8.1)
    * Backward compatible bug fixes

## Steps in detail

**In these instructions, I'm referring to an fictitious release `X.X.X`. Change the release version in the examples accordingly with the real version.**

1. Update the version and tag of a package.

Update the information in `CHANGELOG.md` and send a PR for with the changes.

Be sure to make the commit message `Make ready for release X.X.X-rc-X`

2. Create a GitHub prerelease.

Browse to https://github.com/apache/pulsar-dotpulsar/releases/new

Create a tag `X.X.X-rc-X`

Create a release title: `DotPulsar X.X.X-rc-X`

Generated release notes.

Make sure the `Set as a pre-release` checkbox is checked!

Publish the release.

3. Download the source code.

Browse to https://github.com/apache/pulsar-dotpulsar/releases to download the source code in tar.gz.

4. Sign and stage the artifacts.

The src artifact need to be signed and uploaded to the dist SVN repository for staging.

```
$ gpg -b --armor pulsar-dotpulsar-X.X.X-src.tar.gz
$ shasum -a 512 pulsar-dotpulsar-X.X.X-src.tar.gz > pulsar-dotpulsar-X.X.X-src.tar.gz.sha512 
```

Create a candidate directory for uploading artifacts, and then check it out:

```
$ svn mkdir -m "Create DotPulsar pre-release dir" https://dist.apache.org/repos/dist/dev/pulsar/pulsar-dotpulsar-X.X.X-rc-1
$ svn co https://dist.apache.org/repos/dist/dev/pulsar/pulsar-dotpulsar-X.X.X-rc-1 pulsar-dotpulsar-X.X.X-rc-1
$ cd pulsar-dotpulsar-X.X.X-rc-1
```

Copy the signed artifacts into the rc directory and commit
```
$ cp ../apache-pulsar-dotpulsar-X.X.X-* .
$ svn add *
$ svn ci -m 'Staging artifacts and signature for DotPulsar X.X.X-rc-1'
```

5. Run a vote.

Send an email to the Pulsar Dev mailing list:

```
To: dev@pulsar.apache.org
Subject: [VOTE] DotPulsar Release X.X.X RC 1

Hi everyone,

Please review and vote on the release candidate #1 for the version X.X.X, as follows:
[ ] +1, Approve the release
[ ] -1, Do not approve the release (please provide specific comments)

This is the release candidate for DotPulsar, version X.X.X.

DotPulsar's KEYS file contains PGP keys we used to sign this release:
https://downloads.apache.org/pulsar/KEYS

Please download these packages and review this release candidate:
- Review release notes
- Download the source package (verify shasum, and asc) and follow the
README.md to build and run DotPulsar.

The vote will be open for at least 72 hours. It is adopted by majority approval, with at least 3 PMC affirmative votes.

Source file:
https://dist.apache.org/repos/dist/dev/pulsar/pulsar-dotpulsar-X.X.X-rc-1/

Nuget package:
https://www.nuget.org/packages/DotPulsar/X.X.X-rc.X

The tag to be voted upon:
https://github.com/apache/pulsar-dotpulsar/tree/X.X.X-rc.X

SHA-512 checksums:
97bb1000f70011e9a585186590e0688586590e09  pulsar-dotpulsar-X.X.X-src.tar.gz
```

The vote should be open for at least 72 hours (3 days). Votes from Pulsar PMC members will be considered binding, while anyone else is encouraged to verify the release and vote as well.

If the release is approved here, we can then proceed to the next step.

6. Summary of vote.

```
To: dev@pulsar.apache.org
Subject: [RESULT][VOTE] Apache Pulsar DotPulsar X.X.X released

[RESULT][VOTE] Release Apache DotPulsar X.X.X

The vote to release Apache DotPulsar X.X.X has passed.

The vote PASSED with xxx binding +1 and 0 -1 votes:

Binding votes:
- Yuan Wang
- tison
- hulk
- Liang Chen
- Jean-Baptiste Onofré
- Xiaoqiao He
- donghui liu

Vote thread:

https://lists.apache.org/thread/XXX

Thank you to all the above members to help us to verify and vote for the X.X.X release.

Thanks
```

7. Promote the release.

Promote the artifacts on the release location (need PMC permissions):
```
svn move -m "release 0.X.0" https://dist.apache.org/repos/dist/dev/pulsar/pulsar-dotpulsar-X.X.X-rc-1 \
         https://dist.apache.org/repos/dist/release/pulsar/pulsar-dotpulsar-X.X.X
Remove the old releases (if any). We only need the latest release there, older releases are available through the Apache archive:
```

Get the list of releases
```
svn ls https://dist.apache.org/repos/dist/release/pulsar | grep dotpulsar
```

Delete each release (except for the last one)
```
svn rm https://dist.apache.org/repos/dist/release/pulsar/pulsar-dotpulsar/pulsar-dotpulsar-X.X.X
``` 

8. Create a GitHub release.

Browse to https://github.com/apache/pulsar-dotpulsar/releases/new

If the head of the master branch has moved and therefore the last commit is not named `Make ready for release X.X.X-rc-X`.
Then change the Target from master to Recent Commits that matches `Make ready for release X.X.X-rc-X`

Create a tag `X.X.X`.

Create a release title: `DotPulsar X.X.X`

To Generated release notes select the 'Previous tag: X.X.X' where X.X.X is the last non rc version.

Make sure the `Set as the latest release` checkbox is checked!

9. Announce the release.

Once the release process is available , you can announce the release and send an email as below:

```
To: dev@pulsar.apache.org, users@pulsar.apache.org, announce@apache.org
Subject: [ANNOUNCE] Apache Pulsar C# Client DotPulsar X.X.X released

The Apache Pulsar team is proud to announce DotPulsar version X.X.X.

Pulsar is a highly scalable, low latency messaging platform running on
commodity hardware. It provides simple pub-sub semantics over topics,
guaranteed at-least-once delivery of messages, automatic cursor management for
subscribers, and cross-datacenter replication.

For Pulsar release details and downloads, visit:
https://github.com/apache/pulsar-dotpulsar/releases/tag/X.X.X

Nuget package:
https://www.nuget.org/packages/DotPulsar/X.X.X

Release Notes are at:
https://github.com/apache/pulsar-dotpulsar/blob/master/CHANGELOG.md

We would like to thank the contributors that made the release possible.

Regards,

The Pulsar Team
```
