using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Serverbyhvtham
{
    class Server_DO2
    {
        static object image_lock = new object();
        static byte[] image;
        public static HttpListener listener;
        //public static string url = "http://*:80/";
        public static string url = "http://localhost:8080/";
        public static int requestCount = 0;

        public static void ExtractImage(byte[] input)
        {
            for (var i = 0; i < input.Length - 4; i++)
                //file image png
                //if (input[i] == 0x89 &&
                //    input[i+1] == 0x50 &&
                //    input[i+2] == 0x4E &&
                //    input[i+3] == 0x47)
                //file image jpg
                if (input[i] == 0xFF &&
                input[i + 1] == 0xD8 &&
                input[i + 2] == 0xFF &&
                input[i + 3] == 0xE0
                )
                {
                    lock (image_lock)
                    {
                        image = new byte[input.Length - i];
                        Array.Copy(input, i, image, 0, image.Length);
                    }
                    Console.WriteLine("image has been set;");
                    return;
                }
            File.WriteAllBytes("output", input);
            Console.WriteLine("Image section not found!");
        }

        public static async Task HandleIncomingConnections()
        {
            bool runServer = true;

            while (runServer)
            {
                HttpListenerContext ctx = await listener.GetContextAsync();
                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;

                Console.WriteLine("Request #: {0}", ++requestCount);
                Console.WriteLine(req.Url.ToString());
                Console.WriteLine(req.HttpMethod);
                Console.WriteLine(req.UserHostName);
                Console.WriteLine(req.UserAgent);
                Console.WriteLine();

                if (req.HttpMethod == "POST")
                {
                    Console.WriteLine(req);
                    using (var reader = new StreamReader(req.InputStream,
                                                         req.ContentEncoding))
                    {
                        using (var ms = new MemoryStream())
                        {
                            req.InputStream.CopyTo(ms);
                            ExtractImage(ms.ToArray());
                        }
                    }
                    resp.ContentType = "text/html";
                    resp.ContentEncoding = Encoding.UTF8;
                    resp.Close();
                }
                if (req.HttpMethod == "GET")
                {
                    lock (image_lock)
                    {
                        if (image != null)
                        {
                            resp.OutputStream.Write(image, 0, image.Length);
                            resp.StatusCode = (int)HttpStatusCode.OK;
                        }
                        else
                        {
                            resp.StatusCode = (int)HttpStatusCode.NotFound;
                        }

                    }
                    resp.ContentType = "image/jpeg";
                    resp.Close();
                }
            }
        }

        private static object FileWebResponse()
        {
            throw new NotImplementedException();
        }

        public static void Main(string[] args)
        {
            listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();
            Console.WriteLine("Listening for connections on {0}", url);

            Task listenTask = HandleIncomingConnections();
            listenTask.GetAwaiter().GetResult();

            listener.Close();
        }

    }
}
