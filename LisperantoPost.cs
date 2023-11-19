using System.Linq;
using System.Net;
namespace Lisperanto;
static class LisperantoPost
{
    public static async Task Process(string root_path, System.Net.HttpListenerContext context)
    {
        //https://learn.microsoft.com/en-us/dotnet/api/system.net.httplistenerrequest?view=net-7.0
        var request = context.Request;
        var requested_path = Path.Combine(root_path, request.Url.AbsolutePath.Substring(1));
        if (Directory.Exists(requested_path))
        {
            
        }
        if (File.Exists(requested_path) == false)
        {
            context.Response.StatusCode = (int) HttpStatusCode.NotFound;
            context.Response.Close();
            return;
        }
        Console.WriteLine("Client data content length {0}", request.ContentLength64);

        if (request.HasEntityBody)
        {
            var body = request.InputStream;
            var encoding = request.ContentEncoding;
            using(var reader = new StreamReader(body, encoding))
            {
                if (request.ContentType != null)
                {
                    Console.WriteLine("Client data content type {0}", request.ContentType);
                }
                Console.WriteLine("Client data content length {0}", request.ContentLength64);

                Console.WriteLine("Start of data:");
                string s = reader.ReadToEnd();
                Console.WriteLine(s);
                await File.WriteAllTextAsync(requested_path, s);
                Console.WriteLine("End of data:");
                reader.Close();
            }
            
            body.Close();
            
        }
        
        
        context.Response.StatusCode = (int) HttpStatusCode.OK;
        context.Response.Close();
    }

}