using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

public interface ILoader<Key, Item>
{
    Dictionary<Key, Item> MakeDict();
    bool Validate();
}

public class DataManager
{
    public Dictionary<int, TextData> Texts { get; private set; }

    public  float[] Preference = new float[Enum.GetValues(typeof(Define.Preferences)).Length];



    private readonly string SavedData;
    private readonly string Preferences;


    public void Init()
    {
      
        SetXMLPath();
        CheckAndGenerateXmlFile(nameof(SavedData), settingXmlPath);
        LoadSettingParams();

     

        Texts = LoadXml<TextDataLoader, int, TextData>($"{nameof(TextData)}").MakeDict();
    }

    private Item LoadSingleXml<Item>(string name)
    {
        var xs = new XmlSerializer(typeof(Item));
        var textAsset = Resources.Load<TextAsset>("Data/" + name);
        using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(textAsset.text)))
        {
            return (Item)xs.Deserialize(stream);
        }
    }

    private Loader LoadXml<Loader, Key, Item>(string name) where Loader : ILoader<Key, Item>, new()
    {
        var xs = new XmlSerializer(typeof(Loader));
        var textAsset = Resources.Load<TextAsset>("Data/" + name);
        using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(textAsset.text)))
        {
            return (Loader)xs.Deserialize(stream);
        }
    }


    #region XML을 통한 세팅 초기화 및 저장

    public  XmlDocument xmlDoc_Setting;
    public  string settingXmlPath;


    private void SetXMLPath()
    {
        settingXmlPath = Path.Combine(Application.persistentDataPath, nameof(SavedData) + ".xml");

#if UNITY_EDITOR
        Debug.Log($"current SavedData Path: {settingXmlPath} ");
#endif
    }

    public void LoadSettingParams()
    {
 // read the xml document
    Utils.ReadXML(ref xmlDoc_Setting, settingXmlPath);
    XmlNode root = xmlDoc_Setting.DocumentElement;
    var node = root.SelectSingleNode(nameof(SavedData));
    
    // check if the node exists
    if (node == null)
    {
        Debug.LogWarning($"no {nameof(SavedData)} node found in the settings file.");
        return;
    }

    // load main volume setting
    var mainVol = node.Attributes[nameof(Define.Preferences.MainVol)].Value;
    Managers.Data.Preference[(int)Define.Preferences.MainVol] = float.Parse(mainVol);
    
    // load bgm volume setting
    var bgmVol = node.Attributes[nameof(Define.Preferences.BgmVol)].Value;
    Managers.Data.Preference[(int)Define.Preferences.BgmVol] = float.Parse(bgmVol);
    
    // load effect volume setting
    var effectVol = node.Attributes[nameof(Define.Preferences.EffectVol)].Value;
    Managers.Data.Preference[(int)Define.Preferences.EffectVol] = float.Parse(effectVol);
    
    // load narration volume setting
    var narrationVol = node.Attributes[nameof(Define.Preferences.NarrationVol)].Value;
    Managers.Data.Preference[(int)Define.Preferences.NarrationVol] = float.Parse(narrationVol);
    
    // load fullscreen setting
    var fullscreen = node.Attributes[nameof(Define.Preferences.Fullscreen)].Value;
    Managers.Data.Preference[(int)Define.Preferences.Fullscreen] = int.Parse(fullscreen);
    Debug.Log($"fullscreen Value On Load-----------------------{fullscreen}");
    
    // load resolution setting
    var resolution = node.Attributes[nameof(Define.Preferences.Resolution)].Value;
    Managers.Data.Preference[(int)Define.Preferences.Resolution] = int.Parse(resolution);
    
    // load graphic quality setting
    var graphicQuality = node.Attributes[nameof(Define.Preferences.GraphicQuality)].Value;
    Managers.Data.Preference[(int)Define.Preferences.GraphicQuality] = int.Parse(graphicQuality);
    
    // load english mode setting
    var engMode = node.Attributes[nameof(Define.Preferences.EngMode)].Value;
    Managers.Data.Preference[(int)Define.Preferences.EngMode] = int.Parse(engMode);
    
    // load control guide setting
    var controlGuide = node.Attributes[nameof(Define.Preferences.ControlGuide)].Value;
    Managers.Data.Preference[(int)Define.Preferences.ControlGuide] = int.Parse(controlGuide);
    
    // load mute main setting
    var muteMain = node.Attributes[nameof(Define.Preferences.Mute_Main)].Value;
    Managers.Data.Preference[(int)Define.Preferences.Mute_Main] = int.Parse(muteMain);
    
    // load mute narration setting
    var muteNarration = node.Attributes[nameof(Define.Preferences.Mute_Narration)].Value;
    Managers.Data.Preference[(int)Define.Preferences.Mute_Narration] = int.Parse(muteNarration);
    
    // load mute effect setting
    var muteEffect = node.Attributes[nameof(Define.Preferences.Mute_Effect)].Value;
    Managers.Data.Preference[(int)Define.Preferences.Mute_Effect] = int.Parse(muteEffect);
    
    // load mute bgm setting
    var muteBgm = node.Attributes[nameof(Define.Preferences.Mute_Bgm)].Value;
    Managers.Data.Preference[(int)Define.Preferences.Mute_Bgm] = int.Parse(muteBgm);

    Debug.Log("settings loaded successfully.");
    }

    public void SaveCurrentSetting()
    {
        var tempRootSetting = xmlDoc_Setting.DocumentElement;
        tempRootSetting.RemoveAll();

        var setting = xmlDoc_Setting.CreateElement(nameof(SavedData));
        setting.SetAttribute(nameof(Define.Preferences.MainVol),
            Managers.Data.Preference[(int)Define.Preferences.MainVol].ToString("F2",CultureInfo.InvariantCulture));

        setting.SetAttribute(nameof(Define.Preferences.BgmVol),
            Managers.Data.Preference[(int)Define.Preferences.BgmVol].ToString("F2",CultureInfo.InvariantCulture));

        setting.SetAttribute(nameof(Define.Preferences.EffectVol),
            Managers.Data.Preference[(int)Define.Preferences.EffectVol].ToString("F2",CultureInfo.InvariantCulture));

        setting.SetAttribute(nameof(Define.Preferences.NarrationVol),
            Managers.Data.Preference[(int)Define.Preferences.NarrationVol].ToString("F2",CultureInfo.InvariantCulture));

        setting.SetAttribute(nameof(Define.Preferences.Fullscreen),
            Managers.Data.Preference[(int)Define.Preferences.Fullscreen].ToString());

        setting.SetAttribute(nameof(Define.Preferences.Resolution),
            Managers.Data.Preference[(int)Define.Preferences.Resolution].ToString());

        setting.SetAttribute(nameof(Define.Preferences.GraphicQuality),
            Managers.Data.Preference[(int)Define.Preferences.GraphicQuality].ToString());

        setting.SetAttribute(nameof(Define.Preferences.EngMode),
            Managers.Data.Preference[(int)Define.Preferences.EngMode].ToString());

        setting.SetAttribute(nameof(Define.Preferences.ControlGuide),
            Managers.Data.Preference[(int)Define.Preferences.ControlGuide].ToString());

        setting.SetAttribute(nameof(Define.Preferences.Mute_Main),
            Managers.Data.Preference[(int)Define.Preferences.Mute_Main].ToString());

        setting.SetAttribute(nameof(Define.Preferences.Mute_Narration),
            Managers.Data.Preference[(int)Define.Preferences.Mute_Narration].ToString());

        setting.SetAttribute(nameof(Define.Preferences.Mute_Effect),
            Managers.Data.Preference[(int)Define.Preferences.Mute_Effect].ToString());

        setting.SetAttribute(nameof(Define.Preferences.Mute_Bgm),
            Managers.Data.Preference[(int)Define.Preferences.Mute_Bgm].ToString());

        tempRootSetting.AppendChild(setting);

        int count = 0;
        foreach (var val in Managers.Data.Preference)
        {
            Debug.Log(($"save: {(Define.Preferences)(count)} is {Managers.Data.Preference[count]}"));
            count++;
        }
        WriteXML(xmlDoc_Setting, settingXmlPath);
    }



    public void CheckAndGenerateXmlFile(string fileName, string path)
    {
        if (File.Exists(path))
        {
            Debug.Log(fileName + "XML FILE EXIST");
            Utils.ReadXML(ref xmlDoc_Setting, settingXmlPath);
        }
        else
        {
            var newXml = new XmlDocument();

            var xmlDeclaration = newXml.CreateXmlDeclaration("1.0", "UTF-8", null);
            newXml.AppendChild(xmlDeclaration);

            var root = newXml.CreateElement("Preferences");
            newXml.AppendChild(root);

            var initSetting = newXml.CreateElement(nameof(SavedData));


            initSetting.SetAttribute(nameof(Define.Preferences.MainVol), "0.5");
            initSetting.SetAttribute(nameof(Define.Preferences.NarrationVol), "0.5");
            initSetting.SetAttribute(nameof(Define.Preferences.EffectVol), "0.5");
            initSetting.SetAttribute(nameof(Define.Preferences.BgmVol), "0.15");
            initSetting.SetAttribute(nameof(Define.Preferences.Fullscreen), "1");
            initSetting.SetAttribute(nameof(Define.Preferences.Resolution), "1920");
            initSetting.SetAttribute(nameof(Define.Preferences.GraphicQuality), Define.QaulityLevel.Mid.ToString());
            initSetting.SetAttribute(nameof(Define.Preferences.EngMode), Define.LanguageMode.Kor.ToString());
            initSetting.SetAttribute(nameof(Define.Preferences.ControlGuide), "1"); //Yes
            initSetting.SetAttribute(nameof(Define.Preferences.Mute_Main), "0"); //No
            initSetting.SetAttribute(nameof(Define.Preferences.Mute_Narration), "0"); //No
            initSetting.SetAttribute(nameof(Define.Preferences.Mute_Effect), "0"); //No
            initSetting.SetAttribute(nameof(Define.Preferences.Mute_Bgm), "0"); //No

            root.AppendChild(initSetting); // Append initSetting to the root element

            newXml.Save(path);
            Debug.Log(fileName + ".xml FILE NOT EXIST, new file's been created at " + path);
        }

        Debug.Log("History Checker Active");
    }

    public static void WriteXML(XmlDocument document, string path)
    {
        document.Save(path);
        Debug.Log($"{path}");
        //Debug.Log("SAVED DATA WRITE");
    }

    #endregion
}