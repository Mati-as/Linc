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
    private Transform _drumStickRight;
    
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
       
        
        Solo_BeadsDrum_GameManager.OnLeftDrumClicked -= OnLeftDrumClicked;
        Solo_BeadsDrum_GameManager.OnLeftDrumClicked += OnLeftDrumClicked;
        
        Solo_BeadsDrum_GameManager.OnRightDrumClicked -= OnRightDrumClicked;
        Solo_BeadsDrum_GameManager.OnRightDrumClicked += OnRightDrumClicked;
        
   
        _beadsDrumLeft = transform.GetChild((int)BeadsDrum.Left);
        _beadsDrumRight = transform.GetChild((int)BeadsDrum.Right);
        _defaultSize = _beadsDrumLeft.localScale.x;

        var drumSticks = transform.GetChild(2);
        _drumStickLeft = drumSticks.GetChild((int)DrumStick.DrumstickLeft);
        _drumStickRight = drumSticks.GetChild((int)DrumStick.DrumstickRight);

        _defaultQuatLeft = _drumStickLeft.rotation;
        _defaultQuatRight = _drumStickRight.rotation;


        if (Managers.Network != null)
        {
            NetObj_Mockup_DrumStick_Left = GameObject.Find("NetObj_Mockup_DrumStick_Left").transform;
            NetObj_Mockup_DrumStick_Right = GameObject.Find("NetObj_Mockup_DrumStick_Right").transform;
        }


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
                PlayDrumAndStickAnimation(_beadsDrumRight, _drumStickRight, 10, _defaultQuatRight, OnStickHitRight);
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
        if (Managers.Network !=null &&Managers.Network.Server.isActiveAndEnabled 
                                    && Managers.Network.Client.isActiveAndEnabled
                                    && UI_MainController_NetworkInvolved.IsStartBtnClicked)
        {
            if (Managers.Network.Server.IsHost)
            {
                ClientRPC_SendTransformToOpponent(_drumStickLeft.transform.rotation,_drumStickRight.transform.rotation);
            }
            if (!Managers.Network.Server.IsHost)
            {
                ServerRPC_SendTransformToHostServer(_drumStickLeft.transform.rotation,_drumStickRight.transform.rotation);
            }
         
        }
    }




    
    
    [ClientRpc]
    private void ClientRPC_SendTransformToOpponent(Quaternion left, Quaternion right)
    {
        NetworkIdentity NetId_DrumStick_Left = null;
        NetworkIdentity NetId_DrumStick_Right = null;

        if (!Managers.Network.Server.IsHost)
        {
            var DrumStick_Left = Managers.Network.Client.World.SpawnedIdentities.ToList().Find(x =>
            {
                return x.gameObject.name == "NetObj_Mockup_DrumStick_Left";
            });

           

            var DrumStick_Right = Managers.Network.Client.World.SpawnedIdentities.ToList().Find(x =>
            {
                return x.gameObject.name == "NetObj_Mockup_DrumStick_Right";
            });

            DrumStick_Left.gameObject.transform.rotation = left;
            DrumStick_Right.gameObject.transform.rotation = right;

            
        }
        
      
    }

    [ServerRpc(requireAuthority = false)]

    private void ServerRPC_SendTransformToHostServer(Quaternion left, Quaternion right)
    {
        if (Managers.Network.Server.IsHost)
        {
            NetworkIdentity clientNetId_Drumstick_Left;
            NetworkIdentity clientNetId_Drumstick_Right;
            
            clientNetId_Drumstick_Left =
                Managers.Network.Server.World.SpawnedIdentities.FirstOrDefault(x =>
                    x.gameObject.name == "NetObj_Mockup_DrumStick_Left");
            clientNetId_Drumstick_Right =
                Managers.Network.Server.World.SpawnedIdentities.FirstOrDefault(x =>
                    x.gameObject.name == "NetObj_Mockup_DrumStick_Right");
            

            clientNetId_Drumstick_Left.gameObject.transform.rotation = left;
            clientNetId_Drumstick_Right.gameObject.transform.rotation = right;
            
            Logger.Log("Rotation SentToHostServer ----- drum");
        }

    }
    
    
  







}
