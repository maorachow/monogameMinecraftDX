using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

namespace monogameMinecraftShared.Physics
{
    public struct Ray
    {
        public Vector3 origin;
        public Vector3 direction;

        public Ray(Vector3 origin, Vector3 direction)
        {
            this.origin = origin;
            this.direction = direction;
        }


        public float? Intersects(BoundingBox box, out BlockFaces blockFaceID)
        {
            const float Epsilon = 1e-9f;


            float tMaxTmp = 0;
            blockFaceID = BlockFaces.PositiveY;
            Vector3 maxT = new Vector3(-1.0f);


            if (origin.X >= box.Min.X
                && origin.X <= box.Max.X
                && origin.Y >= box.Min.Y
                && origin.Y <= box.Max.Y
                && origin.Z >= box.Min.Z
                && origin.Z <= box.Max.Z)
                return 0.0f;

            if (Math.Abs(direction.X) < Epsilon)
            {
                return null;
            }
            else
            {
                if (origin.X < box.Min.X)
                {
                    maxT.X = (box.Min.X - origin.X) / direction.X;
                }
                else if (origin.X > box.Max.X)
                {
                    maxT.X = (box.Max.X - origin.X) / direction.X;
                }
            }

            if (Math.Abs(direction.Y) < Epsilon)
            {
                return null;
            }
            else
            {
                if (origin.Y < box.Min.Y)
                {
                    maxT.Y = (box.Min.Y - origin.Y) / direction.Y;
                }
                else if (origin.Y > box.Max.Y)
                {
                    maxT.Y = (box.Max.Y - origin.Y) / direction.Y;
                }
            }

            if (Math.Abs(direction.Z) < Epsilon)
            {
                return null;
            }
            else
            {
                if (origin.Z < box.Min.Z)
                {
                    maxT.Z = (box.Min.Z - origin.Z) / direction.Z;
                }
                else if (origin.Z > box.Max.Z)
                {
                    maxT.Z = (box.Max.Z - origin.Z) / direction.Z;
                }
            }


            if (maxT.X > maxT.Y && maxT.X > maxT.Z)
            {
                if (maxT.X < 0f)
                {
                    return null;
                }

                float intersectPointZ = origin.Z + maxT.X * direction.Z;
                if (intersectPointZ < box.Min.Z || intersectPointZ > box.Max.Z)
                    return null;

                float intersectPointY = origin.Y + maxT.X * direction.Y;
                if (intersectPointY < box.Min.Y || intersectPointY > box.Max.Y)
                    return null;


                if (origin.X < box.Min.X)
                    blockFaceID = BlockFaces.NegativeX;
                else if (origin.X > box.Max.X)
                    blockFaceID = BlockFaces.PositiveX;


                return maxT.X;
            }


            if (maxT.Y > maxT.X && maxT.Y > maxT.Z)
            {
                if (maxT.Y < 0f)
                {
                    return null;
                }

                float intersectPointZ = origin.Z + maxT.Y * direction.Z;
                if (intersectPointZ < box.Min.Z || intersectPointZ > box.Max.Z)
                    return null;

                float intersectPointX = origin.X + maxT.Y * direction.X;
                if (intersectPointX < box.Min.X || intersectPointX > box.Max.X)
                    return null;


                if (origin.Y < box.Min.Y)
                    blockFaceID = BlockFaces.NegativeY;
                else if (origin.Y > box.Max.Y)
                    blockFaceID = BlockFaces.PositiveY;

                return maxT.Y;
            }
            else

            {
                if (maxT.Z < 0f)
                {
                    return null;
                }

                float intersectPointY = origin.Y + maxT.Z * direction.Y;
                if (intersectPointY < box.Min.Y || intersectPointY > box.Max.Y)
                    return null;

                float intersectPointX = origin.X + maxT.Z * direction.X;
                if (intersectPointX < box.Min.X || intersectPointX > box.Max.X)
                    return null;


                if (origin.Z < box.Min.Z)
                    blockFaceID = BlockFaces.NegativeZ;
                else if (origin.Z > box.Max.Z)
                    blockFaceID = BlockFaces.PositiveZ;

                return maxT.Z;
            }
        }
    }
}