using UnityEngine;
using System.Collections;
using OVR;

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
	public OVRCameraController cameraController;

	// Use this for initialization
	void Start ()
	{
		OVRDevice.ResetOrientation ();
		updateCameraTracker ();

		spineTransform = player.transform.Find ("Hip/Spine1");
		spineInitialRotation = spineTransform.rotation;
		Transform neckTransform = player.transform.Find ("Hip/Spine1/Spine2/Spine3/Spine4/Neck1");
		initialRotateFrom = (neckTransform.position - spineTransform.position).normalized;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.R) || Input.GetKeyDown (KeyCode.Return)) {
			OVRDevice.ResetOrientation ();
			updateCameraTracker ();
		}

		Vector3 facePos = (leftEye.transform.position + rightEye.transform.position) / 2;
		cube.transform.position = facePos;
		Vector3 rotateTo = facePos - spineTransform.position;
		
		Quaternion foo = Quaternion.FromToRotation (initialRotateFrom, rotateTo.normalized) * spineInitialRotation;
		spineTransform.rotation = foo;
		
		// Update hit sphere's position
		hitSphere.transform.position = facePos - rotateTo.normalized * 0.02f;
	}
	
	
	// From https://developer.oculusvr.com/forums/viewtopic.php?f=37&t=11779&p=160605#p160605
	void updateCameraTracker()
	{
		Vector3 IRCameraPos = Vector3.zero;
		Quaternion IRCameraRot = Quaternion.identity;
		
		float cameraHFov = 0;
		float cameraVFov = 0;
		float cameraNearZ = 0;
		float cameraFarZ = 0;
		
		GetIRCamera (ref IRCameraPos, ref IRCameraRot, ref cameraHFov, ref cameraVFov, ref cameraNearZ, ref cameraFarZ);
		
		IRCameraPos.z *= -1;
		
		//transform.localPosition = IRCameraPos;
		//transform.rotation = IRCameraRot;
		
		Debug.Log ("HFov " + cameraHFov.ToString ());
		Debug.Log ("VFov " + cameraVFov.ToString ());
		
		float horizontalScale = Mathf.Tan (cameraHFov / 2f);
		float verticalScale = Mathf.Tan (cameraVFov / 2f);
		
		Debug.Log ("HDistance " + horizontalScale.ToString ());
		Debug.Log ("VDistance " + verticalScale.ToString ());
		
		//transform.localScale = new Vector3 (horizontalScale * cameraFarZ, verticalScale * cameraFarZ, cameraFarZ);
	}
	
	bool GetIRCamera(	ref Vector3 position, 
	                 ref Quaternion rotation, 
	                 ref float cameraHFov, 
	                 ref float cameraVFov, 
	                 ref float cameraNearZ, 
	                 ref float cameraFarZ)
	{
		if (OVRDevice.HMD == null || !OVRDevice.SupportedPlatform)
			return false;
		
		ovrTrackingState ss = OVRDevice.HMD.GetTrackingState();
		
		rotation = new Quaternion(	ss.CameraPose.Orientation.x,
		                          ss.CameraPose.Orientation.y,
		                          ss.CameraPose.Orientation.z,
		                          ss.CameraPose.Orientation.w);
		
		position = new Vector3(	ss.CameraPose.Position.x,
		                       ss.CameraPose.Position.y,
		                       ss.CameraPose.Position.z);
		
		ovrHmdDesc desc = OVRDevice.HMD.GetDesc();
		
		cameraHFov = desc.CameraFrustumHFovInRadians;
		cameraVFov = desc.CameraFrustumVFovInRadians;
		cameraNearZ = desc.CameraFrustumNearZInMeters;
		cameraFarZ = desc.CameraFrustumFarZInMeters;
		
		OVRDevice.OrientSensor (ref rotation);
		
		return true;
	}
}
