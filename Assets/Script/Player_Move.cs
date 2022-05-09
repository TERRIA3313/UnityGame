using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Move : MonoBehaviour
{
    public float maxSpeed;
    public float jumpPower;
    Rigidbody2D rigid;
    public SpriteRenderer spriteRenderer;
    public Animator anim;
    BoxCollider2D collider;
    public GameManager gameManager;
    public float HangTime = 0.2f;
    public float HangCounter = 0.2f;
    public bool IsDead = false;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        collider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        // Jump
        if (Input.GetButtonDown("Jump") && !anim.GetBool("IsJumping") && HangCounter > 0f)
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            anim.SetBool("IsJumping", true);
        }

        if (Input.GetButtonUp("Jump") && rigid.velocity.y > 0)
        {
            rigid.velocity = new Vector2(rigid.velocity.x, rigid.velocity.y * 0.5f);
        }

        if (anim.GetBool("IsJumping") == true && rigid.velocity.y < 0)
        {
            anim.SetBool("Down", true);
        }

        //Stop Speed
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.x * 0.5f, rigid.velocity.y);
        }

        if (Input.GetButton("Horizontal") == false)
        {
            rigid.velocity = new Vector2(0, rigid.velocity.y);
        }

        //Direction Sprite
        if (Input.GetButton("Horizontal"))
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;

        if (Input.GetButtonUp("Horizontal") && rigid.velocity.x > 0)
        {
            rigid.velocity = new Vector2(rigid.velocity.x * 0.5f, rigid.velocity.y);
        }

        //Walk Animation
        if (rigid.velocity.normalized.x == 0)
            anim.SetBool("IsWalking", false);
        else
        {
            anim.SetBool("IsWalking", true);
        }

        //Move By Key Control
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        //Speed limit
        if (rigid.velocity.x > maxSpeed)
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        else if (rigid.velocity.x < maxSpeed * (-1))
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);

        //Landing Platfrom
        if (rigid.velocity.y <= 0)
        {
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));
            rigid.AddForce(Vector3.down * 30f);
            if (rayHit.collider != null)
            {
                if (rayHit.distance < 0.9f)
                {
                    anim.SetBool("IsJumping", false);
                    anim.SetBool("Down", false);
                    HangCounter = HangTime;
                }

            }
            else
            {
                if (HangCounter >= 0)
                {
                    HangCounter -= Time.deltaTime;
                }
            }
        }

        if (rigid.velocity == Vector2.zero)
        {
            HangCounter = 0.2f;
            anim.SetBool("Down", false);
            anim.SetBool("IsJumping", false);
        }    
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Item")
        {
            // 점수 획득
            bool isBronze = collision.gameObject.name.Contains("Bronze");
            bool isSilver = collision.gameObject.name.Contains("Silver");
            bool isGold = collision.gameObject.name.Contains("Gold");

            if (isBronze)
                gameManager.stagePoint += 100;
            else if (isSilver)
                gameManager.stagePoint += 50;
            else if (isGold)
            {
                gameManager.CollectionItem += 1;
            }

            // 아이템 삭제
            collision.gameObject.SetActive(false);
        }
        else if (collision.gameObject.tag == "Finish")// && gameManager.Have.text == gameManager.Need.text)
        {
            // 다음 스테이지
            gameManager.NextStage();
        }

        if (collision.gameObject.tag == "Attack")
        {
            OnDamaged(collision.transform.position);
        }

        if (collision.gameObject.tag == "Up")
        {
            rigid.velocity = new Vector2(rigid.velocity.x, 0);
            rigid.AddForce(Vector2.up * 65, ForceMode2D.Impulse);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            bool isSpikes = collision.gameObject.name.Contains("spikes");

            if (rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y + 0.3f)
            {
                OnAttack(collision.transform);
            }
            else
                OnDamaged(collision.transform.position);
        }
        if ( collision.gameObject.tag == "Friend")
        {
            OnDamaged(collision.transform.position);
        }
    }

    void OnAttack(Transform enemy)
    {
        gameManager.stagePoint += 100;
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
        anim.SetBool("Down", false);
        anim.SetTrigger("ReJumping");
        Enemy enemyMove = enemy.GetComponent<Enemy>();
        enemyMove.OnDamaged();
    }

    void OnDamaged(Vector2 targetPos)
    {
        // 체력 감소
        gameManager.HealthDown();

        // 무적 레이어로 변환
        gameObject.layer = 10;

        // 알파값 변경
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        // 피격모션
        int dirc = transform.position.x - targetPos.x > 0? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1) * 7 ,ForceMode2D.Impulse);

        // 트리거
        anim.SetTrigger("DoDamaged");

        Invoke("OffDamaged", 1);
    }

    void OffDamaged()
    {
        // 무적 레이어로 변환
        gameObject.layer = 7;

        // 알파값 변경
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    public void OnDie()
    {
        // 알파값 변경
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        // 뒤집기
        spriteRenderer.flipY = true;

        // 콜라이더 끄기
        collider.enabled = false;

        // 점프 모션
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        IsDead = true;
    }

    public void Resurrection()
    {
        spriteRenderer.color = new Color(1, 1, 1, 1);
        spriteRenderer.flipY = false;
        collider.enabled = true;
        IsDead = false;
    }

    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }

    public void PlayerRigid()
    {
        rigid.constraints = RigidbodyConstraints2D.None;
        rigid.constraints =  RigidbodyConstraints2D.FreezeRotation;
    }
}
