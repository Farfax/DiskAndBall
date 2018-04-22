using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class manualDiskControl : MonoBehaviour
{

	diskController diskCtrl;
	public bool enabledControl = false;
	public float multiplier = 1;

	Vector3 lastMousePosition;

	void Start()
	{
		diskCtrl = GetComponent<diskController>();
		lastMousePosition = Input.mousePosition;
	}

	void Update()
	{
		if (enabledControl)
		{
			diskCtrl.SetAngularVelocity(new Vector3(
				Input.mousePosition.y - lastMousePosition.y,
				0,
				-(Input.mousePosition.x - lastMousePosition.x)
				) * multiplier);
		}else
		{
			diskCtrl.SetAngularVelocity(Vector3.zero);
		}
		if (Input.GetKeyDown(KeyCode.M)) enabledControl = !enabledControl;
		lastMousePosition = Input.mousePosition;

	}

	public void SetManualControl(bool enable)
	{
		enabledControl = enable;
	}
}
