using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scenes : MonoBehaviour
{
    public GameManager gameManager;
    public Player_Move player;
    public bool IsBoss;

    // Stage 3 Boss
    public Text TalkingText;
    public GameObject talking;
    public Camera BossCamera;
    public Text Quiz;
    public InputField inputField;
    public GameObject Submit;

    public GameObject LastBoss;
    public Camera LastBossCamera;
    public Text LastQuiz;

    // Start is called before the first frame update
    void Awake()
    {
        gameManager = GameManager.FindObjectOfType<GameManager>();
        player = Player_Move.FindObjectOfType<Player_Move>();
        if (GameObject.Find("Golds").transform.childCount != 1) gameManager.Need.text = "" + GameObject.Find("Golds").transform.childCount;
        else gameManager.Need.text = "0";
        if (IsBoss)
        {
            if (gameManager.stageIndex == 1)
            {
                gameManager.BossCamera = Camera.FindObjectsOfType<Camera>()[1];
                gameManager.Timer.gameObject.SetActive(true);
                gameManager.UI.worldCamera = gameManager.BossCamera;
                gameManager.SpawnTimer = 60f;
                StartCoroutine(gameManager.coroutine);
            }
            else if (gameManager.stageIndex == 3)
            {
                gameManager.BossCamera = Camera.FindObjectsOfType<Camera>()[1];
                gameManager.Timer.gameObject.SetActive(true);
                gameManager.UI.worldCamera = gameManager.BossCamera;
                Friend friend = Friend.FindObjectOfType<Friend>();
                friend.Timer = gameManager.Timer;
                gameManager.PlayerMove(0, 0, 0);
            }
            else if (gameManager.stageIndex == 5)
            {
                gameManager.Timer.gameObject.SetActive(false);
                gameManager.PlayerMove(-6, -3, 0);
                Rigidbody2D PlayerRigid = player.GetComponent<Rigidbody2D>();
                PlayerRigid.constraints = RigidbodyConstraints2D.FreezePosition;
                gameManager.TalkText = TalkingText;
                gameManager.Talking = talking;
                gameManager.BossCamera = BossCamera;
                gameManager.TalkManager();
                gameManager.Quiz = Quiz;
                gameManager.inputField = inputField;
                gameManager.Submit = Submit;
            }
        }
        else
        {
            gameManager.UI.worldCamera = gameManager.MainCamera;
            gameManager.Timer.gameObject.SetActive(false);
            if (gameManager.stageIndex == 0)
            {
                gameManager.PlayerMove(38, -6.5f, 0);
            }
            else if (gameManager.stageIndex == 2)
            {
                gameManager.PlayerMove(37f, -6f, 0);
            }
            else if (gameManager.stageIndex == 4)
            {
                gameManager.PlayerMove(38, -6f, 0);
                player.VelocityZero();
            }
        }
    }

    public void TextSubmit()
    {
        gameManager.Correct();
    }

    public void O_ButtonSubmit()
    {
        gameManager.O_Submit();
    }

    public void X_ButtonSubmit()
    {
        gameManager.X_Submit();
    }
}
