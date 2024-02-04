using System.Linq;
using System.Net;
namespace lisperanto;
static class lisperantoGet
{
    private static string RemoveLeadingSlash(string input)
    {
        if (input.StartsWith("/"))
              return input.Substring(1);
        return input;
    }

    public static async Task Process(string root_path, System.Net.HttpListenerContext context)
    {
        var request = context.Request;
        string file_path = request.Url.AbsolutePath;
        string url_decoded = System.Web.HttpUtility.UrlDecode(file_path);

Console.WriteLine($"url_decoded : {url_decoded}");

        url_decoded = RemoveLeadingSlash(url_decoded);
        url_decoded = RemoveLeadingSlash(url_decoded);
        var requested_path = Path.Combine(root_path, url_decoded);

Console.WriteLine($"requested path : {requested_path}");


        if (Directory.Exists(requested_path))
        {
            await lisperantoGetFolder.ProcessAsync(root_path, context);
            return;
        }
Console.WriteLine("Not Directory");
        if (File.Exists(requested_path) == false)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            context.Response.Close();
            return;
        }

        byte[] file_content = await File.ReadAllBytesAsync(requested_path);
        var extension = Path.GetExtension(request.Url.AbsolutePath);

        Dictionary<string, string> lookup_for_extension = new Dictionary<string, string>();
        lookup_for_extension.Add(".html", "text/html");
        lookup_for_extension.Add(".js", "application/javascript");
        lookup_for_extension.Add(".txt", "text/plain");

        if (lookup_for_extension.ContainsKey(extension))
        {
            context.Response.ContentType = lookup_for_extension[extension];
        }
        else
        {
            context.Response.ContentType = "text/plain";
        }

        await context.Response.OutputStream.WriteAsync(file_content, 0, file_content.Length);

        context.Response.StatusCode = (int)HttpStatusCode.OK;
        context.Response.Close();
    }
}