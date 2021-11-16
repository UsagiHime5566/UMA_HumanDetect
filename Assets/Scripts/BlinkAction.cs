using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BlinkAction : MonoBehaviour
{
    public Image targetPlane;
    void Start()
    {
        targetPlane.DOFade(0, 1).OnComplete(delegate {
            Destroy(gameObject, 1);
        });
    }
}
