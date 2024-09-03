using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialScene : BaseScene
{
    protected override bool Init()
    {
        if (base.Init() == false)
            return false;

        SceneType = Define.Scene.Dev;
        Managers.UI.ShowSceneUI<UI_Persistent>();
        Managers.UI.ShowPopupUI<UI_Main>();
        Debug.Log($"ui stack count: {Managers.UI.PopupStack.Count}");
        Managers.Sound.Play(SoundManager.Sound.Bgm, "Bgm");
        Debug.Log("Init");
        return true;
    }
}
