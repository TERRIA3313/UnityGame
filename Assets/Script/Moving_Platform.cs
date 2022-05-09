using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moving_Platform : MonoBehaviour
{
    public int PlatformType;
    public int MoveFlag = 1;
    public float MoveSpeed = 3;
    public float MoveTime;
    float MovePower = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("BlockMove");
    }

    private void LateUpdate()
    {
        Move();
    }

    void Move()
    {
        Vector3 MoveVelocity = Vector3.zero;

        if(PlatformType == 1)
        {
            if (this.MoveFlag == 1)
            {
                MoveVelocity = new Vector3(MovePower, 0, 0);
            }
            else
            {
                MoveVelocity = new Vector3(-MovePower, 0, 0);
            }
            transform.position += MoveVelocity * MoveSpeed * Time.deltaTime;
        }
        else
        {
            if (this.MoveFlag == 1)
            {
                MoveVelocity = new Vector3(0, -MovePower, 0);
            }
            else
            {
                MoveVelocity = new Vector3(0, MovePower, 0);
            }
            transform.position += MoveVelocity * MoveSpeed * Time.deltaTime;
        }

    }

    IEnumerator BlockMove()
    {
        if(MoveFlag == 1)
        {
            MoveFlag = 2;
        }
        else
        {
            MoveFlag = 1;
        }

        yield return new WaitForSeconds(MoveTime);
        StartCoroutine("BlockMove");
    }
}
