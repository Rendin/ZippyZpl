using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
//using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
//using System.Drawing;
using ZippyZpl.Model;

namespace ZippyZpl.ViewModel {
    public class LabelViewModel {
        private static string data = null;
        private static bool killListen = false;
        private Thread thread = null;
        private TcpListener listener = null;

        private uint labelWidth = 6;
        private uint labelHeight = 4;


        private ObservableCollection<Label> labels = new ObservableCollection<Label>();

        public LabelViewModel() {
        }

        public void Shutdown() {
            if (listener != null) {
                killListen = true;
                listener.Stop();
                thread.Join(5000);
                thread = null;
            }
        }

        public void SetLabelSize(uint width, uint height) {
            labelWidth = width;
            labelHeight = height;
        }

        public ObservableCollection<Label> Labels {
            get;
            set;
        }

        public void LoadLabels() {

            Labels = labels;
        }

        public void StartListening() {
            if (listener == null && killListen == false) {
                thread = new Thread(ListenForLabels);
                thread.Start();
            }
            else if (killListen == false){
                killListen = true;
                listener.Stop();
                thread.Join(5000);
                thread = null;
            }
        }

        public void ListenForLabels() {
            // Data buffer for incoming data.  
            Byte[] bytes = new Byte[1024];

            // Establish the local endpoint for the socket.  
            // Dns.GetHostName returns the name of the   
            // host running the application.  
            //IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1"); // ipHostInfo.AddressList[0];
            Int32 port = 9100;
            //IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 9100);

            //// Create a TCP/IP socket.  
            //Socket listener = new Socket(ipAddress.AddressFamily,
            //    SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and   
            // listen for incoming connections.  
            try {
                listener = new TcpListener(ipAddress, port);
                listener.Start();

                // Start listening for connections.  
                while (true) {
                    // Program is suspended while waiting for an incoming connection.  
                    TcpClient client = listener.AcceptTcpClient();
                    data = null;

                    if (killListen) {
                        break;
                    }
                    
                    NetworkStream stream = client.GetStream();

                    int i;

                    // Loop to receive all the data sent by the client.
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0) {
                        // Translate data bytes to a ASCII string.
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                        Console.WriteLine("Received: {0}", data);

                        // Process the data sent by the client.
                        data = data.ToUpper();
                    }

                    // Shutdown and end connection
                    client.Close();

                    // Show the data on the console.  
                    Console.WriteLine("Text received : {0}", data);

                    byte[] zpl = Encoding.UTF8.GetBytes(data);

                    // adjust print density (8dpmm), label width (4 inches), label height (6 inches), and label index (0) as necessary
                    var request = (HttpWebRequest)WebRequest.Create("http://api.labelary.com/v1/printers/8dpmm/labels/" +
                                                                    labelWidth.ToString() + "x" + labelHeight.ToString() +
                                                                    "/0/");

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
                        var fileName = Path.GetTempFileName();
                        var fileStream = File.Create(fileName); // change file name for PNG images
                        responseStream.CopyTo(fileStream);
                        responseStream.Close();
                        fileStream.Close();

                        Application.Current.Dispatcher.BeginInvoke(
                            DispatcherPriority.Background,
                            new Action(() => labels.Add(new Label {
                                LabelImage = new BitmapImage(new Uri(fileName))
                            })));
                        ;
                    }
                    catch (WebException e) {
                        Console.WriteLine("Error: {0}", e.Status);
                    }
                }
            }
            catch (Exception e) {
                Console.WriteLine(e.ToString());
            }

            listener = null;
            killListen = false;
        }
    }
}
