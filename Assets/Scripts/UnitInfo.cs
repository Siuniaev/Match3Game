using UnityEngine;
using UnityEngine.UI;
using Match3Types;

/// <summary>
/// Control script for unit's gameobject.
/// </summary>
public class UnitInfo : MonoBehaviour {

    public RectTransform RTrans;
    public Image ImgShadow;
    public Image Img;
    public Button Btn;
    public Position MPos;

    public int Id { get; private set; }
    private float Size { get; set; }
    private Vector3 TargetPos { get; set; }
    private Color TargetColor { get; set; }
    private int TargetId { get; set; }
    private Vector3 diedScale = new Vector3(0.1f, 0.1f, 1f);
    private Vector3 bornScale = new Vector3(1f, 1f, 1f);


    private void Start()
    {
        HideShadow();
        Btn.onClick.AddListener(() => GameManager.Instance.UnitClickHandler(this));
    }

    /// <summary>
    /// Initial object setup
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="size"></param>
    public void InitUnit(Position pos, float size)
    {
        if (size <= 0)
            throw new System.ArgumentException(string.Format("SetupUnit Error: size = {0} ; value must be greater than or equal to 0.", size));

        Size = size;
        RTrans.localPosition = new Vector3(pos.X * size, pos.Y * size, 0f);

        RTrans.sizeDelta = new Vector2(size, size);
        Img.GetComponent<RectTransform>().sizeDelta = new Vector2(size * 0.9f, size * 0.9f); 

        MPos = pos;
    }

    /// <summary>
    /// Set new unit Id and color.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="color"></param>
    public void UpdateUnitIdAndColor(int id, Color color)
    {
        Id = id;
        Img.color = color;
    }

    /// <summary>
    /// Set new unit color.
    /// </summary>
    /// <param name="color"></param>
    public void UpdateUnitColor(Color color)
    {        
        Img.color = color;
    }

    /// <summary>
    /// Disable unit highlight.
    /// </summary>
    public void HideShadow()
    {
        ImgShadow.color = Color.clear;
    }

    /// <summary>
    /// Enable unit highlight.
    /// </summary>
    public void ShowShadow()
    {
        ImgShadow.color = Color.red;
    }

    /// <summary>
    /// Set a new position for the unit to which it will move.
    /// </summary>
    /// <param name="pos"></param>
    public void SetNewPosition(Position pos)
    {
        TargetPos = new Vector3(pos.X * Size, pos.Y * Size, 0f);
        MPos = pos;
    }

    /// <summary>
    /// Move a unit to its target position.
    /// </summary>
    /// <param name="speed"></param>
    /// <returns></returns>
    private bool Move(float speed)
    {
        if (RTrans.localPosition != TargetPos)
        {
            RTrans.localPosition = Vector3.MoveTowards(RTrans.localPosition, TargetPos, Time.deltaTime * speed);
            return true;
        }
        else                    
            return false;        
    }

    /// <summary>
    /// Reduce the size of the unit and turn off the highlight and visibility when size reaches the specified minimum.
    /// </summary>
    /// <param name="speed"></param>
    /// <returns></returns>
    private bool Die(float speed)
    {
        if (RTrans.localScale != diedScale)
        {
            RTrans.localScale = Vector3.MoveTowards(RTrans.localScale, diedScale, Time.deltaTime * speed * 0.04f);
            return true;
        }
        else
        {
            HideShadow();
            ChangeUnitVisibility(show: false);            
            return false;
        }
    }

    /// <summary>
    /// Increase the size of the unit until it reaches the specified size.
    /// </summary>
    /// <param name="speed"></param>
    /// <returns></returns>
    private bool Born(float speed)
    {
        if (RTrans.localScale != bornScale)
        {
            RTrans.localScale = Vector3.MoveTowards(RTrans.localScale, bornScale, Time.deltaTime * speed * 0.04f);
            return true;
        }
        else
            return false;
    }

    /// <summary>
    /// Move a unit to its specified position.
    /// </summary>
    public void Reborn()
    {        
        RTrans.localPosition = new Vector3(MPos.X * Size, MPos.Y * Size, 0f);        
    }

    /// <summary>
    /// Select and perform unit action at a given speed according to the game state.
    /// </summary>
    /// <param name="state"></param>
    /// <param name="speed"></param>
    /// <returns></returns>
    public bool DoAction(GameState state, float speed)
    {
        bool ok = false;

        switch (state)
        {
            case GameState.Swap: ok = Move(speed); break;
            case GameState.Fall: ok = Move(speed * 2f); break;
            case GameState.Burn: ok = Die(speed); break;
            case GameState.Fill: ok = Born(speed); break;
        }

        return ok;
    }    

    /// <summary>
    /// Hide or show unit.
    /// </summary>
    /// <param name="show"></param>
    private void ChangeUnitVisibility(bool show)
    {
        Img.enabled = show;
        ImgShadow.enabled = show;        
    }

    /// <summary>
    /// Show unit with updated id and color.
    /// </summary>
    public void ShowUnit()
    {
        UpdateUnitIdAndColor();
        ChangeUnitVisibility(show: true);
    }

    /// <summary>
    /// Set a new target Id and target color for unit, which will be assigned when it is shown again.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="color"></param>
    public void SetNewUnitIdAndColor(int id, Color color)
    {
        TargetId = id;
        TargetColor = color;
    }

    /// <summary>
    /// Assign target Id and target color to unit.
    /// </summary>
    private void UpdateUnitIdAndColor()
    {
        Id = TargetId;
        Img.color = TargetColor;
    }
}
