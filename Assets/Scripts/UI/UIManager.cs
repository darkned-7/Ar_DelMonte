using DG.Tweening;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject scanMenuCanvas;

    [SerializeField]
    private GameObject activatorMenuCanvas;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.OnScanMenu += ActivateScanMenu;
        GameManager.instance.OnActivatorMenu += ActivateActivatorMenu;
    }

    private void ActivateScanMenu()
    {
        scanMenuCanvas.transform.GetChild(0).transform.DOScale(new Vector3(1, 1, 1), 0.3f);
        scanMenuCanvas.transform.GetChild(1).transform.DOScale(new Vector3(1, 1, 1), 0.3f);

        activatorMenuCanvas.transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 0.5f);
        activatorMenuCanvas.transform.GetChild(1).transform.DOScale(new Vector3(0, 0, 0), 0.5f);
        activatorMenuCanvas.transform.GetChild(1).transform.DOMoveY(100, 0.3f);
    }

    private void ActivateActivatorMenu()
    {
        scanMenuCanvas.transform.GetChild(0).transform.DOScale(new Vector3(0, 0, 0), 0.3f);
        scanMenuCanvas.transform.GetChild(1).transform.DOScale(new Vector3(0, 0, 0), 0.3f);

        activatorMenuCanvas.transform.GetChild(0).transform.DOScale(new Vector3(1, 1, 1), 0.5f);
        activatorMenuCanvas.transform.GetChild(1).transform.DOScale(new Vector3(1, 1, 1), 0.5f);
        activatorMenuCanvas.transform.GetChild(1).transform.DOMoveY(260, 0.3f);
    }
}
