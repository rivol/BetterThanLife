using UnityEngine;
using System.Collections;

public class SplatterEffect : MonoBehaviour {
	public float speed = 3;

 void Awake(){
		StartCoroutine(splash());
	}

	IEnumerator splash(){
		Debug.Log (renderer);
		Debug.Log (renderer.material);
		while(renderer.material.GetColor("_TintColor").a < 0.8f){
			renderer.material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, renderer.material.GetColor("_TintColor").a+Time.deltaTime*speed));
			yield return null;
		}
		while(renderer.material.GetColor("_TintColor").a > 0){
			renderer.material.SetColor("_TintColor", new Color(0.5f, 0.5f, 0.5f, renderer.material.GetColor("_TintColor").a-Time.deltaTime*speed));
			yield return null;
		}
		Destroy(gameObject);
	}
}
