using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;


public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioSource bgmAudioSource; // BGM用のAudioSource
    [SerializeField] AudioSource seAudioSource;  // SE（効果音）用のAudioSource（未使用）
    [SerializeField] AudioMixerGroup mixerGroup; // 出力先のAudioMixer

    [SerializeField] List<BGMSoundData> bgmSoundDatas; // BGMのデータ一覧
    [SerializeField] List<SESoundData> seSoundDatas;   // SEのデータ一覧

    public float masterVolume = 1;     // 全体の音量
    public float bgmMasterVolume = 1;  // BGMのマスター音量
    public float seMasterVolume = 1;   // SEのマスター音量

    int maxSeCount = 30; // 同時に鳴らせるSEの最大数
    List<GameObject> seGameObjects = new(); // 再生中のSE用GameObjectのリスト

    public static SoundManager Instance { get; private set; } // シングルトンインスタンス
    private string currentBGM = null; // 現在再生中のBGM名

    /// <summary>
    /// シングルトン処理
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // シーンをまたいでも破棄されない
            LoadAllAudioClips();            // BGM・SEのAudioClipを読み込む
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// ResourcesフォルダからBGM・SEを自動で読み込む
    /// </summary>
    private void LoadAllAudioClips()
    {
        foreach (var data in bgmSoundDatas)
        {
            if (data.audioClip == null)
            {
                // Resources/BGM/ にあるAudioClipを読み込む
                data.audioClip = Resources.Load<AudioClip>("BGM/" + data.bgmName);
                if (data.audioClip == null)
                {
                    Debug.LogWarning($"BGM '{data.bgmName}' のAudioClipがResources/BGMから見つかりませんでした．");
                }
            }
        }

        foreach (var data in seSoundDatas)
        {
            if (data.audioClip == null)
            {
                // Resources/SE/ にあるAudioClipを読み込む
                data.audioClip = Resources.Load<AudioClip>("SE/" + data.seName);
                if (data.audioClip == null)
                {
                    Debug.LogWarning($"SE '{data.seName}' のAudioClipがResources/SEから見つかりませんでした．");
                }
            }
        }
    }

    /// <summary>
    /// BGMを再生する関数
    /// </summary>
    /// <param name="bgmName"></param>
    public void PlayBGM(string bgmName)
    {
        // すでに同じBGMが再生中なら処理しない
        if (currentBGM != null && currentBGM.ToString() == bgmName) return;

        // 指定名のBGMデータを探す
        BGMSoundData data = bgmSoundDatas.Find(data => data.bgmName == bgmName);
        if (data == null)
        {
            Debug.LogWarning($"BGM '{bgmName}' が見つかりませんでした．");
            return;
        }

        // AudioClipと音量を設定して再生
        bgmAudioSource.clip = data.audioClip;
        bgmAudioSource.volume = data.volume * bgmMasterVolume * masterVolume;
        bgmAudioSource.Play();

        currentBGM = bgmName;  // 現在のBGM名を記録
    }

    /// <summary>
    /// 効果音を再生する関数
    /// </summary>
    /// <param name="seName"></param>
    public void PlaySE(string seName)
    {
        // nullになったGameObjectを削除（破棄されたSEの後処理）
        seGameObjects.RemoveAll(x => x == null);    

        // 同時に鳴らせるSEの上限チェック
        if (seGameObjects.Count > maxSeCount) return;

        // SEデータを検索
        SESoundData data = seSoundDatas.Find(d => d.seName == seName);
        if (data == null)
        {
            Debug.LogWarning($"SE '{seName}' が見つかりませんでした．");
            return;
        }

        // 一時的なGameObjectを生成し，そこにAudioSourceをアタッチ
        GameObject tempGO = new GameObject("TempSE_" + data.audioClip.name);
        AudioSource tempSource = tempGO.AddComponent<AudioSource>();
        tempSource.outputAudioMixerGroup = mixerGroup;

        // SE再生用GameObjectを管理リストに追加し，破棄されないように設定
        seGameObjects.Add(tempGO);
        DontDestroyOnLoad(tempGO);

        // SEのAudioClipと音量を設定して再生
        tempSource.clip = data.audioClip;
        tempSource.volume = data.volume * seMasterVolume * masterVolume;
        tempSource.Play();

        // 再生終了後に自動でGameObjectを破棄
        Destroy(tempGO, data.audioClip.length);
    }

    void OnEnable()
    {
        // シーンがロードされたときに呼び出されるイベントに登録
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // イベント登録を解除
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// シーン切り替え時に実行される関数
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayBgmBySceneName();
    }

    void Start()
    {
        PlayBgmBySceneName();   // 起動時にもBGMを再生（Awake後）
    }

    /// <summary>
    /// シーン名に応じたBGMを再生する
    /// </summary>
    void PlayBgmBySceneName()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "TitleScene":
            case "StageSelectScene":
            case "InformationScene":
                SoundManager.Instance.PlayBGM("TitleStageSelect");
                break;

            case "Stage1":
                SoundManager.Instance.PlayBGM("Stage1"); break;

            case "Stage2":
                SoundManager.Instance.PlayBGM("Stage2"); break;

            case "Stage3":
                SoundManager.Instance.PlayBGM("Stage3"); break;

            case "Stage4":
                SoundManager.Instance.PlayBGM("Stage4"); break;

            case "Stage5":
                SoundManager.Instance.PlayBGM("Stage5"); break;
        }
    }
}

[System.Serializable]
public class BGMSoundData
{
    public string bgmName;          // BGMの識別名
    public AudioClip audioClip;     // BGMの実データ
    [Range(0, 1)]
    public float volume = 1;        // 個別音量（0〜1）
}

[System.Serializable]
public class SESoundData
{
    public string seName;           // SEの識別名
    public AudioClip audioClip;     // SEの実データ
    [Range(0, 1)]
    public float volume = 1;        // 個別音量（0〜1）
}
