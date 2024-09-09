using System;
using System.Linq;
using DG.Tweening;
using Mirage;
using UnityEngine;

public class Solo_BeadsDrum_Controller : NetworkBehaviour
{
    private Transform _beadsDrumLeft;
    private Transform _beadsDrumRight;
    
    private Transform _drumStickLeft;
    private Transform _drurmStickRight;
    
    private float _shrinkAmount = 0.92f;
    private float _defaultSize;
    private Quaternion _defaultQuatLeft;
    private Quaternion _defaultQuatRight;

    public static event Action OnStickHitLeft; 
    public static event Action OnStickHitRight;

    private Transform NetObj_Mockup_DrumStick_Left;
    private Transform NetObj_Mockup_DrumStick_Right;

    enum BeadsDrum
    {
        Left,
        Right,
    }

    enum DrumStick
    {
        DrumstickLeft,
        DrumstickRight
    }

    private void Awake()
    {
        Identity.OnStartServer.AddListener(OnStartServer);
        
        Solo_BeadsDrum_GameManager.OnLeftDrumClicked -= OnLeftDrumClicked;
        Solo_BeadsDrum_GameManager.OnLeftDrumClicked += OnLeftDrumClicked;
        
        Solo_BeadsDrum_GameManager.OnRightDrumClicked -= OnRightDrumClicked;
        Solo_BeadsDrum_GameManager.OnRightDrumClicked += OnRightDrumClicked;
        
        NetObj_Mockup_DrumStick_Left = GameObject.Find("NetObj_Mockup_DrumStick_Left").transform;
        NetObj_Mockup_DrumStick_Right = GameObject.Find("NetObj_Mockup_DrumStick_Right").transform;
        
        
        _beadsDrumLeft = transform.GetChild((int)BeadsDrum.Left);
        _beadsDrumRight = transform.GetChild((int)BeadsDrum.Right);
        _defaultSize = _beadsDrumLeft.localScale.x;

        var drumSticks = transform.GetChild(2);
        _drumStickLeft = drumSticks.GetChild((int)DrumStick.DrumstickLeft);
        _drurmStickRight = drumSticks.GetChild((int)DrumStick.DrumstickRight);

        _defaultQuatLeft = _drumStickLeft.rotation;
        _defaultQuatRight = _drurmStickRight.rotation;

#if UNITY_EDITOR
        Debug.Log("Solo Version Start -----------------");        
#endif
    }

    private void OnStartServer()
    {
  
    }

    private void OnDestroy()
    {
        Solo_BeadsDrum_GameManager.OnLeftDrumClicked -= OnLeftDrumClicked;
        Solo_BeadsDrum_GameManager.OnRightDrumClicked -= OnRightDrumClicked;
    }

    private void OnLeftDrumClicked()
    {
     
            CmdPlayDrumAndStickAnimation(BeadsDrum.Left);
        
    }

    private void OnRightDrumClicked()
    {
       
            CmdPlayDrumAndStickAnimation(BeadsDrum.Right);
        
    }
    
    private void CmdPlayDrumAndStickAnimation(BeadsDrum drum)
    {
     
        RpcPlayDrumAndStickAnimation(drum);
    }

    private void RpcPlayDrumAndStickAnimation(BeadsDrum drum)
    {
        switch (drum)
        {
            case BeadsDrum.Left:
                PlayDrumAndStickAnimation(_beadsDrumLeft, _drumStickLeft, -10, _defaultQuatLeft, OnStickHitLeft);
                break;
            case BeadsDrum.Right:
                PlayDrumAndStickAnimation(_beadsDrumRight, _drurmStickRight, 10, _defaultQuatRight, OnStickHitRight);
                break;
        }
    }

