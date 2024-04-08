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

1. Version Update & Tagging.
2. GitHub Prerelease Creation.
3. Source Code Download.
4. Artifact Staging & Signing.
5. Voting Process Execution.
6. Vote Summary.
7. Release Artifacts.
8. GitHub Release Creation.
9. Release Announcement.

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

Define the version to be released (adjust to the real version):

```shell
export DOTPULSAR_VERSION=3.0.0
export DOTPULSAR_VERSION_RC=3.0.0-rc.1
```

1. Version Update & Tagging.

Update the information in `CHANGELOG.md` and send a PR with the changes.

Be sure to make the commit message `Make ready for release $DOTPULSAR_VERSION_RC`

2. GitHub Prerelease Creation.

Browse to https://github.com/apache/pulsar-dotpulsar/releases/new

Create a tag `$DOTPULSAR_VERSION_RC`

Create a release title: `DotPulsar $DOTPULSAR_VERSION_RC`

Navigate to the GitHub page for Pulsar at https://github.com/apache/pulsar-dotpulsar/blob/master/CHANGELOG.md. Locate the specific version labeled X.X.X-rc.X in the changelog and transfer all the modifications or updates listed for that version into the designated 'Describe this release' text box. 

Make sure the `Set as a pre-release` checkbox is checked!

Publish the release.

3. Source Code Packaging.

Check out the RC tag and package source code into a tarball:

```shell
git checkout $DOTPULSAR_VERSION_RC
git archive --format=tar.gz \
  --output="$(pwd)/pulsar-dotpulsar-$DOTPULSAR_VERSION_RC/pulsar-dotpulsar-$DOTPULSAR_VERSION-src.tar.gz" \
  --prefix="pulsar-dotpulsar-$DOTPULSAR_VERSION-src/" HEAD
```

4. Artifact Staging & Signing.

The src artifact need to be signed and uploaded to the dist SVN repository for staging.

```shell
gpg -b --armor pulsar-dotpulsar-$DOTPULSAR_VERSION-src.tar.gz
shasum -a 512 pulsar-dotpulsar-$DOTPULSAR_VERSION-src.tar.gz > pulsar-dotpulsar-$DOTPULSAR_VERSION-src.tar.gz.sha512 
```

Create a candidate directory for uploading artifacts, and then check it out:

```shell
svn mkdir -m "Create DotPulsar pre-release dir" https://dist.apache.org/repos/dist/dev/pulsar/pulsar-dotpulsar-$DOTPULSAR_VERSION_RC
svn co https://dist.apache.org/repos/dist/dev/pulsar/pulsar-dotpulsar-$DOTPULSAR_VERSION_RC
cd pulsar-dotpulsar-$DOTPULSAR_VERSION_RC
```

Copy the signed artifacts into the RC directory and commit

```shell
cp /path/to/pulsar-dotpulsar-$DOTPULSAR_VERSION-src.* .
svn add *
svn ci -m 'Staging artifacts and signature for DotPulsar $DOTPULSAR_VERSION_RC'
```

5. Voting Process Execution.

Send an email to the Pulsar Dev mailing list:

```
To: dev@pulsar.apache.org
Subject: [VOTE] DotPulsar Release $DOTPULSAR_VERSION_RC

Hi everyone,

Please review and vote on the release candidate $N for the version $DOTPULSAR_VERSION, as follows:

[ ] +1, Approve the release
[ ] -1, Do not approve the release (please provide specific comments)

DotPulsar's KEYS file contains the PGP keys we used to sign this release:
https://downloads.apache.org/pulsar/KEYS

Please download these packages and review this release candidate:
- Review release notes
- Download the source package (verify shasum, and asc) and follow the
README.md to build and run DotPulsar.

The vote will be open for at least 72 hours. It is adopted by majority approval, with at least 3 PMC affirmative votes.

Guide for Validating DotPulsar Release on Linux and MacOS
https://github.com/apache/pulsar-dotpulsar/blob/master/docs/release_validation_linux_macos.md

Source file:
https://dist.apache.org/repos/dist/dev/pulsar/pulsar-dotpulsar-$DOTPULSAR_VERSION_RC/

Nuget package:
https://www.nuget.org/packages/DotPulsar/$DOTPULSAR_VERSION_RC

The tag to be voted upon:
https://github.com/apache/pulsar-dotpulsar/tree/$DOTPULSAR_VERSION_RC

SHA-512 checksums:
97bb1000f70011e9a585186590e0688586590e09  pulsar-dotpulsar-$DOTPULSAR_VERSION-src.tar.gz
```

