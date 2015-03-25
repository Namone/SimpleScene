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
        float num_tiles_in_row;
        float pixelWidth, pixelHeight;
        float totalPixelW, totalPixelH;
        int tileID;

        /// <summary>
        /// For determining tile ID based off of grid width and grid height
        /// </summary>
        /// <param name="numOfTilesX"></param>
        /// <param name="numOfTilesY"></param>
        public SpriteSheet(int numOfTilesX, int numOfTilesY) {
            this.gridWidth = numOfTilesX;
            this.gridHeight = numOfTilesY;

            num_tiles_in_row = numOfTilesX;
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
        public RectangleF getTileByPixel(int id) {

            tileID = id;
            float boundX = (pixelWidth / totalPixelW) * id; // UV coordinates
            float boundY = (pixelHeight / totalPixelH) * id;
            float boxW = (pixelWidth / totalPixelW); // Width of 'box' which contains our texture segment
            float boxH = (pixelHeight / totalPixelH);

            PointF xyLocation = new PointF(boundX, boundY);
            SizeF boxSize = new SizeF(boxW, boxH);

            RectangleF bounds = new RectangleF(xyLocation, boxSize);

            return bounds;
        }

        /// <summary>
        /// Calculate, based on input xID and yID, what texture to use
        /// </summary>
        /// <returns></returns>
        public RectangleF getTileByGrid(int id) {

            // Column = X
            // Row = y
            float columnNumber = (id / num_tiles_in_row); // UV coordinates
            float rowNumber = (id % num_tiles_in_row);

            float boxW = (1f / gridWidth); // 100% / width
            float boxH = (1f / gridHeight); // 100% / height

            Console.WriteLine("Row Number: " + rowNumber); // For some reason it's just pritning out the ID?
            Console.WriteLine("Column Number: " + columnNumber);

            PointF xyLocation = new PointF (columnNumber, rowNumber); // These end up being my UV coordinates
            SizeF boxSize = new SizeF(boxW, boxH);

            RectangleF bounds = new RectangleF(xyLocation, boxSize);

            return bounds;

            // TODO - Save texture ID's assigned so they can be loaded from a file
        }

        public int getTileID() {
            return tileID;
        }

    }
}
