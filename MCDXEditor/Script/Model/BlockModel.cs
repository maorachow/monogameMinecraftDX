using System;

namespace MCDX.Editor.Model;

/// <summary>
/// 方块Json信息
/// 1. 数据世界中的信息
/// 2. 音效信息
/// 3. 渲染世界中的信息
/// 4. 额外组件信息
/// </summary>
[Serializable]
public class BlockModel
{
    /// <summary>
    /// 识别符, 比如Minecraft:Stone,这个Minecraft就是识别符
    /// </summary>
    public string identifier;
    /// <summary>
    /// 名字.比如MiNECRAFT:STONE,这个name就是识别符
    /// </summary>
    public string name;
    /// <summary>
    /// 抗暴性
    /// </summary>
    public bool blastResistance;
    /// <summary>
    /// 硬度
    /// </summary>
    public bool hardness;
    /// <summary>
    /// 方块的音效信息
    /// </summary>
    public AudioModel audioModel;
    /// <summary>
    /// 方块的材质信息
    /// </summary>
    public TextureModel textureModel;
}