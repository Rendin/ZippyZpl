using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using ZippyZpl.Model;

namespace ZippyZpl.ViewModel {
    public class LabelViewModel {
        public static string data = null;
        private ObservableCollection<Label> labels = new ObservableCollection<Label>();

        public LabelViewModel() {
            Thread thread = new Thread(StartListening);
            thread.Start();
        }
        public ObservableCollection<Label> Labels {
            get;
            set;
        }

        public void LoadLabels() {

            //labels.Add(new Label {
            //    LabelImage = new BitmapImage(new Uri(@"D:\projects\ZippyZPL\ZippyZpl\Labels\label.png"))
            //});
            //labels.Add(new Label {
            //    LabelImage = new BitmapImage(new Uri(@"D:\projects\ZippyZPL\ZippyZpl\Labels\label.png"))
            //});

            Labels = labels;
        }
        public void StartListening() {
            // Data buffer for incoming data.  
            byte[] bytes = new Byte[1024];

            // Establish the local endpoint for the socket.  
            // Dns.GetHostName returns the name of the   
            // host running the application.  
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1"); // ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 9100);

            // Create a TCP/IP socket.  
            Socket listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and   
            // listen for incoming connections.  
            try {
                listener.Bind(localEndPoint);
                listener.Listen(10);

                // Start listening for connections.  
                while (true) {
                    Console.WriteLine("Waiting for a connection...");
                    // Program is suspended while waiting for an incoming connection.  
                    Socket handler = listener.Accept();
                    data = null;

                    // An incoming connection needs to be processed.  
                    while (true) {
                        int bytesRec = handler.Receive(bytes);
                        data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                        if (data.IndexOf("^XZ") > -1 || data.IndexOf("^xz") > -1) {
                            break;
                        }
                    }

                    // Show the data on the console.  
                    Console.WriteLine("Text received : {0}", data);

                    //handler.Send(msg);
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();

                    byte[] zpl = Encoding.UTF8.GetBytes(data);

                    // adjust print density (8dpmm), label width (4 inches), label height (6 inches), and label index (0) as necessary
                    var request = (HttpWebRequest)WebRequest.Create("http://api.labelary.com/v1/printers/8dpmm/labels/4x12/0/");

                    var proxy = WebRequest.GetSystemWebProxy();
                    proxy.Credentials = CredentialCache.DefaultCredentials;
                    request.Proxy = proxy;
                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = zpl.Length;

                    var requestStream = request.GetRequestStream();
                    requestStream.Write(zpl, 0, zpl.Length);
                    requestStream.Close();

                    try {
                        var response = (HttpWebResponse)request.GetResponse();
                        var responseStream = response.GetResponseStream();
                        var fileStream = File.Create(@"D:\projects\ZippyZPL\ZippyZpl\Labels\label2.png"); // change file name for PNG images
                        responseStream.CopyTo(fileStream);
                        responseStream.Close();
                        fileStream.Close();
                        //LabelImage.Source = imageStream;
                        responseStream.Close();

                        Application.Current.Dispatcher.BeginInvoke(
                            DispatcherPriority.Background,
                            new Action(()=>labels.Add(new Label {
                                    LabelImage = new BitmapImage(new Uri(@"D:\projects\ZippyZPL\ZippyZpl\Labels\label2.png"))
                                }))
                        );
                    }
                    catch (WebException e) {
                        Console.WriteLine("Error: {0}", e.Status);
                    }

                    //// Echo the data back to the client.  
                    //byte[] msg = Encoding.ASCII.GetBytes(data);

                }

            }
            catch (Exception e) {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();

        }
    }
}
