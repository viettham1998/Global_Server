using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Global_Server
{
    class ServerFinal
    {
        static byte[] image;
        static string tmp;
        //public static HttpListener listener;
        public static string url = "http://localhost:8080/";
        public static int requestCount = 0;
        static Byte[] bytes;
        public static async Task HandleIncomingConnections(HttpListener listener)
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
                Console.WriteLine(req.ContentEncoding);
                Console.WriteLine();
                if (req.HttpMethod == "POST")
                {

                    using (System.IO.BinaryReader r = new System.IO.BinaryReader(req.InputStream))
                    {
                        bytes = r.ReadBytes(Convert.ToInt32(req.InputStream.Length));
                    }
                    MemoryStream mstream = new MemoryStream(bytes);
                }
                resp.ContentType = "text/html";
                resp.ContentEncoding = Encoding.UTF8;
                await resp.OutputStream.WriteAsync(image);
                resp.Close();

                //}
                //if (req.HttpMethod == "GET")
                //{
                //    Console.WriteLine("Get Method ne ne ne");
                //    var context = listener.GetContext();
                //    var request = context.Request;
                //    using (var reader = new StreamReader(request.InputStream,
                //                                         request.ContentEncoding))
                //    {
                //        using (var ms = new MemoryStream(image))
                //        {
                //            request.InputStream.CopyTo(ms);
                //        }
                //    }
                //    Console.WriteLine("hay qua tham oi, get ne");
                //    //resp.ContentType = "image/jpeg";
                //    //resp.ContentLength64 = image.LongLength;
                //    //await resp.OutputStream.WriteAsync(image);
                //    //resp.Close();
                //}
                //resp.ContentType = "image/jpeg";
                //resp.ContentLength64 = image.LongLength;
                //await resp.OutputStream.WriteAsync(image);
                //resp.Close();
            }
        }

        private static object FileWebResponse()
        {
            throw new NotImplementedException();
        }

        public static void Main(string[] args)
        {
            HttpListener listener; 
            listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();
            Console.WriteLine("Listening for connections on {0}", url);

            Task listenTask = HandleIncomingConnections(listener);
            listenTask.GetAwaiter().GetResult();

            listener.Close();
        }
    }
}