The vote should be open for at least 72 hours (3 days). Votes from Pulsar PMC members will be considered binding, while anyone else is encouraged to verify the release and vote as well.

If the release is approved here, we can then proceed to the next step.

6. Vote Summary.

```
To: dev@pulsar.apache.org
Subject: [RESULT][VOTE] Apache Pulsar DotPulsar $DOTPULSAR_VERSION released

[RESULT][VOTE] Release Apache DotPulsar $DOTPULSAR_VERSION

The vote to release Apache DotPulsar $DOTPULSAR_VERSION has passed.

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

Thank you to all the above members for helping us to verify and vote for the $DOTPULSAR_VERSION release.

Thanks
```

7. Release Artifacts.

Promote the artifacts on the release location (need PMC permissions):

```shell
svn move -m "release $DOTPULSAR_VERSION" https://dist.apache.org/repos/dist/dev/pulsar/pulsar-dotpulsar-$DOTPULSAR_VERSION_RC \
         https://dist.apache.org/repos/dist/release/pulsar/pulsar-dotpulsar-$DOTPULSAR_VERSION
```

Remove the old releases (if any). We only need the latest release there, older releases are available through the Apache archive.

Get the list of releases:

```shell
svn ls https://dist.apache.org/repos/dist/release/pulsar | grep dotpulsar
```

Delete each release (except for the last one):

```shell
svn rm https://dist.apache.org/repos/dist/release/pulsar/pulsar-dotpulsar/<old-releases>
``` 

8. GitHub Release Creation.

Browse to https://github.com/apache/pulsar-dotpulsar/releases/new

If the head of the master branch has moved and therefore the last commit is not named `Make ready for release $DOTPULSAR_VERSION_RC`.
Then change the Target from master to Recent Commits that matches `Make ready for release $DOTPULSAR_VERSION_RC`

Create a tag `$DOTPULSAR_VERSION`.

Create a release title: `DotPulsar $DOTPULSAR_VERSION`

Visit https://github.com/apache/pulsar-dotpulsar/releases/edit/$DOTPULSAR_VERSION_RC. Proceed to extract the release notes from that specific version and seamlessly insert them into the final release.

Make sure the `Set as the latest release` checkbox is checked!

9. Release Announcement.

Once the release process is available, you can announce the release and send an email as below:

```
To: dev@pulsar.apache.org, users@pulsar.apache.org, announce@apache.org
Subject: [ANNOUNCE] Apache Pulsar C# Client DotPulsar $DOTPULSAR_VERSION released

The Apache Pulsar team is proud to announce DotPulsar version $DOTPULSAR_VERSION.

Pulsar is a highly scalable, low-latency messaging platform running on
commodity hardware. It provides simple pub-sub semantics over topics,
guaranteed at least once delivery of messages, automatic cursor management for
subscribers, and cross-datacenter replication.

For Pulsar release details and downloads, visit:
https://github.com/apache/pulsar-dotpulsar/releases/tag/X.X.X

Nuget package:
https://www.nuget.org/packages/DotPulsar/X.X.X

Release Notes are at:
https://github.com/apache/pulsar-dotpulsar/blob/master/CHANGELOG.md

We would like to thank the contributors who made the release possible.

Regards,

The Pulsar Team
```
