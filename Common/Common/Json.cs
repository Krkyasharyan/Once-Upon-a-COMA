using System.Numerics;

namespace Common;

public class RoomJson
{
    public int id { get; set; }
    public String password { get; set; }
    
    public int num { get; set; }

    public RoomJson(int id, string password, int num)
    {
        this.id = id;
        this.password = password;
        this.num = num;
    }
}

public class RoomListJson
{
    public List<RoomJson> RoomJsons { get; set; }
}
public class RoomDetailJson
{
    public int id { get; set; }
    public String name { get; set; }
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

public class MoveJson
{
    public int moveX { get; set; }
    public int moveY { get; set; }
}

public class AttackJson
{
    public int attack { get; set; }
    public int jump { get; set; }
}
public class SendAttackJson
{
    public int player_id { get; set; }
    public int attack { get; set; }
    public int jump { get; set; }

    public SendAttackJson(int num, int isAttack, int isJump)
    {
        this.player_id = num;
        this.attack = isAttack;
        this.jump = isJump;
    }
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

public class SendMoveJson
{
    public int player_id { get; set; }
    public int moveX { get; set; }
    public int moveY { get; set; }

    public SendMoveJson(int num, int moveX, int moveY)
    {
        this.player_id = num;
        this.moveX = moveX;
        this.moveY = moveY;
    }
}
/*
public class GamingJson
{
    public int moveX { get; set; }
    public int moveY { get; set; }
    public int attack{ get; set; }
    public int jump  { get; set; }
}

public class SendGamingJson
{
    public int player_id { get; set; }
    public int moveX { get; set; }
    public int moveY { get; set; }
    public int attack{ get; set; }
    public int jump  { get; set; }
}
*/
public class HpJson
{
    public int hp { get; set; }
}

public class SendHpJson
{
    public int player_id;
    public int hp;
}
/*
public class PauseJson
{
    public int pause { get; set; }
}*/