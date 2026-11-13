
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class AlphaRaycastTarget : MonoBehaviour
{
    [Range(0f, 1f)]
    [SerializeField] private float alphaThreshold = 0.2f;

    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();

        image.alphaHitTestMinimumThreshold = alphaThreshold;
    }
}
