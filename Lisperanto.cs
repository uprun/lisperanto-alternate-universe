// start 2023-06-10 17:02 CET, Munich
// new version 2023-07-08 Munich
using System.Reflection;
using System.Text.Json;
using System.Linq;
using System.Net;

namespace Lisperanto;

class Lisperanto
{
    private static void streamReadExample()
    {
        // if (request.HasEntityBody)
        //     {
        //         var body = request.InputStream;
        //         var encoding = request.ContentEncoding;
        //         var reader = new StreamReader(body, encoding);
        //         if (request.ContentType != null)
        //         {
        //             Console.WriteLine("Client data content type {0}", request.ContentType);
        //         }
        //         Console.WriteLine("Client data content length {0}", request.ContentLength64);

        //         Console.WriteLine("Start of data:");
        //         string s = reader.ReadToEnd();
        //         Console.WriteLine(s);
        //         Console.WriteLine("End of data:");
        //         reader.Close();
        //         body.Close();
                
        //     }
    }
    static async Task Main(string[] args)
    {
        var _listener = new HttpListener();
        var host = "http://localhost:" + "8080".ToString() + "/";
        _listener.Prefixes.Add(host);
        _listener.Start();
        var root_path = new DirectoryInfo(Directory.GetCurrentDirectory()).FullName;
        Console.WriteLine(root_path);
        Console.WriteLine("Waiting for context");
        while(true)
        {
            var context = await _listener.GetContextAsync();
            Console.WriteLine("Received context");
            

            // do something with the request
            Console.WriteLine($"{context.Request.Url}");
            
            try
            {
                await LisperantoGet.Process(root_path, context);

            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                context.Response.Close();
                continue;
            }
            
        

        }

    }


}