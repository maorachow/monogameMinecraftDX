namespace MCDX.Editor.Model;

public class AudioModel
{
    /// <summary>
    /// 挖掘音效
    /// </summary>
    public string[] dig;
    public SoundSequenceType digSequenceType;
    /// <summary>
    /// 在上面走路的音效
    /// </summary>
    public string walkOn;

    public SoundSequenceType walkOnSequenceType;
}

/// <summary>
/// 音效播放徐磊是随机还是按照序列播放
/// </summary>
public enum SoundSequenceType
{
    Random,
    Sequence
}