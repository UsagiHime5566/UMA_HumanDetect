using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class UINumberToText : MonoBehaviour
{
    Text _text;
    void Awake(){
        _text = GetComponent<Text>();
    }
    
    public void UpdateTextBySingle(double num){
        string t = num.ToString("0.00");
        _text.text = $"({t})";
    }

    public void UpdateTextBySingle(float num){
        string t = num.ToString("0.00");
        _text.text = $"({t})";
    }
}
