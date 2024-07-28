using monogameMinecraftDX;
using monogameMinecraftShared.World;

namespace MCDX.Editor.Model;

/// <summary>
/// 渲染世界的数据
/// </summary>
public class TextureModel
{
    
    /// <summary>
    /// 是否半透明
    /// </summary>
    public bool transparent;
    /// <summary>
    /// 光照等级
    /// </summary>
    public int lightLevel;
    /// <summary>
    /// 接收阴影
    /// </summary>
    public bool receiveShadow;
    /// <summary>
    /// 投射阴影
    /// </summary>
    public bool shadowCaster;

    public BlockShape blockShape;
    public string leftTexture;
    public string rightTexture;
    public string bottomTexture;
    public string topTexture;
    public string frontTexture;
    public string backTexture;
}