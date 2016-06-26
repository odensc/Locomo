using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

[RequireComponent(typeof(SteamVR_TrackedController))]
public class Controller : MonoBehaviour
{
	public static bool touchpad = false;
	public static Controller used;
	Vector3 lastPos;
	GameObject rig;
	SteamVR_TrackedController tController;
	SteamVR_Camera eye;
	BlurOptimized blur;
	Transform pointerOver;

	void Start()
	{
		rig = GameObject.Find("[CameraRig]");
		tController = GetComponent<SteamVR_TrackedController>();
		eye = GameObject.FindObjectOfType<SteamVR_Camera>();
		blur = eye.GetComponent<BlurOptimized>();
		tController.PadClicked += StartBlur;
		tController.PadUnclicked += StopBlur;
		tController.Gripped += StartBlur;
		tController.Ungripped += StopBlur;
		tController.TriggerClicked += OnTrigger;
		tController.TriggerUnclicked += OnTrigger;
		if (GetComponent<SteamVR_LaserPointer>())
		{
			GetComponent<SteamVR_LaserPointer>().PointerIn += OnPointerIn;
			GetComponent<SteamVR_LaserPointer>().PointerOut += OnPointerOut;
		}

		lastPos = eye.transform.position;
	}

	void OnTrigger(object sender, ClickedEventArgs e)
	{
		if (pointerOver && pointerOver.GetComponent<TextMesh>() && tController.triggerPressed)
		{
			SteamVR_Controller.Device device = SteamVR_Controller.Input((int) GetComponent<SteamVR_TrackedObject>().index);
			device.TriggerHapticPulse();
			touchpad = pointerOver.GetComponent<TextMesh>().text == "Touchpad" ? true : false;
		}
	}

	void FixedUpdate()
	{
		if (used != this && used != null) return;

		if (touchpad && tController.padPressed && blur.enabled)
		{
			var padX = tController.controllerState.rAxis0.x * 3;
			var padY = tController.controllerState.rAxis0.y * 3;
			var movement = eye.transform.forward * padY * Time.fixedDeltaTime;
			var strafe = eye.transform.right * padX * Time.fixedDeltaTime;
			float fixY = rig.transform.position.y;
			rig.transform.position += (movement + strafe);
			rig.transform.position = new Vector3(rig.transform.position.x, fixY, rig.transform.position.z);
		}
		else if (!touchpad && blur.enabled)
		{
			var dot = Vector3.Dot(eye.transform.forward, (eye.transform.position - lastPos));
			if (dot > 0.001)
			{
				float fixY = rig.transform.position.y;
				rig.transform.position += (eye.transform.position - lastPos) * 4;
				rig.transform.position = new Vector3(rig.transform.position.x, fixY, rig.transform.position.z);
			}
		}

		lastPos = eye.transform.position;
	}

	void OnPointerIn(object sender, PointerEventArgs e)
	{
		pointerOver = e.target;
		if (pointerOver.GetComponent<TextMesh>()) pointerOver.GetComponent<TextMesh>().color = Color.blue;
	}

	void OnPointerOut(object sender, PointerEventArgs e)
	{
		if (pointerOver.GetComponent<TextMesh>()) pointerOver.GetComponent<TextMesh>().color = Color.white;
		pointerOver = null;
	}

	void StartBlur(object sender, ClickedEventArgs e)
	{
		if (used != this && used != null) return;
		used = this;
		blur.enabled = true;
		SteamVR_Fade.Start(new Color(0f, 0f, 0f, 0.92f), 0.5f);
	}

	void StopBlur(object sender, ClickedEventArgs e)
	{
		if (used != this && used != null) return;
		used = null;
		blur.enabled = false;
		SteamVR_Fade.Start(new Color(0f, 0f, 0f, 0f), 0.5f);
	}
}
