Usage
=====

Instantiate the client with your DSN:

::

    ravenClient = new RavenClient('http://public:secret@example.com/project-id');


Call out to the client in your catch block:

::

    try
    {
        int i2 = 0;
        int i = 10 / i2;
    }
    catch (Exception e)
    {
        ravenClient.CaptureException(e);
    }
