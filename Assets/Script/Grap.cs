using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Grap : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriteRenderer;
    BoxCollider2D collider;
    public Friend friend;
    public GameManager gameManager;

    public Player_Move player;
    public Transform target;
    Rigidbody2D PlayerRigid;
    public Vector2 direction;
    public float distance;
    public int nextX;
    public int nextY;

    float GrapTimer = 6f;

    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<BoxCollider2D>();
        gameManager = GameManager.FindObjectOfType<GameManager>();
        friend = Friend.FindObjectOfType<Friend>();
        player = Player_Move.FindObjectOfType<Player_Move>();
        target = Player_Move.FindObjectOfType<Player_Move>().transform;
        PlayerRigid = target.gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        direction = (target.position - transform.position).normalized;
        distance = Vector2.Distance(target.position, transform.position);

        if (direction.x < 0.0f) nextX = -1;
        else if (direction.x > 0.0f) nextX = 1;

        if (direction.y < 0.0f) nextY = -1;
        else if (direction.y > 0.0f) nextY = 1;

        if (GrapTimer > 0f)
        {
            GrapTimer -= Time.deltaTime;
            if (distance < 0.5)
            {
                rigid.position = new Vector2(target.position.x, -0.2f);
                PlayerRigid.constraints = RigidbodyConstraints2D.FreezePositionX;
            }
            else if (distance < 3)
            {
                rigid.velocity = new Vector2(nextX * 3, nextY * 3);
            }
            else if (distance < 7)
            {
                rigid.velocity = new Vector2(nextX * 4, nextY * 4);
            }
            else
            {
                rigid.velocity = new Vector2(nextX * 5, nextY * 5);
            }
        }
        else
        {
            spriteRenderer.color = new Color(1, 0, 0, 1);
            rigid.constraints = RigidbodyConstraints2D.FreezePosition;
            Invoke("Grapping", 0.8f);
        }
    }

    void Grapping()
    {
        anim.SetBool("Grap", true);
        if(distance <= 0.15f)
        {
            gameManager.Kill();
        }
        Invoke("DeActive", 0.3f);
    }

    void DeActive()
    {
        PlayerRigid.constraints = RigidbodyConstraints2D.None;
        PlayerRigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        gameObject.SetActive(false);
        friend.GrapAttackEnd();
    }
}
