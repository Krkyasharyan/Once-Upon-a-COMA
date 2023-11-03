using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Timers;
using Common;
using Timer = System.Timers.Timer;

namespace Game;

public class GameService
{
    private Queue<(Socket,byte[])> workQueue;
    private RoomService RoomService;
    private Dictionary<int, List<byte[]>> memory;
    private List<int> runningRooms;
    private Timer timer;
    private bool isWorking { get; set; }
    public GameService(ref RoomService roomService)
    {
        workQueue = new Queue<(Socket,byte[])>();
        RoomService = roomService;
        runningRooms = new List<int>();
        memory = new Dictionary<int, List<byte[]>>();
    }

    public void Start()
    {
        if (isWorking)
        {
            return;
        }
        else
        {
            //开始计时
            timer = new Timer();
            timer.Enabled = true; //设置是否执行Elapsed事件
            timer.Elapsed += new ElapsedEventHandler(Doit); //绑定Elapsed事件
            timer.Interval = 50; //设置时间间隔
            isWorking = true;
        }
    }

    public void Doit(object sender, ElapsedEventArgs e)
    {
        /*if (runningRooms.Count==0)
        {
            workQueue.Clear();
            return;
        }*/
        
        foreach ((Socket, byte []) tuple in workQueue)
        {
            tuple.Item1.Send(tuple.Item2);
        }
        workQueue.Clear();
        //Console.WriteLine(DateTime.Now.ToString()+"执行Timer"); 
    }
/*
    public void AddRoom(int id)
    {
        if (!runningRooms.Contains(id))
        {
            runningRooms.Add(id);
            memory.Add(id,new List<byte[]>());
        }
    }*/
/*
    public void Reconnect(Socket socket)
    {
        int id = RoomService.GetRoomBySocket(socket).id;
        List<byte[]> data = memory[id];
        Thread thread = new Thread((() =>
        {
            foreach (byte[] bytes in memory[id])
            {
                socket.Send(bytes);
            }
        }));
    }*/

    public void PhaseAttack(Socket socket,String json)
    {
        int num = 0;
        AttackJson attackJson = new AttackJson(); 
        attackJson = JsonSerializer.Deserialize<AttackJson>(json);
        
        
        List<User> users = RoomService.GetUsersBySocket(socket);
        int id = RoomService.GetRoomBySocket(socket).id;
        foreach (User user in users)
        {
            if (user.socket==socket)
            {
                num = user.userNum;
            }
        }
        SendAttackJson sendAttackJson = new SendAttackJson(num, attackJson.attack, attackJson.jump);
        String send = JsonSerializer.Serialize(sendAttackJson);
        byte[] buf1 = BitConverter.GetBytes(5);
        byte[] buf2 = Encoding.Unicode.GetBytes(send);
        byte[] result = buf1.Concat(buf2).ToArray();
        int length = result.Length+4;
        byte[] new_result = BitConverter.GetBytes(length).Concat(result).ToArray();
        foreach (User user in users)
        {
            workQueue.Enqueue((user.socket, new_result));
            //memory[id].Add(result);
        }
    }

    public void PhaseMove(Socket socket, String json)
    {
        int num = 0;
        MoveJson moveJson = new MoveJson();
        try
        {
            moveJson = JsonSerializer.Deserialize<MoveJson>(json);
        }
        catch (Exception e)
        {
            return;
        }
        List<User> users = RoomService.GetUsersBySocket(socket);
        foreach (User user in users)
        {
            if (user.socket==socket)
            {
                num = user.userNum;
            }
        }
        SendMoveJson sendMoveJson = new SendMoveJson(num, moveJson.moveX, moveJson.moveY);
        String send = JsonSerializer.Serialize(sendMoveJson);
        byte[] buf1 = BitConverter.GetBytes((int)4);
        byte[] buf2 = Encoding.Unicode.GetBytes(send);
        byte[] result = buf1.Concat(buf2).ToArray();
        int length = result.Length+4;
        byte[] new_result = BitConverter.GetBytes(length).Concat(result).ToArray();
        foreach (User user in users)
        {
            workQueue.Enqueue((user.socket, new_result));
        }
    }

