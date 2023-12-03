// start 2023-06-10 17:02 CET, Munich
// new version 2023-12-02 Munich
// new version 2023-12-03 Munich, fixed url encoding and non-latin alphabet

using System.Reflection;
using System.Text.Json;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

namespace lisperanto;

class lisperanto
{

public static List<string> GetLocalIPAddress()
{
    var host = Dns.GetHostEntry(Dns.GetHostName());
    var result = host.AddressList
        .Where(ip => ip.AddressFamily == AddressFamily.InterNetwork || ip.AddressFamily == AddressFamily.InterNetworkV6)
        .Select(ip => ip.ToString())
        .ToList();
    return result;
}


    static async Task Main(string[] args)
    {
        Console.WriteLine(String.Join(", ", args));
        // this code was inspired by https://thoughtbot.com/blog/using-httplistener-to-build-a-http-server-in-csharp
        var _listener = new HttpListener();
        var this_directory_info = new DirectoryInfo(Directory.GetCurrentDirectory()); 
        DirectoryInfo root_directoryInfo = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), ".."));   

        var root_path = root_directoryInfo.FullName;


        int port_to_start_with = 9001;
        var local_hosts = GetLocalIPAddress().Select(ip => $"http://{ip}:{port_to_start_with}/").ToList();

        var host = $"http://{this_directory_info.Name}.localhost:" + port_to_start_with.ToString() + "/";
        //var host = $"http://[::1]:{port_to_start_with}/";
             Console.WriteLine($"Server available at: {host}");


/*       foreach(var host in local_hosts)
        {
             _listener.Prefixes.Add(host);
             Console.WriteLine($"Server available at: {host}");
        }
*/

             _listener.Prefixes.Add(host);
        _listener.Start();



        

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

                context.Response.AddHeader("Server", "web-server-created-by-Oleksandr-Kryvonos/v-2023-12-03");

                context.Response.AddHeader("Date", DateTime.Now.ToString("yyyy-MM-dd--HH:mm:ss"));

                if (context.Request.HttpMethod == "GET")

                    await lisperantoGet.Process(root_path, context);

                if (context.Request.HttpMethod == "POST")

                    await lisperantoPost.Process(root_path, context);

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