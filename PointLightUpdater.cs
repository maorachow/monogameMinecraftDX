using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using monogameMinecraftDX;
namespace monogameMinecraft
{
    
    public class PointLightUpdater
    {
       
        public GamePlayer player;
        public List<Vector3> lights;
        public List<Vector3> lightsPrev;
        public List<Vector3> lightsDestroying;
        public PointLightUpdater( GamePlayer player)
        {
        
            this.player = player;
            lights = new List<Vector3>();
            lightsPrev = new List<Vector3>();
            lightsDestroying = new List<Vector3>();
        }
        public void UpdatePointLight()
        {
           lights.Clear();
            BoundingFrustum frustum = new BoundingFrustum(player.cam.viewMatrix * player.cam.projectionMatrix);
            foreach(var c in VoxelWorld.currentWorld.chunks.Values)
            {
                if (c.disposed == false && c.isReadyToRender == true)
                {
                if (frustum.Intersects(c.chunkBounds))
                {
                       foreach(var position in c.lightPoints)
                        {
                        if(lights.Count >=4) {
                                break;
                            }
                            lights.Add(position);
                            
                        }
                }
                }
               
            }
         //   Debug.WriteLine(lights.Count);
        //    if(lights.Count > 0)
         //   {
//    Debug.WriteLine(lights?[0].ToString());
        //    }
        
            lightsDestroying.Clear();
                foreach(var light in lightsPrev)
                {
                    if(!lights.Contains(light)) {
                        lightsDestroying.Add(light);
                    }
                }
                lightsPrev = new List<Vector3>(lights);
                
           
           

        }
    }
}
