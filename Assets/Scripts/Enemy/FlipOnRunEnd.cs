using UnityEngine;

/// <summary>
/// オクトパスのアニメーション用のスクリプト
/// </summary>
public class FlipOnRunEnd : MonoBehaviour
{
    private int loopCount = 0;

    private void Start()
    {
        if (transform == null)
        {
            Debug.LogError("Transformが見つかりません");
        }
    }

    // アニメーションイベントから呼ばれる
    public void OnRunLoop()
    {
        loopCount++;
        // Debug.Log($"Runループ {loopCount} 回目");

        // 上下反転（Y軸のスケールをマイナスに）
        Vector3 scale = transform.localScale;

        // scale.x *= -1f;
        // scale.y *= -1f;
        // scale.z *= -1f;
        transform.localScale = scale;
    }
}
