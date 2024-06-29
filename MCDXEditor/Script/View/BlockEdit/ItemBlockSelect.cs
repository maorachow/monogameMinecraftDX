using Microsoft.Xna.Framework;
using Myra.Graphics2D.Brushes;
using Myra.Graphics2D.TextureAtlases;
using Myra.Graphics2D.UI;

namespace MCDX.Editor.View.BlockEdit;

public class ItemBlockSelect: Widget
{
    private Button btnSelect;
    private Label txtTitle;
    private Label txtSelect;
    public ItemBlockSelect()
    {
        btnSelect = new Button();
        txtTitle = new Label();
        txtSelect = new Label();
        txtTitle.Text = "Select";
        txtTitle.Text = "(ID=0) testblock";
        this.Children.Add(txtTitle);
        this.Children.Add(btnSelect);
        btnSelect.Content = txtSelect;
        this.Width = 200;
        this.Height = 50;
        btnSelect.Width = 50;
        btnSelect.Height = 50;
        this.Background = new SolidBrush(Color.Black);
    }
}