﻿using Microsoft.Xna.Framework;
 
using monogameMinecraftShared.Updateables;
using System;
namespace monogameMinecraftShared.World
{
    public class GameTimeManager
    {
     
        public float dateTime = 0.1f;

        public float skyboxMixValue = 0f;
        public Vector3 sunDir;
        public float sunX;
        public float sunY;
        public float sunZ = 0f;
        public bool updatingEnabled;

        public GameTimeManager(IGamePlayer player, bool updatingEnabled = true)
        {
            this.updatingEnabled = updatingEnabled;
        }
        public Vector3 EulerToVec3(Vector3 euler)
        {
            float yaw = euler.Y;
            float pitch = euler.X;


            //        Debug.WriteLine(yaw + " " + pitch);

            Vector3 front = new Vector3();
            front.X = MathF.Cos(MathHelper.ToRadians(yaw)) * MathF.Cos(MathHelper.ToRadians(pitch));
            front.Y = MathF.Sin(MathHelper.ToRadians(pitch));
            front.Z = MathF.Sin(MathHelper.ToRadians(yaw)) * MathF.Cos(MathHelper.ToRadians(pitch));
            front.Normalize();
            return front;
        }
        //0.25f-0.75f night

        public void SetDateTime(float targetDateTime)
        {
            dateTime= targetDateTime;
            if (dateTime >= 1f)
            {
                dateTime = 0f;
            }

            float time = dateTime;

            if (0f <= time && time < 0.1f)
            {
                skyboxMixValue = MathHelper.Lerp(0.5f, 0f, time * 10f);
            }
            else if (0.1f <= time && time < 0.4f)
            {
                skyboxMixValue = 0;
            }
            else if (0.4f <= time && time < 0.6f)
            {
                skyboxMixValue = MathHelper.Lerp(0f, 1f, (time - 0.4f) * 5f);
            }
            else if (0.6f <= time && time < 0.9f)
            {
                skyboxMixValue = 1f;
            }
            else if (0.9f <= time && time < 1f)
            {
                skyboxMixValue = MathHelper.Lerp(1f, 0.5f, (time - 0.9f) * 10f);
            }
            sunX = dateTime * 360f;
            sunY = 20f;
            sunDir = EulerToVec3(new Vector3(sunX, sunY, sunZ)) * 50f;
        }
        public void Update(float deltaTime)
        {
            if (!updatingEnabled)
            {
                return;
            }
            dateTime += deltaTime * 0.005f;
            if (dateTime >= 1f)
            {
                dateTime = 0f;
            }

            float time = dateTime;

            if (0f <= time && time < 0.1f)
            {
                skyboxMixValue = MathHelper.Lerp(0.5f, 0f, time * 10f);
            }
            else if (0.1f <= time && time < 0.4f)
            {
                skyboxMixValue = 0;
            }
            else if (0.4f <= time && time < 0.6f)
            {
                skyboxMixValue = MathHelper.Lerp(0f, 1f, (time - 0.4f) * 5f);
            }
            else if (0.6f <= time && time < 0.9f)
            {
                skyboxMixValue = 1f;
            }
            else if (0.9f <= time && time < 1f)
            {
                skyboxMixValue = MathHelper.Lerp(1f, 0.5f, (time - 0.9f) * 10f);
            }
            sunX = dateTime * 360f;
            sunY = 20f;
            sunDir = EulerToVec3(new Vector3(sunX, sunY, sunZ)) * 50f;
            //     Debug.WriteLine(sunDir);


        }
    }
}
