using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Scene_Modes :MonoBehaviour
{


    public static GameObject InGame_MultiMode;
    public static GameObject SingleMode;


    private void Start()
    {
        Init();
    }

    protected  bool Init()
    {
       // if (base.Init() == false)
         //   return false;

        InGame_MultiMode = GameObject.Find("InGame_Multimode");
        InGame_MultiMode.SetActive(false);
      
        SingleMode = GameObject.Find("UI_Maincontroller_SinglePlay");
        
       // SceneType = Define.Scene.Dev;
        // Managers.UI.ShowSceneUI<UI_MainController_NetworkInvolved>();
        Managers.UI.ShowPopupUI<UI_Loading>();
        Debug.Log($"ui stack count: {Managers.UI.PopupStack.Count}");
        Managers.Sound.Play(SoundManager.Sound.Bgm, "Bgm");
        Debug.Log("Init");
        return true;
    }
}
