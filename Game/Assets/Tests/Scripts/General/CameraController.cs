using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController CameraInstance;

    public float speed;

    public Transform target;

    //public Vector3 curPos;
    // Start is called before the first frame update
    void Awake()
    {
        CameraInstance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            var targetPos = target.position;
            Vector3 newPos = new Vector3(targetPos.x, targetPos.y, -10);
            //curPos = newPos;
            transform.position = Vector3.MoveTowards(transform.position, newPos, speed * Time.deltaTime);
        }
    }

    public void UpdateTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
