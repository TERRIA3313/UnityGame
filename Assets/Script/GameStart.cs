using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStart : MonoBehaviour
{
    Player_Move Player;
    GameManager gameManager;
    Canvas canvas;

    private void Awake()
    {
        try
        {
            Player = Player_Move.FindObjectOfType<Player_Move>();
            gameManager = GameManager.FindObjectOfType<GameManager>();
            canvas = Canvas.FindObjectOfType<Canvas>();

            Destroy(Player);
            Destroy(gameManager);
            Destroy(canvas);
        }
        catch
        {
           
        }
        finally
        {
            SceneManager.LoadScene(1);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
