using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class GoogleSheetManager : Singleton<GoogleSheetManager>
{
	private static GoogleSheetManager instance;
	
	const string URL = "https://docs.google.com/spreadsheets/d/18p6ZGAuyzWREIUGrczyPhMjNdnpAILS_StEJQD00ACk/export?format=tsv";
	//&rang=A2:B2;
	//&gid=5454968
	public bool isDataLoad;
	
	public List<StoryData> storyDB;
	public List<PoolData> poolDB;
	public List<CaseData> caseDB;
	public List<SelectionData> selectionDB;
	public List<SelectEventData> selectEventDB;
	public List<EndingData> endingDB;
	private Coroutine getDataCor;

	void Awake()
	{
		if (instance != null)
		{
			Destroy(gameObject);
			return;
		}
		instance = this;
		// 모든 씬에서 유지
		DontDestroyOnLoad(gameObject);
	}
	
	public void LoadData()
	{
		AllListClear();

		if (getDataCor != null)
		{
			StopCoroutine(getDataCor);
		}
		getDataCor = StartCoroutine(GetDataCor());
	}

	public void AllListClear()
	{
		storyDB.Clear();	
		poolDB.Clear();
		caseDB.Clear();
		selectionDB.Clear();
		selectEventDB.Clear();
		endingDB.Clear();
	}
	
	IEnumerator GetDataCor()
	{
		foreach (TableType value in TableType.GetValues(typeof(TableType)))
		{
			string tableURL = URL + GetTableCodeWithName(value);

			UnityWebRequest www = UnityWebRequest.Get(tableURL);

			yield return www.SendWebRequest();

			string data = www.downloadHandler.text;

			Debug.Log($"{value}\n{data}");

			 switch (value)
			 {
				case TableType.Story:
					StoryDataParse(data);
				break;
				case TableType.Pool:
					PoolDataParse(data);
					break;
				case TableType.Case:
					CaseDataParse(data);
					break;
				case TableType.Selection:
					SelectionDataParse(data);
					break;
				case TableType.SelectEvent:
					SelectEventDataParse(data);
					break;
				case TableType.Ending:
					EndingDataParse(data);
					break;
			 }
		}

		isDataLoad = true;
	}

	string GetTableCodeWithName(TableType tableType)
	{
		string header = "&gid=";

		string tableCode = "";
		
		switch (tableType)
		{
			case TableType.Story:
				tableCode = "0&range=A4:C";
				break;
			case TableType.Pool:
				tableCode = "5454968&range=A4:K";
				break;
			case TableType.Case:
				tableCode = "557199738&range=A4:H";
				break;
			case TableType.Selection:
				tableCode = "2136310630&range=A4:L";
				break;
			case TableType.SelectEvent:
				tableCode = "270773085&range=A4:D";
				break;
			case TableType.Ending:
				tableCode = "1452065059&range=A4:R";
				break;
		}

		return $"{header}{tableCode}";
	}
	public void StoryDataParse(string data)
	{
		storyDB.Clear();
		
		string[] lines = data.Trim().Split(new[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
		
		foreach (string line in lines)
		{
			string[] values = line.Trim().Split(new[] { '\t' }, System.StringSplitOptions.RemoveEmptyEntries);
			
			if (values.Length == 3 && 
			    int.TryParse(values[0], out int storyOrder) &&
			    int.TryParse(values[1], out int poolID) &&
			    int.TryParse(values[2], out int timeLine))
			{
				StoryData storyData = new StoryData();

				storyData.storyOder = storyOrder;
				storyData.poolID    = poolID;
				storyData.timeLine  = timeLine;

				storyDB.Add(storyData);
			}
			else
			{
				Debug.LogError($"Invalid data line: {line}");
			}
		}
	}
	
	public void PoolDataParse(string data)
	{
		poolDB.Clear();
		
		string[] lines = data.Trim().Split(new[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
		
		foreach (string line in lines)
		{
			string[] values = line.Trim().Split(new[] { '\t' }, System.StringSplitOptions.RemoveEmptyEntries);
			
			if (values.Length == 11 && 
			    int.TryParse(values[0], out int poolID) &&
			    int.TryParse(values[1], out int caseID) &&
			    float.TryParse(values[2], out float caseRate) &&
			    float.TryParse(values[4], out float caseStat1Value) &&
			    bool.TryParse(values[5], out bool caseStatus1valueCondition) &&
			    float.TryParse(values[6], out float caseStat1Rate) &&
			    float.TryParse(values[8], out float caseStat2Value) &&
			    bool.TryParse(values[9], out bool caseStatus2valueCondition) &&
			    float.TryParse(values[10], out float caseStat2Rate))
			{
				PoolData poolData = new PoolData();

				poolData.poolID                    = poolID;
				poolData.caseID                    = caseID;
				poolData.caseRate                  = caseRate;
				poolData.caseStat1                 = StringToStatusEnum(values[3]);
				poolData.caseStat1Value            = caseStat1Value;
				poolData.caseStatus1valueCondition = caseStatus1valueCondition;
				poolData.caseStat1Rate             = caseStat1Rate;
				poolData.caseStat2                 = StringToStatusEnum(values[7]);
				poolData.caseStat2Value            = caseStat2Value;
				poolData.caseStatus2valueCondition = caseStatus2valueCondition;
				poolData.caseStat2Rate             = caseStat2Rate;

				poolDB.Add(poolData);
			}
			else
			{
				Debug.LogError($"Invalid data line: {line}");
			}
		}
	}
	
	public void CaseDataParse(string data)
	{
		caseDB.Clear();
		
		string[] lines = data.Trim().Split(new[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
		
		foreach (string line in lines)
		{
			string[] values = line.Trim().Split(new[] { '\t' }, System.StringSplitOptions.RemoveEmptyEntries);
			
			if (values.Length == 8 && 
			    int.TryParse(values[0], out int caseID) &&
			    int.TryParse(values[6], out int selectionL) &&
			    int.TryParse(values[7], out int selectionR))
			{
				CaseData caseData = new CaseData();

				caseData.caseID          = caseID;
				caseData.caseType        = StringToCaseTypeEnum(values[1]);
				caseData.caseDescription = values[2];
				caseData.caseDialogue    = values[3];
				caseData.caseImage       = values[4];
				caseData.caseTitle       = values[5];
				caseData.selectionL      = selectionL;
				caseData.selectionR      = selectionR;
				
				caseDB.Add(caseData);
			}
			else
			{
				Debug.LogError($"Invalid data line: {line}");
			}
		}
	}
	public void SelectionDataParse(string data)
	{
		selectionDB.Clear();
		
		string[] lines = data.Trim().Split(new[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
		
		foreach (string line in lines)
		{
			string[] values = line.Trim().Split(new[] { '\t' }, System.StringSplitOptions.RemoveEmptyEntries);
			
			if (values.Length == 12 && 
			    int.TryParse(values[0], out int selectionID) &&
			    int.TryParse(values[3], out int selectStatus1Value) &&
			    int.TryParse(values[5], out int selectStatus2Value) &&
			    int.TryParse(values[7], out int selectStatus3Value) &&
			    int.TryParse(values[9], out int selectStatus4Value) &&
			    int.TryParse(values[10], out int selectEventID) &&
			    float.TryParse(values[11], out float selectEventRate)
			    )
			{
				SelectionData selectionData = new SelectionData();

				selectionData.selectionID        = selectionID;
				selectionData.selectDescription  = values[1];
				selectionData.selectStatus1      = StringToStatusEnum(values[2]);
				selectionData.selectStatus1Value = selectStatus1Value;
				selectionData.selectStatus2      = StringToStatusEnum(values[4]);
				selectionData.selectStatus2Value = selectStatus2Value;
				selectionData.selectStatus3      = StringToStatusEnum(values[6]);
				selectionData.selectStatus3Value = selectStatus3Value;
				selectionData.selectStatus4      = StringToStatusEnum(values[8]);;
				selectionData.selectStatus4Value = selectStatus4Value;
				selectionData.selectEventID      = selectEventID;
				selectionData.selectEventRate    = selectEventRate;
				
				selectionDB.Add(selectionData);
			}
			else
			{
				Debug.LogError($"Invalid data line: {line}");
			}
		}
	}
	
	public void SelectEventDataParse(string data)
	{
		selectEventDB.Clear();
		
		string[] lines = data.Trim().Split(new[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
		
		foreach (string line in lines)
		{
			string[] values = line.Trim().Split(new[] { '\t' }, System.StringSplitOptions.RemoveEmptyEntries);
			
			if (values.Length == 4 && 
			    int.TryParse(values[0], out int selectEventID) &&
			    float.TryParse(values[2], out float selectEventStatusValue)
			   )
			{
				SelectEventData selectEventData = new SelectEventData();

				selectEventData.selectEventID          = selectEventID;
				selectEventData.selectEventStatus      = StringToStatusEnum(values[1]);
				selectEventData.selectEventStatusValue = selectEventStatusValue;
				selectEventData.selectEventDescription = values[3];
				
				selectEventDB.Add(selectEventData);
			}
			else
			{
				Debug.LogError($"Invalid data line: {line}");
			}
		}
	}
	
	public void EndingDataParse(string data)
	{
		endingDB.Clear();
		
		string[] lines = data.Trim().Split(new[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
		
		foreach (string line in lines)
		{
			string[] values = line.Trim().Split(new[] { '\t' }, System.StringSplitOptions.RemoveEmptyEntries);
			
			if (values.Length == 18 && 
			    int.TryParse(values[0], out int endingID) &&
			    int.TryParse(values[4], out int endingStatus1value) &&
			    bool.TryParse(values[5], out bool endingStatus1valueCondition) &&
			    int.TryParse(values[7], out int endingStatus2value) &&
			    bool.TryParse(values[8], out bool endingStatus2valueCondition) &&
			    int.TryParse(values[10], out int endingStatus3value) &&
				bool.TryParse(values[11], out bool endingStatus3valueCondition) &&
			    int.TryParse(values[13], out int endingStatus4value) &&
			    bool.TryParse(values[14], out bool endingStatus4valueCondition) &&
			    int.TryParse(values[16], out int endingGrade)
			   )
			{
				EndingData endingData = new EndingData();

				endingData.endingID                    = endingID;
				endingData.endingName                  = values[1];
				endingData.EndingImage                 = values[2];
				
				endingData.endingStatus1               = StringToStatusEnum(values[3]);
				endingData.endingStatus1value          = endingStatus1value;
				endingData.endingStatus1valueCondition = endingStatus1valueCondition;
				
				endingData.endingStatus2               = StringToStatusEnum(values[6]);
				endingData.endingStatus2value          = endingStatus2value;
				endingData.endingStatus3valueCondition = endingStatus2valueCondition;
				
				endingData.endingStatus3               = StringToStatusEnum(values[9]);
				endingData.endingStatus3value          = endingStatus3value;
				endingData.endingStatus3valueCondition = endingStatus3valueCondition;
				
				endingData.endingStatus4               = StringToStatusEnum(values[12]);
				endingData.endingStatus4value          = endingStatus4value;
				endingData.endingStatus4valueCondition = endingStatus4valueCondition;

				endingData.endingDescription           = values[15];
				endingData.endingGrade                 = endingGrade;
				endingData.endingGradedescription      = values[17];
				
				endingDB.Add(endingData);
			}
			else
			{
				Debug.LogError($"Invalid data line: {line}");
			}
		}
	}

	private Status StringToStatusEnum(string enumString)
	{
		if (enumString == "None")
		{
			Debug.Log("None 체크");
		}
		
		Status statusType = (Status)Enum.Parse(typeof(Status), enumString);

		return statusType;
	}
	
	private CaseType StringToCaseTypeEnum(string enumString)
	{
		CaseType cardType = (CaseType)Enum.Parse(typeof(CaseType), enumString);

		return cardType;
	}
	
}