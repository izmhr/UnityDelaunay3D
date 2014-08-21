using UnityEngine;
using System.Collections;

public class MakeLineFromMesh : MonoBehaviour {

	private MeshFilter meshFilter;
	private Vector3[] vertices;
	private int[] triangles;

	// Use this for initialization
	void Start () {
		meshFilter = GetComponent<MeshFilter>();

		if (meshFilter.mesh == null) return;

		// MeshTopologyの確認
		MeshTopology topo = meshFilter.mesh.GetTopology(0);
		Debug.Log(topo); // Triangles と出力される

		triangles = meshFilter.mesh.triangles;
		meshFilter.mesh.SetIndices(MakeIndices(), MeshTopology.Lines, 0);

		// 再度MeshTopologyの確認
		topo = meshFilter.mesh.GetTopology(0);
		Debug.Log(topo); // Lines と出力される
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	int[] MakeIndices()
	{
		int[] indices = new int[2 * triangles.Length];
		int i = 0;
		for( int t = 0; t < triangles.Length; t+=3 )
		{
			indices[i++] = triangles[t];		//start
			indices[i++] = triangles[t + 1];	//end
			indices[i++] = triangles[t + 1];	//start
			indices[i++] = triangles[t + 2];	//end
			indices[i++] = triangles[t + 2];	//start
			indices[i++] = triangles[t];		//end
		}
		return indices;
	}
}
