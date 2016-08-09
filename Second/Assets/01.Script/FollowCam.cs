using UnityEngine;
using System.Collections;

public class FollowCam : MonoBehaviour
{
    public Transform targetTr;
    public float dist = 3.0f;
    public float height = 1.0f;
    public float dampTraace = 10.0f;

    private Transform tr;

    // Use this for initialization
    void Start()
    {
        tr = GetComponent<Transform>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        tr.position = Vector3.Lerp(tr.position,new Vector3(targetTr.position.x-dist, targetTr.position.y+height,targetTr.position.z) , Time.deltaTime * dampTraace);
        //x-3 y+1
        tr.LookAt(targetTr.position);
    }
}