using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Ability
{
    // [CreateAssetMenu(fileName = "NewBehavior", menuName = "AbilityTree/BulletBehavior")]
    public class BulletBehavior
    {
        public AbilityAction OnAddAction;
        public AbilityAction OnHitAction;
        public AbilityAction OnRemoveAction;
    }
}
