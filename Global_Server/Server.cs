using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Global_Server
{
    class Server
    {
        static Byte[] image = new Byte[1024];
        
        public static HttpListener listener;
        public static string url = "http://*:80/";
        //public static int pageViews = 0;
        public static int requestCount = 0;
        //public static string pageData =
        //    "<!DOCTYPE>" +
        //    "<html>" +
        //    "  <head>" +
        //    "    <title>HttpListener Example</title>" +
        //    "  </head>" +
        //    "  <body>" +
        //    "    <p>Page Views: {0}</p>" +
        //    "    <form method=\"post\" action=\"shutdown\">" +
        //    "      <input type=\"submit\" value=\"Shutdown\" {1}>" +
        //    "    </form>" +
        //    "  </body>" +
        //    "</html>";
        public static async Task HandleIncomingConnections()
        {
            bool runServer = true;

            // While a user hasn't visited the `shutdown` url, keep on handling requests
            while (runServer)
            {
                // Will wait here until we hear from a connection
                HttpListenerContext ctx = await listener.GetContextAsync();

                // Peel out the requests and response objects
                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;

                // Print out some info about the request
                Console.WriteLine("Request #: {0}", ++requestCount);
                Console.WriteLine(req.Url.ToString());
                Console.WriteLine(req.HttpMethod);
                Console.WriteLine(req.UserHostName);
                Console.WriteLine(req.UserAgent);
                Console.WriteLine();

                // If `shutdown` url requested w/ POST, then shutdown the server after serving the page
                //if ((req.HttpMethod == "POST") && (req.Url.AbsolutePath == "/shutdown"))
                //{
                //    Console.WriteLine("Shutdown requested");
                //    runServer = false;
                //}
                if (req.HttpMethod == "POST")
                {
                    Console.WriteLine("Post Method neeeeeee");
                    var context = listener.GetContext();
                    var request = context.Request;
                    // Byte[] bytes;
                    //using (System.IO.BinaryReader r = new System.IO.BinaryReader(request.InputStream))
                    //{
                    //    // Read the data from the stream into the byte array
                    //    bytes = r.ReadBytes(Convert.ToInt32(request.InputStream.Length));
                    //}
                    //MemoryStream mstream = new MemoryStream(bytes);
                    using (var reader = new StreamReader(request.InputStream,
                                                         request.ContentEncoding))
                    {
                        //using (var fs = new FileStream("output.jpg", FileMode.Create, FileAccess.Write, FileShare.None))
                        //{
                        //    request.InputStream.CopyTo(fs);
                        //}
                        using (var ms = new MemoryStream(image))
                        {
                            request.InputStream.CopyTo(ms);
                        }
                    }
                    Console.WriteLine("hay qua tham oi, post ne");
                    // Make sure we don't increment the page views counter if `favicon.ico` is requested
                }
                //if (req.Url.AbsolutePath != "/favicon.ico")
                //    pageViews += 1;

                //    // Write the response info
                //string disableSubmit = !runServer ? "disabled" : "";
                //byte[] data = Encoding.UTF8.GetBytes(String.Format(pageData, pageViews, disableSubmit));
                resp.ContentType = "text/html";
                resp.ContentEncoding = Encoding.UTF8;
                //resp.ContentLength64 = data.LongLength;
                //resp.ContentLength64 = image.LongLength;
                // Write out to the response stream (asynchronously), then close it
                //await resp.OutputStream.WriteAsync(data, 0, data.Length);
                //Console.WriteLine(image);
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

        //public static void Main(string[] args)
        //{
        //    // Create a Http server and start listening for incoming connections
        //    listener = new HttpListener();
        //    listener.Prefixes.Add(url);
        //    listener.Start();
        //    Console.WriteLine("Listening for connections on {0}", url);

        //    // Handle requests
        //    Task listenTask = HandleIncomingConnections();
        //    listenTask.GetAwaiter().GetResult();

        //    // Close the listener
        //    listener.Close();
        //}
    }
}
