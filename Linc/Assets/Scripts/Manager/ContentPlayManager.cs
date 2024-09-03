using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

/// <summary>
///     // 1.current info tracking
///     2.Control ContentController
/// </summary>
public class ContentPlayData
{
    public static Dictionary<int, int> DEPTH_TWO_COUNT_DATA = new Dictionary<int, int>
    {
        { 1, 2 }, //depth1 +depth2 , Depth3갯수
        { 2, 3 },
        { 3, 5 },
   
    };
    public static Dictionary<int, int> DEPTH_THREE_COUNT_DATA = new Dictionary<int, int>
    {
        { 11, 1 }, //depth1 +depth2 , Depth3갯수 // 1-1 뎁스에는 1개의 depth3가 있음
        { 12, 5 },
        { 21, 1 },
        { 22, 1 },
        { 23, 3 },
        { 31, 3 },
        { 32, 3 },
        { 33, 3 },
        { 34, 3 },
        { 35, 3 }
    };

    public enum CurrentDepthData
    {
        Depth1, //UI- approachable
        Depth2, //UI- approachable
        Depth3, //Non UI- approachable
        Count, //Non UI- approachable
        //Depth1 + 2 + 3 + Count = Narration, Text 순서 및 File Data,
        Max
    }

    

    public string CurrentDepthStatus ="00000";
    private char[] _currentDepthStatusChar = new char[(int)CurrentDepthData.Max];


    private int _depth1;
    private int _depth2;
    private int _depth3;
    private int _count;

    public static readonly int COUNT_MAX = 10;
    
    public int Depth1
    {
        get { return _depth1; }
        set
        {
            Debug.Assert(value <= 3);

            _depth1 = value;


            // Update the relevant character in CurrentDepthStatus
            UpdateCurrentDepthStatus((int)CurrentDepthData.Depth1, (char)(value + '0'));
            
#if UNITY_EDITOR
            Debug.Log($"Current Scene Info : {CurrentDepthStatus[0]}-{CurrentDepthStatus[1]}-" +
                      $"{CurrentDepthStatus[2]} : {CurrentDepthStatus[3]}{CurrentDepthStatus[4]}" +
                      $"\n textNarrNum: {CurrentDepthStatus}");
#endif
        }
    }

//Depth2
    public int Depth2
    {
        get { return _depth2; }
        set
        {
            Debug.Assert(value <= 5);
            _depth2 = value;

            // Update the relevant character in CurrentDepthStatus
            UpdateCurrentDepthStatus((int)CurrentDepthData.Depth2, (char)(value + '0'));
            
#if UNITY_EDITOR
            Debug.Log($"Current Scene Info : {CurrentDepthStatus[0]}-{CurrentDepthStatus[1]}-" +
                      $"{CurrentDepthStatus[2]} : {CurrentDepthStatus[3]}{CurrentDepthStatus[4]}" +
                      $"\n textNarrNum: {CurrentDepthStatus}");
#endif
         
        }
    }

//Depth3
    public int Depth3
    {
        get { return _depth3; }
        set
        {
            Debug.Assert(value <= 5 && value >0);
            _depth3 = value;

            // Update the relevant character in CurrentDepthStatus
            UpdateCurrentDepthStatus((int)CurrentDepthData.Depth3, (char)(value + '0'));

#if UNITY_EDITOR
            Debug.Log($"Current Scene Info : {CurrentDepthStatus[0]}-{CurrentDepthStatus[1]}-" +
                      $"{CurrentDepthStatus[2]} : {CurrentDepthStatus[3]}{CurrentDepthStatus[4]}" +
                      $"\n textNarrNum: {CurrentDepthStatus}");
#endif
          
        }
    }

//Count
    public int Count
    {
        get { return _count; }
        set
        {
            _count = math.clamp(value, 0, COUNT_MAX);//max
            
            // Format the count value
            var formattedValue = value < 10 ? $"0{value}" : value.ToString();
            UpdateCurrentDepthStatus((int)CurrentDepthData.Count, formattedValue[0]);
            UpdateCurrentDepthStatus((int)CurrentDepthData.Count + 1, formattedValue[1]);

#if UNITY_EDITOR
            Debug.Log($"Current Scene Info : {CurrentDepthStatus[0]}-{CurrentDepthStatus[1]}-" +
                      $"{CurrentDepthStatus[2]} : {CurrentDepthStatus[3]}{CurrentDepthStatus[4]}" +
                      $" \n textNarrNum: {CurrentDepthStatus}");
#endif
        }
    }

// Helper method to update CurrentDepthStatus string..
    private void UpdateCurrentDepthStatus(int index, char newValue)
    {
        if (index < CurrentDepthStatus.Length)
        {
            var tempArray = CurrentDepthStatus.ToCharArray();
            tempArray[index] = newValue;
            CurrentDepthStatus = new string(tempArray);
        }
    }
    
    public void ResetOrSetDepthCount(int depth2=1, int depth3=1, int count=1)
    {
        Depth2 = depth2;
        Depth3 = depth3;
        Count = count;
    }
}


public class ContentPlayManager
{
    public ContentPlayData PlayData { get; set; } = new();


    public void Init()
    {
        PlayData = new ContentPlayData();
        
        
    }
}