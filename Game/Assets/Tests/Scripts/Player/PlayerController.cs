using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private Animator animator;
    private Rigidbody2D rb;

    public float moveX, moveY;

    public float spikeTimer = 0;

    public int spikeCnt = 0, stepSpikeCnt = 0;

    private bool spikeBool = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        cdTimer = cdTime;
        maxHP = 100;
        curHP = maxHP;
        ATK = 10;
    }

    public void Update()
    {
        move();
        spike();
    }

    public bool isDead()
    {
        if(curHP <= 0) 
        {
            return true;
        }
        return false;  
    }

    public void move()
    {
        if(isDead()) return;
        moveX = Input.GetAxisRaw("Horizontal");
        moveY = Input.GetAxisRaw("Vertical");

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

        cdTimer += Time.deltaTime;
        if(cdTimer >= iTime && speedX >= 20) speedX /= 2;

        if(iTime > 0) iTime -= Time.deltaTime;
        else iTime = 0;
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
        rb.MovePosition((rb.position + Time.fixedDeltaTime * new Vector2(moveX * speedX, moveY * speedY)));
    }

    public void TakeDamage(int damage) 
    {

        if(iTime > 0) return;
        animator.SetBool("isHit", true);
        iTime = 0.5f;
        curHP -= damage;
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
}
