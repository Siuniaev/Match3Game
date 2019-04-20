using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Control script for tween Text-gameobjects.
/// </summary>
public class TextTweener : MonoBehaviour {

    public RectTransform RTrans;
    public Text Text;
    public float Speed;
    
    private Color _earlyColor;
    private Color _fadedColor;
    private bool _isMoving;
    private Vector3 _earlyPos;
    private Vector3 _targetPos;

    /// <summary>
    /// Save the original parameters for the start of the tweens.
    /// </summary>
    private void Awake()
    {        
        _earlyColor = Text.color;
        _fadedColor = new Color(_earlyColor.r, _earlyColor.g, _earlyColor.b, 0f);
        _earlyPos = RTrans.localPosition;
        _targetPos = new Vector3(_earlyPos.x, _earlyPos.y + 50f, 1f);
    }

    /// <summary>
    /// Set a new text and optionally animate it.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="withMoving"></param>
    public void UpdateText(string text, bool withMoving = true)
    {
        Text.text = text;

        //Set the starting position and color.
        if (withMoving)
        {            
            RTrans.localPosition = _earlyPos;
            Text.color = _earlyColor;            
            _isMoving = true;
        }
    }    

    private void FixedUpdate()
    {
        if (!_isMoving) return;

        //Change the text color and move it to a given position.
        if (RTrans.localPosition != _targetPos)
        {            
            RTrans.localPosition = Vector3.MoveTowards(RTrans.localPosition, _targetPos, Time.deltaTime * Speed * 25);
            Text.color = Color.Lerp(Text.color, _fadedColor, Time.deltaTime * Speed * 2.5f);
        }
        else
            _isMoving = false;
    }
}
