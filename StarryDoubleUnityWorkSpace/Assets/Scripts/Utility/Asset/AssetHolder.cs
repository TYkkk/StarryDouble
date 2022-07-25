using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaseFramework
{
    public abstract class AssetHolder : MonoBehaviour
    {
        protected virtual void ReleaseMethod() { }

        protected virtual void OnDestroy()
        {
            ReleaseMethod();
        }

        public virtual void AddAssetHolder()
        {
        }
    }
}
