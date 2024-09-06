using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Loading : UI_Popup
{
   public enum Sliders
   {
      Slider_LoadingBar
   }

   private Slider _loadingSlider;
   private TextMeshProUGUI _tmp;
   public override bool Init()
   {
      if (base.Init() == false) return false;
      
      BindSlider(typeof(Sliders));

       _loadingSlider =  GetSlider((int)Sliders.Slider_LoadingBar);
       
       _tmp = _loadingSlider.gameObject.GetComponentInChildren<TextMeshProUGUI>();

      DOVirtual.Float(0, _loadingSlider.maxValue, 0.8f, val =>
      {
         _loadingSlider.value = val;
         _tmp.text = $"Loading....{(int)( (val/_loadingSlider.maxValue) * 100 )}%";
      }).OnComplete(() =>
      {
         Managers.UI.ClosePopupUI(this);
         Managers.UI.ShowPopupUI<UI_MultiModeSelection>();
        
      });
      return true;
   }
}
