<p align="center">
  <a href="https://sentry.io" target="_blank" align="center">
    <img src="https://sentry-brand.storage.googleapis.com/sentry-logo-black.png" width="280">
  </a>
  <br />
</p>

## Looking for `ASP.NET Core` integration?

> We've released a preview of our integration with ASP.NET Core and `Microsoft.Extensions.Logging`.
> If you are interested in checking it out, please go to this [GitHub repository](https://github.com/getsentry/sentry-dotnet) for details.
>
> These integrations are based on a new SDK, built as part of the `unified API` initiative from [Sentry](sentry.io). If you'd like to try the new API, we'd love to get some feedback.

Official [Sentry](https://sentry.io/for/csharp/) SDK for .NET.
===========

|                      |             Stable             |      Pre-release     |
| -------------------: | :----------------------------: | :------------------: |
|           **GitHub** |    [![GitHub release][1]][2]   |           -          |
|       **SharpRaven** |       [![NuGet][3]][4]         |   [![NuGet][5]][4]   |
| **SharpRaven.Nancy** |       [![NuGet][6]][7]         |   [![NuGet][8]][7]   |
|     **Travis Build** |      [![Master][12]][14]       | [![Develop][13]][14] |
|   **AppVeyor Build** |      [![Master][9]][10]        | [![Develop][15]][10] |

## Usage
Instantiate the client with your 'Data Source Name' (DSN):

```csharp
var ravenClient = new RavenClient("https://public@sentry.io/project-id");
```

### Capturing Exceptions
Call out to the client in your catch block:

```csharp
try
{
    int i2 = 0;
    int i = 10 / i2;
}
catch (Exception exception)
{
    ravenClient.Capture(new SentryEvent(exception));
}
```

### Logging Non-Exceptions
You can capture a message without being bound by an exception:

```csharp
ravenClient.Capture(new SentryEvent("Hello World!"));
```

### Additional Data
You can add additional data to the [`Exception.Data`][ex] property on
exceptions thrown about in your solution:

```csharp
try
{
    // ...    
}
catch (Exception exception)
{
    exception.Data.Add("SomeKey", "SomeValue");
    throw;
}
```

The data `SomeKey` and `SomeValue` will be captured and presented in the `extra`
property on Sentry.

Additionally, the `SentryEvent` class allow you to provide extra data to be
sent with your request, such as `ErrorLevel`, `Fingerprint`, a custom `Message`
and `Tags`.

### Async Support
In the .NET 4.5 or later build of SharpRaven, there's an `async` version of the `Capture`
method as well:

```csharp
async Task<string> CaptureAsync(SentryEvent @event);
```

### Nancy Support
You can install the
[SharpRaven.Nancy][nuget-nancy] package to capture the HTTP context in
[Nancy][nancy] applications. It will auto-register on the `IPipelines.OnError`
event, so all unhandled exceptions will be sent to Sentry.

The only thing you have to do is provide a DSN, either by registering an
instance of the `Dsn` class in your container:

```csharp
protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
{
    container.Register(new Dsn("https://public@sentry.io/project-id"));
}
```

or through configuration:

```xml
<configuration>
  <configSections>
    <section name="sharpRaven" type="SharpRaven.Nancy.NancyConfiguration, SharpRaven.Nancy" />
  </configSections>
  <sharpRaven>
    <dsn value="https://public@sentry.io/project-id" />
  </sharpRaven>
</configuration>
```

The DSN will be picked up by the auto-registered `IRavenClient` instance, so if
you want to send events to Sentry, all you have to do is add a requirement on
`IRavenClient` in your classes:

```csharp
public class LoggingModule : NancyModule
{
    private readonly IRavenClient ravenClient;

    public LoggingModule(IRavenClient ravenClient)
    {
        this.ravenClient = ravenClient;
    }
}
````

### Debugging SharpRaven
If an exception is raised internally to `RavenClient` it is logged to the
`Console`. To extend this behaviour use the property `ErrorOnCapture`:

```csharp
ravenClient.ErrorOnCapture = exception =>
{
    // Custom code here
};
````

You can also hook into the `BeforeSend` function to inspect or manipulate the
data being sent to Sentry before it is sent:

```csharp
ravenClient.BeforeSend = requester =>
{
    // Here you can log data from the requester
    // or replace it entirely if you want.
    return requester;
};
```


## Get it!
You can clone and build SharpRaven yourself, but for those of us who are happy
with prebuilt binaries, there's NuGet packages of both
[SharpRaven][nuget] and
[SharpRaven.Nancy][nuget-nancy].

## Resources
* [![Join the chat at https://gitter.im/getsentry/raven-csharp][gitter-badge]][gitter-link]
* [Code][github]
* [Bug Tracker](https://github.com/getsentry/raven-csharp/issues)
* [Forum][forum]
* [Mailing List][mail]
* [IRC][irc] (`#sentry` on `irc.freenode.net`)
* Follow [@getsentry](https://twitter.com/getsentry) on Twitter for updates

 [1]: https://img.shields.io/github/release/getsentry/raven-csharp.svg
 [2]: https://github.com/getsentry/raven-csharp/releases/latest
 [3]: https://img.shields.io/nuget/v/SharpRaven.svg
 [4]: https://www.nuget.org/packages/SharpRaven
 [5]: https://img.shields.io/nuget/vpre/SharpRaven.svg
 [6]: https://img.shields.io/nuget/v/SharpRaven.Nancy.svg
 [7]: https://www.nuget.org/packages/SharpRaven.Nancy
 [8]: https://img.shields.io/nuget/vpre/SharpRaven.Nancy.svg
 [9]: https://img.shields.io/appveyor/ci/sentry/raven-csharp/master.svg
[10]: https://ci.appveyor.com/project/sentry/raven-csharp
[12]: https://travis-ci.org/getsentry/raven-csharp.svg?branch=master
[13]: https://travis-ci.org/getsentry/raven-csharp.svg?branch=develop
[14]: https://travis-ci.org/getsentry/raven-csharp
[15]: https://img.shields.io/appveyor/ci/sentry/raven-csharp/develop.svg
[ex]: https://msdn.microsoft.com/en-us/library/system.exception.data.aspx
[gitter-badge]: https://badges.gitter.im/Join%20Chat.svg
[gitter-link]: https://gitter.im/getsentry/raven-csharp?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge
[github]: https://github.com/getsentry/raven-csharp/tree/develop/src
[mail]: https://groups.google.com/group/getsentry
[forum]: https://forum.sentry.io/c/sdks
[irc]: irc://irc.freenode.net/sentry
[nuget]: https://www.nuget.org/packages/SharpRaven
[nuget-nancy]: https://www.nuget.org/packages/SharpRaven.Nancy
[nancy]: http://nancyfx.org/