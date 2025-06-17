using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class InformationSceneManager : MonoBehaviour
{
    [SerializeField] InformationWindow window;

    void Start()
    {
        List<int> list = new() { 1, 2, 3, 4, 5, 6, 7, 8 };
        window.Init(list.Select(x => (EnemyType)x).ToList());
    }

    public void BackButton()
    {
        SoundManager.Instance.PlaySE("Back");
        SceneManager.LoadScene(0);
    }
}
