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

  private const float OFFSET= +0.1f;
  // MusicPlayPart--------------------
  // 타이밍 딕셔너리
  private Dictionary<int, float> _drumPlayEventMap_indexToTiming = new Dictionary<int, float>()
  {
      {1,  8.7f + OFFSET},  //'길'쭉길쭉 당근을 뽑아봐요
      {2,  12.4f + OFFSET}, //'쏙' 
      {3,  13.1f + OFFSET}, //'세'모세모 당근을 뽑아봐요
      {4,  16.8f + OFFSET},  //'쏙'
      {5,  19.52f + OFFSET},//익힌 당근을 자르면 '쓰윽쓰윽'
      {6,  24.02f + OFFSET}, // 한입 베어물면 '물'컹 '물'컹 '무'울컹
      {7,  28.3f + OFFSET},  //딱딱한 당근을 자르면 '통통통통'
      {8,  32.7f + OFFSET},  // 한입 베이물면 '아'삭'아'삭' '아'사삭
      {9,  36.0f + OFFSET},  //아삭아삭 '당'근도 
      {10, 37.75f + OFFSET},  // 물컹물컹 '당'근도 
      {11, 40.0f + OFFSET},  // 모두 '정'말 '정'말
      {12, 43.0f + OFFSET},  //(맛이 있어요) '예'
      {13, 46.3f + OFFSET},  // 나는 당근이 정말 '좋''아''요'
      {14, 51.2f + OFFSET},  //나는 당근이 정말 '좋아요'
      {15, 0.0f + OFFSET}
  };

// 두드림 횟수 딕셔너리
  private Dictionary<int, int> _drumPlayEventDictionary_IndexToCount = new Dictionary<int, int>()
  {
      {1,  1},//'길'쭉길쭉 당근을 뽑아봐요
      {2,  1},//'쏙' 
      {3,  1},//'세'모세모 당근을 뽑아봐요
      {4,  1}, //'쏙'
      {5,  4},//익힌 당근을 자르면 '쓰윽쓰윽'
      {6,  3},// 한입 베어물면 '물'컹 '물'컹 '무'울컹
      {7,  4}, //딱딱한 당근을 자르면 '통통통통'
      {8,  3}, // 한입 베이물면 '아'삭'아'삭' '아'사삭
      {9,  1}, //아삭아삭 '당'근도 
      {10, 1}, // 물컹물컹 '당'근도 
      {11, 2}, // 모두 '정'말 '정'말
      {12, 1}, //(맛이 있어요) '예'
      {13, 3}, // 나는 당근이 정말 '좋''아''요'
      {14, 3}, //나는 당근이 정말 '좋아요'
      {15, 0}
  };

// 간격 딕셔너리
  private Dictionary<int, float> _drumPlayEventDictionary_IndexToInterval = new Dictionary<int, float>()
  {
      {1,  0.0f},//'길'쭉길쭉 당근을 뽑아봐요
      {2,  0f},//'쏙' 
      {3,  0f},//'세'모세모 당근을 뽑아봐요
      {4,  0f}, //'쏙'
      {5,  0.55f},//익힌 당근을 자르면 '쓰윽쓰윽'
      {6,  0.55f},// 한입 베어물면 '물'컹 '물'컹 '무'울컹
      {7,  0.55f}, //딱딱한 당근을 자르면 '통통통통'
      {8,  0.55f}, // 한입 베이물면 '아'삭'아'삭' '아'사삭
      {9,  0.0f}, //아삭아삭 '당'근도 
      {10, 0.0f}, // 물컹물컹 '당'근도 
      {11, 0.55f}, // 모두 '정'말 '정'말
      {12, 0.0f}, //(맛이 있어요) '예'
      {13, 0.245f}, // 나는 당근이 정말 '좋''아''요'
      {14, 0.245f}, //나는 당근이 정말 '좋아요'
      {15, 0.0f}
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
        _indexMax = _drumPlayEventDictionary_IndexToCount.Count;
        
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

  private void OnPlayMusic(bool isReplay)
  {
      if (_isPlayBtnClicked && !isReplay) return;
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
          Managers.Sound.Play(SoundManager.Sound.Effect, "Audio/Effect/Bell" +randomChar,1.0f);
          Managers.Sound.Play(SoundManager.Sound.Effect, "Audio/Effect/BellSub" +randomChar,1.0f);
          yield return _waitForInterval;
      }

      if (_currentIndex >= _indexMax)
      {
          Debug.Log("drun Auto Event With music is over--------");
          _isMusicOver = true;
          _isPlayBtnClicked = false;
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
