public class UI_PlayModeSelection : UI_Popup
{
    public enum Btns
    {
        Btn_FreePlayMode,
        Btn_RhythmGameMode
    }

    public override bool Init()
    {
        if (!Managers.Network.Server.IsHost) gameObject.SetActive(false);

        BindButton(typeof(Btns));
        GetButton((int)Btns.Btn_RhythmGameMode).gameObject.BindEvent(OnRythmGameModeBtnClicked);
        GetButton((int)Btns.Btn_FreePlayMode).gameObject.BindEvent(Btn_FreePlayMode);
        return base.Init();
    }


    public void OnRythmGameModeBtnClicked()
    {
        Managers.Network.ClientObjectManager.PrepareToSpawnSceneObjects();
        Managers.ContentInfo.PlayData.CurrentPlayMode = (int)Define.PlayMode.RhythmGame;
        // Logger.Log($"Spawned Prefab Count: {Managers.Network.ClientObjectManager.spawnPrefabs.Count}");
        // foreach (var spawnPrefab in Managers.Network.ClientObjectManager.spawnPrefabs)
        //     Logger.Log($"spawnPrefab : {spawnPrefab.gameObject.name} NetID: {spawnPrefab.NetId}");


        if (Managers.Network.Server.IsHost)
        {
         
            if (Managers.NetworkObjNetworkIds == null) Logger.LogError("Netork Obj Dictionary Pool is NUll");
           
        }

        Managers.NetworkObjs[(int)Define.NetworkObjs.UI_InstrumentSelection].SetActive(true);
        gameObject.SetActive(false);
    }

    public void Btn_FreePlayMode()
    {
     
        Managers.Network.ClientObjectManager.PrepareToSpawnSceneObjects();
        
        Managers.ContentInfo.PlayData.CurrentPlayMode = (int)Define.PlayMode.Free;
        
        // Logger.Log($"Spawned Prefab Count: {Managers.Network.ClientObjectManager.spawnPrefabs.Count}");
        // foreach (var spawnPrefab in Managers.Network.ClientObjectManager.spawnPrefabs)
        //     Logger.Log($"spawnPrefab : {spawnPrefab.gameObject.name} NetID: {spawnPrefab.NetId}");

        if (Managers.Network.Server.IsHost)
        {
            if (Managers.NetworkObjNetworkIds == null) Logger.LogError("Netork Obj Dictionary Pool is NUll");
        }
        Managers.NetworkObjs[(int)Define.NetworkObjs.UI_InstrumentSelection].SetActive(true);
        gameObject.SetActive(false);
        // Managers.UI.ShowPopupUI<UI_InstrumentSelection>();
        // GameObject.Find("UI_InstrumentSelection").SetActive(true);
    }
}