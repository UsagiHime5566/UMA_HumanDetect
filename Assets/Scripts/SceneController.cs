using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    public GameObject targetScene;
    public GameObject BlinkPrefab;

    public float revertTime = 30;
    public float stackTime = float.MinValue;

    [Header("UI")]
    public InputField INP_RevertTime;

    void Start()
    {
        SignalServer.instance.OnHumanSignal += HideScene;

        float rt = SystemConfig.Instance.GetData<float>("RecoverTime", 30);
        INP_RevertTime.onValueChanged.AddListener(x => {
            float result = 30;

            if(!float.TryParse(x, out result))
                return;
            
            revertTime = result;
            SystemConfig.Instance.SaveData("RecoverTime", result);
        });

        INP_RevertTime.text = rt.ToString("0.00");
    }

    void HideScene(){
        if(targetScene.activeSelf)
            Instantiate(BlinkPrefab);

        targetScene.SetActive(false);
        CustomGenerator.instance.canCreate = false;
        stackTime = 0;
    }

    void Update(){
        stackTime += Time.deltaTime;

        if(stackTime > revertTime){
            targetScene.SetActive(true);
            CustomGenerator.instance.canCreate = true;
            //Instantiate(BlinkPrefab);
            stackTime = float.MinValue;
        }
    }
}
