using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    //敌人的组件信息
    Rigidbody2D rigidbody2D; 
    Collider2D collider2D;
    Animator animator;

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
    private float detectionRange = 10;
    
    //时间信息
    private float attackInterval = 3.0f; // 攻击间隔为
    private float lastAttackTime = 0;

    //flags
    bool ContactPlayer = false;
    public bool DIE = false;


    // 在第一次帧更新之前调用 Start
    void Start()
    {
        // 随机初始敌人朝向
        look_direction = Random.Range(0,2)==0?1:-1;
        x_direction = look_direction;
        // 获取所有带有 Player 标签的游戏对象
        GameObject[] playerGameObjects = GameObject.FindGameObjectsWithTag("Player");
        // 遍历每个玩家游戏对象，获取其 Rigidbody 组件并添加到数组中
        playerRigidbodies = new Rigidbody2D[playerGameObjects.Length];
        for (int i = 0; i < playerGameObjects.Length; i++)
        {
            playerRigidbodies[i] = playerGameObjects[i].GetComponent<Rigidbody2D>();
        }
        collider2D = GetComponent<Collider2D>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    

    void Update()
    {
        if(DIE) return;
        
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
        if(DIE) 
        {
            return;
        }
        
        //更新位置信息
        float xDistance=0,yDistance=0;
        position = rigidbody2D.position;
        if(nearestPlayer!=null)
        {
            target = nearestPlayer.position;
            nearestDistance = Vector2.Distance(position,target);
        }
        
        //根据最近的玩家位置信息生成移动逻辑
        if(nearestDistance > detectionRange) 
        {
            speed = 0;
            x_direction = look_direction;
        }
        else
        {
            if(ContactPlayer) 
            {
                Attack();
                speed = 0;
            }
            else                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         
            {
                speed = 3f;
                xDistance = Mathf.Abs(target.x - position.x);
                yDistance = Mathf.Abs(target.y - position.y);
            }
            x_direction = (position.x < target.x)?1:-1;
            y_direction = (position.y < target.y)?1:-1;
        }
    
        animator.SetFloat("MoveX",x_direction);
        animator.SetFloat("Speed",speed);
        if(xDistance > yDistance) position.x += Time.deltaTime * speed * x_direction;
        else position.y += Time.deltaTime * speed * y_direction;
        rigidbody2D.MovePosition(position);

    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        PlayerController player = other.gameObject.GetComponent<PlayerController>();
        if(player!=null)
        {
            ContactPlayer = true; 
            animator.SetBool("Contact",ContactPlayer);
        }
    }

    private void Attack()
    {
        if(DIE||nearestPlayer.GetComponent<PlayerController>().isDead()) return;
        if(ContactPlayer)
        {
            if (Time.time - lastAttackTime > attackInterval)
            {
                animator.SetTrigger("IsHitting");
                lastAttackTime = Time.time;
                int ATK = GetComponent<Character>().ATK;
                nearestPlayer.GetComponent<PlayerController>().TakeDamage(ATK);
            }

        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        PlayerController player = other.gameObject.GetComponent<PlayerController>();
        if(player!=null)
        {
            ContactPlayer = false;
            animator.SetBool("Contact",ContactPlayer);
        }
    }
}


