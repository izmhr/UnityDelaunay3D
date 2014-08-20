using UnityEngine;

class Line{
	public Vector3 start, end;
	public Line(Vector3 start, Vector3 end) {
		this.start = start;
		this.end = end;
	}

	// inverse
	public void reverse() {
		Vector3 tmp = this.start;
		this.start = this.end;
		this.end = tmp;
	}

	public bool equals(Line l) {
		if( ( this.start == l.start && this.end == l.end) || (this.start == l.end && this.end == l.start ) )
			return true;
		return false;
	}
}
