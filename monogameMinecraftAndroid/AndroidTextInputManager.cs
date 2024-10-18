using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Java.Sql;
using Microsoft.Xna.Framework;
using MonoGame.Framework.Utilities.Deflate;
using monogameMinecraftShared.UI;

namespace monogameMinecraftAndroid
{
    public class AndroidTextInputManager
    {
        public static AlertDialog alert;
        public static bool isDialogOpened = false;
        public static string resultText = "";
        public static void Update()
        {
            if (UITouchscreenInputHelper.androidIsInputPanelOpened == true)
            {
                if (UITouchscreenInputHelper.androidCurEditingElement != null)
                {
                   
                  
                    if (isDialogOpened == true)
                    {
                        return;
                    }
                    isDialogOpened = true;
                    Game.Activity.RunOnUiThread((System.Action)(() =>
                    {
                        alert = new AlertDialog.Builder((Context)Game.Activity).Create();
                        alert.SetTitle("Text Input");
                        alert.SetMessage("Enter Text");
                        EditText input = new EditText((Context)Game.Activity)
                        {
                            Text = ""
                        };



                        alert.SetView((View)input);
                        alert.SetButton(-1, "Ok", (EventHandler<DialogClickEventArgs>)((sender, args) =>
                        {
                            resultText = input.Text;
                            if (UITouchscreenInputHelper.androidCurEditingElement is InputField)
                            {

                                (UITouchscreenInputHelper.androidCurEditingElement as InputField).text = resultText.Substring(0, Math.Min(resultText.Length, (UITouchscreenInputHelper.androidCurEditingElement as InputField).maxAllowedCharacters));
                            }
                            isDialogOpened = false;
                        }));
                        alert.SetButton(-2, "Cancel", (EventHandler<DialogClickEventArgs>)((sender, args) =>
                        {
                            resultText = "";
                            if (UITouchscreenInputHelper.androidCurEditingElement is InputField)
                            {

                                (UITouchscreenInputHelper.androidCurEditingElement as InputField).text = resultText.Substring(0, Math.Min(resultText.Length, (UITouchscreenInputHelper.androidCurEditingElement as InputField).maxAllowedCharacters));
                            }
                            isDialogOpened = false;
                        }));
                        alert.CancelEvent += (EventHandler)((sender, args) =>
                        {
                            resultText = input.Text;
                            if (UITouchscreenInputHelper.androidCurEditingElement is InputField)
                            {

                                (UITouchscreenInputHelper.androidCurEditingElement as InputField).text = resultText.Substring(0, Math.Min(resultText.Length, (UITouchscreenInputHelper.androidCurEditingElement as InputField).maxAllowedCharacters));
                            }
                            isDialogOpened = false;
                        });
                        alert.Show();
                    }));
                    System.Diagnostics.Debug.WriteLine("open dialog");

                }

                UITouchscreenInputHelper. androidIsInputPanelOpened = false;
            }

        }
    }
}
