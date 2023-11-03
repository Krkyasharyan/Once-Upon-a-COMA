using System.Net.Sockets;
using System.Text;
using Game;
using System.Timers;
using Common;

namespace WebServer;

public class Server
{
    private static List<Socket> users;
    public static Game.Game Gamer;
    
    public static void Main()
    {
        users = new List<Socket>();
        Gamer = new Game.Game();
        TcpSocketListener tcpSocketListener = new TcpSocketListener("0.0.0.0", 8500);
        tcpSocketListener.SocketConnect += OnClientConnect;
        users = new List<Socket>();
        tcpSocketListener.start();
        
    }

    public static void start()
    {
        users = new List<Socket>();
        Gamer = new Game.Game();
        TcpSocketListener tcpSocketListener = new TcpSocketListener("0.0.0.0", 8500);
        tcpSocketListener.SocketConnect += OnClientConnect;
        users = new List<Socket>();
        tcpSocketListener.NoHangStart();
        
    }
    

    private static void OnClientConnect(object? sender, Socket e)
    {
        users.Add(e);
        Console.WriteLine("receive {0}",e.RemoteEndPoint);
        StartListening(e);
        foreach (Socket user in users)
        {
            Console.WriteLine("{0} is in user",user.RemoteEndPoint);
        }
    }
    
    private static void StartListening(Socket socket)
    {
        
        byte[] buffer = new byte[1024];
        SocketAsyncEventArgs args = new SocketAsyncEventArgs();
        args.SetBuffer(buffer, 0, buffer.Length);
        args.Completed += OnReceive;
        args.UserToken = socket;
        bool isAsync = socket.ReceiveAsync(args);
        socket.NoDelay = true;
        if (!isAsync)
        {
            // 如果立即完成，直接处理接收到的数据
            ProcessReceivedData(args);
        }
    }

    private static void OnReceive(object sender, SocketAsyncEventArgs e)
    {
        if (e.SocketError == SocketError.Success && e.BytesTransferred > 0)
        {
            ProcessReceivedData(e);
        }
        else if (e.BytesTransferred==0)
        {
            //exit here
            if (e.UserToken is Socket socket)
            {
                users.Remove(socket);
                Gamer.Disconnect(socket);
                socket.Close();
            }
        }
    }

    private static void ProcessReceivedData(SocketAsyncEventArgs e)
    {
        try
        {
            byte[] receivedData = new byte[e.BytesTransferred];
            Array.Copy(e.Buffer, e.Offset, receivedData, 0, e.BytesTransferred);
    
            // 处理接收到的数据
            // ...
            //Console.WriteLine("收到消息长度"+receivedData.Length);
            Int32 type = BitConverter.ToInt32(receivedData,4);
            String body = Encoding.Unicode.GetString(receivedData, 8, receivedData.Length - 8);
            Console.WriteLine("收到类型: " +type);
            Console.WriteLine("收到消息: " +body);
            //Console.WriteLine("收到消息: " + Encoding.Unicode.GetString(receivedData));
        
            // 继续监听下一个数据
            if (e.UserToken is Socket socket)
            {
                //Console.WriteLine(socket.RemoteEndPoint);
                ProcessMessage(socket,receivedData);
                StartListening(socket);
            }
        }
        catch (Exception exception)
        {
            return;
        }
        
    }
    
    static void ProcessMessage(Socket socket,byte[] message)
    {
        try
        {
            Int32 length = 0;
            length = BitConverter.ToInt32(message);
            if (length == message.Length)
            {
                Console.WriteLine("a single message");
                byte[] tmp = new byte[length - 4];
                Array.Copy(message, 4, tmp, 0, length - 4);
                Gamer.HandleMessage(socket, tmp);
            }
            else
            {
                Console.WriteLine("here");
                byte[] tmp = new byte[length - 4];
                byte[] next = new byte[message.Length - length];
                Array.Copy(message, 4, tmp, 0, length - 4);
                Array.Copy(message, length, next, 0, message.Length - length);
                Gamer.HandleMessage(socket, tmp);
                String body = Encoding.Unicode.GetString(next, 4, next.Length - 4);
                ProcessMessage(socket, next);
            }
        }
        catch (Exception e)
        {
            return;
        }
        // 处理接收到的数据
        //Console.WriteLine("收到消息: " + Encoding.Unicode.GetString(message));
        //
        //if (Gamer==null)
        //{
        //    Console.WriteLine("gamer ");
        //    return;
        //}
        
            
    }
        
        //socket.Send(message);
}
    
    
