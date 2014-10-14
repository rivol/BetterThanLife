using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
		public int lives = 5;
		public GameObject splatterPrefab;
		public AudioClip[] hitPlayerSounds;

		public void onHit ()
		{
				GameObject spl = Instantiate (splatterPrefab) as GameObject;
				spl.transform.position = transform.position;
				PlayClipAtPointPitch (hitPlayerSounds [Random.Range (0, hitPlayerSounds.Length)], transform.position, 1.0f, 0.8f + Random.Range (-0.1f, 0.1f));

				lives--;
				if (lives <= 0) {
						LightController.instance.callLightsOut ();
						Enemy.instance.stopFiring ();
				}
		}

		GameObject PlayClipAtPointPitch (AudioClip clip, Vector3 position, float volume, float pitch)
		{
				GameObject obj = new GameObject ();
				obj.transform.position = position;
				obj.AddComponent<AudioSource> ();
				obj.audio.pitch = pitch;
				obj.audio.PlayOneShot (clip, volume);
				Destroy (obj, clip.length / pitch);
				return obj;
		}
}
