using UnityEngine;
using System.Collections;

public class AudioFadeIn : MonoBehaviour
{

		public float startLevel = 0.0f;
		public float endLevel = 0.165f;
		public float duration = 3.0f;
		public float delay = 2.0f;
		public float playDelay = 1.5f;
		private AudioSource audioSource = null;

		// Use this for initialization
		void Start ()
		{
				audioSource = gameObject.GetComponent<AudioSource> ();

				StartCoroutine (FadeAudio (startLevel, endLevel, duration, delay));
				audioSource.PlayDelayed (playDelay);
		}

		IEnumerator FadeAudio (float startLevel, float endLevel, float duration, float delay)
		{
				if (delay > 0) {
						yield return new WaitForSeconds (delay);
				}

				audioSource.volume = startLevel;
				float speed = 1.0f / duration;

				for (float t = 0.0f; t < 1.0; t += Time.deltaTime * speed) {
						audioSource.volume = Mathf.Lerp (startLevel, endLevel, t);
						yield return 1;
				}

				audioSource.volume = endLevel;
		}
}
