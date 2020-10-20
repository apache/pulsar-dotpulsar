# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/), and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- ExceptionHandler method on the IPulsarClientBuilder taking an Action\<ExceptionContext\> for easy sync exception handling


## [0.9.6] - 2020-10-15

### Fixed

- Fixed missing metadata properties in batched messages containing only one message
- Fixed potential torn reads in EventCounters
