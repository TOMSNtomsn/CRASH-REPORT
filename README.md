# CRASH REPORT

[PlayLink](https://unityroom.com/games/crash_report)

[GC甲子園 みんなのゲームパレード](https://gameparade.creators-guild.com/works/3163)

## 概要
- 敵の攻撃を避けながらレーザーで敵を倒す **3Dアクションゲーム**
- 敵は特殊な挙動や攻撃をする独自性をもつ
- 制作人数：5人
- 開発期間：3ヶ月
- プレイ人数：1人
- [プレイ動画](https://youtu.be/TGv2T4rzuc0?si=GgzW9CtHXzBSjLfP)


![Image](https://github.com/user-attachments/assets/6777b44c-cb42-43f6-b556-7d7d729f28db)

---

## 開発環境
- Unity 2021.3.16f1
- C#  
- 対応プラットフォーム：WebGL
- Git, GitHub

---

##  **設計工夫点**

### **1. データ管理の工夫**
- **シリアライズ可能なデータ構造**: `BGMSoundData`, `SESoundData` などで `[System.Serializable]` を活用
- **Resources自動読み込み**: SoundManagerでBGM/SEを動的に読み込む仕組み
- **構造体によるデータ整理**: `LaserData` 構造体でレーザー情報を効率的に管理

```csharp
struct LaserData
{
    public Laser laserPrefab;
    public float duration;
}
```

### **2. デザインパターンの活用**

#### **Singleton パターン**
```csharp
public static SoundManager Instance { get; private set; }

private void Awake()
{
    if (Instance == null)
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    else
    {
        Destroy(gameObject);
    }
}
```

#### **Observer パターン**
```csharp
// PlayerLaserControllerでイベント駆動設計
Action<LaserMode> onLaserModeChanged = (LaserMode mode) => { };
Action<float> onTimerChanged = (float f) => { };

public void AddOnLaserModeChanged(Action<LaserMode> action)
{
    onLaserModeChanged += action;
}
```

#### **Strategy パターン**
- `LaserMode` 列挙型と `laserMap` による異なるレーザー挙動の管理
- `EnemyUpdater` による `EnemyUpdate` の実行

#### **Template Method パターン**
```csharp
public void Attacked(int amount) // 共通処理
{
    SoundManager.Instance.PlaySE("Damage");
    OnAttacked(amount); // 個別処理への委譲
}

protected virtual void OnAttacked(int amount) // オーバーライド可能な固有処理
{
    // デフォルト実装
}
```

### **3. SOLID原則への配慮**

#### **単一責任原則 (SRP)**
- `SoundManager`: 音響管理のみに特化
- `PlayerLaserController`: レーザー制御のみに特化

#### **インターフェース分離原則 (ISP)**
```csharp
public interface IDelayedInput
{
    void GetMouseButtonDown(int button);
    void GetKeyDown(KeyCode keyCode);
    void GetKeyUp(KeyCode keyCode);
}
```

#### **Dependency Injection**
```csharp
public void SetPlayer(Transform player)
{
    this.player = player;
}

public void SetUp() // 外部から初期化
{
    life = startLife;
    gameController = GameObject.Find("GameController").GetComponent<GameController>();
    gameController.Register(this);
}
```

### **4. 再利用性・拡張性の工夫**

#### **柔軟なレーザーシステム**
```csharp
// 新しいレーザータイプを簡単に追加可能
laserMap = new(){
    { LaserMode.Default, new LaserData{laserPrefab = laserDefaultPrefab, duration = Mathf.Infinity }},
    { LaserMode.PowerUp, new LaserData{laserPrefab = laserPowerUpPrefab, duration = powerUpDuration }},
    { LaserMode.Burst, new LaserData{laserPrefab = laserBurstPrefab, duration = burstDuration }},
};
```

#### **シーン依存BGM自動切り替え**
```csharp
void PlayBgmBySceneName()
{
    switch (SceneManager.GetActiveScene().name)
    {
        case "TitleScene":
        case "StageSelectScene":
            SoundManager.Instance.PlayBGM("TitleStageSelect");
            break;
        // 新しいシーンを簡単に追加可能
    }
}
```

### **5. パフォーマンス最適化**

#### **効率的なSE管理**
```csharp
// 一時的なGameObjectで個別SE再生
GameObject tempGO = new GameObject("TempSE_" + data.audioClip.name);
AudioSource tempSource = tempGO.AddComponent<AudioSource>();

// 自動クリーンアップ
Destroy(tempGO, data.audioClip.length);

// 上限制御でメモリ効率化
if (seGameObjects.Count > maxSeCount) return;
```

#### **無駄な処理の回避**
```csharp
// 同じBGMが再生中なら処理しない
if (currentBGM != null && currentBGM.ToString() == bgmName) return;

// デフォルトモードでは不要な更新処理をスキップ
if (currentLaserMode == LaserMode.Default) return;
```








