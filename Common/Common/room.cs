using System.Net.Sockets;
using System.Runtime.InteropServices.JavaScript;

namespace Common;

public class room
{
    public int id { get; set; }
    public String password{ get; set; }
    public List<User> users;
    public int num ;
/*
    public room(int id, String password, Socket host)
    {
        this.id = id;
        this.password = password;
        users = new List<User>();
        num = 0;
    }*/
    public room(){}
/*
    private sealed class IdEqualityComparer : IEqualityComparer<room>
    {
        public bool Equals(room x, room y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.id == y.id;
        }

        public int GetHashCode(room obj)
        {
            return obj.id;
        }
    }
*/
    //public static IEqualityComparer<room> IdComparer { get; } = new IdEqualityComparer();
/*
    public bool access(String password)
    {
        if (password==this.password)
        {
            return true;
        }

        return false;
    }

    bool join(String password, Socket user)
    {
        if (access(password))
        {
            return true;
        }

        return false;
    }
*/
    public bool remove(Socket socket)
    {
        bool flag=false;
        for (int i = 0; i < users.Count; i++)
        {
            if (flag==false&&users[i].socket!=socket)
            {
                continue;
            }else if (flag==true)
            {
                users[i].userNum--;
            }
            else
            {
                users.RemoveAt(i);
                i--;
                flag = true;
            }
        }

        if (flag==false)
        {
            return false;
        }
        else
        {
            num--;
            return true;
        }
    }
    
}