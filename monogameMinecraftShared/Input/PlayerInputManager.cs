﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using monogameMinecraftShared.UI;
using monogameMinecraftShared.Updateables;

namespace monogameMinecraftShared.Input
{
    public class PlayerInputManager
    {
        public IGamePlayer gamePlayer;
        public bool isTouchEnabled;
        public bool suppressMouseMovement=false;
      //  public 
      
        public PlayerInputManager(IGamePlayer gamePlayer, bool isTouchEnabled)
        {
            this.gamePlayer = gamePlayer;
            this.isTouchEnabled = isTouchEnabled;
        }


      public  MouseState lastMouseState;
      public KeyboardState lastKeyboardState;

        public Vector2 mouseDelta;
        public TouchCollection prevTouches;


        public void ResetPlayerInputValues()
        {
            gamePlayer.ResetPlayerInputValues();
        }
        void ProcessPlayerMouseInput(MouseState curMouseState,bool isTouchEnabled = false)
        {

            mouseDelta=new Vector2(curMouseState.X - lastMouseState.X, lastMouseState.Y - curMouseState.Y);



            if (suppressMouseMovement == true)
            {
                mouseDelta = new Vector2();
                suppressMouseMovement = false;

            }
            if (isTouchEnabled)
            {

                foreach (var tc in UITouchscreenInputHelper.allTouches)
                {
                    TouchLocation prevTouch;
                    if (prevTouches.FindById(tc.Id, out prevTouch) == false)
                    {
                        continue;
                    }
                    if (UITouchscreenInputHelper.CheckIsPointColliding(ref UIElement.inGameUIs, tc.Position))
                    {
                        continue;
                    }
                    mouseDelta = new Vector2();
                    mouseDelta.X = (int)(tc.Position.X - prevTouch.Position.X);
                    mouseDelta.Y = (int)(prevTouch.Position.Y - tc.Position.Y);
                }
               
                  
                 //   Debug.WriteLine(mouseDelta);
                 
            }
        }

        public static Vector3 mobilemotionVec;
        public static bool mobileIsFlyingPressed = false;
        public static bool mobileIsSprintingPressed = false;
        public static bool mobileIsJumpPressed =false;
        public static int mobileScrollDelta = 0;
        public static bool mobileIsLMBPressed = false;
        public static bool mobileIsRMBPressed = false;
        void ProcessPlayerMobileTouchInput(ref Vector3 inVec,ref bool inIsFlyingPressed,ref bool inIsJumpPressed,ref bool inIsLMBPressed, ref bool inIsRMBPressed,ref float scrollDelta,ref bool inIsSprintPressed)
        {
            if (mobilemotionVec.Length()>0f)
            {
                inVec = mobilemotionVec;
                mobilemotionVec = new Vector3(0, 0, 0);
            }

            if (mobileIsFlyingPressed == true)
            {
                inIsFlyingPressed = mobileIsFlyingPressed;


                mobileIsFlyingPressed = false;
            }


            if (mobileIsJumpPressed == true)
            {
                inIsJumpPressed = mobileIsJumpPressed;


                mobileIsJumpPressed = false;
            }



            if (mobileIsLMBPressed == true)
            {
                inIsLMBPressed = mobileIsLMBPressed;


                mobileIsLMBPressed = false;
            }

            if (mobileIsRMBPressed == true)
            {
                inIsRMBPressed = mobileIsRMBPressed;


                mobileIsRMBPressed = false;
            }

            if (mobileIsSprintingPressed == true)
            {
                inIsSprintPressed = mobileIsSprintingPressed;


                mobileIsSprintingPressed = false;
            }

            if (mobileScrollDelta !=0)
            {
                scrollDelta = mobileScrollDelta;


                mobileScrollDelta = 0;
            }

        }
        void ProcessPlayerInput(float deltaTime)
        {

            var kState = Keyboard.GetState();
            var mState = Mouse.GetState();
            Vector3 playerVec = new Vector3(0f, 0f, 0f);
            bool isFlyingPressed = false;
            bool isSpeedUpPressed=false;
            bool isLMBPressed=false;
            bool isRMBPressed=false;
            float scrollDelta = 0f;
            ProcessPlayerMouseInput(mState,isTouchEnabled);
            if (kState.IsKeyDown(Keys.T) && !lastKeyboardState.IsKeyDown(Keys.T))
            {
                //   gamePlayer.PlayerTryTeleportToEnderWorld(this,);


            }
            if (kState.IsKeyDown(Keys.W))
            {
                playerVec.Z = 1f;
            }

            if (kState.IsKeyDown(Keys.S))
            {
                playerVec.Z = -1f;
            }

            if (kState.IsKeyDown(Keys.A))
            {
                playerVec.X = -1f;
            }

            if (kState.IsKeyDown(Keys.D))
            {
                playerVec.X = 1f;
            }
            if (kState.IsKeyDown(Keys.Space))
            {
                playerVec.Y = 1f;
            }
            if (kState.IsKeyDown(Keys.LeftShift))
            {
                playerVec.Y = -1f;
            }

            if (kState.IsKeyDown(Keys.F) && !lastKeyboardState.IsKeyDown(Keys.F))
            {
                isFlyingPressed = true;
            }

            if (kState.IsKeyDown(Keys.LeftControl))
            {
                isSpeedUpPressed = true;
            }

            if (mState.LeftButton == ButtonState.Pressed)
            {
                isLMBPressed = true;
            }

            if (mState.RightButton == ButtonState.Pressed)
            {
                isRMBPressed = true;
            }

            bool _=false;
           
         
            if (mState.ScrollWheelValue - lastMouseState.ScrollWheelValue != 0f)
            {
                scrollDelta = (int)((mState.ScrollWheelValue - lastMouseState.ScrollWheelValue));
           
            }

            if (isTouchEnabled)
            {
                ProcessPlayerMobileTouchInput(ref playerVec, ref isFlyingPressed, ref _, ref isLMBPressed, ref isRMBPressed,ref scrollDelta,ref isSpeedUpPressed);
            }
            gamePlayer.ProcessPlayerInputs(playerVec, (float)deltaTime, kState, mState, lastMouseState,isFlyingPressed, isSpeedUpPressed, isLMBPressed, isRMBPressed, scrollDelta);
            gamePlayer.cam.ProcessMouseMovement(mouseDelta.X, mouseDelta.Y);
            lastKeyboardState = kState;
            lastMouseState = mState;
            prevTouches = UITouchscreenInputHelper.allTouches;

        }
        public void Update(float deltaTime)
        {
            ProcessPlayerInput(deltaTime);
        }
    }
}
