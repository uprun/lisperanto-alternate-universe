﻿// start 2023-06-10 17:02 CET, Munich
// new version 2023-07-08 Munich
using System.Reflection;
using System.Text.Json;
using System.Linq;
using System.Net;

namespace Lisperanto;

class Lisperanto
{
    static async Task Main(string[] args)
    {
        Console.WriteLine(String.Join(", ", args));
        // this code was inspired by https://thoughtbot.com/blog/using-httplistener-to-build-a-http-server-in-csharp
        var _listener = new HttpListener();
        var host = "http://localhost:" + "8080".ToString() + "/";
        _listener.Prefixes.Add(host);
        _listener.Start();
        Console.WriteLine($"Server available at: {host}");
        var root_path = new DirectoryInfo(Directory.GetCurrentDirectory()).FullName;
        Console.WriteLine(root_path);
        Console.WriteLine("Waiting for context");
        while(true)
        {
            var context = await _listener.GetContextAsync();
            Console.WriteLine("Received context");
            // do something with the request
            Console.WriteLine($"{context.Request.HttpMethod} {context.Request.Url}");
            
            try
            {
                context.Response.AddHeader("Server", "web-server-created-by-Oleksandr-Kryvonos/v-2023-11-17");
                context.Response.AddHeader("Date", DateTime.Now.ToString("yyyy-MM-dd--HH:mm:ss"));
                if (context.Request.HttpMethod == "GET")
                    await LisperantoGet.Process(root_path, context);
                if (context.Request.HttpMethod == "POST")
                    await LisperantoPost.Process(root_path, context);
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