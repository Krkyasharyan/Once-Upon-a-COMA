using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;


public class NetController : MonoBehaviour
{
    class MoveJson
    {
        public int moveX;
        public int moveY;
        
        public MoveJson(int x, int y)
        {
            moveX = x;
            moveY = y;
            
        }
    }

    class AttackJson
    {
        public int attack;
        public int jump;

        public AttackJson(int attack, int jump)
        {
            this.attack = attack;
            this.jump = jump;
        }
    }
    
    private static Socket client;
    // Start is called before the first frame update
    class Json
    {
        public int id;
    }
    void Start()
    {
        client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        client.Connect("60.204.150.7", 8500);
        Json json = new Json();
        json.id = 1;
        string j = JsonUtility.ToJson(json);
        byte[] header=BitConverter.GetBytes(1);
        byte[] buf = Encoding.Unicode.GetBytes(j);
        header.Concat(buf).ToArray();
        client.Send(header);
        StartListening(client);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private static void StartListening(Socket socket)
    {
        Debug.Log("listen start");
        byte[] buffer = new byte[1024];
        SocketAsyncEventArgs args = new SocketAsyncEventArgs();
        args.SetBuffer(buffer, 0, buffer.Length);
        args.Completed += OnReceive;
        args.UserToken = socket;
        bool isAsync = socket.ReceiveAsync(args);
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
            //关闭连接
        }
    }

    private static void ProcessReceivedData(SocketAsyncEventArgs e)
    {
        byte[] receivedData = new byte[e.BytesTransferred];
        Array.Copy(e.Buffer, e.Offset, receivedData, 0, e.BytesTransferred);
    
        // 处理接收到的数据
        // ...
        Int32 type = BitConverter.ToInt32(receivedData);
        String body = Encoding.Unicode.GetString(receivedData, 4, receivedData.Length - 4);
        execute(type, body);
        //Debug.Log("收到类型: " +type);
        //Debug.Log("收到消息: " +body);
        //Console.WriteLine("收到消息: " + Encoding.Unicode.GetString(receivedData));
        // 继续监听下一个数据
        if (e.UserToken is Socket socket)
        {
            StartListening(socket);
        }
        else
        {
            Console.WriteLine("link has been closed");
        }

    }

    private static void execute(int type, string json)
    {
        switch (type)
        {
            case 0:break;
            case 1:break;
            case 2:
                receiveMove(json);
                break;
            case 3:break;
            case 4:break;
            case 5:break;
        }
    }


    public static void send(int type, object obj)
    {
        int value = type;
        byte[] src = new byte[4];
        src[3] = (byte)((value >> 24) & 0xFF);
        src[2] = (byte)((value >> 16) & 0xFF);
        src[1] = (byte)((value >> 8) & 0xFF);
        src[0] = (byte)(value & 0xFF);
        string json = JsonUtility.ToJson(obj);
        byte[] buf = Encoding.Unicode.GetBytes(json);
        byte[] byteAll = new byte[src.Length + buf.Length];
        Array.Copy(src, 0, byteAll, 0, src.Length);
        Array.Copy(buf, 0, byteAll,src.Length, buf.Length);
        client.Send(byteAll);
    }

    public static void sendAttack(int x, int y)//第一个参数为j是否按下，第二个参数为k是否按下
    {
        AttackJson attackJson= new AttackJson(x, y);
        send(3, attackJson);
    }
    public static void sendMove(int x, int y)
    {
        MoveJson moveJson = new MoveJson(x, y);
        send(2, moveJson);
    }

    public static void receiveMove(string move)
    {
        MoveJson moveJson = JsonUtility.FromJson<MoveJson>(move);
        Debug.Log(moveJson.moveX);
        Debug.Log(moveJson.moveY);
        //PlayerController.PlayerMove(moveJson.moveX,moveJson.moveY);
        //调用移动函数
    }
    //WebController.sendmove（-1，1）；
}

