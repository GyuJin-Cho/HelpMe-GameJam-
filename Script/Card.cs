using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Card : MonoBehaviour
{
    public string leftQuote;
    public string rightQuote;

    public void Left()
    {
        GameManager.Instance.LValueInit();
        
        AudioPlayer.Instance.PlayClip(1);
    }

    public void Right()
    {
        GameManager.Instance.RValueInit();
        
        AudioPlayer.Instance.PlayClip(1);
    }
}
