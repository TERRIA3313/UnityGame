using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fallen : MonoBehaviour
{
    public float[] Coordinate;
    public GameManager gameManager;

    private void Awake()
    {
        gameManager = GameManager.FindObjectOfType<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            // 위치 복귀
            if (gameManager.health >= 1)
                PlayerReposition();

            // 체력 감소
            gameManager.HealthDown();
        }
    }

    void PlayerReposition()
    {
        gameManager.Player.transform.position = new Vector3(Coordinate[0], Coordinate[1], 0);
        gameManager.Player.VelocityZero();
    }
}
