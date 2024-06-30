using Microsoft.Xna.Framework;
using System.Collections.Generic;
using monogameMinecraftDX.World;
using System;
namespace monogameMinecraftDX.Utility
{

    public class PointLightUpdater
    {

        public GamePlayer player;
        public List<Vector3> lights;
        public List<Vector3> lightsPrev;
        public List<Vector3> lightsDestroying;

        public PointLightUpdater(GamePlayer player)
        {

            this.player = player;
            lights = new List<Vector3>();
            lightsPrev = new List<Vector3>();
            lightsDestroying = new List<Vector3>();
        }
        public BoundingFrustum playerViewProjFrustum = new BoundingFrustum(Matrix.Identity);
        public void UpdatePointLight()
        {
            lights.Clear();
            playerViewProjFrustum.Matrix = player.cam.viewMatrix * player.cam.projectionMatrix;
            foreach (var c in VoxelWorld.currentWorld.chunks.Values)
            {
                if (c.disposed == false && c.isReadyToRender == true)
                {
                    if (MathF.Abs(c.chunkPos.x - player.position.X) < 128 &&
                        MathF.Abs(c.chunkPos.y - player.position.Z) < 128)
                    {
                    if (playerViewProjFrustum.Intersects(c.chunkBounds))
                    {
                        foreach (var position in c.lightPoints)
                        {
                            if (lights.Count >= 16)
                            {
                                break;
                            }
                            lights.Add(position);

                        }
                    }
                    }
                   
                }

            }
            while (lights.Count < 16)
            {
                lights.Add(new Vector3(0, 0, 0));
            }
            //   Debug.WriteLine(lights.Count);
            //    if(lights.Count > 0)
            //   {
            //    Debug.WriteLine(lights?[0].ToString());
            //    }






        }
    }
}
