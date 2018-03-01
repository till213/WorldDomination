using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public float speed;
	[Range(10, 500)]
	public int sensitiviy = 400;

	private Rigidbody rigidBody;
	private bool thrusterEnabled;

	Vector3 mouseOrigin;

	void Awake() {
		Cursor.lockState = CursorLockMode.Locked;
		Debug.Log("PlayerController: Cursor locked");


		mouseOrigin = Input.mousePosition;

		Debug.Log ("Cursor position: " + mouseOrigin + " Window width: " + Screen.width + " height: " + Screen.height);
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = false;
	}

	void Start ()
	{
		rigidBody = GetComponent<Rigidbody>();
	}

	void Update()
	{
		if (Input.GetMouseButtonDown(1)) {
			mouseOrigin = Input.mousePosition;
			Debug.Log ("Mouse origin reset to: " + mouseOrigin);
		}

		thrusterEnabled = Input.GetMouseButton (0);

	}
		

	void FixedUpdate ()
	{

		//Debug.Log ("Fixed Update: relative Cursor position: " + (mouseOrigin - Input.mousePosition)) ;

		float moveUp;
		if (thrusterEnabled) {
			moveUp = 1.0f;
			Debug.Log ("Firing thruster...");
		} else {
			moveUp = 0.0f;
		}
			
		Vector3 thrust = new Vector3 (0f, moveUp, 0f);
		rigidBody.AddRelativeForce (thrust * speed);

	

		float rotateX = Mathf.Min(Vector3.Distance(Input.mousePosition, mouseOrigin) / sensitiviy * 180, 180);


		float rotateY = Vector3.SignedAngle (Vector3.up,  Vector3.Normalize(Input.mousePosition - mouseOrigin), Vector3.back);


		//Debug.Log ("rotateX: " + rotateX + " rotate Y: " + rotateY + " mouse pos: " + Input.mousePosition + " origin: " + mouseOrigin + " mouse origin position vec: " + Vector3.Normalize(Input.mousePosition - mouseOrigin));

		rigidBody.rotation = Quaternion.Euler (rotateX, rotateY, 0f);

		Vector3 position = rigidBody.position;
		if (position.x < -TerrainMeshGenerator.NofTiles * TerrainMeshGenerator.TileSize / 2.0f) {
			position.x += TerrainMeshGenerator.NofTiles * TerrainMeshGenerator.TileSize;
		} else if (position.x > TerrainMeshGenerator.NofTiles * TerrainMeshGenerator.TileSize / 2.0f) {
			position.x -= TerrainMeshGenerator.NofTiles * TerrainMeshGenerator.TileSize;
		}
		if (position.z < -TerrainMeshGenerator.NofTiles * TerrainMeshGenerator.TileSize / 2.0f) {
			position.z += TerrainMeshGenerator.NofTiles * TerrainMeshGenerator.TileSize;
		} else if (position.z > TerrainMeshGenerator.NofTiles * TerrainMeshGenerator.TileSize / 2.0f) {
			position.z -= TerrainMeshGenerator.NofTiles * TerrainMeshGenerator.TileSize;
		}
		rigidBody.position = position;


	}
}