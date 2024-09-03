using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class UI_ContentController : UI_Popup
{
    public enum Btns
    {
        Btn_Prev,
        Btn_Next,
        Btn_Depth1_Title,
        Btn_TopMenu_Hide,
        Btn_Script_Hide,
        Btn_ThirdDepth_Hide,
        Btn_Help,
        Btn_Evaluation,
     //  UI_Depth3_List, // Active Area
        //
        Depth3_A,
        Depth3_B,
        Depth3_C,
        Depth3_D,
        Depth3_E,
        
    }

    public enum Toggles
    {
        Toggle_Depth2_A,
        Toggle_Depth2_B,
        Toggle_Depth2_C,
        Toggle_Depth2_D,
        Toggle_Depth2_E
    }

    public enum UI
    {
        UI_Top,
        UI_TextBox,
        UI_Instruction,
        UI_Depth3_List,
        ActiveArea,
        InactiveAreaA,
        InactiveAreaB,
        InactiveAreaC,
        InactiveAreaD,
    }


    public enum Content_TMP
    {
        Text_Depth1_Title,
        Text_Current3Depth,
        Text_Instruction,
        Text_Depth2_A,
        Text_Depth2_B,
        Text_Depth2_C,
        Text_Depth2_D,
        Text_Depth2_E,
        Text_Depth3_A,
        Text_Depth3_B,
        Text_Depth3_C,
        Text_Depth3_D,
        Text_Depth3_E,
     
    }
    private TextMeshProUGUI[] texts = new TextMeshProUGUI[Enum.GetValues(typeof(Content_TMP)).Length];

    private Animator _depthOneTextMoveAnimator;
    private Animator _instructionAnimator;
    private Animator _topMenuAnimator;
    private readonly int UI_ON = Animator.StringToHash("On");
    private readonly int UI_Flip = Animator.StringToHash("Flip");

    private Toggle[] _depth2Toggles = new Toggle[Enum.GetValues(typeof(Toggles)).Length];
    private Button[] _depth3Btns = new Button[Enum.GetValues(typeof(Btns)).Length]; // 방어적으로 사이즈 크게 할당

    private readonly float _btnClickableDelay = 0.85f;
    private WaitForSeconds _waitForClick;
    private bool _clickable = true;


    // UI 클릭관련 설정
    private InputAction _mouseClickAction;
    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        if (Managers.UI.FindPopup<UI_Main>() != null) Managers.UI.ClosePopupUI(Managers.UI.FindPopup<UI_Main>());

        BindButton(typeof(Btns));
        BindToggle(typeof(Toggles));
        BindObject(typeof(UI));
        BindTMP(typeof(Content_TMP));

        // 1.상단 -----------------------------------------------------------------------------------------------
          
        _depthOneTextMoveAnimator = GetButton((int)Btns.Btn_Depth1_Title).gameObject.GetComponent<Animator>();
       
        
        _depthOneTextMoveAnimator.enabled = false;
        
        GetButton((int)Btns.Btn_Depth1_Title).gameObject.BindEvent(OnDepthOneTitleHover, Define.UIEvent.PointerEnter);
        GetButton((int)Btns.Btn_Depth1_Title).gameObject
            .BindEvent(OnDepthOneTitleHoverExit, Define.UIEvent.PointerExit);
       
        
        
        
        GetToggle((int)Toggles.Toggle_Depth2_A).gameObject
            .BindEvent(() => { OnDepth2Clicked(1); });
        _depth2Toggles[(int)Toggles.Toggle_Depth2_A] = GetToggle((int)Toggles.Toggle_Depth2_A);
            
        GetToggle((int)Toggles.Toggle_Depth2_B).gameObject
            .BindEvent(() => { OnDepth2Clicked(2); });
        _depth2Toggles[(int)Toggles.Toggle_Depth2_B] = GetToggle((int)Toggles.Toggle_Depth2_B);
        
        GetToggle((int)Toggles.Toggle_Depth2_C).gameObject
            .BindEvent(() => { OnDepth2Clicked(3);});
        _depth2Toggles[(int)Toggles.Toggle_Depth2_C] = GetToggle((int)Toggles.Toggle_Depth2_C);
        
        GetToggle((int)Toggles.Toggle_Depth2_D).gameObject
            .BindEvent(() => { OnDepth2Clicked(4);});
        _depth2Toggles[(int)Toggles.Toggle_Depth2_D] = GetToggle((int)Toggles.Toggle_Depth2_D);
        
        GetToggle((int)Toggles.Toggle_Depth2_E).gameObject
            .BindEvent(() => { OnDepth2Clicked(5);});
        _depth2Toggles[(int)Toggles.Toggle_Depth2_E] = GetToggle((int)Toggles.Toggle_Depth2_E);
        
        _topMenuAnimator = GetObject((int)UI.UI_Top).gameObject.GetComponent<Animator>();
        GetButton((int)Btns.Btn_TopMenu_Hide).gameObject
            .BindEvent(() => { OnTopMenuAnimBtnClicked(); });
        _topMenuAnimator.SetBool(UI_ON, true);

        Debug.Log($" UI Text Lengths {texts.Length} ");

        GetObject((int)UI.UI_Depth3_List).BindEvent(OnDepth3BtnAreaEnter);

     
        
        // 2.중단(Depth3) UI --------------------------------------------------------------------------------------
        GetButton((int)Btns.Btn_ThirdDepth_Hide).gameObject
            .BindEvent(() => { OnDepthThirdHideBtnHover(); }, Define.UIEvent.PointerEnter);

        GetButton((int)Btns.Btn_ThirdDepth_Hide).gameObject
            .BindEvent(() => { OnDepthThirdHideBtnExit(); }, Define.UIEvent.PointerExit);
        
        GetObject((int)UI.UI_Depth3_List).gameObject.SetActive(false);
        // 3. 앞,뒤 버튼  -------------------------------------------------------------------------------------------
        GetButton((int)Btns.Btn_Prev).gameObject
            .BindEvent(() => { OnPrevBtnClicked(); });
        GetButton((int)Btns.Btn_Next).gameObject
            .BindEvent(() => { OnNextBtnClicked(); });


        // 4.하단 및 스크립트  --------------------------------------------------------------------------------------
        GetObject((int)UI.UI_Instruction).GetComponent<Text>();

        GetButton((int)Btns.Btn_Script_Hide).gameObject
            .BindEvent(() => { OnInstructionClicked(); });
        
        _instructionAnimator = GetObject((int)UI.UI_Instruction).GetComponent<Animator>();
        _instructionAnimator.SetBool(UI_ON, true);
        

        // 5.기타 버튼  ---------------------------------------------------------------------------------------------

        _depth3Btns[(int)Btns.Depth3_A] = GetButton((int)Btns.Depth3_A);
        _depth3Btns[(int)Btns.Depth3_B] = GetButton((int)Btns.Depth3_B);
        _depth3Btns[(int)Btns.Depth3_C] = GetButton((int)Btns.Depth3_C);
        _depth3Btns[(int)Btns.Depth3_D] = GetButton((int)Btns.Depth3_D);
        _depth3Btns[(int)Btns.Depth3_E] = GetButton((int)Btns.Depth3_E);
        

        
        GetButton((int)Btns.Depth3_A).gameObject
            .BindEvent(() => { OnDepth3BtnClicked(1); });
        GetButton((int)Btns.Depth3_B).gameObject
            .BindEvent(() => { OnDepth3BtnClicked(2); });
        GetButton((int)Btns.Depth3_C).gameObject
            .BindEvent(() => { OnDepth3BtnClicked(3); });
        GetButton((int)Btns.Depth3_D).gameObject
            .BindEvent(() => { OnDepth3BtnClicked(4); });
        GetButton((int)Btns.Depth3_E).gameObject
            .BindEvent(() => { OnDepth3BtnClicked(5); });
        
        
        GetButton((int)Btns.Depth3_A).gameObject
            .BindEvent(() => { OnDepth3BtnEnter(); },Define.UIEvent.PointerEnter);
        GetButton((int)Btns.Depth3_B).gameObject
            .BindEvent(() => { OnDepth3BtnEnter(); },Define.UIEvent.PointerEnter);
        GetButton((int)Btns.Depth3_C).gameObject
            .BindEvent(() => { OnDepth3BtnEnter(); },Define.UIEvent.PointerEnter);
        GetButton((int)Btns.Depth3_D).gameObject
            .BindEvent(() => { OnDepth3BtnEnter(); },Define.UIEvent.PointerEnter);
        GetButton((int)Btns.Depth3_E).gameObject
            .BindEvent(() => { OnDepth3BtnEnter(); },Define.UIEvent.PointerEnter);
        
        
        GetButton((int)Btns.Depth3_A).gameObject
            .BindEvent(() => { OnDepth3ABtnExit(); },Define.UIEvent.PointerExit);
        GetButton((int)Btns.Depth3_B).gameObject
            .BindEvent(() => { OnDepth3ABtnExit(); },Define.UIEvent.PointerExit);
        GetButton((int)Btns.Depth3_C).gameObject
            .BindEvent(() => { OnDepth3ABtnExit(); },Define.UIEvent.PointerExit);
        GetButton((int)Btns.Depth3_D).gameObject
            .BindEvent(() => { OnDepth3ABtnExit(); },Define.UIEvent.PointerExit);
        GetButton((int)Btns.Depth3_E).gameObject
            .BindEvent(() => { OnDepth3ABtnExit(); },Define.UIEvent.PointerExit);
   
        
        GetObject((int)UI.ActiveArea)
            .BindEvent(() => { OnDpeth3ActiveAreaEnter(); },Define.UIEvent.PointerEnter);
        
        
        
        GetObject((int)UI.InactiveAreaA)
            .BindEvent(() => { InactiveAreaEnter(); },Define.UIEvent.PointerEnter);

        GetObject((int)UI.InactiveAreaB)
            .BindEvent(() => { InactiveAreaEnter(); },Define.UIEvent.PointerEnter);

        GetObject((int)UI.InactiveAreaC)
            .BindEvent(() => { InactiveAreaEnter(); },Define.UIEvent.PointerEnter);

        GetObject((int)UI.InactiveAreaD)
            .BindEvent(() => { InactiveAreaEnter(); },Define.UIEvent.PointerEnter);

        
    
        _heightPerDepth3Btn = GetButton((int)Btns.Depth3_A).GetComponent<RectTransform>().sizeDelta.y;
        _activeAreaRect = GetObject((int)UI.ActiveArea).GetComponent<RectTransform>();
       
        
        for (int i = 0; i < texts.Length; i++)
        {
           
            texts[i] = GetTMP(i);
        }

       
        GetButton((int)Btns.Btn_Help).gameObject.BindEvent(() => { Managers.UI.ShowPopupUI<UI_Help>();});
        GetButton((int)Btns.Btn_Evaluation).gameObject.BindEvent(() => { Managers.UI.ShowPopupUI<UI_Evaluation>();});

        GetToggle((int)Toggles.Toggle_Depth2_A).isOn = true;
        Refresh();
        return true;
    }

    private void RefreshText()
    {
        var depth1 = Managers.ContentInfo.PlayData.Depth1.ToString();
        texts[(int)Content_TMP.Text_Depth1_Title].text = Managers.Data.Texts[int.Parse(depth1 + "0000")].kor;
        var depth1Int = Managers.ContentInfo.PlayData.Depth1;
        
        var depth2 = Managers.ContentInfo.PlayData.Depth2.ToString();
        var depth2Int = Managers.ContentInfo.PlayData.Depth2;
       // Debug.Log($"depth 2 count : {ContentPlayData.DEPTH_TWO_COUNT_DATA[depth1Int]}");
        //Debug.Log($"depth 3 count : {ContentPlayData.DEPTH_THREE_COUNT_DATA[int.Parse(depth1+depth2)]}");

        var depth2Count = 1;
        for (var i = (int)Content_TMP.Text_Depth2_A;
             i < (int)Content_TMP.Text_Depth2_A + ContentPlayData.DEPTH_TWO_COUNT_DATA[depth1Int] ;
             i++)
        {
            texts[i].text = Managers.Data.Texts[int.Parse(depth1 + "00" + depth2Count+ "0")].kor;
            depth2Count++;
            //Debug.Log($"Depth2 텍스트 변환 완료 :{ texts[i].gameObject.name} : { texts[i].text}");
        }

      
        var depth3Count = 1;
        for (var i = (int)Content_TMP.Text_Depth3_A;
             i < (int)Content_TMP.Text_Depth3_A + ContentPlayData.DEPTH_THREE_COUNT_DATA[int.Parse(depth1+depth2)] ;
             i++)
        {
            
            texts[i].text = Managers.Data.Texts[int.Parse(depth1 + "00" + depth2 + depth3Count)].kor;
            depth3Count++;
           // Debug.Log($"Depth3 텍스트 변환 완료 : { texts[i].text}");
        }
        
        Debug.Log($"{texts[(int)Content_TMP.Text_Current3Depth].text =Managers.Data.Texts[int.Parse((Managers.ContentInfo.PlayData.CurrentDepthStatus[0] + "00"+Managers.ContentInfo.PlayData.CurrentDepthStatus[1] + 1))].kor}");
        
        texts[(int)Content_TMP.Text_Current3Depth].text =
            Managers.Data.Texts[int.Parse((Managers.ContentInfo.PlayData.CurrentDepthStatus[0] + "00"
                + Managers.ContentInfo.PlayData.CurrentDepthStatus[1] + 1))].kor;
    }

    
    private void OnPrevBtnClicked()
    {
        Precheck();

        if (Managers.ContentInfo.PlayData.Count < 1)
        {
#if UNITY_EDITOR
            Debug.Log("current count is 0 ---------");
            return;
#endif
        }
        Managers.ContentInfo.PlayData.Count--;
        ChangeInstructionTextWithAnim();
    }

    private void OnNextBtnClicked()
    {
        Precheck();
        if (Managers.ContentInfo.PlayData.Count >= ContentPlayData.COUNT_MAX)
        {
#if UNITY_EDITOR
            Debug.Log("current count is Max ---------");
            return;
#endif
        }
        Managers.ContentInfo.PlayData.Count++;
        ChangeInstructionTextWithAnim();
    }

    private void ChangeInstructionTextWithAnim()
    {
        StartCoroutine(ChangeTextWithAnimCo());
    }

    private WaitForSeconds animDelay;
    IEnumerator ChangeTextWithAnimCo()
    {
      
        _instructionAnimator.SetTrigger(UI_Flip);
        if (animDelay == null)
        {
            animDelay = new WaitForSeconds(0.15f);
        }
        yield return animDelay;
        
        texts[(int)Content_TMP.Text_Instruction].text = Managers.Data.Texts[int.Parse(Managers.ContentInfo.PlayData.CurrentDepthStatus)].kor;
    }


    private void OnDepth2Clicked(int depth2)
    {
        Precheck();
        Managers.ContentInfo.PlayData.Depth2 = depth2;
        Managers.ContentInfo.PlayData.Depth3 = 1;

        ChangeInstructionTextWithAnim();

        

        RefreshUI();
        RefreshText();
    }

    private RectTransform _activeAreaRect;
    private float _heightPerDepth3Btn;

    private void RefreshUI()
    {
        Debug.Log($"Refreshing UI : CurrentStatus: {Managers.ContentInfo.PlayData.CurrentDepthStatus}");
        for (int i = (int)Btns.Depth3_A; i < (int)Btns.Depth3_E + 1; i++)
        {
            _depth3Btns[i].gameObject.SetActive(false);
        } 
        
        for (int i = (int)Toggles.Toggle_Depth2_A; i < (int)Toggles.Toggle_Depth2_E +1; i++)
        {
            _depth2Toggles[i].gameObject.SetActive(false);
        }

       
       // Debug.Log($"Depth2Toggle Active 갯수: { ContentPlayData.DEPTH_TWO_COUNT_DATA[int.Parse(Managers.ContentInfo.PlayData.CurrentDepthStatus[0].ToString())]}");
        for (int i = (int)Toggles.Toggle_Depth2_A; i < (int)Toggles.Toggle_Depth2_A 
             + ContentPlayData.DEPTH_TWO_COUNT_DATA[int.Parse(Managers.ContentInfo.PlayData.CurrentDepthStatus[0].ToString())] ; i++)
        {
            _depth2Toggles[i].gameObject.SetActive(true);
        }
        
        var currentDepth12 = Managers.ContentInfo.PlayData.CurrentDepthStatus[0].ToString() +
                             Managers.ContentInfo.PlayData.CurrentDepthStatus[1].ToString();
        
     
        //Debug.Log($"Depth3  Active 갯수: {ContentPlayData.DEPTH_THREE_COUNT_DATA[int.Parse(currentDepth12)]}");
        for (int i = (int)Btns.Depth3_A; i < (int)Btns.Depth3_A + ContentPlayData.DEPTH_THREE_COUNT_DATA[int.Parse(currentDepth12)]; i++)
        {
            _depth3Btns[i].gameObject.SetActive(true);
        }


        var resizedHeight = new Vector2(_activeAreaRect.sizeDelta.x, (_heightPerDepth3Btn * 1.45f) *
            (ContentPlayData.DEPTH_THREE_COUNT_DATA[
                int.Parse(currentDepth12)]) + 1);
        _activeAreaRect.sizeDelta = resizedHeight;
        Debug.Log($"_activeAreaRect Height: {_activeAreaRect.sizeDelta.y}");
    }

    private void ResizeActiveArea()
    {
        
    }
    /// <summary>
    /// Text,UI  total Refresh
    /// </summary>
    public void Refresh()
    {
        RefreshUI();
        RefreshText();
        texts[(int)Content_TMP.Text_Instruction].text = Managers.Data.Texts[int.Parse(Managers.ContentInfo.PlayData.CurrentDepthStatus)].kor;
    }

    private void OnDepthOneTitleHover()
    {
        Precheck();
        _depthOneTextMoveAnimator.enabled = true;
    }

    private void OnDepthOneTitleHoverExit()
    {
        Precheck();
        _depthOneTextMoveAnimator.Rebind();
        _depthOneTextMoveAnimator.enabled = false;
    }
    


    private void SetClickable()
    {
        StartCoroutine(SetClickableCo());
    }

    private IEnumerator SetClickableCo()
    {
        _clickable = false;
        if (_waitForClick == null) _waitForClick = new WaitForSeconds(_btnClickableDelay);

        yield return _waitForClick;
        _clickable = true;
    }


    private void OnDepthThirdHideBtnHover()
    {
   

    }

    private bool isOnActiveArea;// 뎁스3내용 Hover시 표출. 표출이후에도 내용범위에 머물러있으면 Hover상태 유지
    private bool isOn3depthArea;
    private void InactiveAreaEnter()
    {

        isOnActiveArea = false;

        if (isOnActiveArea) return;
            GetObject((int)UI.UI_Depth3_List).gameObject.SetActive(false);
            Debug.Log("3depth UI off ---------------------");
        
      
       
    }

    private void OnDpeth3ActiveAreaEnter()
    {
        if (_isTopMenuOn)
        {
            Debug.Log("3depth UI On ---------------------");
            isOnActiveArea = true;
            GetObject((int)UI.UI_Depth3_List).gameObject.SetActive(true);
        }
       
    }


    private void OnDepth3BtnClicked(int depth3Num)
    {
       
        Precheck();
        Managers.ContentInfo.PlayData.Depth3 = depth3Num;
      //  Managers.ContentInfo.PlayData.ResetOrSetDepthCount(depth3Num);

#if UNITY_EDITOR
        Debug.Log($"current Status{Managers.ContentInfo.PlayData.CurrentDepthStatus}");
#endif
        
        for (int i = (int)Btns.Depth3_A; i < (int)Btns.Depth3_E + 1; i++)
        {
            _depth3Btns[i].gameObject.SetActive(false);
        } 
        var currentDepth12 = Managers.ContentInfo.PlayData.CurrentDepthStatus[0].ToString() +
                             Managers.ContentInfo.PlayData.CurrentDepthStatus[1].ToString();
        Debug.Log($"depthCount {currentDepth12}");
        
        for (int i = (int)Btns.Depth3_A; i < (int)Btns.Depth3_A + ContentPlayData.DEPTH_THREE_COUNT_DATA[int.Parse(currentDepth12)]; i++)
        {
            _depth3Btns[i].gameObject.SetActive(true);
        }
        Debug.Log(
            $"depth 3 current: {texts[(int)Content_TMP.Text_Current3Depth].text = Managers.Data.Texts[int.Parse((Managers.ContentInfo.PlayData.CurrentDepthStatus[0] + "00" + Managers.ContentInfo.PlayData.CurrentDepthStatus[1] + 1))].kor}");
        
      
        Managers.ContentInfo.PlayData.Depth3 = depth3Num;
        texts[(int)Content_TMP.Text_Current3Depth].text =
            Managers.Data.Texts[int.Parse((Managers.ContentInfo.PlayData.CurrentDepthStatus[0] + "00"
                + Managers.ContentInfo.PlayData.CurrentDepthStatus[1] + depth3Num.ToString()))].kor;
        Refresh();
        ChangeInstructionTextWithAnim();
        
     
        
    }
    private void OnDepth3BtnEnter()
    {
       
    }
    
    private void OnDepth3BtnAreaEnter()
    {
       
        // OnAnimation
    }

    
    private void OnDepth3ABtnExit()
    {
        isOn3depthArea = false;
        //Animatiorn
    }

    
    
    private void OnDepth3BtnExit()
    {
      
        // OnAnimation
    }
    private void OnDepthThirdHideBtnExit()
    {
       
    
    }

    private void Precheck()
    {
        if (!_clickable)
        {
#if UNITY_EDITOR
            Debug.Log("Clicking Too Fast");
#endif
            return;
        }

        SetClickable();
    }

    private bool _isInstructAnimOn = true;
    private bool _isTopMenuOn = true;

    private void OnInstructionClicked()
    {
        Precheck();
        _isInstructAnimOn = !_isInstructAnimOn;
        _instructionAnimator.SetBool(UI_ON, _isInstructAnimOn);
#if UNITY_EDITOR
        Debug.Log($" Current Script UI Status{_isInstructAnimOn}");
#endif
    }
    
    private void OnTopMenuAnimBtnClicked()
    {
        Precheck();
        _isTopMenuOn = !_isTopMenuOn;
        _topMenuAnimator.SetBool(UI_ON, _isTopMenuOn);
#if UNITY_EDITOR
        Debug.Log($" topMenu Status: {_isTopMenuOn}");
#endif
    }
}