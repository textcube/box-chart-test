using UnityEngine;
using System.Collections;

public class GraphManager : MonoBehaviour {
	
	GameObject[] cubes;
	GameObject[] labels;
	float[] heights;
	Color[] colors;
	string[] items;
	Camera mGameCamera;
	Camera mUICamera;

	// Use this for initialization
	void Start () {
		colors = new Color[]{
			new Color(1f, 0.5f, 0.5f),
			new Color(0.5f, 1f, 0.5f),
			new Color(0.5f, 0.5f, 1f),
			Color.white,
			Color.yellow,
			Color.cyan,
			Color.magenta,
			Color.red
		};
		items = new string[]{"USB2", "FIREWIRE800", "GIGABIT ETHERNET", "SATA2", "USB3", "SATA3", "FIBRE", "THUNDERBOLT"};
		cubes = GameObject.FindGameObjectsWithTag("Cube");
		labels = GameObject.FindGameObjectsWithTag("Label");
		heights = new float[]{0.48f, 0.8f, 1f, 3f, 5f, 6f, 8f, 20f};
		mGameCamera = NGUITools.FindCameraForLayer(gameObject.layer);
		mUICamera = NGUITools.FindCameraForLayer(labels[0].layer);
		for (int i=0;i<cubes.Length;i++) {
			Transform tr = cubes[i].transform;
			tr.localScale = new Vector3(4.8f, heights[i], 2f);
			tr.position = new Vector3((i-heights.Length*0.5f)*5f, heights[i]*0.5f, 0);
			tr.renderer.material.color = colors[i];
			labels[i].GetComponent<UILabel>().text = items[i];
			labels[i].transform.position = GetModelTopPosition(cubes[i]);
		}
	}
	
	Vector3 GetModelTopPosition(GameObject model){
		Bounds bBox = CalculateBoundingBox(model);
		Vector3 topPosition = bBox.center + Vector3.up * bBox.extents.y;
		Vector3 pos = mGameCamera.WorldToViewportPoint(topPosition);
		pos = mUICamera.ViewportToWorldPoint(pos);
		pos = new Vector3(pos.x, pos.y, 0f);
		return pos;
	}

	public static Bounds CalculateBoundingBox(GameObject aObj) {
		if (aObj == null) {
			Debug.LogError("CalculateBoundingBox: object is null");
			return new Bounds(Vector3.zero, Vector3.one);
		}
		Transform myTransform = aObj.transform;
		Mesh mesh = null;
		MeshFilter mF = aObj.GetComponent<MeshFilter>();
		if (mF != null) mesh = mF.mesh;
		else {
			SkinnedMeshRenderer sMR = aObj.GetComponent<SkinnedMeshRenderer>();
			if (sMR != null) mesh = sMR.sharedMesh;
		}
		if (mesh == null) {
			Debug.LogError("CalculateBoundingBox: no mesh found on the given object");
			return new Bounds(aObj.transform.position, Vector3.one);
		}
		Vector3[] vertices = mesh.vertices;
		if (vertices.Length <=0) {
			Debug.LogError("CalculateBoundingBox: mesh doesn't have vertices");
			return new Bounds(aObj.transform.position, Vector3.one);
		}
		Vector3 min, max;
		min = max = myTransform.TransformPoint(vertices[0]);
		for (int i = 1; i < vertices.Length; i++) {
			Vector3 V = myTransform.TransformPoint(vertices[i]);
			for (int n = 0; n < 3; n++) {
				if (V[n] > max[n]) max[n] = V[n];
				if (V[n] < min[n]) min[n] = V[n];
			}
		}
		Bounds B = new Bounds();
		B.SetMinMax(min, max);
		return B;
	}

	// Update is called once per frame
	void Update () {
	}
}
