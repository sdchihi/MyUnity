using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class FireCtr : MonoBehaviour {

    public GameObject bullet;
    public Transform firePos;

    //발사 사운드
    public AudioClip fireSfx;
    private AudioSource source = null;
    
    //MuzzleFlash의 meshRenderer 컴포넌트 연결 변수
    public MeshRenderer muzzleFlash;

    void Start() {
        source = GetComponent<AudioSource>();

        muzzleFlash.enabled = false;
    }
	
	void Update () {

        if (Input.GetMouseButtonDown(0)) {
            Fire();
        }
	}



    void Fire() {
        CreateBullet();
        //사운드 발생 함수라고합니다.
        source.PlayOneShot(fireSfx, 0.9f);

        StartCoroutine(this.ShowMuzzleFlash());
    }
    void CreateBullet() {
        //동적으로 bullet 프리팹에서 bullet오브젝트를 생성
        Instantiate(bullet, firePos.position, firePos.rotation); 
    }

    //MuzzleFlash를 짧은시간동안 활성화/비활성화를 반복하게함
    IEnumerator ShowMuzzleFlash()
    {
        //총구화염의 크기가 불규칙하게 생성되도록.
        float scale = Random.Range(1.0f, 2.0f);
        muzzleFlash.transform.localScale = Vector3.one * scale;

        //총구화염이 z축으로 불규칙하게 회전하도록 만듬.
        Quaternion rot = Quaternion.Euler(0, 0, Random.Range(0, 360));
        muzzleFlash.transform.localRotation = rot;

        muzzleFlash.enabled = true;

        //불규칙적인 시간ㄷ동안 딜레이하고 , MeshRenderer를 비활성화함
        yield return new WaitForSeconds(Random.Range(0.05f, 0.3f));

        muzzleFlash.enabled = false;
    }
}
