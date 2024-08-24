using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;

namespace monogameMinecraftNetworking.Data
{
    [MessagePackObject]
    public struct WorldGenParamsData
    {
        [Key(0)]
        public int worldGenType;
        [Key(1)]
        public float biomeNoiseGeneratorFrequency;
        [Key(2)]
        public float noiseGeneratorFrequency;
        [Key(3)]
        public float frequentNoiseGeneratorFrequency;

        [Key(4)]
        public int noiseGeneratorFractals;
        [Key(5)]
        public int biomeNoiseGeneratorFractals;

        [Key(6)]
        public int frequentNoiseGeneratorFractals;
        [Key(7)]
        public int worldID;
        public WorldGenParamsData(int worldGenType, float biomeNoiseGeneratorFrequency, float noiseGeneratorFrequency,
            float frequentNoiseGeneratorFrequency,int noiseGeneratorFractals,int biomeNoiseGeneratorFractals, int frequentNoiseGeneratorFractals, int worldID)
        {
            this.worldGenType= worldGenType;
            this.frequentNoiseGeneratorFrequency= frequentNoiseGeneratorFrequency;
            this.noiseGeneratorFrequency= noiseGeneratorFrequency;
            this.biomeNoiseGeneratorFrequency= biomeNoiseGeneratorFrequency;
            this.worldID= worldID;
            this.biomeNoiseGeneratorFractals= biomeNoiseGeneratorFractals;
            this.frequentNoiseGeneratorFractals= frequentNoiseGeneratorFractals;
            this.noiseGeneratorFractals= noiseGeneratorFractals;
        }
    }
}
