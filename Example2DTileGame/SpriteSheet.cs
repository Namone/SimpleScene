using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using OpenTK;

namespace Example2DTileGame
{
    public class SpriteSheet
    {
        float gridWidth, gridHeight;
        float pixelWidth, pixelHeight;
        float totalPixelW, totalPixelH;

        /// <summary>
        /// For determining tile ID based off of grid width and grid height
        /// </summary>
        /// <param name="numOfTilesX"></param>
        /// <param name="numOfTilesY"></param>
        public SpriteSheet(int numOfTilesX,int numOfTilesY) {
            this.gridWidth = numOfTilesX;
            this.gridHeight = numOfTilesY;
        }

        /// <summary>
        /// For determing tileID based off of tile pixel height and width - works with getTileById(id)
        /// </summary>
        /// <param name="tileWidth"></param>
        /// <param name="tileHeight"></param>
        public SpriteSheet(float tileWidth, float tileHeight, float gridW, float gridH) {
            this.pixelWidth = tileWidth; 
            this.pixelHeight = tileHeight;
            this.gridWidth = gridW;
            this.gridHeight = gridH;

            // Get total pixels (ignoring borders, for now)
            totalPixelW = pixelWidth * gridWidth;
            totalPixelH = pixelHeight * gridHeight;
        }

        /// <summary>
        /// Get tile by ID (0 - ?)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public RectangleF getTileByID(int id) {
            float boxFractionOfImageX = pixelWidth / totalPixelW; // 0.0833333...
            float boxFractionOfImageY = pixelHeight / totalPixelH; // 0.0833333...

            pixelWidth *= 0.01f; // Convert to percentage
            pixelHeight *= 0.01f; // 0.48
            float boundX = (id / totalPixelW);
            float boundY = (id / totalPixelH);
            float boxW = (boxFractionOfImageX); 
            float boxH = (boxFractionOfImageY);

            PointF xyLocation = new PointF(boundX, boundY);
            SizeF boxSize = new SizeF(boxW, boxH);

            RectangleF bounds = new RectangleF(xyLocation, boxSize);

            return bounds;
        }

        /// <summary>
        /// Calculate, based on input xID and yID, what texture to use
        /// </summary>
        /// <param name="xID"></param>
        /// <param name="yID"></param>
        /// <returns></returns>
        public RectangleF getTileByGrid(int xID, int yID) {
            // I want to, at some point, change it so only one
            // ID needs to be entered by user...
            float boundX = (xID / gridWidth); // Move 'box' over to other tiles based on ID...
            float boundY = (yID / gridHeight);
            float boxW = (1f / gridWidth); // 100% / width
            float boxH = (1f / gridHeight); // 100% / height

            PointF xyLocation = new PointF (boundX, boundY); // These end up being my UV coordinates
            SizeF boxSize = new SizeF(boxW, boxH);

            RectangleF bounds = new RectangleF(xyLocation, boxSize);

            return bounds;
        }

      

    }
}
