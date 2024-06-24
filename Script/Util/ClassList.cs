using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class StoryData
{
    public int storyOder;
    public int poolID;
    public int timeLine;
}

[Serializable]
public class PoolData
{
    public int poolID;
    public int caseID;
    public float caseRate;
    
    public Status caseStat1;
    public float caseStat1Value;
    public bool caseStatus1valueCondition;
    public float caseStat1Rate;

    public Status caseStat2;
    public float caseStat2Value;
    public bool caseStatus2valueCondition;
    public float caseStat2Rate;
}

[Serializable]
public class CaseData
{
    public int caseID;
    public CaseType caseType;
    public string caseDescription;
    public string caseDialogue;
    public string caseImage;
    public string caseTitle;
    public int selectionL;
    public int selectionR;
}

[Serializable]
public class SelectionData
{
    public int selectionID;
    public string selectDescription;
    public Status selectStatus1;
    public int selectStatus1Value;
    public Status selectStatus2;
    public int selectStatus2Value;
    public Status selectStatus3;
    public int selectStatus3Value;
    public Status selectStatus4;
    public int selectStatus4Value;

    public int selectEventID;
    public float selectEventRate;
}

[Serializable]
public class SelectEventData
{
    public int selectEventID;
    public Status selectEventStatus;
    public float selectEventStatusValue;
    public string selectEventDescription;
}

[Serializable]
public class EndingData
{
    public int endingID;
    public string endingName;

    public string EndingImage;
    
    public Status endingStatus1;
    public int endingStatus1value;
    public bool endingStatus1valueCondition;
    
    public Status endingStatus2;
    public int endingStatus2value;
    public bool endingStatus2valueCondition;
    
    public Status endingStatus3;
    public int endingStatus3value;
    public bool endingStatus3valueCondition;
    
    public Status endingStatus4;
    public int endingStatus4value;
    public bool endingStatus4valueCondition;
    
    public string endingDescription;
    public int endingGrade;
    public string endingGradedescription;
}

[Serializable]
public class CalPoolData
{
    public float calPoint;
    public int poolID;
}
