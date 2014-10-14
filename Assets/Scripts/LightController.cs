using UnityEngine;
using System.Collections;

public class LightController : MonoBehaviour
{
		public static LightController instance;
		public Light roomLight1;
		public Light roomLight2;
		public float targetIntensity1;
		public float targetIntensity2;
		public Color ambientLightSave;
		public float time = 10;
		public string readyLevel;

		void Awake ()
		{
				instance = this;
		}

		void Start ()
		{

				ambientLightSave = RenderSettings.ambientLight;
				RenderSettings.ambientLight = new Color (0, 0, 0, 0);

				targetIntensity1 = roomLight1.intensity;
				roomLight1.intensity = 0;
				targetIntensity2 = roomLight2.intensity;
				roomLight2.intensity = 0;
				StartCoroutine (lightsOn ());
		}

		IEnumerator lightsOn ()
		{
				float startTime = Time.time;
				while (Time.time - startTime < time) {
						RenderSettings.ambientLight = Color.Lerp (new Color (0, 0, 0, 0), ambientLightSave, Mathf.Pow (((Time.time - startTime) / time), 3));
						roomLight1.intensity = targetIntensity1 * Mathf.Pow (((Time.time - startTime) / time), 3);
						roomLight2.intensity = targetIntensity2 * Mathf.Pow (((Time.time - startTime) / time), 3);
						yield return null;
				}
		}

		public void callLightsOut ()
		{
				StartCoroutine (lightsOut ());
		}

		IEnumerator lightsOut ()
		{
				float startTime = Time.time;
				while (Time.time - startTime < time) {
						RenderSettings.ambientLight = Color.Lerp (ambientLightSave, new Color (0, 0, 0, 0), Mathf.Pow (((Time.time - startTime) / time), 3));
						roomLight1.intensity = targetIntensity1 * (1 - Mathf.Pow (((Time.time - startTime) / time), 3));
						roomLight2.intensity = targetIntensity2 * (1 - Mathf.Pow (((Time.time - startTime) / time), 3));
						yield return null;
				}
				Application.LoadLevel (readyLevel);
		}

}
