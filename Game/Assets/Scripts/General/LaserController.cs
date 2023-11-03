using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour
{
    public enum Target
        {
            Enemy,
            Player,
            None
        };
  
    public float laserTimer = 0;
    public int laserDamage = 20;
    public Target target = Target.None; 
    public float fireInterval = 3f, fireDuration = 1f;
    public Transform sourcePoint;
    public float laserAngle = 0;
    public Gradient LaserColorGradient, AimColorGradient;

    public LineRenderer fireRenderer, aimRenderer;
    public bool aimBool = false, fireBool = false;
    private Vector2 laserDirection;
    private RaycastHit2D[] hit;
    private Vector2 endPoint;
    
    ///////////////////////////////// test //////////////////////////////////
    private Rigidbody2D[] playerRigidbody2Ds;
    /////////////////////////////////////////////////////////////////////////
    
    // Start is called before the first frame update
    void Start()
    {
        Vector2 source = sourcePoint.position;
        
        fireRenderer.positionCount = 2;
        fireRenderer.SetPosition(0, source);
        fireRenderer.colorGradient = LaserColorGradient;

        aimRenderer.positionCount = 2;
        aimRenderer.SetPosition(0, source);
        aimRenderer.colorGradient = AimColorGradient;
        
        /////////////////////////////////// test //////////////////////////////////////
        GameObject[] playerGameObjects = GameObject.FindGameObjectsWithTag("Player");
        playerRigidbody2Ds = new Rigidbody2D[playerGameObjects.Length];
        for (int i = 0; i < playerGameObjects.Length; i++)
        {
            playerRigidbody2Ds[i] = playerGameObjects[i].GetComponent<Rigidbody2D>();
        }
        ///////////////////////////////////////////////////////////////////////////////
    }

    // Update is called once per frame
    void Update()
    {
        if (laserTimer >= 0)
        {
            laserTimer += Time.deltaTime;
        }
        
        
        if (laserTimer >= 0 && laserTimer < fireInterval)
        {
            aimBool = true;
            fireBool = false;
            AimLaser();
        }

        else if (laserTimer >= fireInterval && laserTimer < fireInterval + fireDuration)
        {
            aimBool = false;
            fireBool = true;
            FireLaser();
        }
        else if (laserTimer > fireInterval + fireDuration)
        {
            laserTimer = 0;
        }
    }

    public void ActivateLaser()
    {
        aimBool = true;
        laserTimer = 0;
        aimRenderer.enabled = true;
    }

    public void StopLaser()
    {
        aimBool = false;
        fireBool = false;
        laserTimer = -1;
        aimRenderer.enabled = false;
    }

    private void AimLaser()
    {
        aimRenderer.SetPosition(0, sourcePoint.position);
        
        if (target == Target.Enemy || target == Target.Player)
        {
            UpdateTarget();
        }

        aimRenderer.startWidth = 0.03f;
        aimRenderer.endWidth = 0.03f;
        
        fireRenderer.enabled = false;
        aimRenderer.enabled = true;

        //Physics2D.IgnoreRaycastLayer;
        
        laserDirection = Quaternion.Euler(0f, 0f, laserAngle) * sourcePoint.right;
        //int layerMask = ~(1 << 2);
        hit = Physics2D.RaycastAll(transform.position, laserDirection);

        foreach (RaycastHit2D hitTarget in hit)
        {
            if (hitTarget.collider != null)
            {
                //Debug.Log(hitTarget.collider);
                if (hitTarget.collider.CompareTag("Wall") || hitTarget.collider.CompareTag("Obstacle"))
                {
                    endPoint = hitTarget.point;
                    aimRenderer.SetPosition(1, endPoint);
                    break;
                }
            }
        }
        
    }

    public void FireLaser()
    {

        fireRenderer.SetPosition(0, sourcePoint.position);
        
        fireRenderer.colorGradient = LaserColorGradient;
        fireRenderer.endWidth = 0.2f;
        fireRenderer.startWidth = 0.2f;
        aimRenderer.enabled = false;
        fireRenderer.enabled = true;
        fireRenderer.SetPosition(1, endPoint);
        
        foreach (RaycastHit2D hitTarget in hit)
        {
            if (hitTarget.collider != null)
            {
                Debug.Log(hitTarget.collider);
                if (hitTarget.collider.CompareTag("Player"))
                {
                    PlayerController player = hitTarget.collider.transform.GetComponent<PlayerController>();
                    player.TakeDamage(laserDamage);
                }
                else if (hitTarget.collider.CompareTag("Wall") || hitTarget.collider.CompareTag("Obstacle"))
                {
                    break;
                }
            }
        }
    }
    
    

    private void UpdateTarget()
    {
        /////////////////////////////////// test ////////////////////////////////////
        if (playerRigidbody2Ds.Length > 0 && laserTimer < fireInterval - 0.5)
        {
            Vector3 pos = playerRigidbody2Ds[0].position;
            pos.y += 0.5f;
            Vector2 dir = pos - sourcePoint.position;
            laserAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        }
        /////////////////////////////////////////////////////////////////////////////
    }
}
