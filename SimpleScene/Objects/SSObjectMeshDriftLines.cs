using System;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace SimpleScene
{
    public class SSObjectMeshDriftLines : SSObjectMesh
    {
        public SSObjectMeshDriftLines(SSAbstractMesh mesh) : base(mesh) { }

        public override void Render(ref SSRenderConfig renderConfig) {
            base.Render(ref renderConfig);
            GL.PushClientAttrib(ClientAttribMask.ClientVertexArrayBit);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.NormalArray);

            //GL.Disable(EnableCap.DepthTest);
            GL.DepthMask(false);
            //GL.Disable(EnableCap.DepthClamp);

            Mesh.RenderMesh(ref renderConfig);
            GL.PopClientAttrib();
        }

        public override void Update(float timeDelta) {
            base.Update(timeDelta);
            Mesh.SetPos(this.Pos);
            Mesh.Update(timeDelta);
        }
    }
}
