using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameUI : MonoBehaviour {

    public Image fadePlane;
    public GameObject gameOverUI;


	void Start () {
        FindObjectOfType<Player>().OnDeath += OnGameOver;
        gameOverUI.SetActive(true);
	}


    void OnGameOver() {
        StartCoroutine(Fade(Color.clear, Color.black, 1));
    }

    //이건 예전에 써봤던것 . 코루틴으로 Color를 Lerp시켜서 Fade시킨다. 
    //이걸 응용시키면 완전히 화면이 검은색으로 변환후에 어떤UI띄우는것도 괜찮을것같다라는 생각이 듬. 검은색 화면에서 갑자기 다른 UI가 뿅
    IEnumerator Fade(Color from, Color to, float time) {

        float speed = 1 / time;
        float percent = 0;

        while (percent < 1) {
            percent = Time.deltaTime * speed;
            fadePlane.color = Color.Lerp(from, to, percent);
            yield return null;
        } 
    }

    // UI Input
    
    //버튼하고 이어줄 씬 불러오는 부분
    public void StartNewGame() {
        Application.LoadLevel("Game");
    }

}
