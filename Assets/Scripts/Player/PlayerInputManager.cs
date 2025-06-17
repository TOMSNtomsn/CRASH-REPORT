using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    IDelayedInput movementController;
    IDelayedInput laserController;

    void Start()
    {
        movementController = GetComponent<PlayerMovementController>();
        laserController = GetComponent<PlayerLaserController>();
    }

    void Update()
    {
        if (GameController.GameParameter.GameEnd || !GameController.GameParameter.gameStart) return;

        CheckKeyDown(movementController, KeyCode.Space, KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.RightArrow, KeyCode.LeftArrow, KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D);
        CheckKeyUp(movementController, KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.RightArrow, KeyCode.LeftArrow, KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D);

        if (GameController.GameParameter.Pause) return;

        CheckMouseDown(laserController, 0);
    }

    void CheckKeyDown(IDelayedInput input, params KeyCode[] keyCodes)
    {
        foreach (KeyCode key in keyCodes)
        {
            if (Input.GetKeyDown(key)) StartCoroutine(InvokeCoroutine(input.GetKeyDown, key));
        }
    }

    void CheckKeyUp(IDelayedInput input, params KeyCode[] keyCodes)
    {
        foreach (KeyCode key in keyCodes)
        {
            if (Input.GetKeyUp(key)) StartCoroutine(InvokeCoroutine(input.GetKeyUp, key));
        }
    }

    void CheckMouseDown(IDelayedInput input, int button)
    {
        if (Input.GetMouseButtonDown(button)) StartCoroutine(InvokeCoroutine(input.GetMouseButtonDown, button));
    }

    IEnumerator InvokeCoroutine<T>(Action<T> action, T input)
    {
        yield return new WaitForSeconds(GameController.GameParameter.Delay);

        action.Invoke(input);
    }
}
