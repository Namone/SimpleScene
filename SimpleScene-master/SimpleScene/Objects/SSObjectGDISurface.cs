﻿using System;
using System.Drawing;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace SimpleScene {
    public abstract class SSObjectGDISurface : SSObject {
        private SSTexture textureSurface;

        public bool Dirty = true;
        Size gdiSize;
        Size textureSize;
        public bool hasAlpha = false;

        public SSObjectGDISurface() { 
			textureSurface = new SSTexture();
		}

        private int nextPowerOf2(int biggerThan) {
            int powof2 = 1;
            while (powof2 < biggerThan)
                powof2 <<= 1;
            return powof2;
        }

        internal Size makeValidTextureSize(int w, int h) {
			#if false
			// if it requires power of two texture sizes
                return new Size(
                    nextPowerOf2(Math.Max(w, 64)),
                    nextPowerOf2(Math.Max(h, 64)));
			#else
                return new Size(w, h);
			#endif
        }

        public void UpdateTexture() {
            if (!Dirty) return;
            Dirty = false;

            // using this method to software GDI+ render to a bitmap, and then copy to texture
            // http://florianblock.blogspot.com/2008/06/copying-dynamically-created-bitmap-to.html          

            Bitmap bitmap = this.RepaintGDI(out gdiSize);
            textureSize = bitmap.Size;

            // download bits into a texture...
            textureSurface.loadFromBitmap(bitmap, hasAlpha: hasAlpha, mipmap:false);
        }

        public abstract Bitmap RepaintGDI(out Size gdiSize);

        public override void Render(ref SSRenderConfig renderConfig) {            
            UpdateTexture();            

            base.Render(ref renderConfig);

			SSShaderProgram.DeactivateAll (); // disable GLSL

            // mode setup
            // GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

            // Step 2: setup our material mode and paramaters...

            GL.Disable(EnableCap.Lighting);
            if (hasAlpha) {
                GL.Enable(EnableCap.AlphaTest);
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            } else {
                GL.Disable(EnableCap.AlphaTest);
                GL.Disable(EnableCap.Blend);
            }
			
			// setup our texture source
			if (textureSurface != null) {
				GL.ActiveTexture(TextureUnit.Texture0);
				GL.Enable(EnableCap.Texture2D);
                GL.BindTexture(TextureTarget.Texture2D, textureSurface.TextureID);
			}

			GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (float)TextureMagFilter.Nearest);
			GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (float)TextureMinFilter.Nearest);

            // draw text rectangle...
			GL.Begin(PrimitiveType.Triangles);
            GL.Color3(Color.White);  // clear the vertex color to white..

            float w = gdiSize.Width;
            float h = gdiSize.Height;

            if (gdiSize != textureSize) {
                // adjust texture coordinates
                throw new Exception("not implemented");
            }

            // upper-left
            GL.TexCoord2(0.0, 0.0); GL.Vertex3(0.0, 0.0, 0.0);
            GL.TexCoord2(0.0, 1.0); GL.Vertex3(0.0, h, 0.0);
            GL.TexCoord2(1.0, 0.0); GL.Vertex3(w, 0.0, 0.0);

            // lower-right
            GL.TexCoord2(0.0, 1.0); GL.Vertex3(0.0, h, 0.0);
            GL.TexCoord2(1.0, 1.0); GL.Vertex3(w, h, 0.0);
            GL.TexCoord2(1.0, 0.0); GL.Vertex3(w, 0.0, 0.0);

            GL.End();
        }

    }
}

