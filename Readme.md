The project shows thread-safety issues with `WebApplicationFactory<T>`.

According to the [documentation](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.testing.webapplicationfactory-1.-ctor?view=aspnetcore-2.1#Microsoft_AspNetCore_Mvc_Testing_WebApplicationFactory_1__ctor), the `WebApplicationFactory<T>` is used to create the instance of the `TestServer` and one or more instances of `HttpClient` that can be used in the test.

This repository presents that while `WebApplicationFactory<T>` creates **one** instance of the `TestServer` when executed from single thread, it is not thread-safe.

To prove it, a sample WebApi project has been created (with `dotnet new webapi`) and it's `Startup` class has been extended with a [NumberOfCalls](https://github.com/Suremaker/WebApplicationFactoryTests/blob/master/SampleApi/Startup.cs#L12) counter capturing number of calls to [Configure()](https://github.com/Suremaker/WebApplicationFactoryTests/blob/master/SampleApi/Startup.cs#L31) method as well as small delay emulating more complex startup operation.

A three tests has been made to verify the `WebApplicationFactory<T>` behaviour.
* [WebApplicationFactory_should_initialize_Api_once](https://github.com/Suremaker/WebApplicationFactoryTests/blob/master/SampleTests/WebApplicationFactoryTests.cs#L21) test verifies that if the factory is used from single thread, the underlying `TestServer` is initialized once. This test pass.
* [WebApplicationFactory_Api_initialization_should_be_thread_safe](https://github.com/Suremaker/WebApplicationFactoryTests/blob/master/SampleTests/WebApplicationFactoryTests.cs#L31) test verifies if factory initializes only one server instance if it's accessed from multiple threads. This tests fails on my machine, showing that `TestServer` was initialized **8** times (I have 8 cores).
* [Server_should_be_available](https://github.com/Suremaker/WebApplicationFactoryTests/blob/master/SampleTests/WebApplicationFactoryTests.cs#L41) test is rather an open question. The `WebApplicationFactory<T>` offers `Server` property to access underlying `TestServer`. When this property is accessed just after instantiation, it returns `null`. As there is no explicit method on `WebApplicationFactory<T>` to ensure/enforce test server initialization, it looks like the `Server` property should always return the server instance.
