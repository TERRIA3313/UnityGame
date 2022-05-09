using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Friend : MonoBehaviour
{
    Rigidbody2D rigid;
    BoxCollider2D collider;
    public SpriteRenderer spriteRenderer;
    public Animator anim;
    public GameManager gameManager;
    public Player_Move player;

    public float BossTimer;
    public Text Timer;
    public Text[] Timers;

    bool Attacking;
    int RandomAttack;

    float FastAttackTimer = 0.0f;

    public GameObject[] Bottles;
    float BottleAttackTimer = 0.0f;
    int RandomNumber;
    int[,] P_List = new int[,] { { 0, 1 }, { 0, 2 }, { 1, 2 } };

    float[,] BottlePosition = new float[,] { { -2.09f, 14.5f }, { 2.81f, 14.5f }, { 7.66f, 14.5f } };
    public GameObject BigBottle;

    public GameObject ThrowBottle;
    float ThrowAttackTimer = 0.0f;
    int ThrowCounter = 4;

    float ParabolaThrowAttackTimer = 0.0f;

    public GameObject grap;

    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        collider = GetComponent<BoxCollider2D>();
        gameManager = GameManager.FindObjectOfType<GameManager>();
        player = Player_Move.FindObjectOfType<Player_Move>();
        BossTimer = 60f;
        Timer = gameManager.Timer;
        Timer.text = $"{BossTimer:N2}";
        Attacking = true;
        RandomAttack = -1;
        Invoke("BossStart", 3);
    }

    // Update is called once per frame
    void Update()
    {
        if (BossTimer <= 0)
        {
            if (Attacking == false)
            {
                GrapAttack();
                Attacking = true;
            }
        }
        else
        {
            BossTimer -= Time.deltaTime;
            Timer.text = $"{BossTimer:N2}";
        }

        if (Attacking == false)
        {
            if (RandomAttack == 0)
            {
                FastAttack();
                Attacking = true;
            }
            else if (RandomAttack == 1)
            {
                BottleAttack();
                Attacking = true;
            }
            else if (RandomAttack == 2)
            {
                ThrowAttack();
                Attacking = true;
            }
            else if (RandomAttack == 3)
            {
                ParabolaThrowAttack();
                Attacking = true;
            }
        }

        if (FastAttackTimer > 0f)
        {
            FastAttackTimer -= Time.deltaTime;
            if (0.0f < FastAttackTimer && FastAttackTimer <= 0.5f)
            {
                spriteRenderer.color = new Color(1, 0, 0, 1);
            }
            if (FastAttackTimer <= 0.0f)
            {
                rigid.AddForce(Vector2.right * 50, ForceMode2D.Impulse);
                Invoke("FastAttackEnd", 0.3f);
            }
        }

        if(BottleAttackTimer > 0f)
        {
            BottleAttackTimer -= Time.deltaTime;
            if (BottleAttackTimer <= 0.0f)
            {
                BottleAttackEnd();
            }
        }

        if(ThrowAttackTimer > 0f)
        {
            ThrowAttackTimer -= Time.deltaTime;
            if (ThrowAttackTimer <= 0.0f && ThrowCounter < 4)
            {
                ThrowAttack();
            }
            else if (ThrowAttackTimer <= 0.0f && ThrowCounter == 4)
            {
                ThrowAttackEnd();
            }
        }

        if(ParabolaThrowAttackTimer > 0f)
        {
            ParabolaThrowAttackTimer -= Time.deltaTime;
            if (ParabolaThrowAttackTimer <= 0.0f && ThrowCounter < 4)
            {
                ParabolaThrowAttack();
            }
            else if (ParabolaThrowAttackTimer <= 0.0f && ThrowCounter == 4)
            {
                ParabolaThrowAttackEnd();
            }
        }
    }

    void BossStart()
    {
        int overlap = Random.Range(0, 4);
        if (RandomAttack == -1) RandomAttack = overlap;
        else
        {
            if (overlap == RandomAttack)
            {
                RandomAttack = ReRoll(overlap);
            }
            else RandomAttack = overlap;
        }
        Attacking = false;
    }

    int ReRoll(int A)
    {
        int overlap = Random.Range(0, 4);
        if (A == overlap)
        {
            return overlap = ReRoll(overlap);
        }
        else return overlap;
    }

    void FastAttack()
    {
        anim.SetBool("Fast", true);
        FastAttackTimer = 3.0f;
    }

    void FastAttackEnd()
    {
        rigid.AddForce(Vector2.left * 50, ForceMode2D.Impulse);
        anim.SetBool("Fast", false);
        spriteRenderer.color = new Color(1, 1, 1, 1);
        Invoke("BossStart", 3);
    }

    void BottleAttack()
    {
        RandomNumber = Random.Range(0, 3);
        Bottles[P_List[RandomNumber, 0]].SetActive(true);
        Bottles[P_List[RandomNumber, 1]].SetActive(true);
        BottleAttackTimer = 2.0f;
    }

    void BottleAttackEnd()
    {
        Bottles[P_List[RandomNumber, 0]].SetActive(false);
        Bottles[P_List[RandomNumber, 1]].SetActive(false);
        Instantiate(BigBottle, new Vector3(BottlePosition[P_List[RandomNumber, 0], 0], BottlePosition[P_List[RandomNumber, 0], 1], 0), Quaternion.identity);
        Instantiate(BigBottle, new Vector3(BottlePosition[P_List[RandomNumber, 1], 0], BottlePosition[P_List[RandomNumber, 1], 1], 0), Quaternion.identity);
        Invoke("BossStart", 3);
    }

    void ThrowAttack()
    {
        if(ThrowCounter == 4)
        {
            ThrowCounter = 0;
        }
        ThrowAttackTimer = 1.4f;
        anim.SetBool("Throw", true);
        GameObject bullet = Instantiate(ThrowBottle, new Vector3(transform.position.x + 1f, transform.position.y + 0.5f , 0), transform.rotation);
        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
        rigid.AddForce(Vector2.right * 5, ForceMode2D.Impulse);
        ThrowCounter += 1;
    }

    void ThrowAttackEnd()
    {
        anim.SetBool("Throw", false);
        Invoke("BossStart", 3);
    }

    void ParabolaThrowAttack()
    {
        if (ThrowCounter == 4)
        {
            ThrowCounter = 0;
        }
        ParabolaThrowAttackTimer = 1.4f;
        anim.SetBool("Throw", true);
        GameObject bullet = Instantiate(ThrowBottle, new Vector3(transform.position.x + 1f, transform.position.y + 0.5f, 0), transform.rotation);
        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
        rigid.constraints = RigidbodyConstraints2D.None;
        float variable = Random.Range(0.0f, 4.0f);
        rigid.AddForce(Vector2.right * (6.5f - variable), ForceMode2D.Impulse);
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
        ThrowCounter += 1;
    }

    void ParabolaThrowAttackEnd()
    {
        anim.SetBool("Throw", false);
        Invoke("BossStart", 3);
    }

    void GrapAttack()
    {
        grap.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        grap.SetActive(true);
    }

    public void GrapAttackEnd()
    {
        if (player.IsDead == false) gameManager.NextStage();
    }
}
