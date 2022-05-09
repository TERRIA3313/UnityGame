using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Camera
    public Camera MainCamera;
    public Camera BossCamera;
    public Camera ChatCameara;
    public Canvas UI;

    //Object
    public Player_Move Player;
    public Friend friend;
    public GameObject[] MosterPrefab;
    public Image Fade;

    // Setting
    public float CreateTime;
    public int MaxMonster = 10;
    public float SpawnTimer;
    public Text Timer;

    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int health;
    public int CollectionItem;
    bool IsStage3 = false;
    bool IsStage3Boss = false;

    //UI
    public Image[] UIhealth;
    public Text UIPoint;
    public Text UIStage;
    public GameObject ReStartButton;

    public Image Must;
    public Text Have;
    public Text Slash;
    public Text Need;

    public GameObject Talking;
    public Text TalkText;
    bool isTalking = false;

    public Image TextBackground;
    public Text Quiz;
    public InputField inputField;
    public GameObject Submit;
    List<string> QuizText = new List<string>();

    public Text LastQuiz;
    public GameObject Boss;
    public Camera LastBossCamera;
    List<string> LastQuizText = new List<string>();
    int number;
    int Clear = 0;

    public GameObject NextStageButton;

    public IEnumerator coroutine;

    void Awake()
    {
        Time.timeScale = 1f;
        Application.targetFrameRate = 60;

        coroutine = Respawn();

        GameStart();
    }

    void Update()
    {
        UIPoint.text = (totalPoint + stagePoint).ToString();
        Have.text = CollectionItem.ToString();
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();

        if(stageIndex == 1 && SpawnTimer > 0.0f)
        {
            SpawnTimer -= Time.deltaTime;
            Timer.text = $"{SpawnTimer:N2}";
        }

        if(stageIndex == 1 && SpawnTimer <= 0.0f)
        {
            StopCoroutine(coroutine);
            NextStage();
        }

        if(IsStage3)
        {
            CreateTime -= Time.deltaTime;
            Timer.text = $"{CreateTime:N2}";

            // Time Over
            if (CreateTime <= 0)
            {
                HealthDown();
                CreateTime = 20f;
            }

            if (QuizText.Count <= 0) Exam();
        }

        if(IsStage3Boss)
        {
            CreateTime -= Time.deltaTime;
            Timer.text = $"{CreateTime:N2}";

            // Time Over
            if (CreateTime <= 0)
            {
                Kill();
            }
        }
    }

    public void Kill()
    {
        while (health > 0)
        {
            HealthDown();
        }
    }

    public void PlayerMove(float x, float y, float z)
    {
        Player.transform.position = new Vector3(x, y, z);
        Player.VelocityZero();
    }

    public void GameStart()
    {
        SceneManager.LoadScene(1);
        DontDestroyOnLoad(this);
        DontDestroyOnLoad(Player);
        DontDestroyOnLoad(UI);
        PlayerMove(38.5f, -5.5f, 0);
    }

    public void NextStage()
    {
        // 스테이지 전환
        if (stageIndex < 5)
        {
            Time.timeScale = 0;
            NextStageButton.SetActive(true);
            Text NextBtnText = GameObject.Find("Placeholder").GetComponentInChildren<Text>();

            string Grade;
            if ((stageIndex + 1) % 2 == 1)
            {
                int Money = GameObject.Find("Bronzes").transform.childCount;
                int Beer = GameObject.Find("Beers").transform.childCount;
                int Controller = GameObject.Find("Controllers").transform.childCount;
                int Mic = GameObject.Find("Mics").transform.childCount;
                float final = Money + Beer + Controller + Mic;

                if (final <= (stagePoint / 100) * 0.9) Grade = "A+";
                else if (final <= (stagePoint / 100) * 0.8) Grade = "A";
                else if (final <= (stagePoint / 100) * 0.7) Grade = "B+";
                else if (final <= (stagePoint / 100) * 0.6) Grade = "B";
                else if (final <= (stagePoint / 100) * 0.5) Grade = "C+";
                else if (final <= (stagePoint / 100) * 0.4) Grade = "C";
                else if (final <= (stagePoint / 100) * 0.3) Grade = "D+";
                else if (final <= (stagePoint / 100) * 0.2) Grade = "D";
                else Grade = "F";
            }
            else Grade = "A+";

            NextBtnText.text = "당신의 성적 : " + Grade;
        }
        else
        {
            Player.VelocityZero();
            // 시간 정지
            Time.timeScale = 0;
            // 리스타트 버튼 UI
            Text btnText = ReStartButton.GetComponentInChildren<Text>();
            btnText.text = "Clear!";
            ReStartButton.SetActive(true);
        }

        // 포인트 계산
        totalPoint += stagePoint;
        stagePoint = 0;

    }

    public void HealthDown()
    {
        if (health > 0)
        {
            health--;
            UIhealth[health].color = new Color(1, 0, 0, 4f);
        }
        else
        {
            // 플레이어 사망 함수
            Player.OnDie();

            // 결과 UI

            // 재도전 버튼 UI
            ReStartButton.SetActive(true);
        }
    }

    public void Restart()
    {
        if(health == 0)
        {
            stagePoint = 0;
            health = 3;
            UIhealth[0].color = new Color(1, 1, 1, 1);
            UIhealth[1].color = new Color(1, 1, 1, 1);
            UIhealth[2].color = new Color(1, 1, 1, 1);
            Player.Resurrection();
            ReStartButton.SetActive(false);

            if (IsStage3) IsStage3 = false;
            if (IsStage3Boss) IsStage3Boss = false;
            if (isTalking) isTalking = false;

            SceneManager.LoadScene(stageIndex + 1);
            if (stageIndex == 5)
            {
                UI.worldCamera = BossCamera;
                BossCamera.gameObject.SetActive(false);
                Timer.gameObject.SetActive(false);
                UIhealth[0].gameObject.SetActive(false);
                UIhealth[1].gameObject.SetActive(false);
                UIhealth[2].gameObject.SetActive(false);
            }
        }
        else
        {
            // Setting
            stagePoint = 0;
            totalPoint = 0;
            stageIndex = 0;
            health = 3;
            UIhealth[0].color = new Color(1, 1, 1, 1);
            UIhealth[1].color = new Color(1, 1, 1, 1);
            UIhealth[2].color = new Color(1, 1, 1, 1);
            Time.timeScale = 1f;
            SpawnTimer = 60f;
            Clear = 0;

            // UI
            Must.gameObject.SetActive(true);
            Have.gameObject.SetActive(true);
            Slash.gameObject.SetActive(true);
            Need.gameObject.SetActive(true);
            UIPoint.gameObject.SetActive(true);

            // Player
            Player.transform.localScale = new Vector3(1, 1, 1);
            Player.PlayerRigid();
            PlayerMove(38.5f, -5.5f, 0);

            ReStartButton.SetActive(false);
            SceneManager.LoadScene(1);
        }
    }

    public void Next()
    {
        NextStageButton.SetActive(false);
        stageIndex++;
        health = 3;
        UIhealth[0].color = new Color(1, 1, 1, 1);
        UIhealth[1].color = new Color(1, 1, 1, 1);
        UIhealth[2].color = new Color(1, 1, 1, 1);
        SceneManager.LoadScene(stageIndex + 1);

        if ((stageIndex + 1) % 2 == 1)
        {
            UIStage.text = "Normal STAGE";
            Timer.gameObject.SetActive(false);
            UI.worldCamera = MainCamera;
        }
        else UIStage.text = "BOSS STAGE";

        Time.timeScale = 1.0f;
        Player.HangCounter = 0.1f;
    }

    IEnumerator Respawn()
    {
        while(SpawnTimer > 0)
        {
            int MonsterCount = (int)GameObject.FindGameObjectsWithTag("Enemy").Length;

            if (MonsterCount < MaxMonster)
            {
                yield return new WaitForSeconds(CreateTime);
                int RandomPoint = Random.Range(0, 2);
                int RandomEnemy = Random.Range(0, 5);
                if (RandomEnemy == 4) RandomEnemy = 2;
                else if (RandomEnemy == 3) RandomEnemy = 1;
                else RandomEnemy = 0;
                if (RandomPoint == 0) Instantiate(MosterPrefab[RandomEnemy], new Vector3(28, -1, 136.3865f), Quaternion.identity);
                else Instantiate(MosterPrefab[RandomEnemy], new Vector3(46, -1, 136.3865f), Quaternion.identity);
            }
            else yield return null;
        }
    }

    public void TalkManager()
    {
        StartCoroutine(Talk());
    }

    public void Stage3()
    {
        // Main Setting
        UIhealth[0].gameObject.SetActive(true);
        UIhealth[1].gameObject.SetActive(true);
        UIhealth[2].gameObject.SetActive(true);
        Talking.SetActive(false);
        BossCamera.gameObject.SetActive(true);
        UI.worldCamera = BossCamera;
        Timer.gameObject.SetActive(true);
        Timer.color = new Color(0, 0, 0, 1);
        IsStage3 = true;
        CreateTime = 20f;

        QuizText = new List<string>();

        QuizText.Add("가상현실은 컴퓨터로 만들어 놓은 가상의 세계에서 사람이 실제와 같은 체험을 할 수 있도록 하는 기술이다.");
        QuizText.Add("사물인터넷은 생활 속 사물들을 유무선 네트워크로 연결해 정보를 공유하는 환경을 말한다.");
        QuizText.Add("인공지능은 인간의 두뇌작용과 같이 컴퓨터 스스로 추론, 학습, 판단하면서 작업하는 시스템이다.");
        QuizText.Add("4차 산업혁명이란 정보통신기술의 융합으로 이뤄지는 차세대 산업혁명이다.");
        QuizText.Add("메타버스는 현실세계와 같은 사회, 경제, 문화 활동이 이뤄지는 3차원 가상세계를 말한다.");
        QuizText.Add("블록체인은 분산 컴퓨팅 기술 기반의 데이터 위변조 방지 기술이다.");
        QuizText.Add("크라우드 펀딩은 프로젝트의 아이디어를 온라인에 알려 자금을 모집하는 방식이다.");
        QuizText.Add("드론은 조종사가 비행체에 탑승하지않고 원격 조종하는 항공시스템이다.");
        QuizText.Add("증강현실은 실제 세계에 가상 물체를 겹쳐 보여주는 기술이다.");

        // UI DeActive
        Must.gameObject.SetActive(false);
        Have.gameObject.SetActive(false);
        Slash.gameObject.SetActive(false);
        Need.gameObject.SetActive(false);
        UIPoint.gameObject.SetActive(false);

        // Player Scale
        Player.transform.localScale = new Vector3(2, 2, 1);

        int RandomNumber = Random.Range(0, QuizText.Count);
        Quiz.text = QuizText[RandomNumber];
        QuizText.RemoveAt(RandomNumber);
    }

    public void Correct()
    {
        if (inputField.text == Quiz.text)
        {
            CreateTime = 20f;
            int RandomNumber = Random.Range(0, QuizText.Count);
            Quiz.text = QuizText[RandomNumber];
            QuizText.RemoveAt(RandomNumber);
        }
        else
        {
            HealthDown();
        }
    }

    public void Exam()
    {
        IsStage3 = false;
        Talking.SetActive(true);
        Timer.gameObject.SetActive(false);
        TalkText.text = "";
        StartCoroutine(Talk2());
    }

    public void Stage3Boss()
    {
        // Main Setting
        Talking.SetActive(false);
        BossCamera.gameObject.SetActive(false);
        Scenes scene = Scenes.FindObjectOfType<Scenes>();
        Boss = scene.LastBoss;
        Boss.SetActive(true);
        LastQuiz = scene.LastQuiz;
        LastBossCamera = scene.LastBossCamera;
        UI.worldCamera = LastBossCamera;
        Timer.color = new Color(1, 0, 0, 1);
        IsStage3Boss = true;
        CreateTime = 20f;

        LastQuizText = new List<string>();

        LastQuizText.Add("현실세계와 같은 사회, 경제, 문화 활동이 이뤄지는 3차원 가상세계를 가상현실이라 한다.");
        LastQuizText.Add("생활 속 사물들을 유무선 네트워크로 연결해 정보를 공유하는 환경을 사물 인터넷이라 한다.");
        LastQuizText.Add("인공지능은 인간의 두뇌작용과 같이 컴퓨터 스스로 추론, 학습, 판단하면서 작업하는 시스템이다.");
        LastQuizText.Add("4차 산업혁명이란 전기 에너지를 기반으로 한 대량생산의 산업혁명이다.");
        LastQuizText.Add("현실세계와 같은 사회, 경제, 문화 활동이 이뤄지는 3차원 가상세계를 메타버스라 한다.");
        LastQuizText.Add("블록체인은 암호화폐를 위한 기술일 뿐, 그 이상도 아니다.");
        LastQuizText.Add("크라우드 펀딩은 프로젝트의 아이디어를 온라인에 알려 자금을 모집하는 방식이다.");
        LastQuizText.Add("드론은 조종사가 비행체에 탑승하지않고 원격 조종하는 항공시스템이다.");
        LastQuizText.Add("증강현실은 가상 세계에 실제 물체를 겹쳐 보여주는 기술이다.");

        number = Random.Range(0, LastQuizText.Count);
        LastQuiz.text = LastQuizText[number];
        LastQuizText.RemoveAt(number);
    }

    public void O_Submit()
    {
        Check(0);
    }

    public void X_Submit()
    {
        Check(1);
    }

    public void Check(int A)
    {
        int[] Answer = new int[10];
        Answer[0] = 1;
        Answer[1] = 0;
        Answer[2] = 0;
        Answer[3] = 1;
        Answer[4] = 0;
        Answer[5] = 1;
        Answer[6] = 0;
        Answer[7] = 0;
        Answer[8] = 0;
        Answer[9] = 0;

        if(A == Answer[number])
        {
            if (Clear < 3)
            {
                number = Random.Range(0, LastQuizText.Count);
                LastQuiz.text = LastQuizText[number];
                LastQuizText.RemoveAt(number);
                Clear++;
            }
            else
            {
                NextStage();
            }

        }
        else
        {
            HealthDown();
            HealthDown();
            HealthDown();
            HealthDown();
        }
    }


    IEnumerator Talk()
    {
        string Sentence = "다왔니?\n\n자, 오늘 수업은 4차 산업혁명에 관해서 공부할거다.\n\n수업 끝나면 쪽지 시험도 칠 예정이니까 열심히 듣고.";

        for (int i = 0; i < Sentence.Length; ++i)
        {
            TalkText.text += Sentence[i];
            yield return new WaitForSeconds(0.1f);
        }
        Invoke("Stage3", 1f);
    }

    IEnumerator Talk2()
    {
        string Sentence = "자, 오늘 수업은 여기 까지고.\n\n이제 오늘 배운 내용으로 쪽지 시험을 칠거다.\n\n아무 생각 없이 필기한게 아니라 재대로 공부했으면 충분히 알 수 있는 문제니까 걱정하지말고.";

        for (int i = 0; i < Sentence.Length; ++i)
        {
            TalkText.text += Sentence[i];
            yield return new WaitForSeconds(0.1f);
        }
        Invoke("Stage3Boss", 1f);
    }
}
