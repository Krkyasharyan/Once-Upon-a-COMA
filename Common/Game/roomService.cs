using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using Common;
using JsonSerializer = System.Text.Json.JsonSerializer;


namespace Game;

public class RoomService

{
    private List<room> rooms;
    private Dictionary<Socket, room> map;

    public RoomService()
    {
        rooms = new List<room>();
        map = new Dictionary<Socket, room>();
    }
    /*
    public List<room> GetRooms()
    {
        return rooms;
    }*/
    

    public bool DestoryRoom(room room)
    {
        try
        {
            room.users.Clear();
            rooms.Remove(room);
        }
        catch (Exception e)
        {
            return false;
            
        }
        return true;
    }

    public bool CreateRoomService(Socket socket,String message)
    {
        //Console.WriteLine("creating room");
        if (message=="")
        {
            return false;
        }
        RoomJson roomJson = JsonSerializer.Deserialize<RoomJson>(message);
        room room = new room();
        //room.id = roomJson.id;
        //此处改为自增id，以简化前端页面
        room.id = rooms.Count;
        if (rooms.Exists(obj =>obj.id==room.id))
        {
            return false;
        }
        try
        {
            map.Add(socket,room);
        }
        catch (Exception e)
        {
            return false;
        }
        
        room.password = roomJson.password;
        room.users = new List<User>();
        room.users.Add(new User(socket,++room.num));
        rooms.Add(room);
        /*foreach (room room1 in rooms)
        {
            Console.WriteLine("现在有房间"+room1.id);
        }*/
        SendCreateRoom(socket,room.id);
        return true;
    }

    public bool JoinRoom(Socket socket,String message)
    {
        if (message=="")
        {
            return false;
        }
        RoomJson roomJson = JsonSerializer.Deserialize<RoomJson>(message);
        int i = rooms.FindIndex(obj => obj.id == roomJson.id);
        if (i==-1)
        {
            return false;
        }

        if (roomJson.password!=rooms[i].password)
        {
            return false;
        }

        if (rooms[i].users.Count<=2)
        {
            RoomDetailJson roomDetailJson = new RoomDetailJson();
            roomDetailJson.id = rooms[i].id;
            roomDetailJson.name = socket.RemoteEndPoint.ToString();
            foreach (var user in rooms[i].users)
            {
                String json = JsonSerializer.Serialize(roomDetailJson);
                //Console.WriteLine("try to send "+json);
                byte[] buf = Encoding.Unicode.GetBytes(json);
                int a = 2;
                byte[] header=BitConverter.GetBytes(a);
                byte[] result= header.Concat(buf).ToArray();int length = result.Length+4;
                byte[] new_result = BitConverter.GetBytes(length).Concat(result).ToArray();
                user.socket.Send(new_result);
                roomDetailJson.name = user.socket.RemoteEndPoint.ToString();
                //user.socket.Send(Encoding.Unicode.GetBytes("a client has connected"));
            }
            map.Add(socket,rooms[i]);
            rooms[i].users.Add(new User(socket,++rooms[i].num));
            String ajson = JsonSerializer.Serialize(roomDetailJson);
            byte[] abuf = Encoding.Unicode.GetBytes(ajson);
            int b = 2;
            byte[] aheader = BitConverter.GetBytes(b);
            byte [] aresult= aheader.Concat(abuf).ToArray();
            int alength = aresult.Length+4;
            byte[] anew_result = BitConverter.GetBytes(alength).Concat(aresult).ToArray();
            socket.Send(anew_result);
            return true;
        }

        return false;
    }
/*
    public room getRoomById(int id)
    {
        return rooms.Find(obj => obj.id == id);
    }
*/
    public List<User> GetUsersBySocket(Socket socket)
    {
        room room = map[socket];
        return room.users;
    }

    public room GetRoomBySocket(Socket socket)
    {
        if (map.ContainsKey(socket))
        {
            return map[socket];
        }
        else
        {
            return null;
        }
    }

    public bool QuitRoom(Socket socket)
    {
        room room;
        try
        {
            room = map[socket];
            map.Remove(socket);
        }
        catch (Exception e)
        {
            return false;
        }
        room.remove(socket);
        if (room.users.Count==0)
        {
            DestoryRoom(room);
        }
        else
        {
            Socket socket1 = room.users[0].socket;
            byte[] result = BitConverter.GetBytes((int)11);
            int length = result.Length+4;
            byte[] new_result = BitConverter.GetBytes(length).Concat(result).ToArray();
            socket1.Send(new_result);
        }
        return true;
    }

    public void QuitRoomWithoutMessage(Socket socket)
    {
        room room;
        try
        {
            room = map[socket];
            map.Remove(socket);
        }
        catch (Exception e)
        {
            return;
        }
        room.remove(socket);
        if (room.users.Count==0)
        {
            DestoryRoom(room);
        }
    }

    public void SendRoomsToUnity(Socket socket)
    {
        List<RoomJson> roomJsons = new List<RoomJson>();
        foreach (room room in rooms)
        {
            roomJsons.Add(new RoomJson(room.id,"",room.num));
        }
        
        RoomListJson roomListJson = new RoomListJson();
        roomListJson.RoomJsons = roomJsons;
        String json = JsonSerializer.Serialize(roomListJson);
        //string json= JsonConvert.SerializeObject(roomJson );
        Console.WriteLine("try to send rooms:" +json);
        Console.WriteLine(socket.RemoteEndPoint);
        byte[] buf = Encoding.Unicode.GetBytes(json);
        int tmp = 0;
        byte[] header=BitConverter.GetBytes(tmp);
        byte[] result = header.Concat(buf).ToArray();
        int length = result.Length+4;
        byte[] new_result = BitConverter.GetBytes(length).Concat(result).ToArray();
        socket.Send(new_result);
    }

    public void SendCreateRoom(Socket socket,int id)
    {
        RoomDetailJson roomDetailJson = new RoomDetailJson();
        roomDetailJson.id = id;
        roomDetailJson.name = socket.RemoteEndPoint.ToString();
        String json = JsonSerializer.Serialize(roomDetailJson);
        byte[] buf = Encoding.Unicode.GetBytes(json);
        int tmp = 1;
        byte[] header=BitConverter.GetBytes(tmp);
        byte[] result = header.Concat(buf).ToArray();
        int length = result.Length+4;
        byte[] new_result = BitConverter.GetBytes(length).Concat(result).ToArray();
        socket.Send(new_result);
    }

    public void KickPlayer(Socket socket)
    {
        room room = map[socket];
        if (room.users[0].socket!=socket)
        {
            return;
        }

        if (room.users.Count==1)
        {
            return;
        }

        Socket victim = room.users[1].socket;
        room.users.RemoveAt(1);
        byte[] result = BitConverter.GetBytes((int)10);
        int length = result.Length+4;
        byte[] new_result = BitConverter.GetBytes(length).Concat(result).ToArray();
        victim.Send(new_result);
    }

}