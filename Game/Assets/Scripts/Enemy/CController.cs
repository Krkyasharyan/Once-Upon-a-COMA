using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CController : MonoBehaviour
{
    //敌人的组件信息
    Rigidbody2D rigidbody2D; 
    Collider2D collider2D;
    Animator animator;
    Character character;
    Spawn spawnItemController;

    //位置、方向、速度
    Vector2 position;
    Vector2 target = new Vector2(Mathf.Infinity,Mathf.Infinity);//追击目标
    int look_direction;//初始朝向
    public int x_direction = 1;
    int y_direction = 1;
    public float speed = 0;

    //玩家和距离
    public Rigidbody2D[] playerRigidbodies; // 所有玩家的 Rigidbody 组件数组
    Rigidbody2D nearestPlayer = null; // 最近的玩家 Rigidbody
    float nearestDistance = Mathf.Infinity; // 最近的距离
    private float detectionRange = 15;
    
    //时间信息
    private float attackInterval = 3.0f; // 攻击间隔为
    private float lastAttackTime = 0;

    //flags
    bool ContactPlayer = false;
    public bool DIE = false;
    bool dropItemSpawned = false;


    // 在第一次帧更新之前调用 Start
    void Start()
    {
        // 随机初始敌人朝向
        look_direction = Random.Range(0,2)==0?1:-1;
        changeXdirTo(look_direction);
        // 获取所有带有 Player 标签的游戏对象
        GameObject[] playerGameObjects = GameObject.FindGameObjectsWithTag("Player");
        // 遍历每个玩家游戏对象，获取其 Rigidbody 组件并添加到数组中
        playerRigidbodies = new Rigidbody2D[playerGameObjects.Length];
        for (int i = 0; i < playerGameObjects.Length; i++)
        {
            playerRigidbodies[i] = playerGameObjects[i].GetComponent<Rigidbody2D>();
        }
        //组件初始化
        collider2D = GetComponent<Collider2D>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        character = GetComponent<Character>();
        spawnItemController = GetComponent<Spawn>();
    }

    

    void Update()
    {
        if(isDie()) return;
        // 遍历每个玩家 Rigidbody，计算距离并找到最近的玩家
        foreach (Rigidbody2D player in playerRigidbodies)
        {
            float distance = Vector2.Distance(rigidbody2D.position, player.position);
            if (distance < nearestDistance)
            {
                nearestPlayer = player;
                nearestDistance = distance;
            }
        }
    }
    
    void FixedUpdate()
    {
        if(DIE)  return;
        
        //更新位置信息
        float xDistance=0,yDistance=0;
        position = rigidbody2D.position;
        if(nearestPlayer!=null)
        {
            target = nearestPlayer.position;
            nearestDistance = Vector2.Distance(position,target);
        }
        
        //根据最近的玩家位置信息生成移动、攻击逻辑
        if(nearestDistance > detectionRange) 
        {
            //检查范围内没有角色，待机，朝向初始方向
            speed = 0;
            x_direction = look_direction;
        }
        else
        {
            //角色进入攻击范围，发起攻击
            if(nearestDistance < 10f)
            {
                Attack();
                speed = 0;
            }
            //角色不在攻击范围内，朝角色移动
            else                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         
            {
                EndAttack();
                speed = 3f;
                xDistance = Mathf.Abs(target.x - position.x);
                yDistance = Mathf.Abs(target.y - position.y);
            }
            //判定移动方向，x方向变化时
            int x_dir =  (position.x < target.x)?1:-1;
            changeXdirTo(x_dir);
            y_direction = (position.y < target.y)?1:-1;
        }

        //移动
        animator.SetFloat("MoveX",x_direction);
        animator.SetFloat("Speed",speed);
        if(xDistance > yDistance) position.x += Time.deltaTime * speed * x_direction;
        else position.y += Time.deltaTime * speed * y_direction;
        rigidbody2D.MovePosition(position);
    }


     private void Attack()//攻击逻辑：进入范围后设置攻击目标，随后自行执行LaserOnEnemy中的逻辑
     {
         if(DIE||nearestPlayer.GetComponent<PlayerController>().isDie())
         {
            EndAttack();
            return;
         }
         GetComponent<LaserOnEnemy>().SetTarget(nearestPlayer);
         if (Time.time - lastAttackTime > attackInterval)
         {
            animator.SetTrigger("IsHitting");
            lastAttackTime = Time.time;
         }
     }

     private void EndAttack()//同理：出范围后将攻击目标置空
     {
        GetComponent<LaserOnEnemy>().SetTarget(null);
     }

     public int getDirection()
     {
        return x_direction;
     }

     private void changeXdirTo(int x_dir)
     {
        if(x_direction!=x_dir)
        {
            x_direction = x_dir;
            GetComponent<LaserOnEnemy>().LaserPosChange(x_dir);
        }
     }

     public bool isDie()
     {
        if(character.DIE)
        {
            DIE = true;
            EndAttack();
            if (!dropItemSpawned)
            {
                spawnItemController.SpawnItem(gameObject);
                dropItemSpawned = true;
            }
            return true;
        }
        else return false;
     }
}


