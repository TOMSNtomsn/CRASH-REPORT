using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageSelectButtonManager : MonoBehaviour
{
    [SerializeField] int[] stageSceneBuildIndex;
    [SerializeField] HoverScaler[] stageButtons;
    [SerializeField] GameObject[] roads;
    [SerializeField] GameObject backButton;

    const int titleSceneBuildIndex = 0;

    void Start()
    {
        for (int i = 0; i < stageButtons.Length; i++)
        {
            stageButtons[i].gameObject.SetActive(SaveData.Instance.clearedStageNumber >= i);
            stageButtons[i].SetSprite(SaveData.Instance.clearedStageNumber > i);
        }

        for (int i = 0; i < roads.Length; i++)
        {
            roads[i].SetActive(stageButtons[i + 1].gameObject.activeSelf);
        }
    }

    public void OnClickBackButton()
    {
        SoundManager.Instance.PlaySE("Back");
        SceneManager.LoadScene(titleSceneBuildIndex);
    }

    public void OnClickStageButton(int index)
    {
        GameController.GameParameter.stageNumber = index + 1;

        SoundManager.Instance.PlaySE("Go");
        stageButtons[index].SetCanHover(false);

        for (int i = 0; i < stageButtons.Length; i++)
        {
            if (i == index)
            {
                stageButtons[i].GetComponent<Button>().interactable = false;
            }
            else
            {
                stageButtons[i].gameObject.SetActive(false);
            }
        }

        foreach (GameObject road in roads)
        {
            road.SetActive(false);
        }

        backButton.SetActive(false);

        StartCoroutine(LoadStageScene(stageSceneBuildIndex[index], stageButtons[index].transform));
    }

    IEnumerator LoadStageScene(int sceneBuildIndex, Transform buttonTransform)
    {
        Image image = buttonTransform.GetComponent<Image>();

        float progress = 0f;
        Vector2 startScale = buttonTransform.localScale;
        Vector2 targetScale = new(20, 20);

        while (progress < 1f)
        {
            progress += Time.deltaTime / 2f;

            buttonTransform.localScale = Vector3.Lerp(startScale, targetScale, progress);
            image.color = Color.Lerp(Color.white, Color.black, progress);

            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(sceneBuildIndex);
    }
}
