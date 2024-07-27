using MessagePack;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monogameMinecraftShared.Core
{
    [MessagePackObject]
    public struct Vector2Int : IEquatable<Vector2Int>
    {
        [Key(0)]
        public int x;
        [Key(1)]
        public int y;
        [SerializationConstructor]
        public Vector2Int(int a, int b)
        {
            x = a;
            y = b;
        }
        [IgnoreMember]
        public float magnitude { get { return MathF.Sqrt(x * x + y * y); } }

        // Returns the squared length of this vector (RO).
        [IgnoreMember]
        public int sqrMagnitude { get { return x * x + y * y; } }
        public override bool Equals(object other)
        {
            if (!(other is Vector2Int)) return false;

            return Equals((Vector2Int)other);
        }
        public bool Equals(Vector2Int other)
        {
            return x == other.x && y == other.y;
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode() << 2;
        }


        public static Vector2Int operator -(Vector2Int v)
        {
            return new Vector2Int(-v.x, -v.y);
        }


        public static Vector2Int operator +(Vector2Int a, Vector2Int b)
        {
            return new Vector2Int(a.x + b.x, a.y + b.y);
        }

        public static Vector2Int operator -(Vector2Int a, Vector2Int b)
        {
            return new Vector2Int(a.x - b.x, a.y - b.y);
        }


        public static Vector2Int operator *(Vector2Int a, Vector2Int b)
        {
            return new Vector2Int(a.x * b.x, a.y * b.y);
        }


        public static Vector2Int operator *(int a, Vector2Int b)
        {
            return new Vector2Int(a * b.x, a * b.y);
        }


        public static Vector2Int operator *(Vector2Int a, int b)
        {
            return new Vector2Int(a.x * b, a.y * b);
        }


        public static Vector2Int operator /(Vector2Int a, int b)
        {
            return new Vector2Int(a.x / b, a.y / b);
        }


        public static bool operator ==(Vector2Int lhs, Vector2Int rhs)
        {
            return lhs.x == rhs.x && lhs.y == rhs.y;
        }

        public static bool operator !=(Vector2Int lhs, Vector2Int rhs)
        {
            return !(lhs == rhs);
        }

    }
    [MessagePackObject]
    public struct Vector3Int : IEquatable<Vector3Int>
    {
        [Key(0)]
        public int x;
        [Key(1)]
        public int y;
        [Key(2)]
        public int z;

        public Vector3Int(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public static int FloorToInt(float f) { return (int)Math.Floor(f); }
        public static Vector3Int FloorToIntVec3(Vector3 v)
        {
            return new Vector3Int(
                FloorToInt(v.X),
                FloorToInt(v.Y),
               FloorToInt(v.Z)
            );
        }
        public static Vector3Int operator +(Vector3Int b, Vector3Int c)
        {
            Vector3Int v = new Vector3Int(b.x + c.x, b.y + c.y, b.z + c.z);
            return v;
        }
        public static Vector3Int operator -(Vector3Int b, Vector3Int c)
        {
            Vector3Int v = new Vector3Int(b.x - c.x, b.y - c.y, b.z - c.z);
            return v;
        }
        public static bool operator ==(Vector3Int lhs, Vector3Int rhs)
        {
            return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z;
        }


        public static bool operator !=(Vector3Int lhs, Vector3Int rhs)
        {
            return !(lhs == rhs);
        }


        public override bool Equals(object other)
        {
            if (!(other is Vector3Int)) return false;

            return Equals((Vector3Int)other);
        }


        public override int GetHashCode()
        {
            var yHash = y.GetHashCode();
            var zHash = z.GetHashCode();
            return x.GetHashCode() ^ yHash << 4 ^ yHash >> 28 ^ zHash >> 4 ^ zHash << 28;
        }
        public bool Equals(Vector3Int other)
        {
            return this == other;
        }
        public override string ToString()
        {
            return "X:" + x + "  Y:" + y + "  Z:" + z;
        }

        public static explicit operator Vector3(Vector3Int v) { return new Vector3(v.x, v.y, v.z); }
        public static explicit operator Vector3Int(Vector3 v) { return new Vector3Int((int)v.X, (int)v.Y, (int)v.Z); }
    }
    public struct RandomGenerator3D
    {
        //  public System.Random rand=new System.Random(0);
        public static FastNoise randomNoiseGenerator = new FastNoise();
        public static bool initNoiseGen = InitNoiseGenerator();
        public static bool InitNoiseGenerator()
        {
            //  randomNoiseGenerator.SetSeed(0);
            randomNoiseGenerator.SetNoiseType(FastNoise.NoiseType.Value);
            randomNoiseGenerator.SetFrequency(100f);
            return true;
            // randomNoiseGenerator.SetFractalType(FastNoise.FractalType.None);
        }
        public static int GenerateIntFromVec3(Vector3Int pos)
        {
            float value = randomNoiseGenerator.GetSimplex(pos.x * 2f, pos.y * 2f, pos.z * 2f);
            value += 1f;
            int finalValue = (int)(value * 53f);
            finalValue = MathHelper.Clamp(finalValue, 0, 100);
            //   Debug.Log(finalValue);
            //   System.Random rand=new System.Random(pos.x*pos.y*pos.z*100);
            return finalValue;
        }
    }

    public static class MathUtility
    {

        public static bool[] GetBooleanArray(byte b)
        {
            bool[] array = new bool[8];
            for (int i = 7; i >= 0; i--)
            { //对于byte的每bit进行判定
                array[i] = (b & 1) == 1;   //判定byte的最后一位是否为1，若为1，则是true；否则是false
                b = (byte)(b >> 1);       //将byte右移一位
            }
            return array;
        }

        public static byte GetByte(bool[] array)
        {
            if (array != null && array.Length > 0)
            {
                byte b = 0;
                for (int i = 0; i <= 7; i++)
                {
                    if (array[i])
                    {
                        int nn = 1 << 7 - i;
                        b += (byte)nn;
                    }
                }
                return b;
            }
            return 0;
        }


    }
}
