# S4M.Core - A Short, Simple, and Straightforward State (S4) Machine Library for .NET

## Overview

S4M is a state machine library that I built to simplify building more resilient components in a distributed system. It was inspired by [this article](https://petabridge.com/blog/akka-actors-finite-state-machines-switchable-behavior/) from [Akka.NET](https://getakka.net/) that uses Actors with switchable behaviours to create components that can effectively manage themselves.

### Why another state machine library?

- I created S4M as a way to introduce other developers to some of the broader features that [Akka.NET](https://getakka.net/) offers, such as the Become/Unbecome/Stash behaviours that give it its resiliency
- I wanted a FSM library that was lightweight, simple, and easy to maintain for almost any C# developer
- I also needed a library that can give its users the same kind of "smart" switching behaviours that Akka.NET offers because its actors can change state, such as:
	- Fault Tolerance
	- Throttling
	- Circuit breakers

### What makes S4M different from all the other .NET FSM libraries out there?

#### Features

- If you familiar with C# async/await features, inheritance, and are familiar with how to use lambda functions or delegates in C#, you can get S4M up and running in minutes
- Infinite states with an almost infinite number of handlers, all encapsulated in a single class per state machine
- State transitions are defined within a state machine derived class itself, so you don't need to declare all the possible transitions at once
- You can 'stash' or defer messages that the state machine should not handle in its current state, and 'unstash' all of the stashed messages in a different state so that a state machine can do things like resume processing after a database connection has been restored, for example.

### Why not just use [Akka.NET](https://getakka.net/) instead of reinventing your own library?

- Akka.NET is an excellent library, but it might be overkill in many cases. S4M just focuses on building the state machine itself. I personally recommend using Akka.NET for bigger jobs, but if you want to start lean or you want to migrate your existing legacy codebase to be more resilient, then having S4M state machines is a good "gateway drug" to moving to a full-blown Akka.NET use case.

## Installing the S4M NuGet Package 
### Prerequisites
- S4M:
  - [Runs on .NET Standard 2.0 compatible binaries](https://dotnet.microsoft.com/platform/dotnet-standard)
  - [Requires .NET 5 to build the source code](https://dotnet.microsoft.com/download/dotnet/5.0)
  - You can find the repository for S4M [here](https://github.com/philiplaureano/S4M)

You can [download the package here](https://www.nuget.org/packages/Laureano.S4M.Core/) from NuGet.
### The Quick Start Guide

1. Grab the [S4M NuGet package here](https://www.nuget.org/packages/Laureano.S4M.Core/)
2. Inherit your state machine from the StateMachine class, just like in this example:
```
	public class SampleCircuitBreaker : StateMachine
	{
		// ...
	}
```
3. Define the initial states as well as the message handlers that will respond to each message as your custom state machine changes state. You can define these handlers inside individual Receive&lt;T&gt; handler methods which will determine how each state responds to a particular message:
```
	private void Closed()
	{
	    Receive<object>(msg =>
	    {
	        try
	        {
	            _commandHandler(msg);
	            _handledMessages.Add(msg);
	        }
	        catch (Exception e)
	        {
	            _exceptionsThrown.Add(e);	            
	            // Something happened - trip the circuit breaker
	            CurrentState = CircuitBreakerState.Open;
	            Become(Open);
	        }
	    });
	}
	private void Open()
	{
	    Receive<object>(msg =>
	    {
	        try
	        {
	            _commandHandler(msg);
	            _handledMessages.Add(msg);	            
	            // If the call worked, let's close the circuit breaker again 
	            CurrentState = CircuitBreakerState.Closed;
	            Become(Closed);
	        }
	        catch (Exception e)
	        {
	            // Ignore the error and keep the circuit breaker open since errors are still occurring
	            _exceptionsThrown.Add(e);
	        }
	    });
	}
```
4. Set your initial state in the class constructor, using the Become method, like this:
```
    public SampleCircuitBreaker(/*...*/)
    {
    	// ...
    	Become(Closed);
	}
```
5. If you need to stash/defer the current message instead of handling it, call `Stash.Stash()` method from within a Receive handler.
6. To unstash all the messages that have been deferred from another state (such as a list of database commands that have been deferred due to a database server outage), call the `Stash.UnstashAll()` method. Here is a complete example:
```
    public class SampleUnstasher : StateMachine
    {
        private readonly ConcurrentBag<object> _messagesHandled = new();
        private readonly ConcurrentBag<object> _messagesStashed = new();    
        public SampleUnstasher()
        {
            Become(StashEverything);
        }
        public void StartHandlingMessages()
        {
            Become(NotStashing);
        }        
        private void StashEverything()
        {
            Receive<object>(msg =>
            {
                _messagesStashed.Add(msg);
                // Stash every message that comes in
                Stash.Stash();
            });
        }
        private void NotStashing()
        {
            Receive<object>(msg =>
            {
                // Unstash every message that wasn't handled
                Stash.UnstashAll();
                // Handle every message that comes in
                _messagesHandled.Add(msg);
            });
        }
        public IEnumerable<object> MessagesStashed => _messagesStashed;
        public IEnumerable<object> MessagesHandled => _messagesHandled;
    }
```
7. And lastly, if you want to use your new state machine, create a new instance of it and call the `TellAsync` method:
```
    var myFSM = new SampleUnstasher();
    var someRandomMessage = Guid.NewGuid();
    await myFSM.TellAsync(someRandomMessage);
```

And that's pretty much all you need to get started 😁

## ChangeLog
You can find the list of the latest changes [here](CHANGELOG.md).
## License
 S4M is [licensed under the MIT License](https://opensource.org/licenses/MIT). It comes with no free warranties expressed or implied, whatsoever.

## Questions, Comments, or Feedback?
- Feel free to [follow me](http://twitter.com/philiplaureano) on Twitter.
