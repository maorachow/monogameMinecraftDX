using Microsoft.Xna.Framework;
using Myra;
using Myra.Graphics2D.UI;

namespace MCDX.Editor;

public class RootNode
{ 
    private Desktop desktop;
    internal RootNode(Game _game)
    { 
        desktop = new Desktop();
        Panel rootPanel = new Panel();
        rootPanel.Height = 1080;
        rootPanel.Width = 1920;
        desktop.Root = rootPanel;
    }

    internal void Render()
    {
        desktop.Render();
    }
}