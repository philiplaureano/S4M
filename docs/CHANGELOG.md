
# Change Log
All notable changes to this project will be documented in this file.
 
The format is based on [Keep a Changelog](http:--keepachangelog.com-)
and this project adheres to [Semantic Versioning](http:--semver.org-).
 
## [Unreleased] - 2021-03-22

## Added
- The ability to combine state machines with other state machines to create higher-order state machines

## [1.0.4] - 2021/03/22

### Changed
- The PipeTo&lt;T&gt; extension method now supports both blocking and non-blocking tasks. This means that you can kick off an operation without having to wait for its completion.

## [1.0.3] - 2021/03/22

### Added
- The PipeTo&lt;T&gt; extension method now allows for redirecting async Task&lt;T&gt; outputs. **Please note:** The extension method does block the thread until the task completes.
### Changed
- The state machines now pause message handling if they're in the middle of changing their message handlers.


## [1.0.2] - 2021-03-22

### Added
- The ICanTellAsync interface, which identifies types that can be told messages, and those types do not have to be state machines
- Added the [S4M.Timers](https://www.nuget.org/packages/Laureano.S4M.Timers/) library, which emulate Akka.NET's ability to defer messages to be sent to a state machine at a later date

### Changed
- The IStateMachineInterface now inherits from ICanTellAsync. This means that you do not need to implement a state machine to use the ICanTellAsync interface