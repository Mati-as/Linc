using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;


public class SavedData
{
    
	
	// SavedData List
	[XmlAttribute]
	private float MainVolume;
	[XmlAttribute]
	private float NarrationVol;
	[XmlAttribute]
	private float EffectVol;
	[XmlAttribute]
	private float BgmVol;
	[XmlAttribute]
	private float Fullscreen;
	[XmlAttribute]
	private float Resolution;
	[XmlAttribute]
	private float GraphicQuality;
	[XmlAttribute]
	private float Language;
	[XmlAttribute]
	private float Controlguide;
	

    
}

// [Serializable, XmlRoot("ArrayOfSettingData")]
// public class SaveDataLoader : ILoader<int, SavedData>
// {
// 	[XmlElement("TextData")]
// 	public List<SavedData> SavData = new List<SavedData>();
//
// 	public Dictionary<int, SavedData> MakeDict()
// 	{
// 		Dictionary<int, SavedData> dic = new Dictionary<int, SavedData>();
//
// 		foreach (SavedData data in SavData)
// 			dic.Add(data.ID, data);
//
// 		return dic;
// 	}
//
// 	public bool Validate()
// 	{
// 		return true;
// 	}
// }