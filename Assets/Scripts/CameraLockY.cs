using UnityEngine;
using Cinemachine;

public class CameraLockY : MonoBehaviour
{
	public CinemachineVirtualCamera virtualCamera;
	public float fixedYPosition = 5f; 

	void LateUpdate()
	{
		if (virtualCamera != null)
		{
			Vector3 camPos = virtualCamera.transform.position;
			virtualCamera.transform.position = new Vector3(camPos.x, fixedYPosition, camPos.z);
		}
	}
}
