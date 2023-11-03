using System.Net.Sockets;

namespace Common;

public class User
{
    public Socket socket;
    public int userNum;
/*
    public User()
    {
        userNum = 1;
    }*/
    public User(Socket socket, int userNum)
    {
        this.socket = socket;
        this.userNum = userNum;
    }
}