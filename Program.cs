// start 2023-06-10 17:02 CET, Munich
// new version 2023-07-08 Munich
using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;


class Lisperanto
{
    static async Task Main(string[] args)
    {
        WebHost.CreateDefaultBuilder(args)
            .Configure(config => config.UseStaticFiles())
            .UseWebRoot("www-root").Build().Run();
    }
}