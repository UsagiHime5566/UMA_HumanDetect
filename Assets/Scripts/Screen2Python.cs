using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Screen2Python : MonoBehaviour
{
    public ScriptsCamera scriptsCamera;
    public float CatchSpan = 1.0f;

    [Header("UI")]
    public InputField INP_CatchSpan;

    WaitForSeconds wait;
    void Start()
    {
        scriptsCamera.OnBackgroundBase64Get += FromScreen;
        wait = new WaitForSeconds(CatchSpan);

        float rt = SystemConfig.Instance.GetData<float>("DetectPeriod", 1);
        INP_CatchSpan.onValueChanged.AddListener(x => {
            float result = 1;

            if(!float.TryParse(x, out result))
                return;
            
            CatchSpan = result;
            wait = new WaitForSeconds(result);
            SystemConfig.Instance.SaveData("DetectPeriod", result);
        });
        INP_CatchSpan.text = rt.ToString("0.00");

        StartCoroutine(PeriodCatch());
    }
    IEnumerator PeriodCatch(){
        while(true){
            yield return wait;
            scriptsCamera.DoTakePhotoBase64();
        }
    }

    void FromScreen(string base64){
        SignalServer.instance.SocketSend(base64);
        //Debug.Log($"UTF-8 send");
        //Debug.Log($"UTF-8 send:\n{base64}");
    }
}
