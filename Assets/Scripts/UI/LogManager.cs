using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public enum LogType
{
    Start,
    TakeDamage,
    Heart,
    HeartMax,
    EnergyCore,
    BurstCore,
    Octpus,
    Troia,
    Rabbit,
    Dome,
    Zip,
    Option,
    Information,
    GameOver,
    Clear
}

public class LogManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI logText;
    [SerializeField] GameObject caution;

    List<string> logs = new();
    Coroutine cautionCoroutine;

    public static LogManager Instance { get; private set; } // シングルトンインスタンス

    void Start()
    {
        Instance = this;
    }

    (string, bool) LogTypeToLogInfo(LogType logType)
    {
        return logType switch
        {
            LogType.Start => ("ステージ" + GameController.GameParameter.stageNumber + "を開始します", false),
            LogType.TakeDamage => ("ダメージを受ける", true),
            LogType.Heart => ("ライフが回復", false),
            LogType.HeartMax => ("ライフ取ったけど最大なので意味なし", false),
            LogType.EnergyCore => ("エネルギーコア取得 ダメージアップ", false),
            LogType.BurstCore => ("バーストコア取得 広範囲攻撃化", false),
            LogType.Octpus => ("ラムダオクトパスの足がヒット", true),
            LogType.Troia => ("トロイの木馬が出現", true),
            LogType.Rabbit => ("ラビットが分裂", true),
            LogType.Dome => ("ドームの影響で乱れが発生", true),
            LogType.Zip => ("zip爆弾が点火", true),
            LogType.Option => ("設定ウィンドウを開く", false),
            LogType.Information => ("敵の情報を確認", false),
            LogType.GameOver => ("死", true),
            LogType.Clear => ("クリア！", false),

            _ => ("", false)
        };
    }

    public void Register(LogType logType)
    {
        (string, bool) logInfo = LogTypeToLogInfo(logType);

        AddLog(logInfo.Item1, logInfo.Item2);
    }

    public void Register(string enemyName)
    {
        AddLog(enemyName + "を撃破", false);
    }

    void AddLog(string message, bool showCaution)
    {
        logs.Add(message);

        while (logs.Count > 6) logs.RemoveAt(0);

        logText.text = string.Join("\n", logs);

        if (showCaution)
        {
            if (cautionCoroutine != null)
            {
                StopCoroutine(cautionCoroutine);
            }
            cautionCoroutine = StartCoroutine(Caution());
        }
    }

    IEnumerator Caution()
    {
        for (int i = 0; i < 3; i++)
        {
            caution.SetActive(i % 2 == 0);
            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(1.5f);

        caution.SetActive(false);

        cautionCoroutine = null;
    }
}
