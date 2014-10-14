using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
		public GameObject player;
		public GameObject hitSphere;
		public GameObject leftEye;
		public GameObject rightEye;
		public GameObject cube;
		private Transform spineTransform;
		private Quaternion spineInitialRotation;
		private Vector3 initialRotateFrom;
	
		// Use this for initialization
		void Start ()
		{
				spineTransform = player.transform.Find ("Hip/Spine1");
				spineInitialRotation = spineTransform.rotation;
				Transform neckTransform = player.transform.Find ("Hip/Spine1/Spine2/Spine3/Spine4/Neck1");
				initialRotateFrom = (neckTransform.position - spineTransform.position).normalized;
		}
	
		// Update is called once per frame
		void Update ()
		{
				Vector3 facePos = (leftEye.transform.position + rightEye.transform.position) / 2;
				cube.transform.position = facePos;
				Vector3 rotateTo = facePos - spineTransform.position;

				Quaternion foo = Quaternion.FromToRotation (initialRotateFrom, rotateTo.normalized) * spineInitialRotation;
				spineTransform.rotation = foo;

				// Update hit sphere's position
				hitSphere.transform.position = facePos - rotateTo.normalized * 0.02f;
		}
}
