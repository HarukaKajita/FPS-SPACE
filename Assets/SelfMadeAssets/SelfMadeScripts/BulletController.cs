using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {

	public int bulletNum;  //弾数
	public int bulletBoxNum;  //弾倉数
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
	private Vector3 detail;  //着弾エフェクト生成座標の微調整のために定義
	private Vector3 landingEffectPosition;  //着弾エフェクト生成座標
	private bool canShot = true;  //銃を撃てる状態かどうか
	private bool onReloading = false; //リロード中かどうか
	private Vector3 maxScoreSpot;  //弾が当たった点がこの変数が示す点に近い方が得られるスコアが高くなる
	public Transform HeadMarker;  //maxScoreSpotの値を定義するためにターゲット自体のポジションを使う
	private ScoreController scoreController;  //スコア管理のスクリプト
	private TargetController targetController;  //ターゲット管理のスクリプト
	public UIController uiController;  //スナイプ時のスナイプ画面のImage表示のため
	private bool OnSniping = false;

	void Start () {
		maxBulletNum = 30;
		bulletNum = maxBulletNum;
		bulletBoxNum = 150;
		coolTime = 0.5f;
		effectLifeTime = 0.2f;
		gunAudioSource = GetComponent<AudioSource> ();
		muzzleEffectPosition = new Vector3 (0, 0.1f, 0.853f);
		maxScoreSpot = new Vector3(HeadMarker.position.x, HeadMarker.position.y + 1.55f, HeadMarker.position.z);
		landingEffectPosition = new Vector3();
	}
	
	void Update () {


		if(Input.GetMouseButtonDown(1)){  //時スナイプモード
			if (OnSniping == false) {
				OnSniping = true;
				//視野角を狭くすることで遠くを見えるようにする
				Camera.main.fieldOfView = 25;
				//スコープ画像をアクティブに
				uiController.snipeImageEnabled ();

			} else {  //スナイプ解除
				OnSniping = false;
				//視野をデフォルトに
				Camera.main.ResetFieldOfView ();
				//スコープ画像を非アクティブに
				uiController.snipeImageNotEnabled ();
			}

		}/* else {
			Camera.main.ResetFieldOfView();
			uiController.snipeImageNotEnabled ();
		}*/
		/*クリック判定
		 * 1.撃てる状態である
		 * 2.弾が装填されている
		 * 3.リロード中じゃない
		 * が条件
		*/
		if(Input.GetMouseButtonDown(0) && canShot == true && bulletNum > 0 && onReloading == false){

			gunAudioSource.PlayOneShot (fire, 1.0f);  //発砲音再生
			muzzleEffect = (GameObject)Instantiate(EffectPrefab);  //銃口にエフェクト
			muzzleEffect.transform.parent = transform;  
			muzzleEffect.transform.localPosition = muzzleEffectPosition;
			muzzleEffect.transform.rotation = transform.rotation;//エフェクトの見え方がプレイヤーの向きに依存しないようにする
			bulletNum --;  //撃てば弾が減る

			Ray gunRay = Camera.main.ViewportPointToRay (new Vector3 (0.5f, 0.5f, 0.0f));  //画面中央からRayを飛ばす
			Invoke("DestroyMuzzleEffect", effectLifeTime);//銃口のエフェクトは少し経ったら消える

			//クールタイム
			canShot = false;
			Invoke ("EnableToShot", coolTime);


			RaycastHit hit;
			if (Physics.Raycast (gunRay, out hit)) {//Rayのヒット判定
				//ベクトルの足し算だと微妙に位置がおかしくなることと
				//Update関数内でnewでメモリを確保するとメモリリークの原因になるのかなと思って回りくどい書き方になっています
				detail.x = (transform.position - hit.point).normalized.x * 0.2f;
				detail.y = (transform.position - hit.point).normalized.y * 0.2f;
				detail.z = (transform.position - hit.point).normalized.z * 0.2f;

				landingEffectPosition.x = hit.point.x + detail.x;
				landingEffectPosition.y = hit.point.y + detail.y;
				landingEffectPosition.z = hit.point.z + detail.z;

				//ヒットした点にエフェクト生成見やすくするためにポシションをプレイヤーに少し近づくように設定してある(欠陥あり)
				landingEffect = (GameObject)Instantiate (EffectPrefab, landingEffectPosition, transform.rotation);
				landingEffect.transform.rotation = transform.rotation;

				//-------スコアに関する処理-------------
				scoreController = hit.collider.transform.parent.GetComponent<ScoreController> ();
				if (scoreController != null) {
					float distance = Vector3.Distance (maxScoreSpot, hit.point);  //スコアを決める基準点からの距離
					scoreController.GetScore (distance);  //スコアを得るメソッドの呼び出し。引数はスコアを決めるための距離
				}

				//-------ターゲットのライフに関する処理----
				targetController = hit.collider.transform.parent.GetComponent<TargetController> ();
				if (targetController != null) {
					targetController.GetDamaged ();//ターゲットがダメージを受ける
				}


				//エフェクトをDestroy
				Invoke ("DestroyLandingEffect", effectLifeTime);
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
			while(bulletNum < maxBulletNum && bulletBoxNum > 0){
				bulletNum++;
				bulletBoxNum--;
			}
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
