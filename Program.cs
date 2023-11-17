﻿// start 2023-06-10 17:02 CET, Munich
// new version 2023-07-08 Munich
using System.Reflection;
using System.Text.Json;
using System.Linq;
using System.Net;


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
            var request = context.Request;

            // do something with the request
            Console.WriteLine($"{request.Url}");
            Console.WriteLine(request.Url.AbsolutePath);
            var requested_path = Path.Combine(root_path, request.Url.AbsolutePath.Substring(1));
            try
            {
                context.Response.AddHeader("Server", "web-server-created-by-Oleksandr-Kryvonos/v-2023-11-17");
                context.Response.AddHeader("Date", DateTime.Now.ToString("yyyy-MM-dd--HH:mm:ss"));
                if (Directory.Exists(requested_path))
                {
                    DirectoryInfo dir = new DirectoryInfo(requested_path);
                    var result = dir.GetFileSystemInfos()
                        .Select(info => info.Name)
                        .Union(new []{".."})
                        .OrderBy(name => name)
                        .ToArray();
                    using(var memory_stream = new MemoryStream())
                    {
                        
                        using(var stream_writer = new StreamWriter(memory_stream))
                        {
                            await stream_writer.WriteLineAsync("<style> a { color: yellow; } </style>");
                            await stream_writer.WriteLineAsync($"<body style='background-color: black; color: yellow;'>");
                            for(int i = 0 ; i < result.Length; ++i)
                            {
                                var file_name = Path.GetFileName(result[i]);
                                var path_to_respond = Path.Join(request.Url.AbsolutePath, file_name);
                                await stream_writer.WriteLineAsync($"<div><a href='{path_to_respond}'>{file_name}</a></div>");
                            }
                            await stream_writer.WriteLineAsync($"</body>");
                        }
                        await context.Response.OutputStream.WriteAsync(memory_stream.ToArray());
                        context.Response.ContentType = "text/plain";
                        context.Response.StatusCode = (int) HttpStatusCode.OK;
                        context.Response.Close();
                    }
                    

                    continue;
                }
                if (File.Exists(requested_path) == false)
                {
                    context.Response.StatusCode = (int) HttpStatusCode.NotFound;
                    context.Response.Close();
                    continue;
                }
                var file_content = await File.ReadAllBytesAsync(requested_path);
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
                
                context.Response.StatusCode = (int) HttpStatusCode.OK;
                context.Response.Close();

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