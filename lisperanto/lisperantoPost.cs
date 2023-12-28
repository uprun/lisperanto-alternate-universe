using System.Linq;
using System.Net;
using System.Web;
namespace lisperanto;
static class lisperantoPost
{

    private static string RemoveLeadingSlash(string input)
    {
        if (input.StartsWith("/"))
              return input.Substring(1);
        return input;
    }

    public static async Task Process(string root_path, System.Net.HttpListenerContext context)
    {
        //https://learn.microsoft.com/en-us/dotnet/api/system.net.httplistenerrequest?view=net-7.0
        var request = context.Request;
        string file_path = request.Url.AbsolutePath.Substring(1);

        // Decode the encoded string.

        string url_decoded = System.Web.HttpUtility.UrlDecode(file_path);

        url_decoded = RemoveLeadingSlash(url_decoded);
        url_decoded = RemoveLeadingSlash(url_decoded);



        Console.WriteLine($"Decoded: {url_decoded}");
        file_path = url_decoded;

        var requested_path = Path.Combine(root_path, file_path);
        var for_humans_backup_file_path = Path.Combine(root_path, file_path);
        var time_stampt = DateTime.Now.ToString("yyyy-MM-dd--HH-mm-ss") + ".txt";
        Directory.CreateDirectory(Path.GetDirectoryName(for_humans_backup_file_path));
        
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
                string text_content = reader.ReadToEnd();
                // test 2023-11-19--09:52
                //Console.WriteLine(s);
                await File.WriteAllTextAsync(for_humans_backup_file_path, text_content);
                Console.WriteLine("End of data:");
                reader.Close();
            }
            
            body.Close();
        }

        context.Response.StatusCode = (int) HttpStatusCode.OK;
        context.Response.Close();
    }

}