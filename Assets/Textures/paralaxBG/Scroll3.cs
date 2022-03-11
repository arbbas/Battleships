using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scroll3 : MonoBehaviour
{
    void Update()
    {
        MeshRenderer meshR = GetComponent<MeshRenderer>();

        Material starMaterial = meshR.material;

        Vector2 offset = starMaterial.mainTextureOffset;

        offset.y += Time.deltaTime / 5;

        starMaterial.mainTextureOffset = offset;
    }
}
