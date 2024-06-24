using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InterfaceManager : MonoBehaviour
{
	private GameManager _gameManager;

    //UI icons
    public Image reliability;
    public Image anger;
	public Image anxiety;
	
    private void Start()
    {
	    _gameManager = GameManager.Instance;
    }

    void Update()
    {
	    if(_gameManager.reliability<0)
		{
            reliability.color = Color.blue;
			reliability.fillAmount = (float)(-_gameManager.reliability) / _gameManager.maxValue;
		}
		else
		{
			reliability.color = Color.yellow;
			reliability.fillAmount = (float)(_gameManager.reliability) / _gameManager.maxValue;
		}
	    
		if(_gameManager.anger<0)
		{
            anger.color = Color.blue;
			anger.fillAmount = (float)(-_gameManager.anger) / _gameManager.maxValue;
		}
		else
		{
            anger.color = Color.yellow;
			anger.fillAmount = (float)(_gameManager.anger) / _gameManager.maxValue;
		}
		
		if(_gameManager.anxiety < 0)
		{
            anxiety.color = Color.blue;
			anxiety.fillAmount = (float)(-_gameManager.anxiety) / _gameManager.maxValue;
		}
		else
		{
            anxiety.color = Color.yellow;
			anxiety.fillAmount = (float)(_gameManager.anxiety) / _gameManager.maxValue;
		}
    }
}
