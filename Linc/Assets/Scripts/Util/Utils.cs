using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;
using UnityEngine;
using static Define;

public class Utils
{
    public static T ParseEnum<T>(string value, bool ignoreCase = true)
    {
        return (T)Enum.Parse(typeof(T), value, ignoreCase);
    }

    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();
        return component;
    }

    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null)
            return null;

        if (recursive == false)
        {
            Transform transform = go.transform.Find(name);
            if (transform != null)
                return transform.GetComponent<T>();
        }
        else
        {
            foreach (T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                    return component;
            }
        }

        return null;
    }

    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(go, name, recursive);
        if (transform != null)
            return transform.gameObject;
        return null;
    }

	const int MAX_HP_ID = 6001;
	const int WORKABILITY_ID = 6011;
	const int LIKEABILITY_ID = 6021;
	const int LUCK_ID = 6041;
	const int STRESS_ID = 6031;
	const int BLOCK_ID = 6061;
	const int MONEY_ID = 6051;
	const int SALARY_ID = 6071;

	//직급
	const int Intern = 5000;
	const int Sinib = 5010;
	const int Daeri = 5020;
	const int Gwajang = 5030;
	const int Bujang = 5040;
	const int Esa = 5050;
	const int Sajang = 5060;

    #region XML
    public static void ReadXML(ref XmlDocument doc, string path)
    {
        var Document = new XmlDocument();
        Document.Load(path);
        doc = Document;
    }
    
    public static void SaveXML(ref XmlDocument xmlDoc,string xmlFilePath)
    {
        xmlDoc.Save(xmlFilePath);
            
        Debug.Log("XML file saved to: " + xmlFilePath);
    }
    #endregion
}
