using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRoot : MonoBehaviour
{
    public Transform LayerRoot;
    public Camera UICamera;
    public RectTransform AdaptResolution;

    private CanvasScaler canvasScaler;

    private void Awake()
    {
        canvasScaler = GetComponent<CanvasScaler>();

        if (Screen.height / Screen.width > canvasScaler.referenceResolution.y / canvasScaler.referenceResolution.x)
        {
            AdaptResolution.offsetMin = new Vector2(AdaptResolution.offsetMin.x, AdaptResolution.offsetMin.y + 44f);

            AdaptResolution.offsetMax = new Vector2(AdaptResolution.offsetMax.x, AdaptResolution.offsetMax.y - 44f);
        }
    }
}
