using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using DG.Tweening;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : Singleton<GameManager>
{
    private static GameManager instance;
    
    // 유저의 스테이터스
    public int reliability = -20;
    public int anger = -20;
    public int anxiety = -20;
    public int hope = -20;
    public int maxValue = 100;
    public int minValue = 0;
    //GameObject
    public GameObject cardGameObject;
    //public GameObject pendingCardGameObject;
    public CardController mainCardController;
    public SpriteRenderer cardSpriteRenderer;
    public Vector2 defaultPositionCard;
   
    //Tweaking Variables
    public float fMovingSpeed;
    public float fRotatingSpeed;
    public float fSideMagine;
    public float fsideTrigger;
    public float divideValue;
    public float backgrounddivideValue;
    private float alphaText;
    public Color textColor;
    public Color selectionBackgroundColor;
    public Color eventBackgroundColor;
    public float fTransparency = 1f;
    public float fRotationCoefficient;
    private Vector3 pos;
    //UI
    public TMP_Text display;
    //public TMP_Text characterDialgoue;
    public TMP_Text actionQuote;
    public TMP_Text actionQuoteEvent;
    public TMP_Text eventText;
    public TMP_Text dialogue;
    public TMP_Text nameText;
    public Image eventSelectBackground;
    public Image actionSelectionBackground;
    //Card Variables
	public string direction;
    private string leftQuote;
    private string rightQuote;
    public Card currentCard;
    public Card testCard;
    public Card eventCard;
    public Card normalCard;
    //Substiuting the card
    public bool isSubstituting = false;
    public Vector3 cardRotation; //default one
    public Vector3 currentRotation; //the current rotation of the card
    public Vector3 initRotation;  // Initial rotation of the card

    [Header("카드 관련")]
    public int storyOder;
    public int currentAddtimeValue; //현재 스토리에서 추가되는 time값
    public CaseData currentCaseData;
    public SelectionData selectionDataL;
    public SelectionData selectionDataR;
    
    private string timeLine;
    public int timeLineValue;
    public int[] timeLineNumbers;
    public List<Image> timeLineTextUI;
    public DOTweenAnimation dotweenAnimation;
    public Animator jokerAnim;

    [Header("엔딩UI")]
    public GameObject endingUIObj;
    public TextMeshProUGUI endingName;
    public Image endingImage;
    public TextMeshProUGUI endingGrade;
    public TextMeshProUGUI endingGradeDescription;
    public TextMeshProUGUI endingDescription;

    void Awake()
    {
        instance = this;
    }
    
    void Start()
    {
        defaultPositionCard = cardGameObject.transform.position ;
        storyOder = 1;
        
        LoadCard(testCard);

        timeLineValue = 0;
        timeLine = "0";
        
        UpdateTimeLineTextUISprite();

        StoryOderStart();

        jokerAnim = ResourceManager.Instance.CharPrefabs[0].GetComponent<Animator>();
    }

	//위에 상황설명 color 나타내주는 함수
    void UpdateDialogue()
    {
        actionQuote.color = textColor;
        actionQuoteEvent.color = textColor;
        actionSelectionBackground.color = selectionBackgroundColor;
        eventSelectBackground.color = eventBackgroundColor;
        if (cardGameObject.transform.position.x > 0 )
        {
            if (currentCaseData.caseType == CaseType.이벤트)
                actionQuoteEvent.text = selectionDataR.selectDescription;
            else
                actionQuote.text = selectionDataR.selectDescription;
        }
        else
        {
            if (currentCaseData.caseType == CaseType.이벤트)
                actionQuoteEvent.text = selectionDataL.selectDescription;
            else
                actionQuote.text = selectionDataL.selectDescription;
        }
    }
    void Update()
    {
        
        textColor.a = Mathf.Min(((Mathf.Abs(cardGameObject.transform.position.x) - fSideMagine)/ divideValue), 1);
        selectionBackgroundColor.a = Mathf.Min(((Mathf.Abs(cardGameObject.transform.position.x) - fSideMagine) / divideValue), 1);
        eventBackgroundColor.a = selectionBackgroundColor.a;
        if (cardGameObject.transform.position.x > fsideTrigger)
        {
			direction = "right";
            if (Input.GetMouseButtonUp(0))
            {
                //Debug.Log("Right Select");
                currentCard.Right();
            }
        }
        else if (cardGameObject.transform.position.x > fSideMagine)
        {
			direction = "right";
        }
        else if (cardGameObject.transform.position.x > -fSideMagine)
        {
			direction = "none";
            textColor.a = 0;
        }
        else if (cardGameObject.transform.position.x > -fsideTrigger)
        {
            direction = "left";
        }
        else
        {
			direction = "left";
            if (Input.GetMouseButtonUp(0))
            {
                
                //Debug.Log("Right Left");
                currentCard.Left();
            }
        }
        UpdateDialogue();

        //MoveMent
        if (Input.GetMouseButton(0) && mainCardController.isMouseOver)
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            cardGameObject.transform.position = pos;
            cardGameObject.transform.eulerAngles = new Vector3(0f, 0f, cardGameObject.transform.position.x * fRotationCoefficient);
        }
        else if (!isSubstituting)
        {
            cardGameObject.transform.position = Vector2.MoveTowards(cardGameObject.transform.position, defaultPositionCard, fMovingSpeed);
            cardGameObject.transform.eulerAngles = new Vector3(0f, 0f, 0f);
        }
        else if (isSubstituting)
        {
            //cardGameObject.transform.eulerAngles = Vector3.MoveTowards(cardGameObject.transform.eulerAngles, cardRotation, fRotatingSpeed);
        }

        //UI
        display.text = "" + textColor.a;

        //Rotating The Card
        //if (cardGameObject.transform.eulerAngles == cardRotation)
        //{
        //    isSubstituting = false;
        //}
    }

    public void LoadCard(Card card)
    {
        leftQuote = card.leftQuote;
        rightQuote = card.rightQuote;
        currentCard = card;
        cardGameObject.transform.position = defaultPositionCard;
        cardGameObject.transform.eulerAngles = new Vector3(0f, 0f, 0f);
        cardGameObject.transform.eulerAngles = initRotation;
    }
    
    public void StoryOderStart()
    {
        if(storyOder >= 31)
            Ending(12);
        
        currentAddtimeValue = 0;
        
        foreach (var storyData in GoogleSheetManager.Instance.storyDB)
        {
            if(storyData.storyOder == storyOder)
            {
                currentAddtimeValue = storyData.timeLine;
                TryPool(storyData.poolID);
            }
        }
    }
    
    public void TryPool(int poolID)
    {
        List<PoolData> poolDataList = new();
        
        foreach (var poolData in GoogleSheetManager.Instance.poolDB)
        {
            if (poolData.poolID == poolID)
            {
                poolDataList.Add(poolData);
            }
        }

        if (poolDataList.Count >= 2)
        {
            poolRateSort(poolDataList);
        }
        else
        {
            CaseEvent(poolDataList[0].caseID);
        }
    }

    public void CaseEvent(int caseID)
    {
        foreach (var caseData in GoogleSheetManager.Instance.caseDB)
        {
            if (caseData.caseID == caseID)
            {
                currentCaseData = caseData;

                FindSelectData(currentCaseData.selectionL, true);
                
                FindSelectData(currentCaseData.selectionR, false);
            }
        }

        if (currentCaseData.caseType == CaseType.이벤트)
        {
            normalCard.gameObject.SetActive(false);
            eventCard.gameObject.SetActive(true);
            dialogue.gameObject.SetActive(false);
            eventText.text = currentCaseData.caseDescription;
        }
        else
        {
            // 설명 처리
            normalCard.gameObject.SetActive(true);
            eventCard.gameObject.SetActive(false);
            dialogue.gameObject.SetActive(true);
            dialogue.text = currentCaseData.caseDialogue;
            nameText.text = currentCaseData.caseTitle;

            GameObject currentChar = ResourceManager.Instance.GetPrefabWithName(currentCaseData.caseImage);
            
            foreach (var gameObject in ResourceManager.Instance.CharPrefabs)
            {
                gameObject.SetActive(false);
            }
            
            currentChar.SetActive(true);
            
            if (currentCaseData.caseImage == "Chr_joker")
            {
                //평균치 계산
                getAverageValue();
            }
        }
    }

    public void FindSelectData(int selectID, bool isLeft)
    {
        foreach (var selectData in GoogleSheetManager.Instance.selectionDB)
        {
            if (selectData.selectionID == selectID)
            {
                if (isLeft)
                {
                    selectionDataL = selectData;
                }
                else
                {
                    selectionDataR = selectData;

                }
            }
        }
    }

    public void AddTimeValue(int amount)
    {
        timeLineValue += amount;

        timeLine = timeLineValue.ToString();
        
        UpdateTimeLineTextUISprite();
    }
    
    public void UpdateTimeLineTextUISprite()
    {
        timeLineNumbers = SplitStringIntoDigits(timeLine);

        foreach (Image image in timeLineTextUI)
        {
            image.gameObject.SetActive(false);
        }

        int startIndex = timeLineTextUI.Count - timeLineNumbers.Length;
        for (int i = 0; i < timeLineNumbers.Length; i++)
        {
            timeLineTextUI[startIndex + i].gameObject.SetActive(true);
            timeLineTextUI[startIndex + i].sprite = ResourceManager.Instance.numberSprites[timeLineNumbers[i]];
        }
    }
   
    int[] SplitStringIntoDigits(string input)
    {
        // 입력 문자열 길이만큼의 정수 배열 생성
        int[] result = new int[input.Length];

        // 각 문자를 정수로 변환하여 배열에 저장
        for (int i = 0; i < input.Length; i++)
        {
            // 문자를 정수로 변환 (ASCII 코드값을 사용)
            result[i] = input[i] - '0';
        }

        return result;
    }

        public void poolRateSort(List<PoolData> getPoolList)
    {
        Dictionary<int, float> sortingDic = new();

        foreach (var poolData in getPoolList)
        {
            // 초기 비율
            float originPoint = poolData.caseRate;

            // 비교할 유저의 수치
            float checkValue1 = FindPointValue1(poolData);
            float checkValue2 = FindPointValue2(poolData);

            // 비교할 수치
            float targetValue1 = poolData.caseStat1Value;
            float targetValue2 = poolData.caseStat2Value;
            
            // 비교 조건
            bool isUporDown1 = poolData.caseStatus1valueCondition;
            bool isUporDown2 = poolData.caseStatus2valueCondition;

            float addValue1 = poolData.caseStat1Rate;
            float addValue2 = poolData.caseStat2Rate;
            
            if (isUporDown1)
            {
                if (checkValue1 >= targetValue1)
                {
                    originPoint *= addValue1;
                }
            }
            else
            {
                if (checkValue1 <= targetValue1)
                {
                    originPoint *= addValue1;
                }
            }

            if (isUporDown2)
            {
                if (checkValue2 >= targetValue2)
                {
                    originPoint *= addValue2;
                }
            }
            else
            {
                if (checkValue2 <= targetValue2)
                {
                    originPoint *= addValue2;
                }
            }
            
            sortingDic.Add(poolData.caseID, originPoint);
        }
        
        List<KeyValuePair<int, float>> sortedList = new List<KeyValuePair<int, float>>(sortingDic);
        
        // 높은 확률을 실행
        //CaseEvent(sortedList[0].Key);

        // 비율의 총합
        float totalRatio = 0;
        
        foreach (KeyValuePair<int, float> pair in sortedList)
        {
            totalRatio += pair.Value;
        }
        
        // 0과 총합 사이의 랜덤 값
        float randomValue = Random.Range(0f, totalRatio);

        float cumulativeSum = 0f;
        foreach (KeyValuePair<int, float> pair in sortedList)
        {
            cumulativeSum += pair.Value;
            if (randomValue <= cumulativeSum)
            {
                CaseEvent(pair.Key);
                break;
            }
        }
    }

    private float FindPointValue1(PoolData poolData) 
    {
        switch (poolData.caseStat1)
        {
            case Status.신뢰도:
                return reliability;
            case Status.분노도:
                return anger;
            case Status.불안도:
                return anxiety;
            case Status.희망도:
                return hope;
        }

        return 0;
    }
    
    private float FindPointValue2(PoolData poolData) 
    {
        switch (poolData.caseStat2)
        {
            case Status.신뢰도:
                return reliability;
            case Status.분노도:
                return anger;
            case Status.불안도:
                return anxiety;
            case Status.희망도:
                return hope;
        }

        return 0;
    }

    public void LValueInit()
    {
        reliability += selectionDataL.selectStatus1Value;
        anger       += selectionDataL.selectStatus2Value;
        anxiety     += selectionDataL.selectStatus3Value;
        hope        += selectionDataL.selectStatus4Value;
        reliability = Math.Clamp(reliability, -100, 100);
        anger       = Math.Clamp(anger, -100, 100);
        anxiety     = Math.Clamp(anxiety, -100, 100);
        hope        = Math.Clamp(hope, -100, 100);
        if (selectionDataR.selectStatus4Value > 0)
        {
            AudioPlayer.Instance.PlayClip(5);
        }
        else
        {
            AudioPlayer.Instance.PlayClip(0);
        }
        
        EndingCheck();
    }
    
    public void RValueInit()
    {
        reliability += selectionDataR.selectStatus1Value;
        anger       += selectionDataR.selectStatus2Value;
        anxiety     += selectionDataR.selectStatus3Value;
        hope        += selectionDataR.selectStatus4Value;
        reliability = Math.Clamp(reliability,-100,100);
        anger       = Math.Clamp(anger, -100, 100);
        anxiety     = Math.Clamp(anxiety, -100, 100);
        hope        = Math.Clamp(hope, -100, 100);
        if (selectionDataR.selectStatus4Value > 0)
        {
            AudioPlayer.Instance.PlayClip(5);
        }
        else
        {
            AudioPlayer.Instance.PlayClip(0);
        }
        
        EndingCheck();
    }

    public void getAverageValue()
    {
        float sumValue = reliability + anger + anxiety;

        Debug.Log($"합계 신뢰도 : {reliability} 분노도 : {anger} 불안도 : {anxiety}");
        
        float average = sumValue / 3;

        Debug.Log($"현재 평균치{average}");
        
        if (average > 0)
        {
            Debug.Log("normal 표정");
            jokerAnim.SetTrigger("Normal");
            
            AudioPlayer.Instance.PlayClip(4);
        }
        else if (average < 0 && average >= -39)
        {
            Debug.Log("Angry 표정");
            jokerAnim.SetTrigger("Angry");
            
            AudioPlayer.Instance.PlayClip(2);
        }
        else if (average <= -40)
        {
            Debug.Log("BigAngry 표정");
            jokerAnim.SetTrigger("BigAngry");
            
            AudioPlayer.Instance.PlayClip(3);
        }
    }
    
    // 엔딩 조건 체크 함수
    public void EndingCheck()
    {
        // 모든 엔딩 조회 시도
        for (int i = 0; i < GoogleSheetManager.Instance.endingDB.Count; i++)
        {
            Debug.Log(GoogleSheetManager.Instance.endingDB[i].endingName + "엔딩 조회");

            bool checkedValue1;
            bool checkedValue2;
            bool checkedValue3;
            bool checkedValue4;
            
            // 조회 스테이터스
            Status CheckStatus1 = GoogleSheetManager.Instance.endingDB[i].endingStatus1;
            Status CheckStatus2 = GoogleSheetManager.Instance.endingDB[i].endingStatus2;
            Status CheckStatus3 = GoogleSheetManager.Instance.endingDB[i].endingStatus3;
            Status CheckStatus4 = GoogleSheetManager.Instance.endingDB[i].endingStatus4;
            
            checkedValue1 = CheckConditionValue(CheckStatus1,
                GoogleSheetManager.Instance.endingDB[i].endingStatus1value,
                GoogleSheetManager.Instance.endingDB[i].endingStatus1valueCondition);
            
            checkedValue2 = CheckConditionValue(CheckStatus2,
                GoogleSheetManager.Instance.endingDB[i].endingStatus2value,
                GoogleSheetManager.Instance.endingDB[i].endingStatus2valueCondition);
            
            checkedValue3 = CheckConditionValue(CheckStatus3,
                GoogleSheetManager.Instance.endingDB[i].endingStatus3value,
                GoogleSheetManager.Instance.endingDB[i].endingStatus3valueCondition);

            checkedValue4 = CheckConditionValue(CheckStatus4,
                GoogleSheetManager.Instance.endingDB[i].endingStatus4value,
                GoogleSheetManager.Instance.endingDB[i].endingStatus4valueCondition);

            Debug.Log($"{GoogleSheetManager.Instance.endingDB[i].endingName}엔딩" +
                      $"1.{checkedValue1} 2.{checkedValue2} 3. {checkedValue3} 4.{checkedValue4}");
            if (checkedValue1 && checkedValue2 && checkedValue3 && checkedValue4)
            {
                Debug.Log("모든 조건 적합판정 엔딩처리");
                Ending(GoogleSheetManager.Instance.endingDB[i].endingID);
                break;
            }
        }
        
        // 조건에 적합한 엔딩이 없음 다음 오더처리
        storyOder++;
        AddTimeValue(currentAddtimeValue);
            
        StoryOderStart();
    }
    
    public bool CheckConditionValue (Status CheckStatus, float gettargetValue, bool getstatusCondition)
    {
        float settingStatus = 0;
        
        switch (CheckStatus)
        {
            case Status.신뢰도:
                settingStatus = reliability;
                break;
            case Status.분노도:
                settingStatus = anger;
                break;
            case Status.불안도:
                settingStatus = anxiety;
                break;
            case Status.희망도:
                settingStatus = hope;
                break;
            default:
                //Debug.Log("None이라 달성처리");
                return true;
        }
        float targetValue = gettargetValue;

        bool statusCondition = getstatusCondition;

        if (statusCondition)
        {
            //Debug.Log($"{CheckStatus}가 {targetValue}이상인 조건 : 현재 스테이터스 : {settingStatus}");

            if (settingStatus >= targetValue)
            {
                //Debug.Log($"{settingStatus} : {targetValue} 엔딩조건 달성");
                return true;
            }
        }
        else
        {
            //Debug.Log($"{CheckStatus}가 {targetValue}이하인 조건 : 현재 스테이터스 : {settingStatus}");
            if (settingStatus <= targetValue)
            {
                //Debug.Log($"{settingStatus} : {targetValue} 엔딩조건 달성");
                return true;
            }
        }

        return false;
    }
    // 엔딩 함수
    public void Ending(int number)
    {
        Debug.Log($"{number}엔딩실행");
        cardGameObject.SetActive(false);
        endingUIObj.SetActive(true);

        if (number == 2 || number == 3)
        {
            //happy
            AudioPlayer.Instance.PlayBGM(3);
        }
        else if (number == 1)
        {
            //hidden
            AudioPlayer.Instance.PlayBGM(4);
        }
        else if( number == 5)
        {
            //sad
            AudioPlayer.Instance.PlayBGM(5);
        }
        else if (number == 6)
        {
            //strange
            AudioPlayer.Instance.PlayBGM(6);
        }
        else if (number == 4)
        {
            //true
            AudioPlayer.Instance.PlayBGM(7);
        }
        else
        {
            //bad
            AudioPlayer.Instance.PlayBGM(2);
        }

        foreach (var ending in GoogleSheetManager.Instance.endingDB)
        {
            if (ending.endingID == number)
            {
                endingImage.sprite = ResourceManager.Instance.GetEndingImageWithName(ending.EndingImage);
                endingName.text = ending.endingName;
                endingGrade.text = $"당신의 점수는 : {ending.endingGrade}";
                endingGradeDescription.text = $"점수의 의미는 : {ending.endingGradedescription}";
                endingDescription.text = ending.endingDescription;
            }
        }

        GoogleSheetManager.Instance.isDataLoad = false;
    }

    public void GotoStartScene()
    {
        SceneManager.LoadScene(0);
    }
}
