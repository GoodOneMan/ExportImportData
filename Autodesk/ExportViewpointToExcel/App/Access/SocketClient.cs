using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace ExportToExcel.App.Access
{
    class SocketClient
    {
        Socket client_socket = null;
        string ip_address = "192.168.0.162";
        //string ip_address = "127.0.0.1";
        int port = 2112;
        string message = "";

        public SocketClient()
        {
            try
            {
                client_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                client_socket.Connect(new IPEndPoint(IPAddress.Parse(ip_address), port));

                NetworkStream stream = new NetworkStream(client_socket);
                BinaryFormatter binFormat = new BinaryFormatter();

                message = (string)binFormat.Deserialize(stream);
            }
            catch(Exception ex)
            {
                //System.Windows.Forms.MessageBox.Show(ex.Message);
            }
            
            if(client_socket.Connected)
            {
                client_socket.Shutdown(SocketShutdown.Both);
                client_socket.Close();
            }
        }

        public string GetResult()
        {
            return message;
        }
    }
}
