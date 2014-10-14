using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
		public static Enemy instance;
		public Transform drawHand;
		public Transform holdHand;
		public Transform zRotationObject;
		public Transform yRotationObject;
		public GameObject arrowPrefab;
		public GameObject flamingArrowPrefab;
		public AudioClip[] windupSounds;
		[HideInInspector]
		public Arrow
				arrow;
		float startTime;
		EnemyState state;
		public float fireRate = 3.0f;
		public float difficulty = 0.9f;
		private int count = 0;
		private AudioSource audioSource = null;

		void Awake ()
		{
				instance = this;
		}

		// Use this for initialization
		void Start ()
		{
				audioSource = gameObject.GetComponent<AudioSource> ();

				arrow = createArrow ();
				startTime += 5.0f;
		}
	
		// Update is called once per frame
		void Update ()
		{
				if (state == EnemyState.firing) {
						if (arrow.state == arrowStates.drawn) {
								zRotationObject.Rotate (5 * Time.deltaTime * difficulty, 0, 0, Space.World);
								if (!arrow.isBetter ()) {
										zRotationObject.Rotate (-10 * Time.deltaTime * difficulty, 0, 0, Space.World);
										if (!arrow.isBetter ()) {
												zRotationObject.Rotate (5 * Time.deltaTime * difficulty, 0, 0, Space.World);
										}
								}
								arrow.setAngle ();
								yRotationObject.Rotate (0, 0.1f, 5 * Time.deltaTime * difficulty, Space.World);
								if (!arrow.isBetter ()) {
										yRotationObject.Rotate (0, -0.2f, -10 * Time.deltaTime * difficulty, Space.World);
										if (!arrow.isBetter ()) {
												yRotationObject.Rotate (0, 0.1f, 5 * Time.deltaTime * difficulty, Space.World);
										}
								}
						}

						if (Time.time - startTime > fireRate / difficulty && arrow.targetAngle < 1) {
								audioSource.Stop ();
								arrow.fire (20);
								difficulty += 0.05f;
								TargetController.instance.precision *= 0.95f;
								TargetController.instance.makeAimedTarget ();
								arrow = createArrow ();
						}
				}
		}

		public void attemptRotation ()
		{

		}

		private Arrow createArrow ()
		{
				count++;
				GameObject prefab = (Random.Range (-8, 0) - 3 + count >= 0) ? flamingArrowPrefab : arrowPrefab;
				Arrow arrow = (Instantiate (prefab) as GameObject).GetComponent<Arrow> ();
				arrow.drawHand = drawHand;
				arrow.holdHand = holdHand;
				arrow.setUp ();

				startTime = Time.time;
				windUpSound ();

				return arrow;
		}

		private void windUpSound ()
		{   
				AudioClip clip = windupSounds [Random.Range (0, windupSounds.Length)];

				float EndTime = startTime + (fireRate / difficulty);
				if (count == 1) {
						EndTime += 5.0f;
				}

				float AudioStartTime = EndTime - clip.length;
				audioSource.clip = clip;

				if (AudioStartTime - startTime > 0) {
						audioSource.PlayDelayed (AudioStartTime - startTime);
				} else {
						audioSource.Play ();
				}
		}

		public void stopFiring ()
		{
				state = EnemyState.stopped;
		}
}

public enum EnemyState
{
		firing,
		stopped
}
