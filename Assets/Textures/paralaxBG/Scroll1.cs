using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scroll1 : MonoBehaviour
{



    // Update is called once per frame
    void Update()
    {
        MeshRenderer meshR = GetComponent<MeshRenderer>();

        Material starMaterial = meshR.material;

        Vector2 offset = starMaterial.mainTextureOffset;

        offset.y += Time.deltaTime / 20f;

        starMaterial.mainTextureOffset = offset;
    }
}
