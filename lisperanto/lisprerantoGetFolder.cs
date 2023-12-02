using System.Linq;
using System.Net;
namespace lisperanto;

static class lisperantoGetFolder
{
    public static async Task Process(string root_path, System.Net.HttpListenerContext context)
    {
        var request = context.Request;
        string file_path = request.Url.AbsolutePath.Substring(1);
        var requested_path = Path.Combine(root_path, file_path);

        DirectoryInfo dir = new DirectoryInfo(requested_path);
            var result = dir.GetFileSystemInfos()
                .OrderBy(info => info.Name)
                .ToArray();
        using (var memory_stream = new MemoryStream())
        {
            using (var stream_writer = new StreamWriter(memory_stream))
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
                for (int i = 0; i < result.Length; ++i)
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
                        await stream_writer.WriteLineAsync($"<a href='/universe/js-repl.html?version=stable&file-path={path_to_respond}'>{file_name}</a>");
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
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.Close();
        }
    }
}