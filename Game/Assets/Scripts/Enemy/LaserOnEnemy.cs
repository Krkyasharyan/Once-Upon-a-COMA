using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserOnEnemy : MonoBehaviour
{
    LaserController laserController;
    GameObject sourcePoint;
    bool onATK = false;
    private Rigidbody2D target = null;
    // Start is called before the first frame update
    void Start()
    {
        // 找到激光预制件的Transform组件
        laserController = GetComponentInChildren<LaserController>();
        sourcePoint = laserController.sourcePoint.gameObject;

    }

    // Update is called once per frame
    void Update()//敌怪激光攻击判定
    {
        if (target != null&&!onATK)// 若攻击范围内有目标，且不在攻击状态，激光，启动！
        {
            laserController.ActivateLaser();
            onATK = true;
        }
        else if(target == null&&onATK) //若攻击范围内无目标，且在攻击状态，激光，关闭~
        {
            laserController.StopLaser();
            onATK = false;
        }
    }

    public void SetTarget(Rigidbody2D newTarget)
    {
        // 设置新的目标
        target = newTarget;
    }

    public void LaserPosChange(int x_dir)
    {
        Vector2 new_position = sourcePoint.transform.position;
        new_position.x += x_dir*0.37f;
        sourcePoint.transform.position = new_position;
    }
}
