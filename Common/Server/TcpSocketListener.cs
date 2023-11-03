using System.Net;
using System.Net.Sockets;

namespace WebServer;

public class TcpSocketListener
{
    private IPEndPoint endPoint;
    private Socket serverSocket;

    public event EventHandler<Socket> SocketConnect;
    public TcpSocketListener(String host, int port)
    {
        endPoint = new IPEndPoint(IPAddress.Parse(host), port);
    }
    public void start()
    {
        if (!IsRunning)
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(endPoint);
            serverSocket.Listen();

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += OnAccept;
            serverSocket.AcceptAsync(args);
            Console.WriteLine("server start");
            Console.Read();
        }
    }
    
    public void NoHangStart()
    {
        if (!IsRunning)
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(endPoint);
            serverSocket.Listen();

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += OnAccept;
            serverSocket.AcceptAsync(args);
            Console.WriteLine("server start");
            //Console.Read();
        }
    }

    private void OnAccept(object? sender, SocketAsyncEventArgs e)
    {
        if (e.SocketError==SocketError.Success)
        {
            Socket socketClient = e.AcceptSocket;
            if (socketClient!=null)
            {
                SocketConnect(this, socketClient);
                Console.WriteLine("receive {0}",socketClient.RemoteEndPoint);
            }

        }


        e.AcceptSocket = null;
        if (IsRunning)
        {
            serverSocket.AcceptAsync(e);
        }

    }


    public bool IsRunning
    {
        get { return serverSocket != null; }
    }
/*
    public void stop()
    {
        if (serverSocket==null)
        {
            return;
        }
        else
        {
            serverSocket.Close();
        }
    }*/
}
    