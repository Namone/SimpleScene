﻿// Copyright(C) David W. Jeske, 2013
// Released to the public domain. 

using System;

using OpenTK;

namespace SimpleScene
{
	public struct SSRay
	{
	    public Vector3 pos;
	    public Vector3 dir;

		public SSRay (Vector3 pos, Vector3 dir) {
			this.pos = pos;
			this.dir = dir;
		}

		public static SSRay FromTwoPoints(Vector3 p1, Vector3 p2) {
		    
		    Vector3 pos = p1;
		    Vector3 dir = (p2 - p1).Normalized();

		    return new SSRay(pos,dir);
		}

		public SSRay Transformed(Matrix4 mat) {
		    // a point is directly transformed
			var transformed_pos = Vector3.Transform(pos,mat);
		    // a direction-vector is transformed with only rotation and scale
			var vector_mat = mat;
			vector_mat.ClearTranslation(); // so make a new matrix with no translation
			var transformed_dir = Vector3.TransformVector(dir,vector_mat);
			
			// put them together into a new ray
			return new SSRay(transformed_pos,transformed_dir );
		}

		public override string ToString() {
		    return String.Format("({0}) -> v({1})",pos,dir);
		}
	}
}

