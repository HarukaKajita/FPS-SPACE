using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour {

	[SerializeField] private int life;
	[SerializeField] private int defaultLife;
	[SerializeField] private Animator anim;
	// Use this for initialization
	void Start () {
		defaultLife = 5;
		life = defaultLife;
		anim = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		if(life <= 0){
			anim.SetBool ("broken", true); //5回撃たれたら倒れる
			Invoke ("UpTarget",10f); //10s後に立ち上がる
			life = defaultLife; //立ち上がった後にまた5回撃たれたら倒れるようにライフを回復
		}
	}

	void UpTarget(){
		anim.SetBool ("broken", false);
	}

	public void GetDamaged(){
		life--;
	}
}
