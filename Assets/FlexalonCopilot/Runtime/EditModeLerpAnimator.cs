using Flexalon;
using UnityEngine;

namespace FlexalonCopilot
{
    [ExecuteAlways]
    internal class EditModeLerpAnimator : MonoBehaviour, TransformUpdater
    {
        private FlexalonNode _node;

        private AnimatedPropertyOrField _localPosition;
        private AnimatedPropertyOrField _localRotation;
        private AnimatedPropertyOrField _localScale;
        private AnimatedPropertyOrField _rectSizeX;
        private AnimatedPropertyOrField _rectSizeY;

        public static bool EnableAnimations = false;

        void OnEnable()
        {
            _node = Flexalon.Flexalon.GetOrCreateNode(gameObject);
            _node.SetTransformUpdater(this);
            hideFlags = HideFlags.HideAndDontSave | HideFlags.HideInInspector;

            _localPosition = new AnimatedPropertyOrField(typeof(Transform), "localPosition");
            _localRotation = new AnimatedPropertyOrField(typeof(Transform), "localRotation");
            _localScale = new AnimatedPropertyOrField(typeof(Transform), "localScale");
            _rectSizeX = new AnimatedPropertyOrField(typeof(RectTransform), "sizeDelta.x");
            _rectSizeY = new AnimatedPropertyOrField(typeof(RectTransform), "sizeDelta.y");
        }

        void OnDisable()
        {
            _node?.SetTransformUpdater(null);
            _node = null;
        }

        public void PreUpdate(FlexalonNode node)
        {
        }

        public bool UpdatePosition(FlexalonNode node, Vector3 position)
        {
            _localPosition.SetValue(node.GameObject.transform, position);
            return _localPosition.Done;
        }

        public bool UpdateRotation(FlexalonNode node, Quaternion rotation)
        {
            _localRotation.SetValue(node.GameObject.transform, rotation);
            return _localRotation.Done;
        }

        public bool UpdateScale(FlexalonNode node, Vector3 scale)
        {
            _localScale.SetValue(node.GameObject.transform, scale);
            return _localScale.Done;
        }

        public bool UpdateRectSize(FlexalonNode node, Vector2 size)
        {
            // This is a bit complicated because we want to avoid animating size
            // when it is set on the rectTransform directly, which may itself be animated.

            var rectTransform = node.GameObject.transform as RectTransform;
            bool done = true;

            if (node.GetSizeType(Axis.X) == SizeType.Component || node.SkipLayout)
            {
                AnimationUpdater.Instance.ForceFinish(rectTransform, "offsetMin.x");
                AnimationUpdater.Instance.ForceFinish(rectTransform, "offsetMax.x");
                AnimationUpdater.Instance.ForceFinish(rectTransform, "anchorMin.x");
                AnimationUpdater.Instance.ForceFinish(rectTransform, "anchorMax.x");
                AnimationUpdater.Instance.Remove(rectTransform, "sizeDelta.x");
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
            }
            else
            {
                _rectSizeX.SetValue(rectTransform, size.x);
                done &= _rectSizeX.Done;
            }

            if (node.GetSizeType(Axis.Y) == SizeType.Component || node.SkipLayout)
            {
                AnimationUpdater.Instance.ForceFinish(rectTransform, "offsetMin.y");
                AnimationUpdater.Instance.ForceFinish(rectTransform, "offsetMax.y");
                AnimationUpdater.Instance.ForceFinish(rectTransform, "anchorMin.y");
                AnimationUpdater.Instance.ForceFinish(rectTransform, "anchorMax.y");
                AnimationUpdater.Instance.Remove(rectTransform, "sizeDelta.y");
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
            }
            else
            {
                _rectSizeY.SetValue(rectTransform, size.y);
                done &= _rectSizeY.Done;
            }

            return done;
        }
    }
}