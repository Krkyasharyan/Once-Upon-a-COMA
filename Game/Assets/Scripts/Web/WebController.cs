using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using System.Timers;
using Newtonsoft.Json;
using UnityEditor;
using Timer = System.Timers.Timer;

/// <summary>
/// 以下为数据类型列表（接受类型）：
/// server：0 create room
/// 1:join room
/// 2:quit room
/// 3:game start
/// 4:game move
/// 5:game attack
/// 6:game next level 
/// 7:search: for example:
/// 8:reconnect
/// 9:kick
/// 10:get room list
///
/// client:
/// 0:room list
/// 1:room detail
/// 2:gamer id
/// 3:game start
/// 4:game move
/// 5:game attack
/// 6:game next level
/// 7:search room
/// 8:reconnect
/// 9:create room's feedback
/// 10:beingkicked

/// </summary>
public class WebController : MonoBehaviour
{
    //实现单例模式
    private static WebController instance;
    public static PlayerController[] players = new PlayerController[0];
    public static Timer timer;
    public static int whoamI;
    public static int seed;

    private static bool moveFlag = false;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 防止场景切换时销毁该对象
        }
        else
        {
            Destroy(gameObject); // 如果已经存在该对象的实例，则销毁新创建的实例
        }
    }

    public static WebController Instance
    {
        get { return instance; }
    }
    private static int id, num;
    private static string ip;
    public static bool connectStatus = false;
    class MoveJson
    {
        public int moveX;
        public int moveY;
        public int player_id;
        public MoveJson(int x, int y)
        {
            moveX = x;
            moveY = y;
            
        }
    }

    [System.Serializable]
    public class RoomJson
    {
        
        public int id { get; set; }
        public string password { get; set; }
    
        public int num { get; set; }

        public RoomJson(int id, string password, int num)
        {
            this.id = id;
            this.password = password;
            this.num = num;
        }
    }
    
    public class StartJson
    {
        public int roomId { get; set; }
        public int seed { get; set; }
        public int playerid { get; set; }

        public StartJson(int seed)
        {
            roomId = 0;
            this.seed = seed;
            playerid = 0;
        }
    }

    public class RoomListJson
    {
        public List<RoomJson> RoomJsons { get; set; }
    }
    
    public class RoomDetailJson
    {
        public int id { get; set; }
        public string name { get; set; }
    }
    class AttackJson
    {
        public int attack;
        public int jump;
        public int player_id;
        public AttackJson(int attack, int jump)
        {
            this.attack = attack;
            this.jump = jump;
        }
    }

    class HpJson
    {
        public int hp;
    }

    class GetHpJson
    {
        public int player_id;
        public int hp;
    }
    
    public class StateJson
    {
        public int x1 { get; set; }
        public int y1{ get; set; }
        public int x2{ get; set; }
        public int y2{ get; set; }
        public int hp1{ get; set; }
        public int hp2{ get; set; }
    }
    
    private static Socket client;
    // Start is called before the first frame update
    class Json
    {
        public int id;
    }
    // void Start()
    // {
    //     client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    //     client.Connect("localhost", 8500);
    //     ip=client.LocalEndPoint.ToString();
    //     StartListening(client);
    
    // }
    void Start()
    {
        client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        
        connectStatus = ConnectToServer();
        
        if (connectStatus)
        {
            // 连接成功
            ip = client.LocalEndPoint.ToString();
            // 执行其他逻辑
            StartListening(client);
        }
        else
        {
            // 连接失败
            // 执行其他逻辑或错误处理
        }
    }

    bool ConnectToServer()
    {
        try
        {
            // client.Connect("60.204.150.7", 80);
            client.Connect("localhost", 8500);
            return true; // 连接成功，返回 1
        }
        catch (Exception e)
        {
            // 连接失败，输出错误信息
            Debug.LogError("连接失败：" + e.Message);
            return false; // 连接失败，返回 0
        }
    }

    // Update is called once per frame
    void Update()
    {
        GetAttackInput();
        //GetMoveInput();
        //if (Time.frameCount % 50 == 0)
        //{
            //GetMoveInput();
        //}
    }

    private void FixedUpdate()
    {
        GetMoveInput();
    }

    void GetMoveInput()
    {
        if (players.Length == 0) return;
        if (players[whoamI-1].isDie()) return;
        float X = Input.GetAxisRaw("Horizontal");
        float Y = Input.GetAxisRaw("Vertical");
        int x = (int)X;
        int y = (int)Y;
        if (x != 0 || y != 0)
        {
            sendMove(x, y);
            //moveFlag = true;
        }
        //sendMove((int)X,(int)Y);
        else
        {
            if (moveFlag)
            {
                sendMove(0, 0);
            }
        }
    }

    void GetAttackInput()
    {
        if(players.Length == 0) return;
        if (players[whoamI-1].isDie()) return;
        //if (players[1 - whoamI].isDie()) return;
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                sendAttack(1, 1);
                return;
            }
            sendAttack(1, 0);
            return;
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            sendAttack(0, 1);
        }
    }

    private void StartListening(Socket socket)
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

    private void OnReceive(object sender, SocketAsyncEventArgs e)
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

    // private void ProcessReceivedData(SocketAsyncEventArgs e)
    // {
    //     byte[] receivedData = new byte[e.BytesTransferred];
    //     Array.Copy(e.Buffer, e.Offset, receivedData, 0, e.BytesTransferred);
    //     Debug.Log("youxiaoxi");
    //     // 处理接收到的数据
    //     // ...
    //     Int32 type = BitConverter.ToInt32(receivedData);
    //     String body = Encoding.Unicode.GetString(receivedData, 4, receivedData.Length - 4);
    //     Debug.Log(type);
    //     Debug.Log(body);
    //     execute(type, body);
    //     //Console.WriteLine("收到消息: " + Encoding.Unicode.GetString(receivedData));
    //     // 继续监听下一个数据
    //     if (e.UserToken is Socket socket)
    //     {
    //         StartListening(socket);
    //     }
    //     else
    //     {
    //         Console.WriteLine("link has been closed");
    //     }
    //
    // }
    
    private void ProcessReceivedData(SocketAsyncEventArgs e)
    {
        byte[] receivedData = new byte[e.BytesTransferred];
        Array.Copy(e.Buffer, e.Offset, receivedData, 0, e.BytesTransferred);
        // 处理接收到的数据
        // ...
        ProcessMessage(receivedData);
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

    private void ProcessMessage(byte[] message)
    {
        Int32 length = BitConverter.ToInt32(message);
        if (length==message.Length)
        {
            //Console.WriteLine("a single message");
            byte[] tmp = new byte[length-4];
            Array.Copy(message, 4, tmp, 0, length-4);
            preexecute(tmp);
        }
        else
        {
            byte[] tmp = new byte[length-4];
            byte[] next = new byte[message.Length - length];
            Array.Copy(message, 4, tmp, 0, length-4);
            Array.Copy(message, length, next, 0, message.Length - length);
            preexecute(tmp);
            ProcessMessage(next);
        
        }
    }

    private void preexecute(byte[] message)
    {
        Int32 type = BitConverter.ToInt32(message);
        String body = Encoding.Unicode.GetString(message, 4, message.Length - 4);
        execute(type, body);
    }


    private void execute(int type, string json)
    {
        //Debug.Log(type);
        switch (type)
        {
            case 0:
                receiveRooms(json);
                break;
            case 1:
                receiveCreateRoom(json);
                break;
            case 2:
                receiveJoinRoom(json);
                break;
            case 3:
                receivestart(json);
                break;
            case 4:
                receiveMove(json);
                break;
            case 5:
                receiveAttack(json);
                break;
            case 6:
                receiveToNextLevel();
                break;
            case 7:break;
            case 8:break;
            case 9:
                Debug.Log(json);
                break;
            case 10:
                receivekick();
                break;
            case 11:
                receivequit();
                break;
            case 12:
                receiveHpUp(json);
                break;
            case 15:
                recieveState(json);
                break;
        }
    }

    public static string getip()
    {
        //Debug.Log("ipipipipipipipi" +  ip);
        return ip;
    }


    // public static void send(int type, object obj)
    // {
    //     //if(obj == null) Debug.Log("有辨识度");
    //     int value = type;
    //     byte[] src = new byte[4];
    //     src[3] = (byte)((value >> 24) & 0xFF);
    //     src[2] = (byte)((value >> 16) & 0xFF);
    //     src[1] = (byte)((value >> 8) & 0xFF);
    //     src[0] = (byte)(value & 0xFF);
    //     string json = JsonConvert.SerializeObject(obj);
    //     Debug.Log("尝试发送json: "+json);
    //     byte[] buf = Encoding.Unicode.GetBytes(json);
    //     byte[] byteAll = new byte[src.Length + buf.Length];
    //     Array.Copy(src, 0, byteAll, 0, src.Length);
    //     Array.Copy(buf, 0, byteAll,src.Length, buf.Length);
    //     client.Send(byteAll);
    // }
    
    public static void send(int type, object obj)
    {
        byte[] buf1 = BitConverter.GetBytes(type);
        string json = JsonConvert.SerializeObject(obj);
        byte[] buf2 = Encoding.Unicode.GetBytes(json);
        byte[] result = buf1.Concat(buf2).ToArray();
        int length = result.Length + 4;
        byte[] new_result = BitConverter.GetBytes(length).Concat(result).ToArray();
        client.Send(new_result);
    }

    public static void sendAttack(int x, int y)//第一个参数为j是否按下，第二个参数为k是否按下
    {
        AttackJson attackJson= new AttackJson(x, y);
        send(5, attackJson);
    }
    public static void sendMove(int x, int y)
    {
        MoveJson moveJson = new MoveJson(x, y);
        send(4, moveJson);
    }

    public static void sendReconnect()
    {
        //send(8, null);
    }
    public static void sendRoomSearchMessage()
    {
        send(7, null);
    }
    
    public static void sendCreateRoom(int id, string password)
    {
        RoomJson roomJson = new RoomJson(id, password, 1);
        //Debug.Log(password);
        // if(roomJson == null) Debug.Log("啊？");
        // string json = JsonConvert.SerializeObject(roomJson);
        // Debug.Log("你是？"+json);
        // byte[] buf = Encoding.Unicode.GetBytes(json);
        // int tmp = 0;
        // byte[] header=BitConverter.GetBytes(tmp);
        // byte[] data = header.Concat(buf).ToArray();
        // client.Send(data);
        send(0, roomJson);
    }
    
    public static void sendJoinRoom(int id, string password)
    {
        RoomJson roomJson = new RoomJson(id, password, 0);
        send(1, roomJson);
    }

    public static void sendToNextLevel()
    {
        send(6, null);
    }

    public static void sendHpUp(int hp)
    {
        HpJson HP = new HpJson();
        HP.hp = hp;
        send(11, HP);
        Debug.Log("hp++ sent, cur hp is " + hp);
    }

    public static void receiveHpUp(string json)
    {
        Debug.Log("hp++ received! " + json);
        GetHpJson HP = JsonConvert.DeserializeObject<GetHpJson>(json);
        int id = HP.player_id;
        int hp = HP.hp;
        Debug.Log("id " + id);
        Debug.Log("hp " + hp);
        if (id != whoamI)
        {
            players[id - 1].curHP = hp;
        }
    }

    public static void receiveToNextLevel()
    {
        ToNextLevel.goFlag = true;
    }

    public static void receiveCreateRoom(string json)
    {
        RoomDetailJson roomDetailJson = JsonConvert.DeserializeObject<RoomDetailJson>(json);
        ButtonController.sendCreateRoom(roomDetailJson.id);
    }

    public static void receiveJoinRoom(string json)
    {
        RoomDetailJson roomDetailJson = JsonConvert.DeserializeObject<RoomDetailJson>(json);
        Debug.Log("另一玩家ip"+roomDetailJson.name);
        //ButtonController.sendJoinRoom1(roomDetailJson.name);
        ButtonController.sendJoinRoom(roomDetailJson.id, roomDetailJson.name);
    }
    
    public static void receiveMove(string move)
    {
        int pos = move.IndexOf('}');
        string newMove = move;
        if (pos > -1)
        {
            newMove = move.Substring(0, pos + 1);
        }
        Debug.Log(newMove);
        
        MoveJson moveJson = JsonConvert.DeserializeObject<MoveJson>(newMove);
        //Debug.Log(moveJson.moveX);
        //Debug.Log(moveJson.moveY);
        //Debug.Log(moveJson.player_id);
        int x = moveJson.moveX;
        int y = moveJson.moveY;
        
        players[moveJson.player_id - 1].move(x, y);
        if (x == 0 && y == 0)
        {
            moveFlag = false;
        }
        else
        {
            moveFlag = true;
        }
        // if ()
        // {
        //     //Player1.move
        // }
        //PlayerController.PlayerMove(moveJson.moveX,moveJson.moveY);
        //调用移动函数
    }
    //WebController.sendmove（-1，1）；
    public static void receiveRooms(string json)
    {
        //调用ui相关函数
        Debug.Log(json);
        ButtonController.SetRoomList(json);
    }

    public static void receiveAttack(string json)
    {
        Debug.Log(json);
        AttackJson attackJson = JsonConvert.DeserializeObject<AttackJson>(json);
        //ReceiveAttackJson attackJson = JsonConvert.DeserializeObject<ReceiveAttackJson>(json);
        Debug.Log(attackJson.player_id);
        Debug.Log(attackJson.attack);
        Debug.Log(attackJson.jump);
        if(attackJson.attack == 1) players[attackJson.player_id - 1].attack();
        if(attackJson.jump == 1) players[attackJson.player_id - 1].dodge();
        //调用人物相关函数
    }

    public static void createRoom(string password)
    {
        sendCreateRoom(0,password);
    }

    public static void joinRoom(int id,string password)
    {
        sendJoinRoom(id,password);
    }

    public static void getroomlist()
    {
        
        send(7,null);
    }

    public static void kick()
    {
        send(9, null);
    }

    public static void receivekick()
    {
        Debug.Log("我被踢啦");
        OnlineController.iskicked();
    }

    public static void quit()
    {
        Debug.Log("想要退出");
        send(2, null);
    }

    public static void receivequit()
    {
        ButtonController.p2quit();
    }

    public static void findPlayers()
    {
        GameObject[] playerGameObjects = GameObject.FindGameObjectsWithTag("Player");
        players = new PlayerController[playerGameObjects.Length];
        for (int i = 0; i < playerGameObjects.Length; i++)
        {
            PlayerController p = playerGameObjects[i].GetComponent<PlayerController>();
            players[p.player_id - 1] = p;
            //p.GetMyId();
            p.connected();
            Debug.Log("玩家"+(p.player_id)+"连接状态"+p.connect);
        }
        Debug.Log("玩家数量"+playerGameObjects.Length);
    }

    public static void sendstart()
    {
        send(3, null);
        Debug.Log("send start");
    }

    public static void receivestart(string json)
    {
        Debug.Log(json);
        StartJson startJson = JsonConvert.DeserializeObject<StartJson>(json);
        seed = startJson.seed;
        if (whoamI == 0)
        {
            whoamI = 2;
        }
        
        //Debug.Log("ready to set seed");
        // PlayerPrefs.SetInt("seed", seed);
        // PlayerPrefs.Save();
        
        Debug.Log("seed saved");
        
        //GameLoader.LoadMultiplayerScene();
        GameLoader.startFlag = true;
    }

    public static void gameover()
    {
        send(13, null);
    }

    public static void timerstart()
    {
        if (whoamI == 2) return;
        Debug.Log("Time Start");
        timer = new Timer();
        timer.Enabled = true; //设置是否执行Elapsed事件
        timer.Elapsed += new ElapsedEventHandler(Doit); //绑定Elapsed事件
        timer.Interval = 50; //设置时间间隔
        
    }
    
    public static void Doit(object sender, ElapsedEventArgs e)
    {
        
        StateJson state = new StateJson();
        state.hp1 = players[0].curHP;
        
        state.hp2 = players[1].curHP;
        Debug.Log("位置");
        state.x1 = (int)players[0].getrbx();
        state.x2 = (int)players[1].getrbx();
        state.y1 = (int)players[0].getrby();
        state.y2 = (int)players[1].getrby(); 
        
        Debug.Log("好位置" + players[0].getrbx());
        send(14,state);
        
    }

    static void recieveState(string json)
    {
        StateJson state = new StateJson();
        state = JsonConvert.DeserializeObject<StateJson>(json);
        players[0].curHP = state.hp1;
        players[0].rb.MovePosition(new Vector2(state.x1,state.y1));
        players[1].curHP = state.hp2;
        players[1].rb.MovePosition(new Vector2(state.x2,state.y2));
    }
}

