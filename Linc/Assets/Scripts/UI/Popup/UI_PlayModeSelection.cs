using UnityEngine.SceneManagement;

public class UI_PlayModeSelection : UI_Popup
{
    public enum Btns
    {
        Btn_FreePlayMode,
        Btn_RhythmGameMode,
        Btn_Back
    }

    public override bool Init()
    {
      
        if (Managers.Network !=null && !Managers.Network.Server.IsHost && Managers.ContentInfo.PlayData.isMultiMode ==false) gameObject.SetActive(false);
        BindButton(typeof(Btns));
        GetButton((int)Btns.Btn_RhythmGameMode).gameObject.BindEvent(OnRythmGameModeBtnClicked);
        GetButton((int)Btns.Btn_FreePlayMode).gameObject.BindEvent(Btn_FreePlayMode);
        GetButton(((int)Btns.Btn_Back)).gameObject.BindEvent(() =>
        {
            Managers.RestartSceneWithRemoveDontDestroy();
        });
        return base.Init();
    }


    
    public void OnRythmGameModeBtnClicked()
    {
        if (Managers.Network == null && Managers.ContentInfo.PlayData.isMultiMode == false)
        {
            Managers.ContentInfo.PlayData.CurrentPlayMode = (int)Define.PlayMode.RhythmGame;
            Managers.UI.ClosePopupUI(this);
            Managers.UI.ShowPopupUI<UI_InstrumentSelection>();
            return;
        }
        
        
        Managers.Network.ClientObjectManager.PrepareToSpawnSceneObjects();
        Managers.ContentInfo.PlayData.CurrentPlayMode = (int)Define.PlayMode.RhythmGame;
        if (Managers.Network.Server.IsHost && Managers.ContentInfo.PlayData.isMultiMode ==false)
        {
         
            if (Managers.NetworkObjNetworkIds == null) Logger.LogError("Netork Obj Dictionary Pool is NUll");
           
        }

        Managers.NetworkObjs[(int)Define.NetworkObjs.UI_InstrumentSelection].SetActive(true);
        gameObject.SetActive(false);
    }

    public void Btn_FreePlayMode()
    {
     
        
        if (Managers.Network == null && Managers.ContentInfo.PlayData.isMultiMode == false)
        {
            Managers.ContentInfo.PlayData.CurrentPlayMode = (int)Define.PlayMode.Free;
            Managers.UI.ClosePopupUI(this);
            Managers.UI.ShowPopupUI<UI_InstrumentSelection>();
            return;
        }
        
        
        
        Managers.Network.ClientObjectManager.PrepareToSpawnSceneObjects();
        Managers.ContentInfo.PlayData.CurrentPlayMode = (int)Define.PlayMode.Free;
        if (Managers.Network.Server.IsHost&& Managers.ContentInfo.PlayData.isMultiMode ==false)
        {
            if (Managers.NetworkObjNetworkIds == null) Logger.LogError("Netork Obj Dictionary Pool is NUll");
        }
        Managers.NetworkObjs[(int)Define.NetworkObjs.UI_InstrumentSelection].SetActive(true);
        gameObject.SetActive(false);;
    }
}