using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
 

namespace monogameMinecraftDX.Updateables
{
   
    public struct ParticleEmittingParams
    {
        [JsonInclude]
        public ParticleType type;
        [JsonInclude]
        public Vector4 uvWidthCorner;
        [JsonInclude]
        public int particleCount;
        [JsonInclude]
        public float radius;
        [JsonInclude]
        public float size;
        [JsonInclude]
        public float motionVectorSpeed;
        [JsonInclude]
        public float friction;
        [JsonInclude]
        public float lifeTime;
    public ParticleEmittingParams(ParticleType type,Vector4 uvWidthCorner,int particleCount,float radius,float size,float motionVectorSpeed,float friction,float lifeTime)
    {
        this.type = type;
        this.uvWidthCorner= uvWidthCorner;
        this.particleCount= particleCount;
        this.radius= radius;
        this.size= size;
        this.motionVectorSpeed= motionVectorSpeed;
        this.friction=friction;
        this.lifeTime= lifeTime;
    }
    }

    public enum ParticleType
    {
        TexturedGravityParticle=0,
        TexturedMotionParticle=1,
    }
}
