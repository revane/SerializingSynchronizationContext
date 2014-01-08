SerializingSynchronizationContext
=================================

C# code for and an example using a custom SynchronizationContext that serializes threads running code run under the context's protection.

The code here was developed as part of my team's work on an application using
async/await. The application deals with user input and wireless communication
and so asynchronous events are flowing from one to the other. Thread
serializaiton was required to protect concurrent access to class state. Through
a process of iterative refinement, we came up with this custom
SerializationContext.

Rules for using the context:
* Never use volatile
* Never use Interlocked
* Never use lock
* Always use SerializingSynchronizationContext
* Every public method uses myContext.Invoke() or await myContext.
* Don’t make blocking calls from code protected by a context.
* Every event handler is "async void" and uses await _synchronizationContext.
  Because state can change meanwhile, all meaningful state is preserved in the
  EventArgs.
* Don't use ConfigureAwait(false) as it may cause the continuation to be run not under
  the protection of the context.

