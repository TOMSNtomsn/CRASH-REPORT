using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyInfo", menuName = "ScriptableObjects/EnemyInfo")]
public class EnemyInfo : ScriptableObject
{
    public EnemyType enemyType;
    public string japaneseName;
    public string englishName;

    [TextArea(1, 12)] public string explain;

    public GameObject objectForInformation;
    public Texture nameTex;
}
