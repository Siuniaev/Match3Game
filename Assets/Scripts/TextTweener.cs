using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Control script for tween Text-gameobjects.
/// </summary>
public class TextTweener : MonoBehaviour {

    public RectTransform RTrans;
    public Text Text;
    public float Speed;
    
    private Color EarlyColor { get; set; }
    private Color FadedColor { get; set; }
    private bool IsMoving { get; set; }
    private Vector3 EarlyPos { get; set; }
    private Vector3 TargetPos { get; set; }

    /// <summary>
    /// Save the original parameters for the start of the tweens.
    /// </summary>
    private void Awake()
    {        
        EarlyColor = Text.color;
        FadedColor = new Color(EarlyColor.r, EarlyColor.g, EarlyColor.b, 0f);
        EarlyPos = RTrans.localPosition;
        TargetPos = new Vector3(EarlyPos.x, EarlyPos.y + 50f, 1f);
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
            RTrans.localPosition = EarlyPos;
            Text.color = EarlyColor;            
            IsMoving = true;
        }
    }    

    private void FixedUpdate()
    {
        if (!IsMoving) return;

        //Change the text color and move it to a given position.
        if (RTrans.localPosition != TargetPos)
        {            
            RTrans.localPosition = Vector3.MoveTowards(RTrans.localPosition, TargetPos, Time.deltaTime * Speed * 25);
            Text.color = Color.Lerp(Text.color, FadedColor, Time.deltaTime * Speed * 2.5f);
        }
        else
            IsMoving = false;
    }
}
