<%@ Page Language="C#" %>
<%@ Import Namespace="SharpRaven" %>
<!DOCTYPE html>
<script runat="server">

        private const string dsnUrl = "https://7d6466e66155431495bdb4036ba9a04b:4c1cfeab7ebd4c1cb9e18008173a3630@app.getsentry.com/3739";


    private static void DivideByZero(int stackFrames = 10)
    {
        if (stackFrames == 0)
        {
            var a = 0;
            var b = 1 / a;
        }
        else
            DivideByZero(--stackFrames);
    }


    private void Page_Load(object sender, EventArgs e)
    {
        Title = "Capture exception";

        if (Request.HttpMethod != "POST")
            return;

        Title = "Exception captured!";

        var client = new RavenClient(dsnUrl);

        try
        {
            DivideByZero();
        }
        catch (Exception exception)
        {
            client.CaptureException(exception);
        }
    }


</script>
<html>
    <head id="Head1" runat="server">
        <title><%= Title %></title>
    </head>
    <body>
        <h1><%= Title %></h1>
        <form method="post">
            <input type="hidden" name="Hidden" value="I'm hidden!">
            <input type="submit" name="Button" value="Capture!">
        </form>
    </body>
</html>