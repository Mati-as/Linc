using System;
using DG.Tweening;
using Mirage;
using UnityEngine;

public class BeadsDrum_Controller : NetworkBehaviour
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
        
        BeadsDrum_NetworkGameManager.OnLeftDrumClicked -= OnLeftDrumClicked;
        BeadsDrum_NetworkGameManager.OnLeftDrumClicked += OnLeftDrumClicked;
        
        BeadsDrum_NetworkGameManager.OnRightDrumClicked -= OnRightDrumClicked;
        BeadsDrum_NetworkGameManager.OnRightDrumClicked += OnRightDrumClicked;
    }

    private void OnStartServer()
    {
        _beadsDrumLeft = transform.GetChild((int)BeadsDrum.Left);
        _beadsDrumRight = transform.GetChild((int)BeadsDrum.Right);
        _defaultSize = _beadsDrumLeft.localScale.x;

        var drumSticks = transform.GetChild(2);
        _drumStickLeft = drumSticks.GetChild((int)DrumStick.DrumstickLeft);
        _drurmStickRight = drumSticks.GetChild((int)DrumStick.DrumstickRight);

        _defaultQuatLeft = _drumStickLeft.rotation;
        _defaultQuatRight = _drurmStickRight.rotation;

#if UNITY_EDITOR
        Debug.Log("Server Start -----------------");        
#endif
    }

    private void OnDestroy()
    {
        BeadsDrum_NetworkGameManager.OnLeftDrumClicked -= OnLeftDrumClicked;
        BeadsDrum_NetworkGameManager.OnRightDrumClicked -= OnRightDrumClicked;
    }

    private void OnLeftDrumClicked()
    {
        if (IsLocalPlayer) // 클라이언트에서 발생
        {
            CmdPlayDrumAndStickAnimation(BeadsDrum.Left);
        }
    }

    private void OnRightDrumClicked()
    {
        if (IsLocalPlayer) // 클라이언트에서 발생
        {
            CmdPlayDrumAndStickAnimation(BeadsDrum.Right);
        }
    }

    [ServerRpc(requireAuthority = false)]
    private void CmdPlayDrumAndStickAnimation(BeadsDrum drum)
    {
        Debug.LogError("By Server");
        // 서버에서 동작 후, 클라이언트들에게 동기화
        RpcPlayDrumAndStickAnimation(drum);
    }

    [ClientRpc]
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
                (rotateAmount, 0, 0), 0.07f).SetEase(Ease.InOutSine))
            .AppendCallback(() =>
            {
                action?.Invoke();
            })
            .AppendInterval(0.02f)
            .Append(drum.DOScale(_shrinkAmount, 0.10f).SetEase(Ease.InOutSine))
            .AppendInterval(0.01f)
            .Append(drum.DOScale(_defaultSize, 0.08f).SetEase(Ease.InOutSine))
            .Append(stick.DORotateQuaternion(defaultRotation, 0.04f).SetEase(Ease.InOutSine))
            .AppendCallback(() => { _seq = null; });
    }
}
