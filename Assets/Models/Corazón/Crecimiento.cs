using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crecimiento : MonoBehaviour
{
    public List<MeshRenderer> rootMeshes;
    public float timeToGrow = 5;
    public float refreshRate = 0.05f;
    [Range(0, 1)]
    public float minGrow;
    [Range(0, 1)]
    public float maxGrow;

    private List<Material> rootMaterials = new List<Material>();
    private bool fullyGrow;

    public void GrowUp()
    {
        for (int i = 0; i < rootMeshes.Count; i++)
        {
            for (int j = 0; j < rootMeshes[i].materials.Length; j++)
            {
                if (rootMeshes[i].materials[j].HasProperty("Grow_"))
                {
                    rootMeshes[i].materials[j].SetFloat("Grow_", minGrow);
                    rootMaterials.Add(rootMeshes[i].materials[j]);
                }
            }
        }

        for (int i = 0; i < rootMaterials.Count; i++)
        {
            StartCoroutine(RootsVines(rootMaterials[i]));
        }
    }

    IEnumerator RootsVines(Material mat)
    {
        float growValue = mat.GetFloat("Grow_");
        if (!fullyGrow)
        {
            while (growValue < maxGrow)
            {
                growValue += 1 / (timeToGrow / refreshRate);
                mat.SetFloat("Grow_", growValue);

                yield return new WaitForSeconds(refreshRate);
            }
        }
        else
        {
            while (growValue > minGrow)
            {
                growValue -= 1 / (timeToGrow / refreshRate);
                mat.SetFloat("Grow_", growValue);

                yield return new WaitForSeconds(refreshRate);
            }
        }

        if (growValue >= maxGrow)
        {
            fullyGrow = true;
        }
        else
        {
            fullyGrow = false;
        }
    }
}
