using System;
using System.Collections.Generic;
using Flexalon;
using UnityEditor;
using UnityEngine;

namespace FlexalonCopilot
{
    internal class AnimatedPropertyOrField
    {
        public static bool EnableAnimations = false;

        private PropertyOrField _prop;

        private UnityEngine.Object _obj;
        public UnityEngine.Object Obj => _obj;

        private object _value;

        private bool _done = true;
        public bool Done => _done;

        private string _name;
        public string Name => _name;

        public Type Type => _prop.Type;

        private float _time;

        public bool Valid => _prop.Valid;

        private static readonly List<Type> _doNotAnimate = new List<Type>()
        {
            typeof(FlexalonObject),
            typeof(FlexalonFlexibleLayout),
            typeof(FlexalonGridLayout)
        };

        private static readonly List<Type> _animatedTypes = new List<Type>()
        {
            typeof(float),
            typeof(Vector2),
            typeof(Vector3),
            typeof(Quaternion),
            typeof(Color),
            typeof(int),
            typeof(string),
        };

        public AnimatedPropertyOrField(Type type, string name)
        {
            _name = name;
            _prop = new PropertyOrField(type, name);
        }

        public bool SetValue(UnityEngine.Object obj, object value)
        {
            _obj = obj;
            _value = value;

            AnimationUpdater.Instance.Remove(_obj, Name);

            var current = _prop.GetValue(obj);
            if ((value == null && current == null) || (value != null && value.Equals(current)))
            {
                return false;
            }

            if (!EnableAnimations)
            {
                Finish();
                return true;
            }

            if (_obj is LayoutBase layout)
            {
                EnsureHasAnimator(layout.gameObject);
                foreach (Transform child in layout.transform)
                {
                    EnsureHasAnimator(child.gameObject);
                }
            }
            else if (_obj is FlexalonObject fxobj)
            {
                EnsureHasAnimator(fxobj.gameObject);
            }

            if (_doNotAnimate.Contains((_prop.DeclaringType)) || !_animatedTypes.Contains(_prop.Type))
            {
                Finish();
                return true;
            }

            _done = false;
            AnimationUpdater.Instance.Register(this);
            return true;
        }

        private void EnsureHasAnimator(GameObject go)
        {
            if (go.GetComponent<EditModeLerpAnimator>() == null)
            {
                Flexalon.Flexalon.AddComponent<EditModeLerpAnimator>(go);
            }
        }

        public void Update(float deltaTime)
        {
            if (_obj is UnityEngine.Object uobj && !uobj)
            {
                _done = true;
                return;
            }

            if (_prop.Type == typeof(float))
            {
                var currentValue = (float)_prop.GetValue(_obj);
                var targetValue = (float)_value;

                if (Mathf.Abs(currentValue - targetValue) < 0.001f)
                {
                    Finish();
                    return;
                }

                var newValue = Mathf.Lerp(currentValue, targetValue, deltaTime * 10f);
                _prop.SetValue(_obj, newValue);
            }
            else if (_prop.Type == typeof(Vector2))
            {
                var currentValue = (Vector2)_prop.GetValue(_obj);
                var targetValue = (Vector2)_value;

                if (Vector2.Distance(currentValue, targetValue) < 0.001f)
                {
                    Finish();
                    return;
                }

                var newValue = Vector2.Lerp(currentValue, targetValue, deltaTime * 10f);
                _prop.SetValue(_obj, newValue);
            }
            else if (_prop.Type == typeof(Vector3))
            {
                var currentValue = (Vector3)_prop.GetValue(_obj);
                var targetValue = (Vector3)_value;

                if (Vector3.Distance(currentValue, targetValue) < 0.001f)
                {
                    Finish();
                    return;
                }

                var newValue = Vector3.Lerp(currentValue, targetValue, deltaTime * 10f);
                _prop.SetValue(_obj, newValue);
            }
            else if (_prop.Type == typeof(Quaternion))
            {
                var currentValue = (Quaternion)_prop.GetValue(_obj);
                var targetValue = (Quaternion)_value;

                if (Quaternion.Angle(currentValue, targetValue) < 0.001f)
                {
                    Finish();
                    return;
                }

                var newValue = Quaternion.Lerp(currentValue, targetValue, deltaTime * 10f);
                _prop.SetValue(_obj, newValue);
            }
            else if (_prop.Type == typeof(Color))
            {
                var currentValue = (Color)_prop.GetValue(_obj);
                var targetValue = (Color)_value;

                if (Vector4.Distance(currentValue, targetValue) < 0.001f)
                {
                    Finish();
                    return;
                }

                var newValue = Color.Lerp(currentValue, targetValue, deltaTime * 10f);
                _prop.SetValue(_obj, newValue);
            }
            else if (_prop.Type == typeof(int))
            {
                var currentValue = (int)_prop.GetValue(_obj);
                var targetValue = (int)_value;

                var newValue = Mathf.RoundToInt(Mathf.Lerp(currentValue, targetValue, deltaTime * 10f));
                if (newValue == targetValue)
                {
                    Finish();
                }
                else
                {
                    _prop.SetValue(_obj, newValue);
                }
            }
            else if (_prop.Type == typeof(string))
            {
                var currentValue = (string)_prop.GetValue(_obj);
                var targetValue = (string)_value;

                _time += deltaTime;
                if (_time < 0.02f)
                {
                    return;
                }

                _time -= 0.02f;

                for (int i = 1; i <= targetValue.Length; i++)
                {
                    if (currentValue.Length < i || currentValue[i - 1] != targetValue[i - 1])
                    {
                        currentValue = targetValue.Substring(0, i);
                        break;
                    }
                }

                if (currentValue == targetValue)
                {
                    Finish();
                }
                else
                {
                    _prop.SetValue(_obj, currentValue);
                }
            }
            else
            {
                throw new Exception("Unexpected type: " + _prop.Type);
            }
        }

        private void Finish()
        {
            #if UNITY_EDITOR
                Undo.RecordObject(_obj, "Set Property");
            #endif

            _prop.SetValue(_obj, _value);

            #if UNITY_EDITOR
                PrefabUtility.RecordPrefabInstancePropertyModifications(_obj);
                EditorUtility.SetDirty(_obj);
            #endif

            _done = true;
        }
    }
}