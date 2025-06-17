using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 遅延を伴うインプット判定をしたい時に継承する
// 継承したらPlayerInputManagerに処理を追加する

public interface IDelayedInput
{
    public void GetMouseButtonDown(int button);
    public void GetKeyDown(KeyCode keyCode);
    public void GetKeyUp(KeyCode keyCode);
}
