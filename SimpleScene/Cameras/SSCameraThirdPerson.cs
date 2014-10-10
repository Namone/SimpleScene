// Copyright(C) David W. Jeske, 2013
// Released to the public domain. Use, modify and relicense at will.

using System;

using OpenTK;

namespace SimpleScene
{
	public class SSCameraThirdPerson : SSCamera
	{
		protected readonly SSObject m_followTarget = null;
        protected readonly SSObjectMeshDriftLines m_driftLines = null;
		public float followDistance = 10.0f;
		public float minFollowDistance = 0.5f;
		public float maxFollowDistance = 500.0f;
		public Vector3 basePos;

        public SSCameraThirdPerson (SSObject followTarget, SSObjectMeshDriftLines driftLines = null)
            : base() {
			m_followTarget = followTarget;
            m_driftLines = driftLines;
		}
		public SSCameraThirdPerson() : base() {
		}

		public SSCameraThirdPerson (Vector3 origin) : base() {
			this.basePos = origin;
		}
		public override void Update(float timeDelta) {
			Vector3 targetPos = basePos;
			// FPS follow the target
			if (this.m_followTarget != null) {
				targetPos = this.m_followTarget.Pos;
			} 
			
			followDistance = OpenTKHelper.Clamp(followDistance,minFollowDistance,maxFollowDistance);
			
			// one way to have a third person camera, is to position ourselves
			// relative to our target object, and our current camera-direction
			
			// TODO: why are positive follow distances producing the correct orientation? 
			//    it feels like something is inverted

			this.Pos = targetPos + (this.Dir * followDistance);
			// Console.WriteLine("Camera Up {0} / Dir {1} / Right {2}",this.Up,this.Dir,this.Right);
			// Console.WriteLine("Camera Pos = {0}",this.Pos);
			base.Update(timeDelta);
            
            if (m_driftLines != null) {
                m_driftLines.Pos = this.Pos;
                m_driftLines.Update(timeDelta);
            }
		}
	}
}

