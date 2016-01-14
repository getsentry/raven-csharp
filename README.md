Usage
=====
Instantiate the client with your DSN:

```csharp
var ravenClient = new RavenClient("http://public:secret@example.com/project-id");
```

Capturing Exceptions
--------------------
Call out to the client in your catch block:

```csharp
try
{
    int i2 = 0;
    int i = 10 / i2;
}
catch (Exception e)
{
    ravenClient.CaptureException(e);
}
```

Logging Non-Exceptions
----------------------
You can capture a message without being bound by an exception:

```csharp
ravenClient.CaptureMessage("Hello World!");
```

Additional Data
---------------
The capture methods allow you to provide additional data to be sent with your request. `CaptureException` supports both the
`tags` and `extra` properties, and `CaptureMessage` additionally supports the `level` property.

The full argument specs are:

```csharp
string CaptureException(Exception exception,
                        SentryMessage message = null,
                        ErrorLevel level = ErrorLevel.Error,
                        IDictionary<string, string> tags = null,
                        string[] fingerprint = null,
                        object extra = null)

string CaptureMessage(SentryMessage message,
                      ErrorLevel level = ErrorLevel.Info,
                      IDictionary<string, string> tags = null,
                      string[] fingerprint = null,
                      object extra = null)

```

Async Support
-------------
In the .NET 4.5 build of SharpRaven, there are `async` versions of the above methods as well:

```csharp
Task<string> CaptureExceptionAsync(Exception exception,
                                   SentryMessage message = null,
                                   ErrorLevel level = ErrorLevel.Error,
                                   IDictionary<string, string> tags = null,
                                   string[] fingerprint = null,
                                   object extra = null);

Task<string> CaptureMessageAsync(SentryMessage message,
                                 ErrorLevel level = ErrorLevel.Info,
                                 IDictionary<string, string> tags = null,
                                 string[] fingerprint = null,
                                 object extra = null);
```

Nancy Support
-------------
You can install the [SharpRaven.Nancy](https://www.nuget.org/packages/SharpRaven.Nancy) package to capture the HTTP context
in [Nancy](http://nancyfx.org/) applications. It will auto-register on the `IPipelines.OnError` event, so all unhandled
exceptions will be sent to Sentry.

The only thing you have to do is provide a DSN, either by registering an instance of the `Dsn` class in your container:

```csharp
protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
{
    container.Register(new Dsn("http://public:secret@example.com/project-id"));
}
```

or through configuration:

```xml
<configuration>
  <configSections>
    <section name="sharpRaven" type="SharpRaven.Nancy.NancyConfiguration, SharpRaven.Nancy" />
  </configSections>
  <sharpRaven>
    <dsn value="http://public:secret@example.com/project-id" />
  </sharpRaven>
</configuration>
```

The DSN will be picked up by the auto-registered `IRavenClient` instance, so if you want to send events to
Sentry, all you have to do is add a requirement on `IRavenClient` in your classes:

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

Get it!
-------
You can clone and build SharpRaven yourself, but for those of us who are happy with prebuilt binaries, there's [a NuGet package](https://www.nuget.org/packages/SharpRaven).

Resources
---------
* [![Build Status](http://teamcity.codebetter.com/app/rest/builds/buildType:(id:bt1000)/statusIcon)](http://teamcity.codebetter.com/viewType.html?buildTypeId=bt1000&guest=1)
* [![Join the chat at https://gitter.im/getsentry/raven-csharp](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/getsentry/raven-csharp?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)
* [Code](http://github.com/getsentry/raven-csharp)
* [Mailing List](https://groups.google.com/group/getsentry)
* [IRC](irc://irc.freenode.net/sentry) (irc.freenode.net, #sentry)