using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

/// <summary>
///     // 1.current info tracking
///     2.
/// Control ContentController
/// </summary>
public class ContentPlayData
{
    
    public int CurrentPlayMode; // free, RhythmGame
    public int HostInstrument;
    public int ClientInstrument; 
    public bool isMultiMode =false;
}




public class ContentPlayManager
{
    public ContentPlayData PlayData { get; set; } = new();


    public void Init()
    {
        PlayData = new ContentPlayData();
        
        
    }
}