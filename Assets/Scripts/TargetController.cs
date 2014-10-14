using UnityEngine;
using System.Collections;

public class TargetController : MonoBehaviour
{
		public static TargetController instance;
		public Transform head;
		public float precision = 0.1f;

		void Awake ()
		{
				instance = this;
				Random.seed = (int)System.DateTime.Now.Ticks;
		}

		void Start ()
		{
				makeMissedTarget ();
		}

		public Vector3 target;

		public void makeAimedTarget ()
		{
				float y = Random.Range (-precision, precision);
				float x = Random.Range (-precision, precision);
				float z = Random.Range (-precision, precision);
				target = head.position + new Vector3 (x, y, z);
		}

		public void makeMissedTarget ()
		{
				float y = 0.6f;
				float x = Random.Range (-precision, precision);
				float z = Random.Range (-precision, precision);
				target = head.position + new Vector3 (x, y, z);
		}
}
