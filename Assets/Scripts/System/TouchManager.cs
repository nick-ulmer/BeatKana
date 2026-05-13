using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;


// Alias EnhancedTouch.Touch to "Touch" for less typing.
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;


public class TouchManager : MonoBehaviour
{
    //static Dictionary<string, int> touchIdKey = new Dictionary<string, int>();
    static BiDictionary<string, int> touchIdKey = new BiDictionary<string, int>();

    public delegate void TouchAction(Touch touch);
    public static event TouchAction TouchBegan;
    public static event TouchAction TouchEnded;
    public static event TouchAction TouchMoved;
    public static event TouchAction TouchCanceled;
    public static event TouchAction TouchStationary;


    void Start()
    {
        EnhancedTouchSupport.Enable();
        //TouchSimulation.Enable(); // Unnecessary even in editor mode. Caused a bug that forced you to "drag" in order to activate the actual menu. 
    }

    public static TouchManager instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }


    void Update()
    {
        //return;
        // Illustrates how to examine all active touches once per frame and show their last recorded position
        // in the associated screen-space.
        foreach (var touch in Touch.activeTouches)
        {
            string key;
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    TouchBegan?.Invoke(touch);
                    break;
                case TouchPhase.Ended:
                    TouchEnded?.Invoke(touch);
                    if (touchIdKey.TryGetKey(touch.touchId, out key)) { /*Debug.Log(key + " button ended.");*/ touchIdKey.RemoveByValue(touch.touchId); }
                    break;
                case TouchPhase.Moved:
                    TouchMoved?.Invoke(touch);
                    break;
                case TouchPhase.Canceled:
                    TouchCanceled?.Invoke(touch);
                    if (touchIdKey.TryGetKey(touch.touchId, out key)) { /*Debug.Log(key + " button ended.");*/ touchIdKey.RemoveByValue(touch.touchId); }
                    break;
                case TouchPhase.Stationary:
                    TouchStationary?.Invoke(touch);
                    break;
            }
        }
    }

    public static bool GetTouch(string key, out Touch touch)
    {
        touch = default;
        if (touchIdKey.TryGetValue(key, out int touchId))
        {
            return GetTouchByTouchId(touchId, out touch);
        }
        return false;
    }

    public static bool IsPressed(string key)
    {
        return touchIdKey.ContainsKey(key);
    }

    public static bool GetTouchByTouchId(int id, out Touch result_touch)
    {
        result_touch = default;
        foreach (var touch in Touch.activeTouches)
        {
            if (touch.touchId == id) 
            {
                result_touch = touch;
                return true; 
            }
        }
        return false;
    }
    public static bool Claim(string key, int id)
    {
        if (!touchIdKey.ContainsKey(key))
        {
            touchIdKey.Add(key, id);
            return true;
        }
        else
        {
            return false;
        }
    }
}
