# Change Log
All notable changes to this project will be documented in this file.
 
The format is based on [Keep a Changelog](http:--keepachangelog.com-)
and this project adheres to [Semantic Versioning](http:--semver.org-).
 
## 2021-03-29
### Unreleased
- S4M.Reactive [1.0.4] - Support for the fixed rate buffer sampling, which aggregates items into groups for batch processing downstream
### Added
- S4M.Reactive [1.0.3] - Added support for one to many message routing using Round Robin rotations via the RoundRobinGroup class
- [S4M.Reactive](https://www.nuget.org/packages/Laureano.S4M.Reactive/) [1.0.3] - A library that combines state S4M machines with other state machines to create higher-order state machines
- [S4M.Reactive](https://www.nuget.org/packages/Laureano.S4M.Reactive/) supports Fork, Join, Map, Filter, and Combine functions out of the box
### Changed
- S4M.Timers [1.0.3] - Upgraded the timers to use the more reliable System.Timers.Timer, rather than using TPL to create an ad-hoc timer
- S4M.Reactive [1.0.3] - Sorted the classes into different namespaces in order to make the code easier to browse
## 2021-03-22
### Added
- S4M.Core [1.0.3] - The PipeTo&lt;T&gt; extension method now allows for redirecting async Task&lt;T&gt; outputs. **Please note:** The extension method does block the thread until the task completes.
- S4M.Core [1.0.2] - The ICanTellAsync interface, which identifies types that can be told messages, and those types do not have to be state machines
- S4M.Core [1.0.2] - Added the [S4M.Timers](https://www.nuget.org/packages/Laureano.S4M.Timers/) library, which emulate Akka.NET's ability to defer messages to be sent to a state machine at a later date

### Changed
- S4M.Core [1.0.4] - The PipeTo&lt;T&gt; extension method now supports both blocking and non-blocking tasks. This means that you can kick off an operation without having to wait for its completion.
- S4M.Core [1.0.3] - The state machines now pause message handling if they're in the middle of changing their message handlers.
- S4M.Core [1.0.2]- The IStateMachineInterface now inherits from ICanTellAsync. This means that you do not need to implement a state machine to use the ICanTellAsync interface