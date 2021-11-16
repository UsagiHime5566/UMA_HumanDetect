using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UMA;
using System.Threading.Tasks;
using DG.Tweening;
using System.Linq;

public class CustomGenerator : HimeLib.SingletonMono<CustomGenerator>
{
    public Slider SLD_ReduceTime;
    public Slider SLD_MinTime;
    public int move_x = 20;
    public float rand_z = 15;
    public float time = 15;
    public Transform LeftEdge;
    public Transform RightEdge;

    public UMARandomAvatar umaRandomer;
    public GameObject currentTrack;

    public Transform humanPool;
    public List<GameObject> humans = new List<GameObject>();

    public Vector3 bornPos = Vector3.zero;

    public bool canCreate = false;
    public float initDelay = 3;
    public float minDelay = 0.33f;
    public float decreaseDelay = 0.9f;
    [SerializeField] float currentDelay = 3;
    void Start()
    {
        //StartGenerate();

        SignalServer.instance.OnHumanSignal += HumanComing;

        currentDelay = initDelay;
        GenerateLoop();

        float reduceTime = SystemConfig.Instance.GetData<float>("ReduceTime", 0.9f);
        SLD_ReduceTime.onValueChanged.AddListener(x => {
            decreaseDelay = x;
            SystemConfig.Instance.SaveData("ReduceTime", x);
        });

        SLD_ReduceTime.value = reduceTime;


        float minTime = SystemConfig.Instance.GetData<float>("MinTime", 0.33f);
        SLD_MinTime.onValueChanged.AddListener(x => {
            minDelay = x;
            SystemConfig.Instance.SaveData("MinTime", x);
        });

        SLD_MinTime.value = minTime;
    }

    async void GenerateLoop(){
        await Task.Delay(3000);
        canCreate = true;

        while(this != null){
            if(canCreate){
                StartGenerate();
                currentDelay = Mathf.Max(minDelay, currentDelay * decreaseDelay);

                int delay = Mathf.FloorToInt(currentDelay * 1000);
                await Task.Delay(delay);
            } else {
                await Task.Delay(1000);
            }
        }
    }

    void HumanComing(){
        ResetDelay();

        foreach (var hu in humans.ToList())
        {
            hu.transform.DOComplete(true);
        }
    }

    public void ResetDelay(){
        currentDelay = initDelay;
    }

    void StartGenerate(){
        // if(currentTrack != null)
        //     Destroy(currentTrack);
        
        var last = umaRandomer.GenerateRandomCharacter(bornPos, Quaternion.Euler(0, 180, 0), "Generate Avator");
        last.AddComponent<HumanMove>().Init();
        last.transform.SetParent(humanPool);

        humans.Add(last);
        //RemoveRigidBody(currentTrack);
    }

    public GameObject GetNewUMA(Vector3 pos, Quaternion rot){
        GameObject temp = umaRandomer.GenerateRandomCharacter(pos, rot, "Generate Avator");
        temp.transform.SetParent(humanPool);

        //RemoveRigidBody(temp);
        return temp;
    }

    // async void RemoveRigidBody(GameObject avatarObj){
    //     Debug.Break();
	// 	await Task.Delay(100);
	// 	Debug.Log("Try to remove:" + avatarObj.GetComponent<Rigidbody>());
	// 	Destroy(avatarObj.GetComponent<Rigidbody>());
	// 	Destroy(avatarObj.GetComponent<Collider>());
	// }


    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)){
            StartGenerate();
        }
    }
}
