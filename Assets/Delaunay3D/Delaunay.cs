using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class Delaunay {
	
	List<Vector3> vertices;     // 与えられた点列
	List<Tetrahedron> tetras;   // 四面体リスト
	
	public List<Line> edges;
	
	public List<Line> surfaceEdges;
	public List<Triangle> triangles;

	public int[] triangleIndexes;
	
	
	public Delaunay() {
		vertices = new List<Vector3>();
		tetras = new List<Tetrahedron>();
		edges = new List<Line>();
		surfaceEdges = new List<Line>();
		triangles = new List<Triangle>();
	}
	
	public void SetData(List<Vector3> seq) {
		
		tetras.Clear();
		edges.Clear();
		
		// 1    : 点群を包含する四面体を求める
		//   1-1: 点群を包含する球を求める
		Vector3 vMax = new Vector3(-999, -999, -999);
		Vector3 vMin = new Vector3( 999,  999,  999);
		foreach(Vector3 v in seq) {
			if (vMax.x < v.x) vMax.x = v.x;
			if (vMax.y < v.y) vMax.y = v.y;
			if (vMax.z < v.z) vMax.z = v.z;
			if (vMin.x > v.x) vMin.x = v.x;
			if (vMin.y > v.y) vMin.y = v.y;
			if (vMin.z > v.z) vMin.z = v.z;

			vertices.Add (v);
		}
		
		Vector3 center = new Vector3();     // 外接球の中心座標
		center.x = 0.5f * (vMax.x - vMin.x);
		center.y = 0.5f * (vMax.y - vMin.y);
		center.z = 0.5f * (vMax.z - vMin.z);
		float r = -1;                       // 半径
		foreach(Vector3 v in seq) {
			if (r < Vector3.Distance(center, v)) r = Vector3.Distance(center, v);
		}
		r += 0.1f;                          // ちょっとおまけ

		float root2 = Mathf.Sqrt(2.0f);
		float root6 = Mathf.Sqrt(6.0f);
		//   1-2: 球に外接する四面体を求める
		Vector3 v1 = new Vector3();
		v1.x = center.x;
		v1.y = center.y + 3.0f*r;
		v1.z = center.z;
		
		Vector3 v2 = new Vector3();
		v2.x = center.x - 2.0f*root2*r;
		v2.y = center.y - r;
		v2.z = center.z;
		
		Vector3 v3 = new Vector3();
		v3.x = center.x + root2*r;
		v3.y = center.y - r;
		v3.z = center.z + root6*r;
		
		Vector3 v4 = new Vector3();
		v4.x = center.x + root2*r;
		v4.y = center.y - r;
		v4.z = center.z - root6*r;
		
		Vector3[] outer = new Vector3[]{v1, v2, v3, v4};
		tetras.Add(new Tetrahedron(v1, v2, v3, v4));
		
		// 幾何形状を動的に変化させるための一時リスト
		List<Tetrahedron> tmpTList = new List<Tetrahedron>();
		List<Tetrahedron> newTList = new List<Tetrahedron>();
		List<Tetrahedron> removeTList = new List<Tetrahedron>();
		foreach(Vector3 v in seq) {
			tmpTList.Clear();
			newTList.Clear();
			removeTList.Clear();
			foreach (Tetrahedron t in tetras) {
				if((t.o != Tetrahedron.NOASSIGN) && (t.r > Vector3.Distance(v, t.o))) {
					tmpTList.Add(t);
				}
			}
			
			foreach (Tetrahedron t1 in tmpTList) {
				// まずそれらを削除
				tetras.Remove(t1);
				
				v1 = t1.vertices[0];
				v2 = t1.vertices[1];
				v3 = t1.vertices[2];
				v4 = t1.vertices[3];
				newTList.Add(new Tetrahedron(v1, v2, v3, v));
				newTList.Add(new Tetrahedron(v1, v2, v4, v));
				newTList.Add(new Tetrahedron(v1, v3, v4, v));
				newTList.Add(new Tetrahedron(v2, v3, v4, v));
			}

			bool[] _isRedundancy = new bool[newTList.Count];
			for (int i = 0; i < _isRedundancy.Length; i++) _isRedundancy[i] = false;
			for (int i = 0; i < newTList.Count-1; i++) {
				for (int j = i+1; j < newTList.Count; j++) {
//					if(newTList.get(i).equals(newTList.get(j))) {
					if(newTList[i].equals (newTList[j])){
						_isRedundancy[i] = _isRedundancy[j] = true;
					}
				}
			}
			for (int i = 0; i < _isRedundancy.Length; i++) {
				if (!_isRedundancy[i]) {
//					tetras.Add(newTList.get(i));
					tetras.Add (newTList[i]);
				}
				
			}
			
		}
		
		
		bool isOuter = false;
//		foreach (Tetrahedron t4 in tetras) {
		for( int i = tetras.Count - 1; i >=0; i--) {
			Tetrahedron t4 = tetras[i];
			isOuter = false;
			foreach (Vector3 p1 in t4.vertices) {
				foreach (Vector3 p2 in outer) {
					if (p1.x == p2.x && p1.y == p2.y && p1.z == p2.z) {
						isOuter = true;
					}
				}
			}
			if (isOuter) {
				tetras.Remove(t4);
			}
		}
		
		triangles.Clear();
		bool isSame = false;
		foreach (Tetrahedron t in tetras) {
			foreach (Line l1 in t.getLines()) {
				isSame = false;
				foreach (Line l2 in edges) {
					if (l2.equals(l1)) {
						isSame = true;
						break;
					}
				}
				if (!isSame) {
					edges.Add(l1);
				}
			}
		}
		
		// ===
		// 面を求める
		
		List<Triangle> triList = new List<Triangle>();
		foreach (Tetrahedron t in tetras) {
			v1 = t.vertices[0];
			v2 = t.vertices[1];
			v3 = t.vertices[2];
			v4 = t.vertices[3];
			
			Triangle tri1 = new Triangle(v1, v2, v3);
			Triangle tri2 = new Triangle(v1, v3, v4);
			Triangle tri3 = new Triangle(v1, v4, v2);
			Triangle tri4 = new Triangle(v4, v3, v2);
			
			Vector3 n;
			// 面の向きを決める
			n = tri1.getNormal();
			if(Vector3.Dot(n, v1) > Vector3.Dot (n, v4)) tri1.turnBack();
//			if(n.dot(v1) > n.dot(v4)) tri1.turnBack();
			
			n = tri2.getNormal();
			if(Vector3.Dot(n, v1) > Vector3.Dot (n, v2)) tri2.turnBack();
//			if(n.dot(v1) > n.dot(v2)) tri2.turnBack();
			
			n = tri3.getNormal();
			if(Vector3.Dot(n, v1) > Vector3.Dot (n, v3)) tri3.turnBack();
//			if(n.dot(v1) > n.dot(v3)) tri3.turnBack();
			
			n = tri4.getNormal();
			if(Vector3.Dot(n, v2) > Vector3.Dot (n, v1)) tri4.turnBack();
//			if(n.dot(v2) > n.dot(v1)) tri4.turnBack();
			
			triList.Add(tri1);
			triList.Add(tri2);
			triList.Add(tri3);
			triList.Add(tri4);
		}
		bool[] isSameTriangle = new bool[triList.Count];
		for(int i = 0; i < triList.Count-1; i++) {
			for(int j = i+1; j < triList.Count; j++) {
//				if (triList.get(i).equals(triList.get(j))) isSameTriangle[i] = isSameTriangle[j] = true;
				if (triList[i].equals(triList[j])) isSameTriangle[i] = isSameTriangle[j] = true;
			}
		}
		for(int i = 0; i < isSameTriangle.Length; i++) {
			if (!isSameTriangle[i]) triangles.Add(triList[i]);
		}
		
		surfaceEdges.Clear();
		List<Line> surfaceEdgeList = new List<Line>();
		foreach(Triangle tri in triangles) {
//			surfaceEdgeList.addAll(Arrays.asList(tri.getLines()));
			surfaceEdges.AddRange(tri.getLines());
		}
		bool[] isRedundancy = new bool[surfaceEdgeList.Count];
		for(int i = 0; i < surfaceEdgeList.Count-1; i++) {
			for (int j = i+1; j < surfaceEdgeList.Count; j++) {
				if (surfaceEdgeList[i].equals(surfaceEdgeList[j])) isRedundancy[j] = true;
			}
		}
		
		for (int i = 0; i < isRedundancy.Length; i++) {
			if (!isRedundancy[i]) surfaceEdges.Add(surfaceEdgeList[i]);
		}
		
	}
}