    private Sequence _seq;
    private Sequence _stickSeq;
    private void PlayDrumAndStickAnimation(Transform drum, Transform stick, float rotateAmount, Quaternion defaultRotation, Action action)
    {
        if (_seq != null && _seq.IsActive() && _seq.IsPlaying()) return;
        _seq = DOTween.Sequence();

        _seq.Append(stick.DORotateQuaternion(defaultRotation * Quaternion.Euler
                (rotateAmount, 0, 0), 0.03f).SetEase(Ease.InOutSine))
            .AppendCallback(() =>
            {
                action?.Invoke();
            })
            .AppendInterval(0.01f)
            .Append(drum.DOScale(_shrinkAmount, 0.05f).SetEase(Ease.InOutSine))
            .AppendInterval(0.005f)
            .Append(drum.DOScale(_defaultSize, 0.03f).SetEase(Ease.InOutSine))
            .Append(stick.DORotateQuaternion(defaultRotation, 0.03f).SetEase(Ease.InOutSine))
            .AppendCallback(() => { _seq = null; });
    }
    
        
    /// <summary>
    /// 클라이언트와 이벤트 호출동기화 해주는 함수
    /// </summary>
    private void Update()
    {
        if (Managers.Network.Server.isActiveAndEnabled && Managers.Network.Client.isActiveAndEnabled
                                                       && UI_MainController_NetworkInvolved.IsStartBtnClicked)
        {
            if (Managers.Network.Server.IsHost  && Managers.ContentInfo.PlayData.HostInstrument == (int)Define.Instrument.Drum)
            {
                ClientRPC_SendTransformToOpponent(_drumStickLeft.transform.rotation,_drurmStickRight.transform.rotation);
            }
            else if(Managers.Network.Server.IsHost && Managers.ContentInfo.PlayData.HostInstrument == (int)Define.Instrument.HandBell)
            {
                ClientRPC_SyncDrumStickPosFromOpponent();
            }
        }
    }
    
        
    [ClientRpc]
    private void ClientRPC_SendTransformToOpponent(Quaternion left, Quaternion right)
    {
        if (!Managers.Network.Server.IsHost)
        {
            var DrumStick_Left = Managers.Network.Client.World.SpawnedIdentities.ToList().Find(x =>
            {
                Logger.Log($"Client: {x.gameObject.name} -> NetID {x.NetId}");
                // x가 UI_WaitForHost에 해당하는 NetworkIdentity와 같은지 비교
                return x.gameObject.name == "NetObj_Mockup_DrumStick_Left";
            });

            DrumStick_Left.gameObject.transform.rotation = left;

            var DrumStick_Right = Managers.Network.Client.World.SpawnedIdentities.ToList().Find(x =>
            {
                Logger.Log($"Client: {x.gameObject.name} -> NetID {x.NetId}");
                // x가 UI_WaitForHost에 해당하는 NetworkIdentity와 같은지 비교
                return x.gameObject.name == "NetObj_Mockup_DrumStick_Right";
            });
        
            DrumStick_Right.gameObject.transform.rotation = right;

        }
     

    }
    
    
  

    [ClientRpc]
    private void ClientRPC_SyncDrumStickPosFromOpponent()
    {
        NetworkIdentity NetId_DrumStick_Left =null;
        NetworkIdentity NetId_DrumStick_Right =null;
        if (!Managers.Network.Server.IsHost)
        {
  
            if (NetId_DrumStick_Left == null)
            {
                NetId_DrumStick_Left = Managers.Network.Client.World.SpawnedIdentities.ToList().Find(x =>
                {
                    Logger.Log($"Client: {x.gameObject.name} -> NetID {x.NetId}");
                    // x가 UI_WaitForHost에 해당하는 NetworkIdentity와 같은지 비교
                    return x.gameObject.name == "DrumStick_Left";
                });

            }
            NetObj_Mockup_DrumStick_Left.rotation = NetId_DrumStick_Left.gameObject.transform.rotation;



            if (NetId_DrumStick_Right == null)
            {
                NetId_DrumStick_Right = Managers.Network.Client.World.SpawnedIdentities.ToList().Find(x =>
                {
                    Logger.Log($"Client: {x.gameObject.name} -> NetID {x.NetId}");
                    // x가 UI_WaitForHost에 해당하는 NetworkIdentity와 같은지 비교
                    return x.gameObject.name == "DrumStick_Right";
                });

            }
      
            NetObj_Mockup_DrumStick_Right.rotation = NetId_DrumStick_Right.gameObject.transform.rotation;

        }
     

    }



}
