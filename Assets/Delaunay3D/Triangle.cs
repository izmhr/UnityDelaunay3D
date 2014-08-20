using UnityEngine;

class Triangle {
	public Vector3 v1, v2, v3;
	public Triangle(Vector3 v1, Vector3 v2, Vector3 v3) {
		this.v1 = v1;
		this.v2 = v2;
		this.v3 = v3;
	}

	// calc normal
	public Vector3 getNormal() {
		Vector3 edge1 = new Vector3(v2.x-v1.x, v2.y-v1.y, v2.z-v1.z);
		Vector3 edge2 = new Vector3(v3.x-v1.x, v3.y-v1.y, v3.z-v1.z);
		
		// クロス積
		Vector3 normal = Vector3.Cross(edge2, edge1);
		normal.Normalize();
		return normal;
	}

	// 面を裏返す
	public void turnBack() {
		Vector3 tmp = this.v3;
		this.v3 = this.v1;
		this.v1 = tmp;
	}

	// 線分のリストを得る
	public Line[] getLines() {
		Line[] l = {
			new Line(v1, v2),
			new Line(v2, v3),
			new Line(v3, v1)
		};
		return l;
	}

	// 同じかどうか。すげー簡易的なチェック
	public bool equals(Triangle t) {
		Line[] lines1 = this.getLines();
		Line[] lines2 = t.getLines();
		
		int cnt = 0;
		for(int i = 0; i < lines1.Length; i++) {
			for(int j = 0; j < lines2.Length; j++) {
				if (lines1[i].equals(lines2[j]))
					cnt++;
			}
		}
		if (cnt == 3) return true;
		else return false;
	}
}
