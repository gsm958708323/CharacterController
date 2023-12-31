using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ability
{
    /// <summary>
    /// 打断当前行为
    /// </summary>
    public class CancelAction : AbilityAction
    {
        protected override void OnTick(int curFrame)
        {
            base.OnTick(curFrame);

            tree.curNode.CanCancel = true;
        }
    }
}

