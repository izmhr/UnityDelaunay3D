// based on UnityでありもののMeshをワイヤフレームで描画する
// http://izmiz.hateblo.jp/entry/2014/02/27/224651

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DelaunayMeshUpdater : MonoBehaviour {
	
	MeshFilter meshFilter;
	Delaunay d;

	// Use this for initialization
	void Start () {
		meshFilter = GetComponentInChildren<MeshFilter>();
		d = new Delaunay();
	}
	
	// Update is called once per frame
	void Update () {
		ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleSystem.particleCount];
		particleSystem.GetParticles(particles);
		List<Vector3> particlePoss = new List<Vector3>();
		for( int i = 0; i < particles.Length; i++) {
			particlePoss.Add (particles[i].position);
		}
		if( particles.Length > 5 ){
			d.SetData(particlePoss);

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
			meshFilter.mesh = mesh;
		}
	}
}
