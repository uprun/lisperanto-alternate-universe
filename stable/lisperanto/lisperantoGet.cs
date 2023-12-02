using System.Linq;
using System.Net;
namespace lisperanto;
static class lisperantoGet
{
    public static async Task Process(string root_path, System.Net.HttpListenerContext context)
    {
        var request = context.Request;
        string file_path = request.Url.AbsolutePath.Substring(1);
        var requested_path = Path.Combine(root_path, file_path);
        var branch = request.QueryString["branch"] ?? "draft";

        Console.WriteLine($"branch: {branch}");


        if (Directory.Exists(requested_path))
        {
            lisperantoGetFolder.Process(root_path, context);
            return;
        }
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