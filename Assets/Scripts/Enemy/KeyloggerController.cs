using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyloggerController : BaseEnemyController
{
    [SerializeField] float keySpeed;
    [SerializeField] GameObject[] keysOnDisplay;
    [SerializeField] KeyBehaviour keyPrefab;

    enum Arrow
    {
        Up,
        Down,
        Right,
        Left
    }

    public override void EnemyUpdate()
    {
        foreach (KeyCode key in PlayerKey.UpKeys)
        {
            if (Input.GetKeyUp(key)) ShotKey(Arrow.Up);
        }

        foreach (KeyCode key in PlayerKey.DownKeys)
        {
            if (Input.GetKeyUp(key)) ShotKey(Arrow.Down);
        }

        foreach (KeyCode key in PlayerKey.RightKeys)
        {
            if (Input.GetKeyUp(key)) ShotKey(Arrow.Right);
        }

        foreach (KeyCode key in PlayerKey.LeftKeys)
        {
            if (Input.GetKeyUp(key)) ShotKey(Arrow.Left);
        }

        if (GameController.GameParameter.Pause) return;

        foreach (KeyCode key in PlayerKey.UpKeys)
        {
            if (Input.GetKeyDown(key)) keysOnDisplay[(int)Arrow.Up].SetActive(true);
        }

        foreach (KeyCode key in PlayerKey.DownKeys)
        {
            if (Input.GetKeyDown(key)) keysOnDisplay[(int)Arrow.Down].SetActive(true);
        }

        foreach (KeyCode key in PlayerKey.RightKeys)
        {
            if (Input.GetKeyDown(key)) keysOnDisplay[(int)Arrow.Right].SetActive(true);
        }

        foreach (KeyCode key in PlayerKey.LeftKeys)
        {
            if (Input.GetKeyDown(key)) keysOnDisplay[(int)Arrow.Left].SetActive(true);
        }

    }

    void ShotKey(Arrow arrow)
    {
        SoundManager.Instance.PlaySE("KeyloggerShot");
        keysOnDisplay[(int)arrow].SetActive(false);
        KeyBehaviour key = Instantiate(keyPrefab, keysOnDisplay[(int)arrow].transform.position, keysOnDisplay[(int)arrow].transform.rotation);
        key.Init(player, keySpeed);
    }
}
