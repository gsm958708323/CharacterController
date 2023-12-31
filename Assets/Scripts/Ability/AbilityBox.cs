using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ability
{
    public abstract class AbilityBox : MonoBehaviour, ILogicT<ActorModel>
    {
        protected ActorModel model;
        public virtual void Init()
        {

        }

        public virtual void Enter(ActorModel model)
        {
            Debugger.Log($"Enter {GetType()}", LogDomain.AbilityBox);

            this.model = model;
            gameObject.SetActive(false);
            gameObject.SetActive(true);
        }

        public virtual void Enter(ActorModel model, Vector3 pos, Vector3 scale, Quaternion rot)
        {
            Debugger.Log($"Enter {GetType()}", LogDomain.AbilityBox);

            this.model = model;
            // 触发器一直显示OnTriggerEnter只会触发一次，这里保证每次都会触发
            gameObject.SetActive(false);
            gameObject.SetActive(true);

            transform.localPosition = pos;
            transform.localScale = scale;
            transform.localRotation = rot;
        }

        public virtual void Exit()
        {
            Debugger.Log($"Exit {GetType()}", LogDomain.AbilityBox);

            gameObject.SetActive(false);

            this.model = null;
        }

        public virtual void Tick(int frame)
        {

        }
    }
}
