using System.Linq;
using System.Net;
using System.Web;
namespace lisperanto;

static class lisperantoGetFolder
{
    private static string RemoveLeadingSlash(string input)
    {
        if (input.StartsWith("/"))
              return input.Substring(1);
        return input;
    }
    public static async Task ProcessAsync(string root_path, System.Net.HttpListenerContext context)
    {
        var request = context.Request;
        string file_path = request.Url.AbsolutePath;
        string url_decoded = System.Web.HttpUtility.UrlDecode(file_path);
        url_decoded = RemoveLeadingSlash(url_decoded);
        url_decoded = RemoveLeadingSlash(url_decoded);
        var requested_path = Path.Combine(root_path, url_decoded);

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
                await stream_writer.WriteLineAsync("<meta charset=\"utf-8\">");

                await stream_writer.WriteLineAsync($"<body style='background-color: black; color: yellow;'>");
                await stream_writer.WriteLineAsync($"<div><a href='/{Path.Join(url_decoded, "..")}'>..</a></div>");
                for (int i = 0; i < result.Length; ++i)
                {
                    FileSystemInfo fileSystemInfo = result[i];
                    var file_name = Path.GetFileName(fileSystemInfo.Name);

                    Console.WriteLine($"Processing {file_name}");

                    var path_to_respond = "/" + Path.Join(url_decoded, file_name);

Console.WriteLine($"path_to_respond {path_to_respond}");

                    string myEncodedString = HttpUtility.HtmlEncode(file_name);
                    string url_encoded = System.Web.HttpUtility.UrlEncode(path_to_respond);
                    await stream_writer.WriteLineAsync($"<div>");
                    if (Directory.Exists(fileSystemInfo.FullName))
                    {
                        await stream_writer.WriteLineAsync($"<a href='{url_encoded}'>{myEncodedString}</a>");
                    }
                    else
                    {
                        await stream_writer.WriteLineAsync($"<a href='/universe/text-editor-2024-02-04-tabs/text-editor.html?file-path={url_encoded}'>{myEncodedString}</a>");
                        if (file_name.EndsWith(".html"))
                        {
                            await stream_writer.WriteLineAsync($"<a href='{url_encoded}'>[Open app]</a>");
                        }
                    }
                    await stream_writer.WriteLineAsync($"</div>");
                }
                await stream_writer.WriteLineAsync($"</body>");
            }
            await context.Response.OutputStream.WriteAsync(memory_stream.ToArray());
            context.Response.ContentType = "text/html";
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.Close();
        }
    }
}
