using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

	[SerializeField] private ScoreController scoreController;  //UIに表示させる数値の参照元
	[SerializeField] private BulletController bulletController;  //UIに表示させる数値の参照元

	[SerializeField] private Text scoreText;
	[SerializeField] private Text timerText;
	[SerializeField] private Text bulletNumText;
	[SerializeField] private Text bulletBoxNumText;
	[SerializeField] private Image snipeImage;

	[SerializeField] private float timer;

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



		scoreText.text = "Pt:" + scoreController.score;
		timerText.text = "Time:" + timer.ToString("0.0") + "s";
		bulletNumText.text = "Bullet:" + bulletController.bulletNum;
		bulletBoxNumText.text = "BulletBox:" + bulletController.bulletBoxNum;
	
	}

	public void snipeImageEnabled(){
		snipeImage.enabled = true;
	}

	public void snipeImageNotEnabled(){
		snipeImage.enabled = false;
	}
}
