using System.Net.Sockets;
using System.Text;
using Common;

namespace Game;
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
/// 11:hp++
/// 12:pause
/// 13:game over
/// 14:state update
/// 
/// client：
/// 0:room list
/// 1:room detail
/// 2:gamer id(name)
/// 3:game start
/// 4:game move
/// 5:game attack
/// 6:game next level
/// 7:search room
/// 8:reconnect
/// 9:create room's feedback
/// 10：kick
/// 11:another quit
/// 12:hp++
/// 13:pause
/// 14:game over
/// 15:state update
/// </summary>
public class Game
{
    public GameService GameService;
    public RoomService RoomService;
    class MessageDetail
    {
        //Encoding:Unicode;  header: 4 bytes; type : 4 byte;no tail
        public Int32 type;
        public String body;
    }

    public Game()
    {
        RoomService = new RoomService();
        GameService = new GameService(ref RoomService);
        Thread t = new Thread(GameService.Start);
        t.Start();
    }

    public void Disconnect(Socket socket)
    {
        room room;
        try
        {
            room = RoomService.GetRoomBySocket(socket);
        }
        catch (Exception e)
        {
            return;
        }
        RoomService.QuitRoom(socket);
    }
    
    public void HandleMessage(Socket socket, byte[] message)
    {
        Int32 type = BitConverter.ToInt32(message);
        String body = Encoding.Unicode.GetString(message, 4, message.Length - 4);
        switch (type)
        {
            case 0: //create room
                if (RoomService.CreateRoomService(socket, body))
                {
                    //给前端返回id以及玩家name
                };
                //Console.WriteLine("chenggong chuangjianfangjian");
                //Console.WriteLine("当前房间数为 {0}",RoomService.GetRooms().Count);
                /*room room = RoomService.GetRoomBySocket(socket);
                foreach (User user in room.users)
                {
                    Console.WriteLine("当前用户{0}",user.socket.RemoteEndPoint);
                }*/
                break;
            case 1: //join room
                Console.WriteLine("someone try to join");
                if (RoomService.JoinRoom(socket, body))
                {
                    //发送成功通知 type:9 includes:id num
                    Console.WriteLine("success");
                }
                else
                {
                    //发送失败通知  type:9 includes :why failed
                    
                }
                room room = RoomService.GetRoomBySocket(socket);
                if (room==null)
                {
                    break;
                }
                foreach (User user in room.users)
                {
                    Console.WriteLine("当前用户{0}",user.socket.RemoteEndPoint);
                }
                break;
            case 2://quit room
                RoomService.QuitRoom(socket);
                break;
            case 3://game start
                GameService.GameStart(socket,body);
                break;
            case 4://gaming : move
                GameService.PhaseMove(socket,body);
                break;
            case 5://gaming : attack
                GameService.PhaseAttack(socket,body);
                break;
            case 6://gaming : enter next level 
                GameService.EnterNextLevel(socket);
                break;
            case 7://search body; returns rooms in need
                RoomService.SendRoomsToUnity(socket);
                break;
            case 8://reconnect
                //GameService.Reconnect(socket);
                break;
            case 9://kick
                RoomService.KickPlayer(socket);
                break;
            case 10://get room list
                RoomService.SendRoomsToUnity(socket);
                break;
            case 11://hp++
                GameService.Hp(socket,body);
                break;
            case 12:
                break;
            case 13:
                GameService.GameOver(socket);
                break;
            case 14://update state
                GameService.GameUpdate(socket, body);
                break;
            default: break;
        }
    }
}