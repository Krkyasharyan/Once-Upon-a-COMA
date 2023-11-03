using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
public class ButtonController : MonoBehaviour {

    static bool roomflag = false;
    [System.Serializable]
    public class RoomJson {
        public int id;
        public string password;
        public int num;
        // 无参数构造函数
       
    }
    [System.Serializable]
    public class RoomListJson
    {
        public List<RoomJson> RoomJsons;
    }
    [SerializeField] GameObject inputext;
    [SerializeField] UIController uIController;
    [SerializeField] OnlineController onlineController;

    public static List<RoomJson> buttonList = new List<RoomJson>();
    public static int maxpage;
    public int flag, index, page;

    private static string ipadr = "test";
    private string password;

	void Start ()
    {
        Refresh();
        inputext.SetActive(false);
        flag = 0;
        page = 1;

	}

    public int getmaxpage()
    {
        return maxpage;
    }
    public void closeinput()
    {
        inputext.SetActive(false);
    }

    public void OnInputEndEdit(string value)
    {
        inputext.SetActive(false);
        GetPassword(value);
    }

    public void pageup()
    {
        if(page + 1 > maxpage) return;
        page ++;
    }

    public void inputpassword(int f, int ind)
    {
        flag = f;
        index = ind;
        inputext.SetActive(true);
    }

    public void pagedown()
    {
        if(page - 1 < 1) return;
        page --;
    }

    public void create()
    {
        inputpassword(1, 0);
    }

	void Update ()
    {
        if(roomflag) 
        {
            continueCR();
            roomflag = false;
        }
       
	}

    public void GetPassword(string password)
    {
        if(flag == 1)
        {
            flag = 0;
            WebController.createRoom(password);
        }
        else if(flag == 2)
        {
            flag = 0;
            OnlineController.Setp(2);
            WebController.joinRoom(index, password);
        }
    }

    public static string getipadr()
    {
        return ipadr;
    }

    public static void setipadr(string ip)
    {
        ipadr = ip;
    }

    public void Refresh()
    {
        ipadr = WebController.getip();
        WebController.getroomlist();
    }

    public static void sendCreateRoom(int roomid)
    {
        OnlineController.Setid(roomid);
        OnlineController.Setp1(ipadr);
        OnlineController.Setp(1);
        roomflag = true;
        // ButtonController bt = new ButtonController();
        // bt.continueCR();
    }

    public void continueCR()
    {
        uIController.UIOnline();
    }

    public static void sendJoinRoom(int roomid, string p1)
    {
        if(OnlineController.getp() == 1)
            OnlineController.Setp2(p1);
        else
        {
            OnlineController.Setid(roomid);
            OnlineController.Setp1(p1);
            OnlineController.Setp2(ipadr);
            roomflag = true; 
        }
    }

    public static void SetRoomList(string json)
    {
        RoomListJson roomListJson = JsonConvert.DeserializeObject<RoomListJson>(json);
        buttonList = roomListJson.RoomJsons;
        maxpage = (buttonList.Count + 8) / 9;
        if (maxpage == 0) maxpage = 1;
    }
   

    public static void p2quit()
    {
        OnlineController.Setp2("");
    }
}