using UnityEngine;

public class PickUp : MonoBehaviour
{
    [SerializeField]
    private TMPro.TMP_Text Text;

    public void Initialize(string text)
    {
        Text.text = text;
    }
}
