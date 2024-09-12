using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using Cysharp.Threading.Tasks.Triggers;
using Mirage;
using UnityEngine;

public class Network_HandBellController : NetworkBehaviour
{
  public enum NetObj
  {
      Xylophone,
      NetObj_Mockup_Handbell_Left,
      NetObj_Mockup_Handbell_Right,
  }

  private Collider _colliderRight;
  private Collider _colliderLeft;

  public Transform handbell_Left;
  public Transform handbell_Right;
  private GameObject[] _networkMockups;
    
  
  // MusicPlayPart--------------------
  private Dictionary<int, float> _drumPlayEventMap_indexToTiming = new Dictionary<int, float>()
  {
      {1, 12.5f},
      {2, 20.3f},
      {3, 24.0f},
      {4, 28.5f},
      {5, 34.0f},
      {6, 36.0f},
      {7, 37.0f},
      {8, 43.0f},
      {9, 46.0f},
      {10, 51.0f}
  };
  private Dictionary<int, int> _drumPlayEventDictionary_IndexToCount = new Dictionary<int, int>()
  {
      {1, 1},
      {2, 2},
      {3, 3},
      {4, 3},
      {5, 2},
      {6, 3},
      {7, 2},
      {8, 3},
      {9, 3},
      {10, 3}
  };
  private Dictionary<int, float> _drumPlayEventDictionary_IndexToInterval = new Dictionary<int, float>()
  {
      {1, 0},
      {2, 0.5f},
      {3, 0.5f},
      {4, 4},
      {5, 3},
      {6, 1},
      {7, 2},
      {8, 1},
      {9, 3},
      {10, 3}
  };
    
  private int _currentIndex =1;
  private float _currentTime;
  private bool _isPlayBtnClicked;
  private float _playInterval = 0.25f;
  private int _indexMax;
  private bool _isMusicOver;
  private WaitForSeconds _waitForInterval;
  private bool _isBellPlaying; // 중복실행 방지
  
  /// <summary>
  /// Manager로 인해 active false됨, Start로 옮기지 말 것. 09/09
  /// </summary>
    void Awake()
    {
        UI_Maincontroller_SinglePlay.PlayMusicEvent -= OnPlayMusic;
        UI_Maincontroller_SinglePlay.PlayMusicEvent += OnPlayMusic;
        UI_MainController_NetworkInvolved.PlayMusicEvent -= OnPlayMusic;
        UI_MainController_NetworkInvolved.PlayMusicEvent += OnPlayMusic;
        handbell_Left = GameObject.Find("Handbell_Left").transform;
        handbell_Right = GameObject.Find("Handbell_Right").transform;

        if (Managers.Network == null) return;

        _networkMockups = new GameObject[3];
        _networkMockups[(int)NetObj.Xylophone] = transform.GetChild((int)NetObj.NetObj_Mockup_Handbell_Left).gameObject;
        _networkMockups[(int)NetObj.NetObj_Mockup_Handbell_Left] = transform.GetChild((int)NetObj.NetObj_Mockup_Handbell_Left).gameObject;
        _networkMockups[(int)NetObj.NetObj_Mockup_Handbell_Right] = transform.GetChild((int)NetObj.NetObj_Mockup_Handbell_Right).gameObject;
    }

  private void OnPlayMusic()
  {
      _isPlayBtnClicked = true;
      _isMusicOver = false;
      _currentIndex = 1;
      _currentTime = 0;
  }
  protected  void OnDestroy()
  {
      UI_MainController_NetworkInvolved.PlayMusicEvent -= OnPlayMusic;
      UI_Maincontroller_SinglePlay.PlayMusicEvent -= OnPlayMusic;
  }

  void Update()
    {
        SyncHandBellTransform();
        SetHandbellTransformFromHapticStick();
        if (_isPlayBtnClicked && Managers.ContentInfo.PlayData.HostInstrument == (int)Define.Instrument.HandBell)
        {
            Debug.Assert(_drumPlayEventMap_indexToTiming.ContainsKey(_currentIndex));

            _currentTime += Time.deltaTime;
            if (_currentTime >= _drumPlayEventMap_indexToTiming[_currentIndex])
            {
                if (_isBellPlaying || _isMusicOver) return;
                _isBellPlaying = true;
                StartCoroutine(PlayDrumWithMusic(_currentIndex));
            }
        }
    }
  
