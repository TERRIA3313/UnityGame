using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Door : MonoBehaviour
{
    public float FadeTime = 0.4f;
    public Image fadeImg;
    float start;
    float end;
    float time = 0f;
    bool isPlaying = false;

    public float[] coordinate;

    public Player_Move player;
    public GameManager gameManager;
    Animator anim;

    // Start is called before the first frame update

    private void Awake()
    {
        anim = GetComponent<Animator>();
        player = Player_Move.FindObjectOfType<Player_Move>();
        gameManager = GameManager.FindObjectOfType<GameManager>();
        fadeImg = gameManager.Fade;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            fadeImg.gameObject.SetActive(true);
            anim.SetTrigger("Open");
            Invoke("OutStartFadeAnim", 1f);
        }
    }

    public void OutStartFadeAnim()
    {
        if (isPlaying)
        {
            return;
        }
        start = 1f;
        end = 0f;
        StartCoroutine("fadeoutplay");
    }

    public void InStartFadeAnim()
    {
        player.transform.position = new Vector3(coordinate[0], coordinate[1], 0);
        player.VelocityZero();
        if (isPlaying)
        {
            return;
        }
        start = 0f;
        end = 1f;
        StartCoroutine("fadeIntanim");
    }

    IEnumerator fadeoutplay()
    {
        isPlaying = true;
        float fadeCount = 0f;
        while(fadeCount < 1.0f)
        {
            fadeCount += 0.01f;
            yield return new WaitForSeconds(0.01f);
            fadeImg.color = new Color(0, 0, 0, fadeCount);
        }
        isPlaying = false;
        Invoke("InStartFadeAnim", 0.5f);
    }

    IEnumerator fadeIntanim()
    {
        isPlaying = true;
        float fadeCount = 1f;
        while (fadeCount > 0f)
        {
            fadeCount -= 0.01f;
            yield return new WaitForSeconds(0.01f);
            fadeImg.color = new Color(0, 0, 0, fadeCount);
        }
        isPlaying = false;
        fadeImg.gameObject.SetActive(false);
    }
}
