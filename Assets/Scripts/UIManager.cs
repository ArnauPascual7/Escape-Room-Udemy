using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public GameObject HandCursor;
    public GameObject BackImg;
    public TextMeshProUGUI CaptionText;
    public Image InteractionImage;

    private void Awake()
    {
        Instance = this;
    }

    public void SetHandCursor(bool state)
    {
        HandCursor.SetActive(state);
    }

    public void SetBackImg(bool state)
    {
        BackImg.SetActive(state);

        if (!state)
        {
            InteractionImage.enabled = false;
        }
    }

    public void SetCaptionText(string text)
    {
        CaptionText.text = text;
    }

    public void SetImage(Sprite spr)
    {
        InteractionImage.sprite = spr;
        InteractionImage.enabled = true;
    }
}
