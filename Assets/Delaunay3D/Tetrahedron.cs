using UnityEngine;
using System.Collections;

class Tetrahedron {
	// 4頂点を順序づけて格納
	public Vector3[] vertices;
	public Vector3 o;      // 外接円の中心
	public float   r;      // 外接円の半径

	public static Vector3 NOASSIGN = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
	
	public Tetrahedron(Vector3[] v) {
		this.vertices = v;
		getCenterCircumcircle();
	}
	
	public Tetrahedron(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4) {
		this.vertices = new Vector3[4];
		vertices[0] = v1;
		vertices[1] = v2;
		vertices[2] = v3;
		vertices[3] = v4;
		getCenterCircumcircle();
	}
	
	public bool equals(Tetrahedron t) {
		int count = 0;
		foreach (Vector3 p1 in this.vertices) {
			foreach (Vector3 p2 in t.vertices) {
				if (p1.x == p2.x && p1.y == p2.y && p1.z == p2.z) {
					count++;
				}
			}
		}
		if (count == 4) return true;
		return false;
	}
	
	public Line[] getLines() {
		Vector3 v1 = vertices[0];
		Vector3 v2 = vertices[1];
		Vector3 v3 = vertices[2];
		Vector3 v4 = vertices[3];
		
		Line[] lines = new Line[6];
		
		lines[0] = new Line(v1, v2);
		lines[1] = new Line(v1, v3);
		lines[2] = new Line(v1, v4);
		lines[3] = new Line(v2, v3);
		lines[4] = new Line(v2, v4);
		lines[5] = new Line(v3, v4);
		return lines;
	}
	
	// 外接円も求めちゃう
	private void getCenterCircumcircle() {
		Vector3 v1 = vertices[0];
		Vector3 v2 = vertices[1];
		Vector3 v3 = vertices[2];
		Vector3 v4 = vertices[3];
		
		double[,] A = new double[3,3]{
			{v2.x - v1.x, v2.y-v1.y, v2.z-v1.z},
			{v3.x - v1.x, v3.y-v1.y, v3.z-v1.z},
			{v4.x - v1.x, v4.y-v1.y, v4.z-v1.z}
		};
		double[] b = new double[]{
			0.5 * (v2.x*v2.x - v1.x*v1.x + v2.y*v2.y - v1.y*v1.y + v2.z*v2.z - v1.z*v1.z),
			0.5 * (v3.x*v3.x - v1.x*v1.x + v3.y*v3.y - v1.y*v1.y + v3.z*v3.z - v1.z*v1.z),
			0.5 * (v4.x*v4.x - v1.x*v1.x + v4.y*v4.y - v1.y*v1.y + v4.z*v4.z - v1.z*v1.z)
		};
		double[] x = new double[3];
		if (gauss(A, b, x) == 0) {
			o = NOASSIGN;	// TODO: OK??
			r = -1;
		} else {
			o = new Vector3((float)x[0], (float)x[1], (float)x[2]);
			r = Vector3.Distance(o, v1);
		}
	}
	
	/** LU分解による方程式の解法 **/
	private double lu(double[,] a, int[] ip) {
		int n = a.Length / 3;
		double[] weight = new double[n];
		
		for(int k = 0; k < n; k++) {
			ip[k] = k;
			double u = 0;
			for(int j = 0; j < n; j++) {
				double t = System.Math.Abs(a[k,j]);
				if (t > u) u = t;
			}
			if (u == 0) return 0;
			weight[k] = 1/u;
		}
		double det = 1;
		for(int k = 0; k < n; k++) {
			double u = -1;
			int m = 0;
			for(int i = k; i < n; i++) {
				int ii = ip[i];
				double t = System.Math.Abs(a[ii,k]) * weight[ii];
				if(t>u) { u = t; m = i; }
			}
			int ik = ip[m];
			if (m != k) {
				ip[m] = ip[k]; ip[k] = ik;
				det = -det;
			}
			u = a[ik,k]; det *= u;
			if (u == 0) return 0;
			for (int i = k+1; i < n; i++) {
				int ii = ip[i]; double t = (a[ii,k] /= u);
				for(int j = k+1; j < n; j++) a[ii,j] -= t * a[ik,j];
			}
		}
		return det;
	}
	private void solve(double[,] a, double[] b, int[] ip, double[] x) {
		int n = a.Length / 3;
		for(int i = 0; i < n; i++) {
			int ii = ip[i]; double t = b[ii];
			for (int j = 0; j < i; j++) t -= a[ii,j] * x[j];
			x[i] = t;
		}
		for (int i = n-1; i >= 0; i--) {
			double t = x[i]; int ii = ip[i];
			for(int j = i+1; j < n; j++) t -= a[ii,j] * x[j];
			x[i] = t / a[ii,i];
		}
	}
	private double gauss(double[,] a, double[] b, double[] x) {
		int n = a.Length / 3;
		int[] ip = new int[n];
		double det = lu(a, ip);
		
		if(det != 0) { solve(a, b, ip, x);}
		return det;
	}
}
