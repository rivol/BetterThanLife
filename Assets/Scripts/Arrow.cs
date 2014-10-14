using UnityEngine;
using System.Collections;

public class Arrow : MonoBehaviour
{
		public Transform drawHand;
		public Transform holdHand;
		public float l;
		public arrowStates state;
		public GameObject theFlame;
		public AudioClip[] shootSounds;
		public AudioClip[] hitWallSounds;
		float speed = 1;
		Vector3 direction = Vector3.forward;
		[HideInInspector]
		public float targetAngle;

		// Use this for initialization
		void Start ()
		{
				l = GetComponent<Renderer> ().bounds.size.z;
		}

		public void setUp ()
		{
				direction = holdHand.position - drawHand.position;
				transform.rotation = Quaternion.LookRotation (direction);
				transform.position = drawHand.position + l * direction.normalized;
				Vector3 targetDirection = TargetController.instance.target - drawHand.position;
				targetAngle = Vector3.Angle (direction, targetDirection);
				state = arrowStates.drawn;
		}
	
		// Update is called once per frame
		void Update ()
		{
				if (state == arrowStates.drawn) {
						direction = holdHand.position - drawHand.position;
						transform.rotation = Quaternion.LookRotation (direction);
						transform.position = drawHand.position + l * direction.normalized;
						Vector3 targetDirection = TargetController.instance.target - drawHand.position;
						targetAngle = Vector3.Angle (direction, targetDirection);
				} else if (state == arrowStates.firing) {
						transform.position += speed * Time.deltaTime * direction;
				}
		}

		public void fire (float speed)
		{
				if (state == arrowStates.drawn) {
						this.speed = speed;
						state = arrowStates.firing;

						AudioSource.PlayClipAtPoint (shootSounds [Random.Range (0, shootSounds.Length)], transform.position);
				}
		}

		public bool isBetter ()
		{
				direction = holdHand.position - drawHand.position;
				transform.rotation = Quaternion.LookRotation (direction);
				transform.position = drawHand.position + l * direction.normalized;
				Vector3 targetDirection = TargetController.instance.target - drawHand.position;
				float newTargetAngle = Vector3.Angle (direction, targetDirection);
				if (newTargetAngle < targetAngle)
						return true;
				return false;
		}

		public void setAngle ()
		{
				direction = holdHand.position - drawHand.position;
				transform.rotation = Quaternion.LookRotation (direction);
				transform.position = drawHand.position + l * direction.normalized;
				Vector3 targetDirection = TargetController.instance.target - drawHand.position;
				targetAngle = Vector3.Angle (direction, targetDirection);
		}

		void OnTriggerEnter (Collider other)
		{
				if (other.GetComponent<Player> ()) {
						Debug.Log ("hit player");
						other.GetComponent<Player> ().onHit ();
				} else {
						transform.parent = other.transform;
						state = arrowStates.walled;
						Debug.Log ("Triggered");
						AudioSource.PlayClipAtPoint (hitWallSounds [Random.Range (0, hitWallSounds.Length)], transform.position);

						StartCoroutine (FlameDestroyer ());
				}
		}

		IEnumerator FlameDestroyer ()
		{
				yield return new WaitForSeconds (5.0f);
				Destroy (theFlame);
		}
}

public enum arrowStates
{
		drawn,
		firing,
		walled
}