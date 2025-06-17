using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InformationCube : MonoBehaviour
{
    [SerializeField] TextMeshPro[] japaneseNameTexts;
    [SerializeField] TextMeshPro[] englishNameTexts;
    [SerializeField] TextMeshPro[] explainTexts;
    [SerializeField] MeshRenderer nameCube;

    GameObject obj;

    public void Set(EnemyInfo info)
    {
        foreach (TextMeshPro explain in explainTexts)
        {
            explain.text = info.explain;
        }

        foreach (TextMeshPro text in japaneseNameTexts)
        {
            text.text = info.japaneseName;
        }

        foreach (TextMeshPro text in englishNameTexts)
        {
            text.text = info.englishName;
        }

        if (obj) Destroy(obj);
        obj = Instantiate(info.objectForInformation, transform);
    }
}
