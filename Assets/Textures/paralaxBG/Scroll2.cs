using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scroll2 : MonoBehaviour
{
    void Update()
    {
        MeshRenderer meshR = GetComponent<MeshRenderer>();

        Material starMaterial = meshR.material;

        Vector2 offset = starMaterial.mainTextureOffset;

        offset.y += Time.deltaTime / 10f;

        starMaterial.mainTextureOffset = offset;
    }
}
