using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class OctopusUIManager : MonoBehaviour
{
    public static OctopusUIManager Instance;

    [SerializeField] private GameObject[] legImages;  // ヒエラルキーに置いた10個のImageオブジェクト

    List<Vector3> startPositions;

    [Header("足のUIを表示しておく秒数")][SerializeField] private float legDisplayDuration = 10f;
    [Header("ずれ落ちる時間")][SerializeField] private float fallDuration = 0.5f;
    [Header("落下距離（Y方向）")][SerializeField] private float fallDistance = 50f;

    private void Awake()
    {
        startPositions = new();
        foreach (GameObject leg in legImages)
        {
            startPositions.Add(leg.transform.localPosition);
        }

        Instance = this;
    }

    public void ShowRandomLegImage()
    {
        List<int> hiddenIndexes = new();
        for (int i = 0; i < legImages.Length; i++)
        {
            if (!legImages[i].activeSelf)
            {
                hiddenIndexes.Add(i);
            }
        }

        if (hiddenIndexes.Count == 0) return;

        int chosenIndex = hiddenIndexes[Random.Range(0, hiddenIndexes.Count)];
        legImages[chosenIndex].SetActive(true);
        legImages[chosenIndex].transform.localPosition = startPositions[chosenIndex];

        StartCoroutine(HideAfterSeconds(legImages[chosenIndex], legDisplayDuration));
    }

    IEnumerator HideAfterSeconds(GameObject obj, float seconds)
    {
        yield return new WaitForSeconds(seconds);

        yield return StartCoroutine(PlayFallAnimation(obj));

        obj.SetActive(false);
    }

    IEnumerator PlayFallAnimation(GameObject obj)
    {
        RectTransform rect = obj.GetComponent<RectTransform>();
        CanvasGroup group = obj.GetComponent<CanvasGroup>();

        if (rect == null) yield break;

        if (group == null)
        {
            group = obj.AddComponent<CanvasGroup>();
        }

        Vector2 startPos = rect.anchoredPosition;
        Vector2 endPos = startPos - new Vector2(0, fallDistance);

        float time = 0f;
        while (time < fallDuration)
        {
            float t = time / fallDuration;

            // イージング（加速度的な動きに）
            float easeT = t * t;

            rect.anchoredPosition = Vector2.Lerp(startPos, endPos, easeT);

            time += Time.deltaTime;
            yield return null;
        }

        rect.anchoredPosition = endPos;
    }
}
