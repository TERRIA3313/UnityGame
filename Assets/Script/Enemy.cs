using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //시스템
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriteRenderer;
    BoxCollider2D collider;
    //이동
    public Transform target;
    public Vector2 direction;
    public float distance;
    public int nextMove;

    public int Enemy_Type;

    public bool NearPlayer;
    public bool IsLive;
    public bool IsCliff;
    public bool IsAttacking;

    public float CoolTime;

    public GameObject Mike_bullet;
    public GameObject Controller_Attack;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<BoxCollider2D>();
        IsLive = true;
        NearPlayer = false;
        IsCliff = false;
        CoolTime = 0.1f;
        IsAttacking = false;
        target = Player_Move.FindObjectOfType<Player_Move>().transform;
    }

    void Update()
    {
        // 걷기 판정
        if (nextMove == 0)
            anim.SetBool("IsWalking", false);
        else
        {
            anim.SetBool("IsWalking", true);
        }

        direction = (target.position - transform.position).normalized;
        distance = Vector2.Distance(target.position, transform.position);

        if (Math.Abs(distance) <= 12.0f) NearPlayer = true;
        else NearPlayer = false;

        if (NearPlayer && Enemy_Type == 0) Move();

        else if (NearPlayer && Enemy_Type == 1 && IsLive)
        {
            if (Math.Abs(distance) <= 3.0f && CoolTime <= 0.0f && rigid.velocity.y >= -0.1f)
            {
                spriteRenderer.color = new Color(1, 0, 0, 1);
                IsAttacking = true;
                nextMove = 0;
                Invoke("Attack", 0.8f);
                Invoke("ChangeColor", 0.8f);
                CoolTime = 4f;
            }
            else if (Math.Abs(distance) <= 3.0f && CoolTime > 0.0f)
            {
                nextMove = 0;
                CoolTime -= Time.deltaTime;
            }
            else
            {
                if (CoolTime >= 0.0f) CoolTime -= Time.deltaTime;
                ChangeAttackMode();
                Move();
            }
        }

        else if (NearPlayer && Enemy_Type == 2 && IsLive)
        {
            if (Math.Abs(distance) <= 7.0f && CoolTime <= 0.0f && rigid.velocity.y >= -0.1f)
            {
                spriteRenderer.color = new Color(1, 0, 0, 1);
                IsAttacking = true;
                nextMove = 0;
                Invoke("Fire", 0.8f);
                Invoke("ChangeColor", 0.8f);
                CoolTime = 3f;
            }
            else if (Math.Abs(distance) <= 7.0f && CoolTime > 0.0f)
            {
                nextMove = 0;
                CoolTime -= Time.deltaTime;
            }
            else
            {
                if (CoolTime >= 0.0f) CoolTime -= Time.deltaTime;
                ChangeAttackMode();
                Move();
            }
        }

        // 좌우 반전
        if (nextMove != 0)
            spriteRenderer.flipX = nextMove == 1;

        // 바닥 체크
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.3f, rigid.position.y);
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 3, LayerMask.GetMask("Platform"));

        if (rayHit.collider == null) IsCliff = true;
        else IsCliff = false;

    }

    public void OnDamaged()
    {
        //죽음 설정
        IsLive = false;

        // 알파값 변경
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        // 뒤집기
        spriteRenderer.flipY = true;

        // 콜라이더 끄기
        collider.enabled = false;

        // 점프 모션
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        // 제거
        Invoke("DeActive", 5);
    }

    void Move()
    {
        if (IsCliff || IsLive == false || IsAttacking) nextMove = 0;
        else
        {
            if (NearPlayer && direction.x < 0.0f) nextMove = -1;
            else if (NearPlayer && direction.x > 0.0f) nextMove = 1;
            else nextMove = 0;

            rigid.velocity = new Vector2(nextMove, rigid.velocity.y);
        }
    }

    void DeActive()
    {
        gameObject.SetActive(false);
    }

    void Attack()
    {
        if(IsLive)
        {
            anim.SetBool("IsAttacking", true);
            IsAttacking = true;
            GameObject Melee_Attack = Instantiate(Controller_Attack, transform.position, transform.rotation);
        }
    }

    void Fire()
    {
        if(IsLive)
        {
            anim.SetBool("IsAttacking", true);
            GameObject bullet = Instantiate(Mike_bullet, transform.position, transform.rotation);
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
            if (direction.x < 0.0f) rigid.AddForce(Vector2.left * 3, ForceMode2D.Impulse);
            else rigid.AddForce(Vector2.right * 3, ForceMode2D.Impulse);
        }
    }

    void ChangeColor()
    {
        spriteRenderer.color = new Color(1, 1, 1, 1);
        Invoke("ChangeAttackMode", 0.5f);
    }

    void ChangeAttackMode()
    {
        IsAttacking = false;
        anim.SetBool("IsAttacking", false);
    }
}
