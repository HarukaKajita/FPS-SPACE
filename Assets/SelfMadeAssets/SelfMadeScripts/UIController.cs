using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

	public ScoreController scoreController;  //UIに表示させる数値の参照元
	public BulletController bulletController;  //UIに表示させる数値の参照元

	public Text scoreText;
	public Text timerText;
	public Text bulletNumText;
	public Text bulletBoxNumText;
	public Image snipeImage;

	public int scoreNum;
	private float timer;
	public int bulletNum;
	public int bulletBoxNum;

	// Use this for initialization
	void Start () {
		timer = 90;
	}
	
	// Update is called once per frame
	void Update () {
		timer -= Time.deltaTime;

		if(timer <= 0){//タイマーが0秒になったらどうなるのかが分からないので一旦0sの表記のまま何も変化ない状態にする
			timer = 0.0f;
		}

		scoreNum = scoreController.score;
		bulletNum = bulletController.bulletNum;
		bulletBoxNum = bulletController.bulletBoxNum;

		scoreText.text = "Pt:" + scoreNum;
		timerText.text = "Time:" + timer.ToString("0.0") + "s";
		bulletNumText.text = "Bullet:" + bulletNum;
		bulletBoxNumText.text = "BulletBox:" + bulletBoxNum;
	
	}

	public void snipeImageEnabled(){
		snipeImage.enabled = true;
	}

	public void snipeImageNotEnabled(){
		snipeImage.enabled = false;
	}
}
