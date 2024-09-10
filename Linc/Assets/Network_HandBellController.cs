using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirage;
using UnityEngine;

public class Network_HandBellController : NetworkBehaviour
{
  public enum NetObj
  {
      Xylophone,
      NetObj_Mockup_Handbell_Left,
      NetObj_Mockup_Handbell_Right
  }


  public Transform handbell_Left;
  public Transform handbell_Right;
  private GameObject[] _networkMockups;
  
#if UNITY_EDITOR
    private float _elapsed;
    private float _interval=0.5f;
#endif
  
  /// <summary>
  /// Manager로 인해 active false됨, Start로 옮기지 말 것. 09/09
  /// </summary>
    void Awake()
    {
        _networkMockups = new GameObject[3];
        _networkMockups[(int)NetObj.Xylophone] = transform.GetChild((int)NetObj.NetObj_Mockup_Handbell_Left).gameObject;
        _networkMockups[(int)NetObj.NetObj_Mockup_Handbell_Left] = transform.GetChild((int)NetObj.NetObj_Mockup_Handbell_Left).gameObject;
        _networkMockups[(int)NetObj.NetObj_Mockup_Handbell_Right] = transform.GetChild((int)NetObj.NetObj_Mockup_Handbell_Right).gameObject;

        handbell_Left = GameObject.Find("Handbell_Left").transform;
        handbell_Right = GameObject.Find("Handbell_Right").transform;
    }
    

    // Update is called once per frame
    void Update()
    {
        
#if UNITY_EDITOR
    _elapsed += Time.deltaTime;
    
#endif
        if (Managers.Network.Server.isActiveAndEnabled && Managers.Network.Client.isActiveAndEnabled
            && UI_MainController_NetworkInvolved.IsStartBtnClicked)
        {
            if (Managers.Network.Server.IsHost)
            {
                ClientRPC_SyncHandbellTransform(handbell_Left.transform.rotation,handbell_Right.transform.rotation);
            }
        }

        if (Managers.DeviceManager.IsConnected)
        {
            var quatFromDevice =Managers.DeviceManager.StickData.ParseAndConvertToQuaternion();
            handbell_Left.rotation = quatFromDevice;
            handbell_Right.rotation = quatFromDevice;
            if (_elapsed > _interval)
            {
                Logger.Log($"currentQuat : {handbell_Left.rotation}");
                _elapsed = 0;
            }
        }
      
    }


    private void Local_ControlHandBellWithHapticStick()
    {
        
    }
  


    [ClientRpc]
    private void ClientRPC_SyncHandbellTransform(Quaternion left, Quaternion right)
    {
      NetworkIdentity NetId_NetObj_Mockup_Handbell_Left =null;
      NetworkIdentity NetId_NetObj_Mockup_Handbell_Right =null ;
        if (!Managers.Network.Server.IsHost)
        {
            if (NetId_NetObj_Mockup_Handbell_Left == null)
                NetId_NetObj_Mockup_Handbell_Left = Managers.Network.Client.World.SpawnedIdentities.ToList().Find(x =>
                {
                    Logger.Log($"Client: {x.gameObject.name} -> NetID {x.NetId}");
                    // x가 UI_WaitForHost에 해당하는 NetworkIdentity와 같은지 비교
                    return x.gameObject.name == "NetObj_Mockup_Handbell_Left";
                });


            NetId_NetObj_Mockup_Handbell_Left.gameObject.transform.rotation = left;
            
            if (NetId_NetObj_Mockup_Handbell_Right == null)
            {
                 NetId_NetObj_Mockup_Handbell_Right = Managers.Network.Client.World.SpawnedIdentities.ToList().Find(
                    x =>
                    {
                        Logger.Log($"Client: {x.gameObject.name} -> NetID {x.NetId}");
                        // x가 UI_WaitForHost에 해당하는 NetworkIdentity와 같은지 비교
                        return x.gameObject.name == "NetObj_Mockup_Handbell_Right";
                    });
            }
            
            NetId_NetObj_Mockup_Handbell_Right.gameObject.transform.rotation = right;
        }
     

    }
}
