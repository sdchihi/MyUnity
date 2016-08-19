using UnityEngine;
using System.Collections;

public class FollowCam : MonoBehaviour
{
    public Transform targetTr;
    public Transform bossTr;

    public float dist = 3.0f;
    public float height = 1.0f;
    public float dampTraace = 10.0f;

    private Transform tr;

    // Use this for initialization
    void Start()
    {
        tr = GetComponent<Transform>();
        bossTr = GetComponent<Transform>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        
        tr.position = Vector3.Lerp(tr.position,new Vector3(targetTr.position.x-dist, targetTr.position.y+height,targetTr.position.z) , Time.deltaTime * dampTraace);
        //x-3 y+1
        tr.LookAt(targetTr.position);
        
     
        
    }

    void StartBoss()
    {
        float Bdist = 5.0f;
        float Bheight = 3.0f;
        float BdampTraace = 1;
        tr.position = Vector3.Lerp(tr.position, new Vector3(bossTr.position.x - Bdist, bossTr.position.y + Bheight, bossTr.position.z), Time.deltaTime * BdampTraace);
    }

}