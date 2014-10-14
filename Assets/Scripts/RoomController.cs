using UnityEngine;
using System.Collections;

public class RoomController : MonoBehaviour
{
		public GameObject ceilingLight1;
		public GameObject ceilingLight2;

		void Start ()
		{
		}
	
		// Update is called once per frame
		void Update ()
		{
				float scaledTime = Time.time * 0.25f;
				float phase = scaledTime * Mathf.PI * 2;

				ceilingLight1.transform.localRotation = getLightTransform (phase, 30.0f, 0.3f, 0.15f);
				ceilingLight2.transform.localRotation = getLightTransform (phase + 2, 160.0f, 0.3f, 0.2f);
		}

		private Quaternion getLightTransform (float phase, float y_rot, float x_amplitude, float z_amplitude)
		{
				Vector3 targetPos = new Vector3 (Mathf.Sin (phase) * x_amplitude, -1f, Mathf.Cos (phase) * z_amplitude);
				return Quaternion.AngleAxis (y_rot, Vector3.up) * Quaternion.FromToRotation (-Vector3.up, targetPos);
		}
}
