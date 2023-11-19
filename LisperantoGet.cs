using System.Linq;
using System.Net;
namespace Lisperanto;
static class LisperantoGet
{
    public static async Task Process(string root_path, System.Net.HttpListenerContext context)
    {
        var request = context.Request;
        var requested_path = Path.Combine(root_path, request.Url.AbsolutePath.Substring(1));
        context.Response.AddHeader("Server", "web-server-created-by-Oleksandr-Kryvonos/v-2023-11-17");
        context.Response.AddHeader("Date", DateTime.Now.ToString("yyyy-MM-dd--HH:mm:ss"));
        if (Directory.Exists(requested_path))
        {
            DirectoryInfo dir = new DirectoryInfo(requested_path);
            var result = dir.GetFileSystemInfos()
                .OrderBy(info => info.Name)
                .ToArray();
            using(var memory_stream = new MemoryStream())
            {
                
                using(var stream_writer = new StreamWriter(memory_stream))
                {
                    await stream_writer.WriteLineAsync("<style> html, body {");
                    await stream_writer.WriteLineAsync("height: 100%;");
                    await stream_writer.WriteLineAsync("width: 100%;");
                    await stream_writer.WriteLineAsync("font-family: 'Input Mono', monospace;");
                    await stream_writer.WriteLineAsync("margin: 0;");
                    await stream_writer.WriteLineAsync("}</style>");
                    await stream_writer.WriteLineAsync("<style> a { color: yellow; } </style>");
                    await stream_writer.WriteLineAsync($"<body style='background-color: black; color: yellow;'>");
                    await stream_writer.WriteLineAsync($"<div><a href='{Path.Join(request.Url.AbsolutePath, "..")}'>..</a></div>");
                    for(int i = 0 ; i < result.Length; ++i)
                    {
                        FileSystemInfo fileSystemInfo = result[i];
                        var file_name = Path.GetFileName(fileSystemInfo.Name);
                        var path_to_respond = Path.Join(request.Url.AbsolutePath, file_name);
                        await stream_writer.WriteLineAsync($"<div>");
                        if (Directory.Exists(fileSystemInfo.FullName))
                        {
                            await stream_writer.WriteLineAsync($"<a href='{path_to_respond}'>{file_name}</a>");
                        }
                        else
                        {
                            await stream_writer.WriteLineAsync($"<a href='/universe/js-repl.html?file-path={path_to_respond}'>{file_name}</a>");
                            if (file_name.EndsWith(".html"))
                            {
                                await stream_writer.WriteLineAsync($"<a href='{path_to_respond}'>[Open app]</a>");
                            }
                        }
                        await stream_writer.WriteLineAsync($"</div>");
                    }
                    await stream_writer.WriteLineAsync($"</body>");
                }
                await context.Response.OutputStream.WriteAsync(memory_stream.ToArray());
                context.Response.ContentType = "text/plain";
                context.Response.StatusCode = (int) HttpStatusCode.OK;
                context.Response.Close();
            }
            

            return;
        }
        if (File.Exists(requested_path) == false)
        {
            context.Response.StatusCode = (int) HttpStatusCode.NotFound;
            context.Response.Close();
            return;
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

}