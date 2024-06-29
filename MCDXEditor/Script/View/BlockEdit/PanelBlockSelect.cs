using System.Drawing;
using Myra.Graphics2D;
using Myra.Graphics2D.UI;
using Myra.Graphics2D.UI.Properties;
using Color = Microsoft.Xna.Framework.Color;
using SolidBrush = Myra.Graphics2D.Brushes.SolidBrush;

namespace MCDX.Editor.View.BlockEdit;

public class PanelBlockSelect: Widget
{
     private Label txtTitle;
     private Grid content;
     private Grid root;

     public PanelBlockSelect()
     {
          this.Width = 420;
          this.Height = 720;
          txtTitle = new Label();
          txtTitle.Text = "Editing Block Selector";
          content = new Grid();
          this.Children.Add(txtTitle);
          this.Children.Add(content);
          this.Background = new SolidBrush(Color.DarkKhaki);
          txtTitle.Padding = new Thickness(50, 0, 40, 50);
          content.Padding = new Thickness(50, 20, 50, 20);
          content.Widgets.Add(new ItemBlockSelect());


     }
}