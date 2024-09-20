using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartScript : MonoBehaviour
{
    [SerializeField] private Image heart1;
    [SerializeField] private Image heart2;
    [SerializeField] private Image heart3;

    public void RemoveHeartImage() {
        if (heart3.IsActive()) heart3.enabled = false;
        else if (heart2.IsActive()) heart2.enabled = false;
        else if (heart1.IsActive()) heart1.enabled = false; 
    }
}
