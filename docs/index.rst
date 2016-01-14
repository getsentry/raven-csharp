.. sentry:edition:: self

    Raven-CSharp
    ============

.. sentry:edition:: hosted, on-premise

    .. class:: platform-csharp

    C#
    ==

.. sentry:support-warning::

    The C# SDK is maintained and supported by the Sentry
    community.  Learn more about the project on `GitHub
    <https://github.com/getsentry/raven-csharp>`__.

Raven is the C# client for Sentry. Raven relies on the most popular
logging libraries to capture and convert logs before sending details to a
Sentry instance.

Installation
------------

A `NuGet Package <https://www.nuget.org/packages/SharpRaven>`_ is
available for SharpRaven if you don't want to compile it yourself.

Instantiate the client with your DSN:

.. sourcecode:: csharp

    var ravenClient = new RavenClient("___DSN___");

Capturing Exceptions
--------------------

Call out to the client in your catch block:

.. sourcecode:: csharp

    try
    {
        int i2 = 0;
        int i = 10 / i2;
    }
    catch (Exception e)
    {
        ravenClient.CaptureException(e);
    }

Logging Non-Exceptions
----------------------

You can capture a message without being bound by an exception:

.. sourcecode:: csharp

    ravenClient.CaptureMessage("Hello World!");

Additional Data
---------------

The capture methods allow you to provide additional data to be sent with
your request. ``CaptureException`` supports both the ``tags`` and extra
``properties``, and ``CaptureMessage`` additionally supports the
``level`` property.

The full argument specs are:

.. sourcecode:: csharp

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


Async Support
-------------
In the .NET 4.5 build of SharpRaven, there are ``async`` versions of the
above methods as well:

.. sourcecode:: csharp

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

Nancy Support
-------------
You can install the `SharpRaven.Nancy <https://www.nuget.org/packages/SharpRaven.Nancy>`_
package to capture the HTTP context in `Nancy <http://nancyfx.org/>`_ applications. It
will auto-register on the ``IPipelines.OnError`` event, so all unhandled exceptions will be
sent to Sentry.

The only thing you have to do is provide a DSN, either by registering an instance of the
``Dsn`` class in your container:

.. sourcecode:: csharp

    protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
    {
        container.Register(new Dsn("http://public:secret@example.com/project-id"));
    }

or through configuration:

.. sourcecode:: xml

    <configuration>
      <configSections>
        <section name="sharpRaven" type="SharpRaven.Nancy.NancyConfiguration, SharpRaven.Nancy" />
      </configSections>
      <sharpRaven>
        <dsn value="http://public:secret@example.com/project-id" />
      </sharpRaven>
    </configuration>

The DSN will be picked up by the auto-registered ``IRavenClient`` instance, so if you want to send events to
Sentry, all you have to do is add a requirement on ``IRavenClient`` in your classes:

.. sourcecode:: csharp

    public class LoggingModule : NancyModule
    {
        private readonly IRavenClient ravenClient;

        public LoggingModule(IRavenClient ravenClient)
        {
            this.ravenClient = ravenClient;
        }
    }


Resources
---------

* `Bug Tracker <http://github.com/getsentry/raven-csharp/issues>`_
* `Github Project <http://github.com/getsentry/raven-csharp>`_
