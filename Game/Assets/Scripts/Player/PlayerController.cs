using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Timers;
using Timer = System.Timers.Timer;
public class PlayerController : MonoBehaviour
{
    public float speedX = (float)10;
    public float speedY = (float)10;
    public float cdTime = (float)1;//翻滚技能CD
    public float iTime = (float)0;//iTime = 0.5 代表获得0.5s无敌时间
    public float cdTimer;//翻滚计时器
    public int maxHP;
    public int curHP;
    public int ATK;
    public bool connect = false;
    public static int tolerance=10;//耐受力
    public static int mental=10;//精神力
    public static int magic=10;//魔力值

    public int player_id = 0;
    private int my_id;

    public static int getMagic()
    {
        return magic;
    }
    public static int getTolerance()
    {
        return tolerance;
    }
    public static int getMental()
    {
        return mental;
    }

    public void connected()
    {
        connect = true;
        GetMyId();
    }
    
/// <summary>
/// 计算耐受力改变
/// </summary>
/// <param name="change">为tolerance改变量，其余同理</param>
/// <returns></returns>
    public static void tolerancechange(int change)
    {
        tolerance += change;
    }

    public static void mentalchange(int change)
    {
        mental += change;
    }

    public static void magicchange(int change)
    {
        magic += change;
    }

    public void atkChange(int change)
    {
        this.ATK += change;
    }

    private Animator animator;
    private PlayerAnimation playerAnimation;
    public Rigidbody2D rb;
    public GameObject backpackUI;
    public Healthbar healthbar;

    public float moveX, moveY;
    public bool moveFlag = false;

    public float spikeTimer = 0;
    private Timer timer;
    public int spikeCnt = 0, stepSpikeCnt = 0;

    private bool spikeBool = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        playerAnimation = GetComponent<PlayerAnimation>();
        rb = GetComponent<Rigidbody2D>();
        cdTimer = cdTime;
        maxHP = 100;
        curHP = maxHP;
        ATK = 10;
        timer = new Timer();
        timer.Enabled = true; //设置是否执行Elapsed事件
        timer.Elapsed += new ElapsedEventHandler(Doit); //绑定Elapsed事件
        timer.Interval = 10000; //设置时间间隔
        my_id = player_id;
        backpackUI.SetActive(false);
        healthbar.SetMaxHealth(maxHP);
        
