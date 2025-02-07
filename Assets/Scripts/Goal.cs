using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class Goal : MonoBehaviour
{
	private MeshRenderer meshRenderer;

	// Custom constant scroll speed for the texture
	public float scrollSpeed = 0.5f;

	private void Awake()
	{
		meshRenderer = GetComponent<MeshRenderer>();
	}

	private void Update()
	{
		// Scroll texture at a constant speed, independent of time
		meshRenderer.material.mainTextureOffset += scrollSpeed * Time.deltaTime * Vector2.right;
	}
}
