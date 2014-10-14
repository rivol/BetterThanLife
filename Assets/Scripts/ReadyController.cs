using UnityEngine;
using System.Collections;

public class ReadyController : MonoBehaviour
{
		public string LayerName = "Default";
		public Font FontReplace = null;
		public string targetLevel;

    #region privates

		private string currentText = "Are you ready?";
		private Color textColor = new Color (255, 255, 255, 0);

		// Handle to OVRCameraController
		private OVRCameraController CameraController = null;

		// Handle to OVRPlayerController
		private OVRPlayerController PlayerController = null;
		private OVRGUI GuiHelper = new OVRGUI ();
		private GameObject GUIRenderObject = null;
		private RenderTexture GUIRenderTexture = null;
		private int VRVarsSX = 553;
		private int VRVarsSY = 350;
		private int VRVarsWidthX = 200;
		private int VRVarsWidthY = 58;

		private delegate void Callback ();

		private enum MenuState
		{
				START = 0,
				PRE_INIT,
				MAIN_QUESTION,
				LOADING_LEVEL,
				PRE_QUIT_CONFIRM,
				QUIT_CONFIRM,
				CANCEL_QUIT
    }
		;

		private MenuState state;
		private float waitTimer = 2.0f;
		private AudioSource audioSource = null;

    #endregion privates

		void Awake ()
		{
				// Find camera controller
				OVRCameraController[] CameraControllers;
				CameraControllers = gameObject.GetComponentsInChildren<OVRCameraController> ();

				if (CameraControllers.Length == 0)
						Debug.LogWarning ("MenuHelperController: No OVRCameraController attached.");
				else if (CameraControllers.Length > 1)
						Debug.LogWarning ("MenuHelperController: More then 1 OVRCameraController attached.");
				else {
						CameraController = CameraControllers [0];
				}

				// Find player controller
				OVRPlayerController[] PlayerControllers;
				PlayerControllers = gameObject.GetComponentsInChildren<OVRPlayerController> ();

				if (PlayerControllers.Length == 0)
						Debug.LogWarning ("MenuHelperController: No OVRPlayerController attached.");
				else if (PlayerControllers.Length > 1)
						Debug.LogWarning ("MenuHelperController: More then 1 OVRPlayerController attached.");
				else {
						PlayerController = PlayerControllers [0];
				}
		}

		void Start ()
		{
				audioSource = gameObject.GetComponent<AudioSource> ();

				StartCoroutine (FadeAudio (0.0f, 0.5f, 3.0f, 1.0f));

				// Ensure that camera controller variables have been properly
				// initialized before we start reading them
				if (CameraController != null)
						CameraController.InitCameraControllerVariables ();

				// Set the GUI target
				GUIRenderObject = GameObject.Instantiate (Resources.Load ("OVRGUIObjectMain")) as GameObject;

				if (GUIRenderObject != null) {
						// Chnge the layer
						GUIRenderObject.layer = LayerMask.NameToLayer (LayerName);

						if (GUIRenderTexture == null) {
								int w = Screen.width;
								int h = Screen.height;

								// We don't need a depth buffer on this texture
								GUIRenderTexture = new RenderTexture (w, h, 0);
								GuiHelper.SetPixelResolution (w, h);
								// NOTE: All GUI elements are being written with pixel values based
								// from DK1 (1280x800). These should change to normalized locations so 
								// that we can scale more cleanly with varying resolutions
								//GuiHelper.SetDisplayResolution(OVRDevice.HResolution, 
								//								 OVRDevice.VResolution);
								GuiHelper.SetDisplayResolution (1280.0f, 800.0f);
						}
				}

				// Attach GUI texture to GUI object and GUI object to Camera
				if (GUIRenderTexture != null && GUIRenderObject != null) {
						GUIRenderObject.renderer.material.mainTexture = GUIRenderTexture;

						if (CameraController != null) {
								// Grab transform of GUI object
								Vector3 ls = GUIRenderObject.transform.localScale;
								Vector3 lp = GUIRenderObject.transform.localPosition;
								Quaternion lr = GUIRenderObject.transform.localRotation;

								// Attach the GUI object to the camera
								CameraController.AttachGameObjectToCamera (ref GUIRenderObject);
								// Reset the transform values (we will be maintaining state of the GUI object
								// in local state)

								GUIRenderObject.transform.localScale = ls;
								GUIRenderObject.transform.localRotation = lr;

								// Deactivate object until we have completed the fade-in
								// Also, we may want to deactive the render object if there is nothing being rendered
								// into the UI
								// we will move the position of everything over to account for the IPD camera offset. 
								float ipdOffsetDirection = 1.0f;
								Transform guiParent = GUIRenderObject.transform.parent;
								if (guiParent != null) {
										OVRCamera ovrCamera = guiParent.GetComponent<OVRCamera> ();
										if (ovrCamera != null && ovrCamera.RightEye)
												ipdOffsetDirection = -1.0f;
								}

								float ipd = 0.0f;
								CameraController.GetIPD (ref ipd);
								lp.x += ipd * 0.5f * ipdOffsetDirection;
								GUIRenderObject.transform.localPosition = lp;

								GUIRenderObject.SetActive (false);
						}
				}

				// Make sure to hide cursor 
				if (Application.isEditor == false) {
						Screen.showCursor = false;
						Screen.lockCursor = true;
				}

				// CameraController updates
				if (CameraController != null) {
						// Set LPM on by default
						OVRDevice.SetLowPersistenceMode (true);
				}

				state = MenuState.START;
		}

