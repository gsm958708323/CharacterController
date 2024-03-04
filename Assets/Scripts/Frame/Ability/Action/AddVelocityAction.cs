using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ability
{
    public class AddVelocityAction : AbilityAction
    {
        public Vector3 Velocity;
        // 存在过程，在回调中执行逻辑，需要在开始前将参数准备好，执行播放特效节点

        protected override void OnTick(int frame)
        {
            base.OnTick(frame);

            var transform = tree.ActorModel.transform;
            var add = transform.forward * Velocity.z + transform.right * Velocity.x + transform.up * Velocity.y;
            tree.ActorModel.Velocity += add;
        }
    }
}