        LoadStats();
        healthbar.SetHealth(curHP);
    }

    public void Update()
    {
        //move();
        spike();
        // Check if the 'i' key is pressed
        if (Input.GetKeyDown(KeyCode.I) && my_id == player_id)
        {
            // Toggle the backpack UI's active state
            backpackUI.SetActive(!backpackUI.activeSelf);
        }
    }

    public void GetMyId()
    {
        my_id = WebController.whoamI;
        if (my_id != player_id)
        {
            healthbar.gameObject.SetActive(false);
        }
    }
    
    public void Doit(object sender, ElapsedEventArgs e)
    {
        if (mental>5)
        {
            mentalchange(-1);
        }

        if (tolerance>5)
        {
            tolerancechange(-1);
        }
    }

    public bool isDie()
    {
        if(curHP <= 0) 
        {
            return true;
        }
        return false;  
    }

    public float getrbx()
    {
        return this.transform.position.x;
    }
    public float getrby()
    {
        return this.transform.position.y;
    }
    public void move()
    {
        if(isDie()) return;
        
        
        if(connect)
        {
            //if (my_id == player_id)
            //{
               //int x = (int)X;
               //int y = (int)Y;
               // if (x != 0 || y != 0)
               // {
               //     moveFlag = true;
               //     WebController.sendMove(x, y);
               // }
               //
               // if (x == 0 && y == 0 && moveFlag)
               // {
               //     moveFlag = false;
               //     while (!(moveX == 0 && moveY == 0))
               //     {
               //         SendStopMovement();
               //     }
               // }
               //WebController.sendMove(x, y); 
            //}
            
        }
        else
        {
            float X = Input.GetAxisRaw("Horizontal");
            float Y = Input.GetAxisRaw("Vertical");
            move((int)X,(int)Y);
        }
        if(Time.timeScale!=0)
        {
            int faceDir = (int)transform.localScale.x;
            if(moveX > 0)
            {
                faceDir = 1;
            }
            else if(moveX < 0)
            {
                faceDir = -1;
            }
            transform.localScale = new Vector3(faceDir, 1, 1);
            //获取朝向并翻转
        }

        cdTimer += Time.deltaTime;
        if(cdTimer >= iTime && speedX >= 20) speedX /= 2;

        if(iTime > 0) iTime -= Time.deltaTime;
        else iTime = 0;
    }

    IEnumerator SendStopMovement()
    {
        WebController.sendMove(0, 0);
        yield return new WaitForSeconds(0.05f);
    }

    public void spike()
    {
        if (spikeBool)
        {
            spikeTimer += Time.deltaTime;
            if (spikeTimer > SpikeTrapController.ContinuousDamageInterval)
            {
                spikeTimer = 0;
                TakeDamage((int)SpikeTrapController.Damage);
            }
        }
    }

    private void FixedUpdate()
    {
        move();
        rb.MovePosition((rb.position + Time.fixedDeltaTime * new Vector2(moveX * speedX, moveY * speedY)));
    }

    public void TakeDamage(int damage) 
    {
        if(iTime > 0) return;
        animator.SetBool("isHit", true);
        iTime = 0.5f;
        curHP -= damage;

        healthbar.SetHealth(curHP); 
    }

    public void StartSpikeTimer()
    {
        spikeBool = true;
    }

    public void StopSpikeTimer()
    {
        spikeBool = false;
        spikeTimer = 0;
    }

    public void move(int x, int y)
    {
        if (isDie())
            return;
        moveX = x * 1.0f;
        moveY = y * 1.0f;
    }

    public void attack()
    {
        Debug.Log("attack!");
        playerAnimation.attackFlag = true;
        //playerAnimation.Attack();
        
    }

    public void dodge()
    {
        if (cdTimer >= cdTime)
        {
            playerAnimation.dodgeFlag = true;
            //playerAnimation.Dodge();
            //Debug.Log("dodge!");
            // animator.SetTrigger("dodgeKey");
            // cdTimer = 0;
            // iTime = 0.5f;
            // speedX = 20; 
        }
    }
    
    public void SaveStats()
    {
        if (!connect)
        {
            PlayerPrefs.SetInt("curHP", curHP);
            PlayerPrefs.SetInt("mental", mental);
            PlayerPrefs.SetInt("magic", magic);
            PlayerPrefs.SetInt("ATK", this.ATK);
            
            this.GetComponent<Inventory>().SaveItems();
            
            PlayerPrefs.Save();            
        }
        else
        {
            if (player_id == 1)
            {
                PlayerPrefs.SetInt("P1curHP", curHP);
                PlayerPrefs.SetInt("P1mental", mental);
                PlayerPrefs.SetInt("P1magic", magic);
                PlayerPrefs.SetInt("P1ATK", this.ATK);
            
                this.GetComponent<Inventory>().SaveItems();
            
                PlayerPrefs.Save();  
            }
            else if (player_id == 2)
            {
                PlayerPrefs.SetInt("P2curHP", curHP);
                PlayerPrefs.SetInt("P2mental", mental);
                PlayerPrefs.SetInt("P2magic", magic);
                PlayerPrefs.SetInt("P2ATK", this.ATK);
            
                this.GetComponent<Inventory>().SaveItems();
            
                PlayerPrefs.Save();  
            }
        }
    }

    public void LoadStats()
    {
        if (player_id == 0)
        {
            curHP = PlayerPrefs.GetInt("curHP", maxHP);
            mental = PlayerPrefs.GetInt("mental", mental);
            magic = PlayerPrefs.GetInt("magic", magic);
            this.ATK = PlayerPrefs.GetInt("ATK", this.ATK);        
        }
        else
        {
            if (player_id == 1)
            {
                curHP = PlayerPrefs.GetInt("P1curHP", maxHP);
                mental = PlayerPrefs.GetInt("P1mental", mental);
                magic = PlayerPrefs.GetInt("P1magic", magic);
                this.ATK = PlayerPrefs.GetInt("P1ATK", this.ATK);        
            }
            else if (player_id == 2)
            {
                curHP = PlayerPrefs.GetInt("P2curHP", maxHP);
                mental = PlayerPrefs.GetInt("P2mental", mental);
                magic = PlayerPrefs.GetInt("P2magic", magic);
                this.ATK = PlayerPrefs.GetInt("P2ATK", this.ATK);      
            }
        }
    }
}
