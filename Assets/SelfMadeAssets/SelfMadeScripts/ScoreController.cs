using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreController : MonoBehaviour {

	public int score;

	// Use this for initialization
	void Start () {
		score = 0;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void GetScore(float distance){
		if(distance < 0.2 ){
			score += 100;
		}else if(distance < 0.4){
			score += 80;
		}else if(distance < 0.6){
			score += 60;
		}else if(distance < 0.8){
			score += 40;
		}else if(distance < 1.0){
			score += 20;
		}else if(distance < 1.6){
			score += 10;
		}
	}
}
