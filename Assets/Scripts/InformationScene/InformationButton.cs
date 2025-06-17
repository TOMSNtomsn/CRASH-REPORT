using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InformationButton : MonoBehaviour
{
    [SerializeField] int index;
    [SerializeField] EnemyInfo enemyInfo;
    [SerializeField] Sprite notOpenedSprite;

    InformationWindow window;
    Vector2 baseLocalPos;

    public void Init(InformationWindow window)
    {
        this.window = window;
        baseLocalPos = transform.localPosition;
    }

    public void SetNotOpened(EnemyInfo notOpened)
    {
        enemyInfo = notOpened;
        GetComponent<Image>().sprite = notOpenedSprite;
    }

    public void OnClick()
    {
        window.OnClickButton(enemyInfo, this, index);
    }

    public void ResetPos()
    {
        transform.localPosition = baseLocalPos;
    }

    public void SetSelectedPos()
    {
        transform.localPosition -= new Vector3(Mathf.Sign(baseLocalPos.x) * 50, 0);
    }
}
