using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {

	private int bulletNum;  //弾数
	private int bulletBoxNum;  //弾倉数
	private int maxBulletNum;  //装弾上限数
	private float coolTime;  //クールタイム
	private float effectLifeTime; //パーティクルの継続時間
	private AudioSource gunAudioSource;  //発砲音のスピーカー
	public AudioClip fire;  //発砲音
	public AudioClip reload;//リロード音
	public GameObject EffectPrefab;  //エフェクトのプレハブ
	private GameObject landingEffect;  //着弾時のエフェクト
	private GameObject muzzleEffect;  //銃口のエフェクト
	private Vector3 muzzleEffectPosition;  //銃口エフェクトのポジション
	private Vector3 landingEffectScale;
	private bool canShot = true;  //銃を撃てる状態かどうか
	private bool onReloading = false;

	void Start () {
		maxBulletNum = 30;
		bulletNum = maxBulletNum;
		bulletBoxNum = 150;
		coolTime = 0.5f;
		effectLifeTime = 0.2f;
		gunAudioSource = GetComponent<AudioSource> ();
		muzzleEffectPosition = new Vector3 (0, 0.1f, 0.853f);
	}
	
	void Update () {
		/*クリック判定
		 * 1.撃てる状態である
		 * 2.弾が装填されている
		 * 3.リロード中じゃない
		 * が条件
		*/
		if(Input.GetMouseButtonDown(0) && canShot == true && bulletNum > 0 && onReloading == false){

			gunAudioSource.PlayOneShot (fire, 1.0f);  //発砲音再生
			muzzleEffect = (GameObject)Instantiate(EffectPrefab);
			muzzleEffect.transform.parent = transform;
			muzzleEffect.transform.localPosition = muzzleEffectPosition;
			muzzleEffect.transform.rotation = transform.rotation;
			bulletNum --;  //撃てば弾が減る

			Ray gunRay = Camera.main.ViewportPointToRay (new Vector3 (0.5f, 0.5f, 0.0f));  //画面中央からRayを飛ばす
			Invoke("DestroyMuzzleEffect", effectLifeTime);

			//クールタイム
			canShot = false;
			Invoke ("EnableToShot", coolTime);


			RaycastHit hit;
			if(Physics.Raycast(gunRay, out hit)){//Rayのヒット判定
				//ヒットした点にエフェクト生成見やすくするためにポシションをプレイヤーに少し近づくように設定してある
				landingEffect = (GameObject)Instantiate(EffectPrefab, hit.point + (transform.position - hit.point).normalized, Quaternion.identity);
				landingEffect.transform.rotation = transform.rotation;

				//エフェクトをDestroy
				Invoke("DestroyLandingEffect", effectLifeTime);
			}
		}

		/*
		 * rキーでリロード
		 * 1.弾倉が残っている
		 * 2.装弾数が装弾上限数を下回っている
		 * がリロード条件
		*/
		if(Input.GetKeyDown("r") && bulletBoxNum > 0 && bulletNum < maxBulletNum){
			onReloading = true;        //リロード時に少しの間撃てなくなる
			gunAudioSource.PlayOneShot (reload, 1.0f);
			bulletBoxNum--;            //装弾数を上限値まで増やす。
			bulletNum = maxBulletNum;  //弾倉数を減らす。
			Invoke ("FinishReloading", 2.1f);//リロード音がなっている間は撃てないようにしたいから2.1
		}
	}

	void DestroyMuzzleEffect(){
		 //エフェクトは0.2秒で消滅する
		Destroy (muzzleEffect);
	}

	void DestroyLandingEffect(){
		//エフェクトは0.2秒で消滅する
		Destroy (landingEffect);
	}

	void EnableToShot(){
		canShot = true;
	}

	void FinishReloading(){
		onReloading = false;  //撃てる状態にする。
	}
}
