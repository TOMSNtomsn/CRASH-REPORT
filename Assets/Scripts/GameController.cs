using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameParameter
{
    public int stageNumber;
    public int StageNum = 5;

    public bool gameStart;
    public bool gameOver;
    public bool gameClear;

    float delay;

    public float Delay => delay;
    public bool GameEnd => gameOver || gameClear;
    public bool Pause => Time.timeScale == 0;

    public GameParameter()
    {
        OnExitMainScene();
    }

    public void Init()
    {
        gameStart = false;
        gameOver = false;
        gameClear = false;

        SetDefaultFrameRate();
        SetDelay(0);
    }

    public void SetFrameRate(int frameRate)
    {
        Application.targetFrameRate = frameRate;
    }

    public void SetDefaultFrameRate()
    {
        Application.targetFrameRate = 60;
    }

    public void SetDelay(float delay)
    {
        this.delay = delay;
    }

    public void SetPause(bool pause)
    {
        Time.timeScale = pause ? 0 : 1;
    }

    public void OnExitMainScene()
    {
        SetDefaultFrameRate();
        SetPause(false);
        SetDelay(0);
        gameStart = true; // タイトルシーンのオムちゃんを動かすためにtrue
        gameOver = false;
        gameClear = false;
    }
}

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerLifeController lifeController;
    [SerializeField] Image gameoverUI;
    [SerializeField] GameObject[] restartSelects;
    [SerializeField] Animator clearUI;    
    [SerializeField] TextMeshProUGUI enemyCountText;
    [SerializeField] InformationWindow informationWindow;
    [SerializeField] TextMeshProUGUI countdownText;
    [SerializeField] GameObject optionWindow;
    [SerializeField] GlitchFx glitchFx;
 
    List<EnemyType> enemyTypes;

    int enemyCount;
    bool firstOpenWindow = true;
    int countdown;

    public static GameParameter GameParameter;

    void Awake()
    {
        lifeController.AddOnLifeChanged(OnLifeChanged);

        GameParameter.Init();

        enemyCount = 0;
        enemyTypes = new();
        countdown = 3;
    }

    void Start()
    {
        countdown++;
        InvokeRepeating(nameof(Countdown), 0, 1);
    }

    void Countdown()
    {
        countdown--;

        if (countdown == -1)
        {
            countdownText.gameObject.SetActive(false);
            CancelInvoke();
        }
        else if (countdown == 0)
        {
            countdownText.text = "Start!";
            SoundManager.Instance.PlaySE("CountGo");
            GameParameter.gameStart = true;
            LogManager.Instance.Register(LogType.Start);
        }
        else
        {
            countdownText.text = countdown.ToString();
            SoundManager.Instance.PlaySE("CountNum");
        }
    }

    void OnLifeChanged(int life)
    {
        if (life == 0)
        {
            OnGameOver();
        }
    }

    void OnGameOver()
    {
        if (GameParameter.GameEnd) return;

        GameParameter.gameOver = true;

        SoundManager.Instance.PlaySE("GameOver");
        LogManager.Instance.Register(LogType.GameOver);

        gameoverUI.gameObject.SetActive(true);
        informationWindow.gameObject.SetActive(false);
        optionWindow.SetActive(false);

        StartCoroutine(GameOverCoroutine());
    }

    IEnumerator GameOverCoroutine()
    {
        float progress = 0;

        while (progress < 1)
        {
            progress += Time.deltaTime / 3f;
            glitchFx.intensity = 2 * progress;

            gameoverUI.color = Color.Lerp(Color.clear, Color.black, (progress - 0.25f) / 0.75f);

            yield return null;
        }
    }

    void OnClear()
    {
        if (GameParameter.GameEnd) return;

        GameParameter.gameClear = true;

        SoundManager.Instance.PlaySE("Clear");
        LogManager.Instance.Register(LogType.Clear);
        SaveData.Instance.StageClear(GameParameter.stageNumber);

        clearUI.gameObject.SetActive(true);
        clearUI.SetBool("isNormal", GameParameter.stageNumber < 5);
        if(GameParameter.stageNumber == 5) SoundManager.Instance.PlaySE("Congratulations");

        informationWindow.gameObject.SetActive(false);
        optionWindow.SetActive(false);
    }

    void Update()
    {
        if (!GameParameter.gameStart) return;

        if (GameParameter.gameOver)
        {
            List<KeyCode> keys = new();
            keys.AddRange(PlayerKey.RightKeys);
            keys.AddRange(PlayerKey.LeftKeys);

            foreach (KeyCode key in keys)
            {
                if (Input.GetKeyDown(key))
                {
                    restartSelects[0].SetActive(!restartSelects[0].activeSelf);
                    restartSelects[1].SetActive(!restartSelects[1].activeSelf);
                }
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (restartSelects[0].activeSelf) RestartYes();
                else RestartNo();
            }
        }
        else if (GameParameter.gameClear)
        {

        }
        else if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (optionWindow.activeSelf) return;

            SoundManager.Instance.PlaySE("DisplayEnemiesDec");
            if (firstOpenWindow)
            {
                informationWindow.Init(enemyTypes);
                firstOpenWindow = false;
            }

            informationWindow.gameObject.SetActive(!informationWindow.gameObject.activeSelf);

            if (informationWindow.gameObject.activeSelf)
            {
                LogManager.Instance.Register(LogType.Information);
                GameParameter.SetPause(true);
            }
            else
            {
                GameParameter.SetPause(false);
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt))
        {
            if (informationWindow.gameObject.activeSelf) return;

            SoundManager.Instance.PlaySE("DisplayOption");

            optionWindow.SetActive(!optionWindow.activeSelf);

            if (optionWindow.activeSelf)
            {
                LogManager.Instance.Register(LogType.Option);
                GameParameter.SetPause(true);
            }
            else
            {
                GameParameter.SetPause(false);
            }
        }
    }

    public void Register(BaseEnemyController enemy)
    {
        SaveData.Instance.OnEncountEnemy(enemy.EnemyInfo.enemyType);

        if (!enemyTypes.Contains(enemy.EnemyInfo.enemyType)) enemyTypes.Add(enemy.EnemyInfo.enemyType);

        enemyCount++;
        enemyCountText.text = "×" + enemyCount;
        
        enemy.SetPlayer(lifeController.transform);
    }

    public void EnemyKilled(string enemyName, bool log)
    {
        enemyCount--;
        enemyCountText.text = "×" + enemyCount;

        if (log)
        {
            LogManager.Instance.Register(enemyName);
        }

        if (enemyCount == 0) 
        {
            OnClear();
        }
    }

    public void RestartYes()
    {
        LoadScene(SceneManager.GetActiveScene().name, "Restart");
    }

    public void RestartNo()
    {
        LoadScene("StageSelectScene", "Back");
    }

    public void StageSelect()
    {
        LoadScene("StageSelectScene", "Back");
    }

    public void NextStage()
    {
        GameParameter.stageNumber += 1;
        LoadScene("Stage" + GameParameter.stageNumber, "Restart");
    }

    void LoadScene(string sceneName, string se)
    {
        GameParameter.OnExitMainScene();
        SoundManager.Instance.PlaySE(se);
        SceneManager.LoadScene(sceneName);
    }
}
