using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using WebServer;
using Common;

namespace TestProject1;

[TestFixture]
public class Tests
{
    private Server Server;
    private Socket player1;
    private Socket player2;
    private byte[] buf; 
    // Tests()
    // {
    //     Server = new Server();
    //     Server.start();
    //     Socket=new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    // }
    
    [SetUp]
    public void Setup()
    {
        
    }

    [Test]
    public void Test1()
    {
        Server = new Server();
        Server.start();
        player1 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        player1.Connect("localhost", 80);
        Assert.True(player1.Connected);
        //Assert.Pass();
        RoomJson roomJson = new RoomJson(0, "123", 1);
        send(player1, 0, roomJson);
        //创建房间 0，123
        byte[] buf = new byte[512];
        Console.WriteLine(buf.Length);
        player1.Receive(buf);
        print(buf);
        Assert.NotZero(buf.Length);
        //完成创建房间
        
    }
    
    //开始创建player2
    [Test]
    public void Test2()
    {
            player2 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            player2.Connect("localhost", 80);
            send(player2, 7, null);
            buf = new byte[512];
            player2.Receive(buf);
            print(buf);
            Assert.NotZero(buf.Length);
        }
    //player2试图加入
    [Test]
    public void Test3()
    {
        
        RoomJson roomJson = new RoomJson(0, "123", 1);
        buf = new byte[512];
        send(player2, 1, roomJson);
        player1.Receive(buf);
        print(buf);
        Assert.NotZero(buf.Length);
        send(player2,2,null);
        send(player2, 1, roomJson);
        send(player1,9,null);
        send(player2, 1, roomJson);
        //反正是可以了
    }
    
    //player1试图开始游戏
    [Test]
    public void Test4()
    {
        
        StartJson startJson = new StartJson(123);
        send(player1, 3, startJson);
        player1.Receive(buf);
        //Assert.AreEqual(2, GetType(buf));
        //player1开始操作，player2收信息}
        print(buf);
    }
    
    //player1试图移动
    [Test]
    public void Test5()
    {
        send(player1,6,null);
        MoveJson moveJson = new MoveJson();
        moveJson.moveY = -1;
        moveJson.moveX = 1;
        send(player1,4,moveJson);
        player2.Receive(buf);
        //Assert.AreEqual(4,GetType(buf));
        print(buf);
    }
    
    //player1试图攻击
    [Test]
    public void Test6()
    {
        AttackJson attackJson = new AttackJson();
        attackJson.attack = 1;
        attackJson.jump = 0;
        send(player1,5,attackJson);
        player2.Receive(buf);
        print(buf);
    }

    [Test]
    public void Test6_1()
    {
        HpJson hpJson = new HpJson();
        hpJson.hp = 20;
        send(player1,11,hpJson);
        print(buf);
    }

    [Test]
    public void Test6_2()
    {
        send(player1,6,null);
        print(buf);
    }

    [Test]
    public void Test6_3()
    {
        send(player1,13,null);
        //Server.Gamer.GameService.GameOver(player1);
    }
    
//错误流测试
    [Test]
    public void Test7()
    {
        player2.Close();
        player1.Close();
        Assert.False(player1.Connected);
        //Server = new Server();
        //Server.start();
        player1 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        player1.Connect("localhost",80);
        Assert.True(player1.Connected);
        //试图不加入房间而开始游戏
        StartJson startJson = new StartJson(0);
        send(player1,3,startJson);
        //试图加入不存在的房间
        RoomJson roomJson = new RoomJson(123, "132", 1);
        send(player1,1,startJson);
        //试图不加入房间而退出
        send(player1,2,null);
        
        //试图两次创建房间
        send(player1,0,roomJson);
        send(player1,0,roomJson);
        send(player1,2,null);
        //
    }

    [Test]
    public void Test8()
    {
        player1.Close();
        player2.Close();
        player1=new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        player2=new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        player1.Connect("localhost",80);
        player2.Connect("localhost",80);
        RoomJson roomJson = new RoomJson(0, "123", 1);
        send(player1,0,roomJson);
        send(player2,1,roomJson);
        send(player1,9,null);
        
    }

    [Test]
    public void Test123()
    {
        //Server.
    }
    
    public void print(byte[] message)
    {
        Int32 type = BitConverter.ToInt32(message,4);
        String body = Encoding.Unicode.GetString(message, 4,128);
        Console.WriteLine("测试端收到消息："+type+" 内容为： ");
        //Console.WriteLine(body);
    }

    public int GetType(byte[] message)
    {
        return BitConverter.ToInt32(message,4);
    }

    public void send(Socket socket, int type, object obj)
    {
        byte[] buf1 = BitConverter.GetBytes(type);
        String json = JsonSerializer.Serialize(obj);
        byte[] buf2 = Encoding.Unicode.GetBytes(json);
        byte[] result = buf1.Concat(buf2).ToArray();
        int length = result.Length+4;
        byte[] new_result = BitConverter.GetBytes(length).Concat(result).ToArray();
        socket.Send(new_result);
    }
}

