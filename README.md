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
The capture methods allow you to provide additional data to be sent with your request. CaptureException supports both the
`tags` and `extra` properties, and CaptureMessage additionally supports the `level` property.

The full argument specs are:

```csharp
CaptureException(Exception e, IDictionary<string, string> tags = null, object extra = null)
CaptureMessage(string message, ErrorLevel level = ErrorLevel.info, Dictionary<string, string> tags = null, object extra = null)
```

Get it!
-------
You can clone and build SharpRaven yourself, but for those of us who are happy with prebuilt binaries, there's [a NuGet package](https://www.nuget.org/packages/SharpRaven).

Resources
---------
* [Build Status](http://teamcity.codebetter.com/project.html?projectId=project344&tab=projectOverview) (requires registration)
* [Code](http://github.com/getsentry/raven-csharp)
* [Mailing List](https://groups.google.com/group/getsentry)
* [IRC](irc://irc.freenode.net/sentry) (irc.freenode.net, #sentry)
