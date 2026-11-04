using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowMapsSections : MonoBehaviour
{
    public GameObject MapSectionToShow;

    public void ShowMapSection()
    {
        MapSectionToShow.SetActive(true);
    }
}
