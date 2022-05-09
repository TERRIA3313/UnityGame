using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Character_Select : MonoBehaviour
{
    private RaycastHit2D hit;

    public GameObject[] Players;
    public SpriteRenderer BoySprite;
    public SpriteRenderer GirlSprite;
    public string SelectedPlayer;

    // Start is called before the first frame update
    private void Awake()
    {
        SelectedPlayer = "Don't";
        BoySprite = Players[0].GetComponent<SpriteRenderer>();
        GirlSprite = Players[1].GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {

            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            hit = Physics2D.Raycast(pos, Vector2.zero);

            if(hit.collider != null)
            {
                if(hit.collider.gameObject.name == "Boy")
                {
                    BoySprite.color = new Color(1, 1, 1, 1);
                    GirlSprite.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                    SelectedPlayer = "Boy";
                }

                else
                {
                    GirlSprite.color = new Color(1, 1, 1, 1);
                    BoySprite.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                    SelectedPlayer = "Girl";
                }
            }
        }
    }

    public void OnClick()
    {
        if(SelectedPlayer != "Don't")
        {
            SceneManager.LoadScene("SampleScene");
            DontDestroyOnLoad(gameObject);
        }
    }
}
