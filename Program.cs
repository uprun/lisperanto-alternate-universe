// start 2023-06-10 17:02 CET, Munich
// new version 2023-07-08 Munich
using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Net;


class Lisperanto
{
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
            var request = context.Request;

            // do something with the request
            Console.WriteLine($"{request.Url}");
            Console.WriteLine(request.Url.AbsolutePath);
            var requested_path = Path.Combine(root_path, "www-root", request.Url.AbsolutePath.Substring(1));
            var file_content = await File.ReadAllBytesAsync(requested_path);
            context.Response.ContentType = "text/html";
            context.Response.OutputStream.Write(file_content, 0, file_content.Length);
            
            context.Response.StatusCode = (int) HttpStatusCode.OK;
            context.Response.Close();
        

        }
        
        // WebHost.CreateDefaultBuilder(args)
        //     .Configure(config => 
        //     {
        //         config.UseStaticFiles();
        //     }
                
        //         )
        //     .UseWebRoot("www-root")
        //     .Build().Run();

    }
}