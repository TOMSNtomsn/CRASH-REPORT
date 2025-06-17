using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class InformationWindow : MonoBehaviour
{
    [SerializeField] InformationButton[] buttons;
    [SerializeField] Transform buttonContainer;

    [SerializeField] InformationCube mainCube;
    [SerializeField] InformationCube subCube;

    [SerializeField] EnemyInfo notOpened;

    bool inputEnable = true;
    int currentIndex = -1;
    EnemyInfo currentEnemyInfo;

    public void Init(List<EnemyType> enemyTypes)
    {
        int firstIndex = -1;

        for (int i = 0; i < buttons.Length; i++)
        {
            EnemyType enemyType = (EnemyType)(i + 1);

            if (enemyTypes.Contains(enemyType))
            {
                buttons[i].Init(this);

                if (!SaveData.Instance.HasEncountered(enemyType))
                {
                    buttons[i].SetNotOpened(notOpened);
                }
                if (firstIndex == -1)
                {
                    firstIndex = i;
                }
            }
            else
            {
                buttons[i].gameObject.SetActive(false);
            }
        }

        buttons[firstIndex].OnClick();
    }

    public void OnClickButton(EnemyInfo enemyInfo, InformationButton informationButton, int index)
    {
        if (currentIndex == index || !inputEnable)
        {
            return;
        }
        else if (currentEnemyInfo == null)
        {
            mainCube.Set(enemyInfo);
        }
        else if (GameController.GameParameter.Pause)
        {
            mainCube.Set(enemyInfo);
            SoundManager.Instance.PlaySE("ChangeEnemies");
        }
        else
        {
            SoundManager.Instance.PlaySE("ChangeEnemies");
            subCube.Set(enemyInfo);
            StartCoroutine(MoveCube(enemyInfo));
            inputEnable = false;
        }

        currentEnemyInfo = enemyInfo;

        foreach (InformationButton button in buttons) button.ResetPos();
        informationButton.SetSelectedPos();        
    }

    IEnumerator MoveCube(EnemyInfo info)
    {
        float progress = 0;

        while (progress < 1)
        {
            progress += Time.deltaTime / 0.5f;

            mainCube.transform.localPosition = new Vector3(0, -2.5f + 4.5f * progress, 0);
            subCube.transform.localPosition = new Vector3(0, -7f + 4.5f * progress, 0);
            yield return null;
        }

        mainCube.Set(info);

        mainCube.transform.localPosition = new Vector3(0, -2.5f, 0);
        subCube.transform.localPosition = new Vector3(0, -7f, 0);
        inputEnable = true;
    }
}
