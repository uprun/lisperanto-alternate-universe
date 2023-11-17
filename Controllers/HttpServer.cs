using System;
using System.Net;
using System.IO;

public class HttpServer
{
    public int Port = 8080;

    private HttpListener _listener;

    public async Task Start()
    {
        _listener = new HttpListener();
        _listener.Prefixes.Add("http://localhost:" + Port.ToString() + "/");
        _listener.Start();
        while(true)
        {
            // var context = await _listener.BeginGetContext();
            // Console.WriteLine("received context");
            // await ListenerCallback(context);
        }
        
    }

    public void Stop()
    {
        _listener.Stop();
    }

    private  async Task ListenerCallback(HttpListenerContext context)
    {
        {
            var request = context.Request;

            // do something with the request
            Console.WriteLine($"{request.Url}");

            //Receive();

            Console.WriteLine($"{request.HttpMethod} {request.Url}");

            if (request.HasEntityBody)
            {
                var body = request.InputStream;
                var encoding = request.ContentEncoding;
                var reader = new StreamReader(body, encoding);
                if (request.ContentType != null)
                {
                    Console.WriteLine("Client data content type {0}", request.ContentType);
                }
                Console.WriteLine("Client data content length {0}", request.ContentLength64);

                Console.WriteLine("Start of data:");
                string s = reader.ReadToEnd();
                Console.WriteLine(s);
                Console.WriteLine("End of data:");
                reader.Close();
                body.Close();
                
            }

            var response = context.Response;
                response.StatusCode = (int) HttpStatusCode.OK;
                response.ContentType = "text/plain";
                response.OutputStream.Write(new byte[] {}, 0, 0);
                response.OutputStream.Close();
                response.Close();

        }
    }
}