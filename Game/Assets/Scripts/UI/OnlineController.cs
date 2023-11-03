using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlineController : MonoBehaviour
{
    public static string roomid = null;
    public static string p1; 
    public static string p2;
    public static int p;
    public static bool kickflag = false;
    [SerializeField] UIController uIController;

    void Start()
    {
    }

    public static void multistart()
    {
        WebController.whoamI = p;
        WebController.sendstart();
        
        //GameLoader.LoadMultiplayerScene();
    }

    public static void Setp2(string play2)
    {
        p2 = play2;
    }

    public static void Setp1(string play1)
    {
        p1 = play1;
    }

    public static void Setp(int ppp)
    {
        p = ppp;
    }
    public static int getp()
    {
        return p;
    }
    public static void Setid(int rid)
    {
        roomid = rid.ToString();
    }

    public string getp1()
    {
        return p1;
    }
    public string getp2()
    {
        return p2;
    }
    public string getroomid()
    {
        return roomid;
    }
    public void back()
    {
        uIController.UIMultiplayer();
        if(p == 1)
        {
            if(p2 != "") kick();
            p1 = "";
            WebController.quit();
        }
        else if(p == 2)
        {
            WebController.quit();
        }
    }

    public void kick()
    {
        if(p == 2) back();
        WebController.kick();
        p2 = "";
    }

    void Update()
    {
        if(kickflag) 
        {
            Debug.Log("its not fair");
            continuekick();
            kickflag = false;
        }
    }

    public static void iskicked()
    {
        kickflag = true;
        Debug.Log("啊？");
    }

    public void continuekick()
    {
        Debug.Log("怎么没被踢？");
        uIController.UIMultiplayer();
    }
    
}
