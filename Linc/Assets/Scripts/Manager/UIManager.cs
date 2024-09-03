using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager
{
    
	int _order = -20;

	public static readonly bool KOR = false;
	public static readonly bool ENG = true;
	public static readonly bool FULLSCREEN_MODE = true;
	public static readonly bool WINDOWED_MODE = false;
	public static readonly bool CONTROL_GUIDE_ON = true;
	public static readonly bool CONTROL_GUIDE_OFF = false;
	
	private bool _isFullScreen;
	private bool _isGuideOn;

	public bool isFullScreen { get; set; }
	
	private int _languageSetting;
	
	
// 	
// 	//Cavnas 관련
// 	public GraphicRaycaster GR;
// 	public PointerEventData PED { get; private set; }
// 	public List<RaycastResult> RaycastResults { get; protected set; }
// 	public Vector3 screenPosition;
// 	public Button btn;
// 	public InputAction _mouseAction;
// 	public void Init()
// 	{
// 		_mouseAction = new InputAction("Space", binding: "<Mouse>/leftButton", interactions: "press");
// 		_mouseAction.performed += OnKeyPressed;
// 	}
// 	public void OnKeyPressed(InputAction.CallbackContext context)
// 	{
// 		//UI클릭을 위한 RayCast를 발생 및 Ray저장 
// 		ShootRay();
// 	}
// 	public virtual void ShootRay()
// 	{
// 		//마우스 및 포인터 위치를 기반으로 하고싶은경우.
// 		screenPosition = Mouse.current.position.ReadValue();
//
// 		PED.position = screenPosition;
//
// 		RaycastResults = new List<RaycastResult>();
// 		GR.Raycast(PED, RaycastResults);
//         
// 		foreach (RaycastResult result in RaycastResults)
// 		{
// 			result.gameObject.TryGetComponent(out btn);
// 			btn?.onClick?.Invoke();
//             
// 			result.gameObject.TryGetComponent(out UI_EventHandler eventHandler);
// 			eventHandler?.OnClickHandler?.Invoke();
// 		}
//
//
//
// #if UNITY_EDITOR
//
// #endif
// 	}
	public int languageSetting
	{
		get
		{
			return _languageSetting;
		}
		set
		{
			if (value != _languageSetting)
			{
				_languageSetting = value;
		
			}
		}
	}

	public bool GuideOn { get; set; }

	public Stack<UI_Popup> PopupStack = new Stack<UI_Popup>();

	public UI_Scene SceneUI { get; private set; }



	public GameObject Root
	{
		get
		{
			GameObject root = GameObject.Find("@UI_Root");
			if (root == null)
				root = new GameObject { name = "@UI_Root" };

			return root;
		}
	}

	public void SetCanvas(GameObject go, bool sort = true)
	{
		Canvas canvas = Utils.GetOrAddComponent<Canvas>(go);
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		canvas.overrideSorting = true;

		if (sort)
		{
			canvas.sortingOrder = _order;
			_order++;
		}
		else
		{
			canvas.sortingOrder = 0;
		}
	}

	public T MakeSubItem<T>(Transform parent = null, string name = null) where T : UI_Base
	{
		if (string.IsNullOrEmpty(name))
			name = typeof(T).Name;

		GameObject prefab = Managers.Resource.Load<GameObject>($"Prefabs/UI/SubItem/{name}");

		GameObject go = Managers.Resource.Instantiate(prefab);
		if (parent != null)
			go.transform.SetParent(parent);

		go.transform.localScale = Vector3.one;
		go.transform.localPosition = prefab.transform.position;

		return Utils.GetOrAddComponent<T>(go);
	}

	public T ShowSceneUI<T>(string name = null) where T : UI_Scene
	{
		if (string.IsNullOrEmpty(name))
			name = typeof(T).Name;

		GameObject go = Managers.Resource.Instantiate($"UI/Scene/{name}");
		T sceneUI = Utils.GetOrAddComponent<T>(go);
		SceneUI = sceneUI;

		go.transform.SetParent(Root.transform);

		return sceneUI;
	}

	public T ShowPopupUI<T>(string name = null, Transform parent = null) where T : UI_Popup
	{
		if (string.IsNullOrEmpty(name))
			name = typeof(T).Name;

		GameObject prefab = Managers.Resource.Load<GameObject>($"Prefabs/UI/Popup/{name}");

		GameObject go = Managers.Resource.Instantiate($"UI/Popup/{name}");
		T popup = Utils.GetOrAddComponent<T>(go);
		PopupStack.Push(popup);

		if (parent != null)
			go.transform.SetParent(parent);
		else if (SceneUI != null)
			go.transform.SetParent(SceneUI.transform);
		else
			go.transform.SetParent(Root.transform);

		go.transform.localScale = Vector3.one;
		go.transform.localPosition = prefab.transform.position;

		return popup;
	}

	public T FindPopup<T>() where T : UI_Popup
	{
		return PopupStack.Where(x => x.GetType() == typeof(T)).FirstOrDefault() as T;
	}

	public T PeekPopupUI<T>() where T : UI_Popup
	{
		if (PopupStack.Count == 0)
			return null;

		return PopupStack.Peek() as T;
	}

	public void ClosePopupUI(UI_Popup popup)
	{
		if (PopupStack.Count == 0)
			return;

		if (PopupStack.Peek() != popup)
		{
			Debug.Log($"Close Popup Failed! {PopupStack.Peek().gameObject.name}");
			return;
		}

		ClosePopupUI();
	}

	public void ClosePopupUI()
	{
		if (PopupStack.Count == 0)
			return;

		UI_Popup popup = PopupStack.Pop();
		Managers.Resource.Destroy(popup.gameObject);
		popup = null;
		_order--;
	}

	public void CloseAllPopupUI()
	{
		while (PopupStack.Count > 0)
			ClosePopupUI();
	}

	public void Clear()
	{
		CloseAllPopupUI();
		SceneUI = null;
	}
	
	public void SetResolution(int width, int height, bool fullscreen)
	{
		Screen.SetResolution(width, height, fullscreen);
		isFullScreen = fullscreen;
		// UI 레이아웃이나 스케일 조정을 위한 추가 로직을 여기에 추가
	}
	
	public void SetGraphicQuality(Define.QaulityLevel qualityLevel)
	{
     	QualitySettings.SetQualityLevel((int)qualityLevel);
		// UI 레이아웃이나 스케일 조정을 위한 추가 로직을 여기에 추가
	}

	public void SetScreenMode(bool isFullScreenMode)
	{

		Screen.fullScreen = isFullScreenMode;
		isFullScreen = isFullScreenMode;
		Managers.Data.Preference[(int)Define.Preferences.Fullscreen] = isFullScreenMode? Define.YES : Define.NO ;
#if UNITY_EDITOR
		Debug.Log($"Full Screen mode is {isFullScreenMode} :Preference{	Managers.Data.Preference[(int)Define.Preferences.Fullscreen]}");
#endif
	}
	
	

	public void SetEngMode(bool isEngMode)
	{

#if UNITY_EDITOR
		string modename = isEngMode ? "ENG" : "KOR";
		Debug.Log($"EngMode is  {isEngMode}");
#endif
		
		Managers.Data.Preference[(int)Define.Preferences.EngMode] = isEngMode? Define.YES  : Define.NO;
	
	}

	public void SetControlGuideOnMode(bool isGuideOn)
	{
#if UNITY_EDITOR
		Debug.Log($"Control Guide Mode is {isGuideOn}");
#endif
		this.GuideOn = isGuideOn;
		Managers.Data.Preference[(int)Define.Preferences.ControlGuide] = GuideOn? Define.YES : Define.NO ;
	}
}
