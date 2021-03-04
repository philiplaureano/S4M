# S4M.Core - A Short, Simple, and Straightforward State (S4) Machine Library for .NET

## Overview

S4M is a state machine library that I built to simplify building more resilient components in a distributed system. It was inspired by [this article](https://petabridge.com/blog/akka-actors-finite-state-machines-switchable-behavior/) from [Akka.NET](https://getakka.net/) that uses Actors with switchable behaviours to create components that can effectively manage themselves.

### Why another state machine library?

- I created S4M as a way to introduce other developers to some of the broader features that [Akka.NET](https://getakka.net/) offers, such as the Become/Unbecome/Stash behaviours that give it its resiliency
- I wanted a FSM library that was lightweight, simple, and easy to maintain, and a library that can give its users the same kind of "smart" switching behaviours that Akka.NET offers because its actors can change state, such as:
	- Fault Tolerance
	- Throttling
	- Circuit breakers

### What makes S4M different from all the other .NET FSM libraries out there?

#### Features

- Infinite states with an almost infinite number of handlers, all encapsulated in a single class per state machine
- State transitions are defined within a state machine derived class itself, so you don't need to declare all the possible transitions at once
- You can 'stash' or defer messages that the state machine should not handle in its current state, and 'unstash' all of the stashed messages in a different state so that a state machine can do things like resume processing after a database connection has been restored, for example.

### Why not just use [Akka.NET](https://getakka.net/) instead of reinventing your own library?

- Akka.NET is an excellent library, but it might be overkill in many cases. S4M just focuses on building the state machine itself. I personally recommend using Akka.NET for bigger jobs, but if you want to start lean or you want to migrate your existing legacy codebase to be more resilient, then having S4M state machines is a good "gateway drug" to Akka.NET

## Installing the MetaFoo NuGet Package 
### Prerequisites
- S4M:
  - [Runs on .NET Standard 2.0 compatible binaries](https://dotnet.microsoft.com/platform/dotnet-standard)
  - [Requires .NET 5 to build the source code](https://dotnet.microsoft.com/download/dotnet/5.0)
  - You can find the repository for S4M [here](https://github.com/philiplaureano/S4M)

You can [download the package here](https://www.nuget.org/packages/Laureano.S4M.Core/) from NuGet.

## License
 S4M is [licensed under the MIT License](https://opensource.org/licenses/MIT). It comes with no free warranties expressed or implied, whatsoever.

## Questions, Comments, or Feedback?
- Feel free to [follow me](http://twitter.com/philiplaureano) on Twitter.
