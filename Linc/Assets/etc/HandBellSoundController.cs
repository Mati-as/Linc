using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class HandBellSoundController : MonoBehaviour
{

    private bool _isSoundable =true;
    private WaitForSeconds _wait;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Collider_SoundableCheckDetector_Left" || other.gameObject.name =="Collider_SoundableCheckDetector_Right")
        {
            // perform specific actions related to colliderRight
        }
        else
        {
            Logger.Log("it's different collider");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Collider_SoundableCheckDetector_Left" || other.gameObject.name =="Collider_SoundableCheckDetector_Right")
        {
            if (_isSoundable && Managers.DeviceManager.StickData.AccelerationValue > 0.8f)
            {
                _isSoundable = false;
                var randomChar = (char)Random.Range('A', 'D' + 1);
                Managers.Sound.Play(SoundManager.Sound.Effect, "Audio/Effect/Bell" +randomChar);
                StartCoroutine(VibrateHapticStickCo());
            }
        }
        else
        {
            Logger.Log("it's different collider");
        }
    }

    IEnumerator VibrateHapticStickCo()
    {
        if (_wait == null)
        {
            _wait = new WaitForSeconds(0.135f);
        }
        if (Managers.DeviceManager.IsConnected)
        {
            Managers.DeviceManager.On_Data_Only();
            Managers.DeviceManager.SendDataAndVibrate();
            yield return _wait;
            Managers.DeviceManager.HapticStick_Off();
            Managers.DeviceManager.On_Data_Only();
        }
   
        _isSoundable = true;
    }
}
