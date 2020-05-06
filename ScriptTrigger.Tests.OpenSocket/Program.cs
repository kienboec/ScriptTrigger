using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;

namespace ScriptTrigger.Tests.OpenSocket
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpListenerBasedListener();
        }

        private static void HttpListenerBasedListener()
        {
            HttpListener listener = null;
            try
            {
                listener = new HttpListener() { IgnoreWriteExceptions = true }; // IsSupported... we assume so.
                listener.Prefixes.Add("http://localhost:5000/");
                listener.Start();
                Console.WriteLine("Waiting for a connection on http://localhost:5000 ...");

                byte[] answer = Encoding.UTF8.GetBytes("Hello World!");

                HttpListenerContext context;
                while ((context = listener.GetContext()) != null)
                {
                    context.Response.ContentLength64 = answer.Length;
                    context.Response.OutputStream.Write(answer, 0, answer.Length);
                    context.Response.OutputStream.Close();
                }
            }
            catch (Exception exc)
            {
                Console.Error.WriteLine(exc.Message);
            }
            finally
            {
                listener?.Stop();
            }
        }

        private static void SocketBasedListener()
        {
            try
            {
                Socket listener = new Socket(IPAddress.Loopback.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listener.Bind(new IPEndPoint(IPAddress.Loopback, 5000));
                listener.Listen(10); // queue length

                Console.WriteLine("Waiting for a connection on http://localhost:5000 ...");

                Socket handler;
                while ((handler = listener.Accept()) != null)
                {
                    Console.Write("-");

                    string data = "";
                    while (true)
                    {
                        byte[] bytes = new byte[1024];
                        int bytesRec = handler.Receive(bytes);
                        var newData = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        data += newData;
                        if (data.EndsWith("\n\n") || data.EndsWith("\r\n\r\n"))
                        {
                            break;
                        }
                        else if (newData.Length == 0)
                        {
                            Console.Write("E");
                            break;
                        }
                    }

                    handler.Send(Encoding.ASCII.GetBytes(
                        @"HTTP/1.1 200 OK
Content-Language: de
Content-Type: text/html; charset=utf-8

Hello World!"));
                    handler.Shutdown(SocketShutdown.Send);
                    handler.Close();
                    Console.Write("|");
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
        }
    }
}