  IEnumerator PlayDrumWithMusic(int currentIndex)
  {
      if (_waitForInterval == null)
      {
          _waitForInterval = new WaitForSeconds(_playInterval);
      }

      for (var i = 0; i < _drumPlayEventDictionary_IndexToCount[currentIndex]; i++)
      {
          var randomChar = (char)UnityEngine.Random.Range('A', 'D' + 1);
          Managers.Sound.Play(SoundManager.Sound.Effect, "Audio/Effect/Bell" +randomChar);
          yield return _waitForInterval;
      }

      if (_currentIndex >= _indexMax)
      {
          Debug.Log("drun Auto Event With music is over--------");
          _isMusicOver = true;
      }
      else
      {
          _currentIndex++;
      }

      _isBellPlaying = false;


  }
  
  

    private void SetHandbellTransformFromHapticStick()
    {
        if (Managers.DeviceManager.IsConnected)
        {
            var quatFromDevice = Managers.DeviceManager.StickData.StickA_Quaternion;
            if (quatFromDevice != new Quaternion(0, 0, 0, 0))
            {
                var eulerRotation = quatFromDevice.eulerAngles;
                handbell_Left.localRotation = Quaternion.Euler(eulerRotation);
                handbell_Right.localRotation = Quaternion.Euler(eulerRotation);
            }
            
        }
    }

    private void SyncHandBellTransform()
    {
     
        
        if (Managers.Network !=null && Managers.Network.Server.isActiveAndEnabled && Managers.Network.Client.isActiveAndEnabled
            && UI_MainController_NetworkInvolved.IsStartBtnClicked )
        {
            Debug.Assert(Managers.Network != null);
            if (Managers.Network.Server.IsHost)
                ClientRPC_SyncHandbellTransform(handbell_Left.transform.rotation, handbell_Right.transform.rotation);

            if (!Managers.Network.Server.IsHost)
            {
                ServerRPC_SendTransformToHostServer(handbell_Left.transform.rotation,handbell_Right.transform.rotation);
            }
        }
    }
    
    

    [ClientRpc]
    private void ClientRPC_SyncHandbellTransform(Quaternion left, Quaternion right)
    {
        NetworkIdentity NetId_NetObj_Mockup_Handbell_Left = null;
        NetworkIdentity NetId_NetObj_Mockup_Handbell_Right = null;
        NetworkIdentity Handbell_Left = null;
        NetworkIdentity Handbell_Right = null;


        if (!Managers.Network.Server.IsHost)
        {
            if (NetId_NetObj_Mockup_Handbell_Left == null)
                NetId_NetObj_Mockup_Handbell_Left = Managers.Network.Client.World.SpawnedIdentities.ToList().Find(
                    x => { return x.gameObject.name == "NetObj_Mockup_Handbell_Left"; });


            if (NetId_NetObj_Mockup_Handbell_Right == null)
                NetId_NetObj_Mockup_Handbell_Right = Managers.Network.Client.World.SpawnedIdentities.ToList().Find(
                    x => { return x.gameObject.name == "NetObj_Mockup_Handbell_Right"; });
            
            NetId_NetObj_Mockup_Handbell_Left.transform.rotation = left;
            NetId_NetObj_Mockup_Handbell_Right.transform.rotation = right;
        }
        
    }
    [ServerRpc(requireAuthority = false)]
    private void ServerRPC_SendTransformToHostServer(Quaternion left, Quaternion right)
    {
        if (Managers.Network.Server.IsHost)
        {
             
            NetworkIdentity Handbell_Left;
            NetworkIdentity Handbell_Right;
            Handbell_Left = Managers.Network.Server.World.SpawnedIdentities.ToList().Find(x =>
            {
                return x.gameObject.name == "NetObj_Mockup_Handbell_Left";
            });
            
            Handbell_Right = Managers.Network.Server.World.SpawnedIdentities.ToList().Find(
                x => { return x.gameObject.name == "NetObj_Mockup_Handbell_Right"; });
            
            Logger.Log($"Sync From Client : Handbell---------------------------------------");
            Handbell_Left.gameObject.transform.rotation = left;
            Handbell_Right.gameObject.transform.rotation = right;
        }

    }




 

}