    public void Hp(Socket socket, String json)
    {
        int num = 0;
        HpJson hpJson = new HpJson();
        try
        {
            hpJson = JsonSerializer.Deserialize<HpJson>(json);
        }
        catch (Exception e)
        {
            return;
        }
        
        List<User> users = RoomService.GetUsersBySocket(socket);
        foreach (User user in users)
        {
            if (user.socket==socket)
            {
                num = user.userNum;
            }
        }
        SendHpJson sendHpJson = new SendHpJson();
        sendHpJson.player_id = num;
        sendHpJson.hp = hpJson.hp;
        Console.WriteLine("num is "+sendHpJson.player_id);
        Console.WriteLine("hp is "+sendHpJson.hp);
        //String send = JsonSerializer.Serialize(sendHpJson);
        String send = "{\"player_id\":" + num.ToString() + ",\"hp\":" + hpJson.hp.ToString() + "}";
        Console.WriteLine("hp try to send"+send);
        byte[] buf1 = BitConverter.GetBytes((int)12);
        byte[] buf2 = Encoding.Unicode.GetBytes(send);
        byte[] result = buf1.Concat(buf2).ToArray();
        int length = result.Length+4;
        byte[] new_result = BitConverter.GetBytes(length).Concat(result).ToArray();
        foreach (User user in users)
        {
            workQueue.Enqueue((user.socket, new_result));
        }
    }

    /*public void PhasePause(Socket socket, String json)
    {
        PauseJson pauseJson = new PauseJson();
        try
        {
            pauseJson = JsonSerializer.Deserialize<PauseJson>(json);
        }
        catch (Exception e)
        {
            return;
        }

        List<User> users = RoomService.GetUsersBySocket(socket);
        foreach (User user in users)
        {
            byte[] buf1 = BitConverter.GetBytes((int)13);
            byte[] buf2 = Encoding.Unicode.GetBytes(json);
            byte[] result = buf1.Concat(buf2).ToArray();
            int length = result.Length+4;
            byte[] new_result = BitConverter.GetBytes(length).Concat(result).ToArray();
            user.socket.Send(new_result);
        }
    }*/

    public void GameStart(Socket socket,String Json)
    {
        if (RoomService.GetRoomBySocket(socket)==null)
        {
            return;
        }
        else
        {
            //phase json
            room room = RoomService.GetRoomBySocket(socket);
            runningRooms.Add(room.id);
            int seed = RandomNumber.GetRandom();//get seed
            StartJson startJson = new StartJson(seed);
            startJson.seed = seed;
            startJson.roomId = room.id;
            for (int i = 0; i < room.users.Count; i++)
            {
                startJson.playerid = i+1;
                String send = JsonSerializer.Serialize(startJson);
                byte[] buf1 = BitConverter.GetBytes((int)3);
                byte[] buf2 = Encoding.Unicode.GetBytes(send);
                byte[] result = buf1.Concat(buf2).ToArray();
                int length = result.Length+4;
                byte[] new_result = BitConverter.GetBytes(length).Concat(result).ToArray();
                room.users[i].socket.Send(new_result);
            }
        }
    }

    public void EnterNextLevel(Socket socket)
    {
        List<User> users = RoomService.GetUsersBySocket(socket);
        foreach (User user in users)
        {
            byte[] result = BitConverter.GetBytes((int)6);
            int length = result.Length+4;
            byte[] new_result = BitConverter.GetBytes(length).Concat(result).ToArray();
            user.socket.Send(new_result);
        }
    }

    public void GameOver(Socket socket)
    {
        List<User> users = RoomService.GetUsersBySocket(socket);
        foreach (User user in users)
        {
            if (user.socket==socket)
            {
                continue;
            }
            byte[] result = BitConverter.GetBytes((int)14);
            int length = result.Length+4;
            byte[] new_result = BitConverter.GetBytes(length).Concat(result).ToArray();
            user.socket.Send(new_result);
        }

        if (users.Count==1)
        {
            User user = users[0];
            RoomService.QuitRoomWithoutMessage(user.socket);
        }
        else
        {
            User user = users[0];
            User user1 = users[1];
            RoomService.QuitRoomWithoutMessage(user.socket);
            RoomService.QuitRoomWithoutMessage(user1.socket);
        }
        
    }

    public void GameUpdate(Socket socket, String json)
    {
        byte[] buf1 = BitConverter.GetBytes((int)15);
        byte[] buf2 = Encoding.Unicode.GetBytes(json);
        byte[] result = buf1.Concat(buf2).ToArray();
        int length = result.Length+4;
        byte[] new_result = BitConverter.GetBytes(length).Concat(result).ToArray();
        List<User> users = RoomService.GetUsersBySocket(socket);
        foreach (User user in users)
        {
            user.socket.Send(new_result);
        }
        
    }
}