using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BaseFramework
{
    public class UIBase : BaseMonoBehaviour
    {
        public string UIName => uiName;

        private string uiName;

        public virtual void Awake()
        {
        }

        public virtual void Register()
        {

        }

        public virtual void Opening()
        {
        }

        public virtual void Open()
        {
        }

        public virtual void Start()
        {
        }

        public virtual void UnRegister()
        {

        }

        public virtual void Close()
        {
        }

        public virtual void OnDestroy()
        {
        }

        public void SetName(string uiName)
        {
            this.uiName = uiName;
        }
    }

}