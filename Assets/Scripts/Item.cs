using UnityEngine;

[CreateAssetMenu]
public class Item : ScriptableObject
{
    public bool grabbable;

    public AudioClip AudioClip;

    [TextArea(4,1)]
    public string Text;

    public Sprite Image;
}
