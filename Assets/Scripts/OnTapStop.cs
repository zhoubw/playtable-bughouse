using UnityEngine;
using System.Collections;
using TouchScript.Gestures;

public class OnTapStop : MonoBehaviour {

	TapGesture tGesture;

	void Awake(){
		tGesture = GetComponent<TapGesture> ();
	}

	void OnEnable(){
		tGesture.Tapped += OnTap;
	}

	void OnTap(object sender, System.EventArgs e){
		GameManager gm = GetComponentInParent<GameManager> ();
		gm.stop ();
	}

	void OnDisable(){
		tGesture.Tapped -= OnTap;
	}
}
