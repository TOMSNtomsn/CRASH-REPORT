using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleButtonManager : MonoBehaviour
{
    [SerializeField] int stageSelectSceneIndex = 1;
    [SerializeField] int enemyDexSceneIndex = 2;
    [SerializeField] GameObject creditsContainer;
    [SerializeField] GameObject optionsContainer;

    bool isCreditsOrOptionsOpen = false;

    void Start()
    {
        if (SaveData.Instance == null)
        {
            SaveData.Load();
        }

        if (GameController.GameParameter == null)
        {
            GameController.GameParameter = new();
        }
    }

    void Update()
    {

    }

    public void OnClickStageSelectButton()
    {
        if (isCreditsOrOptionsOpen)
        {
            // If credits or options are open, do not proceed
            return;
        }
        SoundManager.Instance.PlaySE("Go");
        SceneManager.LoadScene(stageSelectSceneIndex);
    }

    public void OnClickEnemyDexButton()
    {
        if (isCreditsOrOptionsOpen)
        {
            // If credits or options are open, do not proceed
            return;
        }
        SoundManager.Instance.PlaySE("Go");
        SceneManager.LoadScene(enemyDexSceneIndex);
    }

    public void OnClickCreditsButton()
    {
        if (isCreditsOrOptionsOpen)
        {
            // If credits or options are open, do not proceed
            return;
        }
        isCreditsOrOptionsOpen = true;
        SoundManager.Instance.PlaySE("Go");
        creditsContainer.SetActive(true);
    }

    public void OnClickOptionsButton()
    {
        if (isCreditsOrOptionsOpen)
        {
            // If credits or options are open, do not proceed
            return;
        }
        isCreditsOrOptionsOpen = true;
        SoundManager.Instance.PlaySE("Go");
        optionsContainer.SetActive(true);
    }

    public void OnClickBackButton()
    {
        SoundManager.Instance.PlaySE("Back");
        creditsContainer.SetActive(false);
        optionsContainer.SetActive(false);
        isCreditsOrOptionsOpen = false;
    }

    public void ResetSaveData()
    {
        SaveData.Reset();
    }
}
