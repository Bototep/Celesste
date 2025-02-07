using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class Goal : MonoBehaviour
{
	private MeshRenderer meshRenderer;
	public float scrollSpeed = 0.5f;
	public bool scrollUpwards = true;  

	private void Awake(){
		meshRenderer = GetComponent<MeshRenderer>();
	}

	private void Update(){
		if (scrollUpwards)
		{
			meshRenderer.material.mainTextureOffset -= scrollSpeed * Time.deltaTime * Vector2.up;
		}
		else
		{
			meshRenderer.material.mainTextureOffset += scrollSpeed * Time.deltaTime * Vector2.right;
		}
	}
}
