LINKDUMP
*******

https://bugzilla.xamarin.com/show_bug.cgi?id=35727
https://gist.githubusercontent.com/ludovic-henry/2ab3dc14c1640ea47ba5/raw/01533181f3f03aa264a39b67757058b5a1766ca4/mono.diff
https://github.com/mono/mono/pull/2593
https://github.com/ludovic-henry/mono/commit/12a4f74287b471a9a4e823ba043fbbdb8630570e

Coop GC
=======

WatchOS requires a cooperative GC, which means our runtime
needs to use the proper API for the cooperative GC to work
properly.

http://www.mono-project.com/docs/advanced/runtime/docs/coop-suspend/#gc-unsafe-mode
https://docs.google.com/presentation/d/1Sa361Ru8ccRRZCm4BN2NUXPf2k5VYYrjqpGKsV1cPnI/edit#slide=id.p


Rules
=====

1. When touching (reading or writing) managed memory, we must
   be in GC UNSAFE mode.

2. When calling code we don't have control over (that can take
   an unbound amount of time), we must switch to GC SAFE mode.
   This includes calling any ObjC selector on any object.

Review
======

* bindings-generated.m [DONE]
* bindings.m [DONE]
* extension-main.m [DONE]
* launcher.m [MAC ONLY; NOT DONE]
* mono-runtime.m [PASS-THROUGH ONLY]
* monotouch-debug.m [DONE]
* monotouch-main.m [COMPLETE-ISH]
* nsstring-localization.m [DONE]
* runtime.m [FIRST PASS]
* shared.m [DONE]
* trampolines-i386.m [DONE]
* trampolines-invoke.m [FIRST PASS DONE; SEE PENDING NOTE BELOW]
* trampolines-x86_64.m [DONE]
* trampolines.m [FIRST PASS DONE]
* xamarin-support.m [DONE]
* zlib-helper.c [NO MANAGED CODE AT ALL]

StaticRegistrar.cs [FIRST PASS DONE; SEE PENDING NOTE BELOW]

Pending
=======

Exceptions
----------

Exceptions must be caught when returning from managed code.

* Provide a **exc to mono_runtime_invoke. Then we can either:

  1. Convert to NSException, and throw that.
  2. Abort.

* Somehow handle exceptions when calling into our delegates
  (look at mono_method_get_unmanaged_thunk)

* Review all usages of mono_raise_exception. We have the same
  choice as before: either convert to NSException, or abort.

* See also mono_method_get_unmanaged_thunk for when invoking delegates.

* Add support for catching ObjC exceptions just before
  returning to managed code. Installing an unhandled
  ObjC exception hook is not possible, because we can't
  throw managed exceptions in native code (we can set
  a flag that a managed exception should be thrown
  when returning from native to managed code (on the 
  n2m boundary), and to do that we need to handle ObjC
  exceptions in that n2m boundary).

xamarin_invoke_trampoline
-------------------------

Looks like we need a handle API to properly construct the
parameters to the managed function while at the same time
being able to switch to safe mode when we call code we
don't control (we have an array of data that can be pointers
to managed objects, and need a way to register those pointers
with managed code).

Static registrar
----------------

This has the same problem as xamarin_invoke_trampoline,
having to construct parameters to a managed function
while at the same time being able to switch to safe mode.

ObjectWrapper
-------------

This is not safe, and must be replaced with safe alternatives:

* icall
* GCHandle

mono_jit_thread_attach
----------------------

If a thread is not attached, it will change the state from
STARTING to RUNNING. If the thread is attached, it will do nothing.
The caller needs to know when this happens, to be able to react
accordingly.

Debugging tips
==============

* Tell lldb to attach when the watch extension launches:

    process attach --waitfor --name "com.xamarin.monotouch-test.watchkitapp.watchkitextension"

* Something went wrong in a thread, and now that thread is doing something very different.

    1. Put a breakpoint on pthread_setname_np, and check the assigned name: `p (char *) *(void **) ($esp + 4)`
    2. Once you've located the thread you're debugging, set a thread-specific breakpoint:

        break set -n mono_threads_reset_blocking_start -t 0x42b005 
        break set -n mono_threads_reset_blocking_end   -t 0x42b005 

* Display the current thread state:

    display (void *) mono_thread_info_current ()->thread_state
