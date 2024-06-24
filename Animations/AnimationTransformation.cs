using Microsoft.Xna.Framework;
namespace monogameMinecraftDX
{
    namespace Animations
    {
  public class AnimationTransformation
    {


        public Vector3 Offset;


        public Vector3 Rotation;


        public Vector3 Scale;


        static float ToRads = (System.MathF.PI / 180f);


        public Matrix ToMatrix()
        {
            var scale = Matrix.CreateScale(Scale);
            var rotateMatrix = Matrix.CreateFromYawPitchRoll(Rotation.X * ToRads, Rotation.Y * ToRads, Rotation.Z * ToRads);
            var translation = Matrix.CreateTranslation(Offset.X, Offset.Y, Offset.Z);
            return scale * rotateMatrix * translation;
        }


        public static AnimationTransformation Lerp(AnimationTransformation first, AnimationTransformation second, float amount)
        {
            if (first == null)
            {
                if (second == null)
                {
                    return null;
                }
                return second;
            }
            if (second == null)
            {
                if (first == null)
                {
                    return null;
                }
                return first;
            }
            return new AnimationTransformation()
            {
                Offset = Vector3.Lerp(first.Offset, second.Offset, amount),
                Rotation = Vector3.Lerp(first.Rotation, second.Rotation, amount),
                Scale = Vector3.Lerp(first.Scale, second.Scale, amount)
            };
        }
        public AnimationTransformation() { }
        public AnimationTransformation(Vector3 offset, Vector3 rotation, Vector3 scale)
        {
            Offset = offset;
            Rotation = rotation;
            Scale = scale;
        }

        public static readonly AnimationTransformation Identity = new AnimationTransformation() { Offset = Vector3.Zero, Rotation = Vector3.Zero, Scale = Vector3.One };


        public static readonly AnimationTransformation Empty = new AnimationTransformation();
    }
    }
  
}
