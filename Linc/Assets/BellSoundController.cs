using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BellSoundController : MonoBehaviour
{

    private bool _isSoundable =true;
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Collider_SoundableCheckDetector_Left" || other.gameObject.name =="Collider_SoundableCheckDetector_Right")
        {
   

            if (_isSoundable)
            {
                _isSoundable = false;
                var randomChar = (char)Random.Range('A', 'D' + 1);
                Managers.Sound.Play(SoundManager.Sound.Effect, "Audio/Effect/Bell" +randomChar);
                //Logger.Log("SoundPlaying");
            }
    
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
            DOVirtual.Float(0, 0, 0.35f, _ => { }).OnComplete(() =>
            {
                _isSoundable = true;
                
            });
            
          
            // perform specific actions related to colliderRight
        }
        else
        {
            Logger.Log("it's different collider");
        }
    }
}
