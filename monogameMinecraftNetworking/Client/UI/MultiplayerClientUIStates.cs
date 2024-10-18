using monogameMinecraftShared.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace monogameMinecraftNetworking.Client.UI
{
  
    public class UIStateClientInGame : IUIState
    {
        private IMultiplayerClient client;

        public UIStateClientInGame(UIStateManager state,IMultiplayerClient client)
        {
            this.client= client;
            TextListUI chatMessageListElement = (state.inGameUIs.Find((item) => { return item.optionalTag== "chatMessageList"; })) as TextListUI;
            if (chatMessageListElement != null)
            {
                chatMessageListElement.texts = new List<string>(); 
                client.chatMessageReceivedAction += chatMessageListElement.AppendText;
            }
        }
        public void OnAttachedToManager(UIStateManager state)
        {
            OnResize(state);
        }
        public void Draw(UIStateManager state)
        {

            foreach (var el in state.inGameUIs)
            {
                el.DrawString(state, el.text);
            }


        }

        public void Update(float deltaTime, UIStateManager state)
        {

            foreach (var el in state.inGameUIs)
            {
                el.Update(state);
            }

        }

        public void OnResize(UIStateManager state)
        {
            foreach (var el in state.inGameUIs)
            {
                el.OnResize(state);
            }

        }
    }
}
