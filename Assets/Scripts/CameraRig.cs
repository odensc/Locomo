using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class CameraRig : MonoBehaviour
{
	void Start()
	{
		GetComponentInChildren<BlurOptimized>().enabled = false;
	}
}