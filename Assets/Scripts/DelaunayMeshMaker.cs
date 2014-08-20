using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DelaunayMeshMaker : MonoBehaviour {

	List<Vector3> vec;
	Delaunay d;

	public int vtxnum;
	public float R;

	MeshFilter mf;

	// Use this for initialization
	void Awake () {
		d = new Delaunay();

		vec = new List<Vector3>();
		for( int i = 0; i < vtxnum; i++) {
			vec.Add (Random.insideUnitSphere * R);
		}

		d.SetData(vec);

		Mesh mesh = new Mesh();
		Vector3[] vertices = new Vector3[d.triangles.Count * 3];
		int[] indices = new int[d.triangles.Count * 3];
		for( int i = 0; i < d.triangles.Count; i++) {
			vertices[3 * i + 0] = d.triangles[i].v1;
			vertices[3 * i + 1] = d.triangles[i].v2;
			vertices[3 * i + 2] = d.triangles[i].v3;
			for( int j = 0; j <3; j++) {
				indices[3 * i + j] = 3 * i + j;
			}
		}
		mesh.vertices = vertices;
		// TODO mesh.uv
		mesh.triangles = indices;
		mesh.RecalculateNormals();

		mf = gameObject.GetComponent<MeshFilter>();
		mf.mesh = mesh;
	}
	
	// Update is called once per frame
	void Update () {

	}
}
