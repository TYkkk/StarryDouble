using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BaseFramework
{
    public class TextureAssetHolder : AssetHolder
    {
        protected override void ReleaseMethod()
        {
            base.ReleaseMethod();

            if (gameObject.GetComponent<RawImage>() != null)
            {
                gameObject.GetComponent<RawImage>().texture = null;
            }
        }
    }
}
