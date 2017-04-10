using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {

	//private int bulletNum;  //弾数
	//private int bulletBoxNum;  //弾倉数
	private float coolTime;  //クールタイム
	private float effectLifeTime; //パーティクルの継続時間
	private AudioSource gunAudioSource;  //発砲音のスピーカー
	public AudioClip fire;  //発砲音
	public GameObject EffectPrefab;  //エフェクトのプレハブ
	private GameObject landingEffect;  //着弾時のエフェクト
	private GameObject muzzleEffect;  //銃口のエフェクト
	private Vector3 muzzleEffectPosition;  //銃口エフェクトのポジション
	private Vector3 landingEffectScale;
	private bool canShot = true;  //銃を撃てる状態かどうか

	void Start () {
		//bulletNum = 30;
		//bulletBoxNum = 150;
		coolTime = 0.5f;
		effectLifeTime = 0.2f;
		gunAudioSource = GetComponent<AudioSource> ();
		muzzleEffectPosition = new Vector3 (0, 0.1f, 0.853f);
		landingEffectScale = new Vector3 (0.2f, 0.2f, 0.2f);
	}
	
	void Update () {
		if(Input.GetMouseButtonDown(0) && canShot == true/*&& bulletNum > 0*/){  //クリック判定 + 撃てる弾があるか判定

			gunAudioSource.PlayOneShot (fire, 1.0f);  //発砲音再生
			muzzleEffect = (GameObject)Instantiate(EffectPrefab);
			muzzleEffect.transform.parent = transform;
			muzzleEffect.transform.localPosition = muzzleEffectPosition;
			muzzleEffect.transform.rotation = transform.rotation;
			//bulletNum --;  //撃てば弾が減る

			Ray gunRay = Camera.main.ViewportPointToRay (new Vector3 (0.5f, 0.5f, 0.0f));  //画面中央からRayを飛ばす
			Invoke("DestroyMuzzleEffect", effectLifeTime);

			//クールタイム
			canShot = false;
			Invoke ("EnableToShot", coolTime);


			RaycastHit hit;
			if(Physics.Raycast(gunRay, out hit)){//Rayのヒット判定
				landingEffect = (GameObject)Instantiate(EffectPrefab, hit.point + (transform.position - hit.point).normalized, Quaternion.identity);  //ヒットした点にパーティクル生成
				landingEffect.transform.rotation = transform.rotation;
				landingEffect.transform.localScale = landingEffectScale;//銃口のエフェクトよりも少し大きめにする



				//エフェクトをDestroy
				Invoke("DestroyLandingEffect", effectLifeTime);


			}
		}

		if(Input.GetKeyDown("x") /*&& bulletBoxNum > 0*/){  //xキーでリロード
			StartCoroutine ("Reload");
		}
	}

	void DestroyMuzzleEffect(){
		 //パーティクルは0.2秒で消滅する
		Destroy (muzzleEffect);
	}

	void DestroyLandingEffect(){
		//パーティクルは0.2秒で消滅する
		Destroy (landingEffect);
	}

	void EnableToShot(){
		canShot = true;
	}

	/*IEnumerator Reload(){
		yield return new WaitForSeconds (2.0f);  //リロード時間は2秒
		bulletBoxNum--;
	}*/
}
