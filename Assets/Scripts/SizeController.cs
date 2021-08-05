using System;
using TMPro;
using UnityEngine;

public class SizeController : MonoBehaviour
{
    [Header("Components"), SerializeField]
    private TMP_Text value;

    private int currentValue = 2;

    private void Start()
    {
        value.text = currentValue.ToString();
    }

    public void OnNextClicked()
    {
        if (currentValue + 1 < 9999999)
        {
            currentValue++;
            value.text = currentValue.ToString();
        }
    }

    public void OnPreviousClicked()
    {
        if (currentValue - 1 > 1)
        {
            currentValue--;
            value.text = currentValue.ToString();
        }
    }

    public int GetValue()
    {
        return Convert.ToInt32(value.text) + 1;
    }
}