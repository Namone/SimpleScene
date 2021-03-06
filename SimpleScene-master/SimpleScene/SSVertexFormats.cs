// Copyright(C) David W. Jeske, 2013
// Released to the public domain. Use, modify and relicense at will.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace SimpleScene
{

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SSVertex_PosNormDiffTex1 : IEqualityComparer<SSVertex_PosNormDiffTex1>, ISSVertexLayout {
        public float Tu, Tv;
        public Int32 DiffuseColor;

        public Vector3 Normal;
        public Vector3 Position;

        public unsafe void  bindGLAttributes() {
            // this is the "transitional" GLSL 120 way of assigning buffer contents
            // http://www.opentk.com/node/80?page=1

            GL.EnableClientState (ArrayCap.VertexArray);
            GL.VertexPointer (3, VertexPointerType.Float, sizeof(SSVertex_PosNormDiffTex1), (IntPtr) Marshal.OffsetOf (typeof(SSVertex_PosNormDiffTex1), "Position"));

            GL.EnableClientState (ArrayCap.NormalArray);
            GL.NormalPointer (NormalPointerType.Float, sizeof(SSVertex_PosNormDiffTex1), (IntPtr) Marshal.OffsetOf (typeof(SSVertex_PosNormDiffTex1), "Normal"));

            GL.EnableClientState (ArrayCap.TextureCoordArray);
            GL.TexCoordPointer(2, TexCoordPointerType.Float, sizeof(SSVertex_PosNormDiffTex1), (IntPtr) Marshal.OffsetOf (typeof(SSVertex_PosNormDiffTex1), "Tu"));
        }

        public bool Equals(SSVertex_PosNormDiffTex1 a, SSVertex_PosNormDiffTex1 b) {
            return 
                a.Position==b.Position 
                && a.Normal==b.Normal 
                && a.DiffuseColor==b.DiffuseColor
                && a.Tu==b.Tu
                && a.Tv==b.Tv;
        }
        public int GetHashCode(SSVertex_PosNormDiffTex1 a) {
            return a.GetHashCode();
        }
        public unsafe int sizeOf() {
            return sizeof (SSVertex_PosNormDiffTex1);
        }
        public override bool Equals( object ob ){
            if( ob is SSVertex_PosNormDiffTex1 ) {
                SSVertex_PosNormDiffTex1 c = (SSVertex_PosNormDiffTex1) ob;
                return this.Equals(this,c);
            }
            else {
                return false;
            }
        }
        public override int GetHashCode ()
        {
            return base.GetHashCode ();
        }
    }

    ///////////////////////////////////////////////////////

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SSVertex_PosNormDiff {
        public Vector3 Position;
        public Vector3 Normal;

        public int DiffuseColor;
    }

    ///////////////////////////////////////////////////////

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SSVertex_Pos : IEqualityComparer<SSVertex_Pos>, ISSVertexLayout
    {
        public Vector3 Position;

        public SSVertex_Pos(float x, float y, float z) {
            Position = new Vector3 (x, y, z);
        }

        public unsafe void bindGLAttributes() {
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.VertexPointer(3, VertexPointerType.Float, sizeof(SSVertex_Pos), (IntPtr)Marshal.OffsetOf(typeof(SSVertex_Pos), "Position"));
        }

        unsafe public int sizeOf() {
            return sizeof(SSVertex_Pos);
        }

        public bool Equals(SSVertex_Pos a, SSVertex_Pos b) {
            return a.Position == b.Position;
        }

        public override bool Equals(object ob) {
            if (ob is SSVertex_Pos) {
                SSVertex_Pos c = (SSVertex_Pos)ob;
                return this.Equals(this, c);
            } else {
                return false;
            }
        }

        public int GetHashCode(SSVertex_Pos a) {
            return a.GetHashCode();
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }
    }

    ///////////////////////////////////////////////////////

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SSVertex_PosTex1 : IEqualityComparer<SSVertex_PosTex1>, ISSVertexLayout
    {
        public Vector2 TexCoord;
        public Vector3 Position;

        public SSVertex_PosTex1(float x, float y, float z, float u, float v) {
            TexCoord = new Vector2 (u, v);
            Position = new Vector3 (x, y, z);
        }

        public unsafe void bindGLAttributes() {
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.VertexPointer(3, VertexPointerType.Float, sizeof(SSVertex_PosTex1), (IntPtr)Marshal.OffsetOf(typeof(SSVertex_PosTex1), "Position"));

            GL.EnableClientState (ArrayCap.TextureCoordArray);
            GL.TexCoordPointer(2, TexCoordPointerType.Float, sizeof(SSVertex_PosTex1), (IntPtr) Marshal.OffsetOf (typeof(SSVertex_PosTex1), "TexCoord"));
        }

        unsafe public int sizeOf() {
            return sizeof(SSVertex_PosTex1);
        }

        public bool Equals(SSVertex_PosTex1 a, SSVertex_PosTex1 b) {
            return a.Position == b.Position
                && a.TexCoord == b.TexCoord;
        }

        public override bool Equals(object ob) {
            if (ob is SSVertex_PosTex1) {
                SSVertex_PosTex1 c = (SSVertex_PosTex1)ob;
                return this.Equals(this, c);
            } else {
                return false;
            }
        }

        public int GetHashCode(SSVertex_PosTex1 a) {
            return a.GetHashCode();
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }
    }
}