		// Update is called once per frame
		void Update ()
		{
				if (state == MenuState.START) {
						waitTimer -= Time.deltaTime;
						if (waitTimer < 0) {
								enterPreInitState ();
						}
				} else if (state == MenuState.MAIN_QUESTION) {
						if (Input.GetKeyDown (KeyCode.Return)) {
								state = MenuState.LOADING_LEVEL;

								StartCoroutine (goToLevel ());
						} else if (Input.GetKeyDown (KeyCode.Escape)) {
								state = MenuState.PRE_QUIT_CONFIRM;
								StartCoroutine (Fade (1.0f, 0.0f, 0.5f, showQuitConfirm));
						}
				} else if (state == MenuState.QUIT_CONFIRM) {
						if (Input.GetKeyDown (KeyCode.Return)) {
								Application.Quit ();
						} else if (Input.GetKeyDown (KeyCode.Escape)) {
								state = MenuState.CANCEL_QUIT;
								StartCoroutine (Fade (1.0f, 0.0f, 0.5f, enterPreInitState));
						}
				}
		}

		void loadTargetLevel ()
		{
				Application.LoadLevel (targetLevel);
		}

		void showQuitConfirm ()
		{
				currentText = "Quit the game?\n\nNod your head or press Enter for yes,\nshake or Escape for no";
				StartCoroutine (Fade (0.0f, 1.0f, 0.5f, enterQuitConfirmState));
		}

		void enterPreInitState ()
		{
				state = MenuState.PRE_INIT;

				currentText = "Are you ready?\n\nNod your head or press Enter for yes,\nshake or Escape for no";
				StartCoroutine (Fade (0.0f, 1.0f, 1.0f, enterMainQuestionState));
		}

		void enterMainQuestionState ()
		{
				state = MenuState.MAIN_QUESTION;
		}

		void enterQuitConfirmState ()
		{
				state = MenuState.QUIT_CONFIRM;
		}

		public void TriggerYes ()
		{
				if (state == MenuState.MAIN_QUESTION) {
						state = MenuState.LOADING_LEVEL;
						StartCoroutine (Fade (1.0f, 0.0f, 1.0f, loadTargetLevel));
				} else if (state == MenuState.QUIT_CONFIRM) {
						Application.Quit ();
				}
		}

		public void TriggerNo ()
		{
				if (state == MenuState.MAIN_QUESTION) {
						state = MenuState.PRE_QUIT_CONFIRM;
						StartCoroutine (Fade (1.0f, 0.0f, 0.5f, showQuitConfirm));
				} else if (state == MenuState.QUIT_CONFIRM) {
						state = MenuState.CANCEL_QUIT;
						StartCoroutine (Fade (1.0f, 0.0f, 0.5f, enterPreInitState));
				}
		}

		void OnGUI ()
		{
				// Important to keep from skipping render events
				if (Event.current.type != EventType.Repaint)
						return;

				// We can turn on the render object so we can render the on-screen menu
				if (GUIRenderObject != null) {
						GUIRenderObject.SetActive (true);
				}

				//***
				// Set the GUI matrix to deal with portrait mode
				Vector3 scale = Vector3.one;
				Matrix4x4 svMat = GUI.matrix; // save current matrix
				// substitute matrix - only scale is altered from standard
				GUI.matrix = Matrix4x4.TRS (Vector3.zero, Quaternion.identity, scale);

				// Cache current active render texture
				RenderTexture previousActive = RenderTexture.active;

				// if set, we will render to this texture
				if (GUIRenderTexture != null && GUIRenderObject.activeSelf) {
						RenderTexture.active = GUIRenderTexture;
						GL.Clear (false, true, new Color (0.0f, 0.0f, 0.0f, 0.0f));
				}

				// Update OVRGUI functions (will be deprecated eventually when 2D renderingc
				// is removed from GUI)
				GuiHelper.SetFontReplace (FontReplace);

				GuiHelper.StereoBox (VRVarsSX, VRVarsSY, VRVarsWidthX, VRVarsWidthY,
                             ref currentText, textColor);

				// Restore active render texture
				if (GUIRenderObject.activeSelf) {
						RenderTexture.active = previousActive;
				}

				// ***
				// Restore previous GUI matrix
				GUI.matrix = svMat;
		}

		IEnumerator goToLevel ()
		{
				StartCoroutine (FadeAudio (0.5f, 0.0f, 2.0f, 0.0f));
				yield return new WaitForSeconds (1.2f);
				StartCoroutine (Fade (1.0f, 0.0f, 1.0f, loadTargetLevel));
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

		IEnumerator Fade (float startLevel, float endLevel, float duration, Callback callBack)
		{
				Color col = new Color (textColor.r, textColor.g, textColor.b);
				float speed = 1.0f / duration;

				for (float t = 0.0f; t < 1.0; t += Time.deltaTime * speed) {
						textColor = new Color (col.r, col.g, col.b, Mathf.Lerp (startLevel, endLevel, t));
						yield return 1;
				}
				textColor = new Color (col.r, col.g, col.b, Mathf.Lerp (startLevel, endLevel, 1.0f));

				callBack ();
		}
}
