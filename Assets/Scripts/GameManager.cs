using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Match3;
using GameStates;

/// <summary>
/// The main control script of the game.
/// </summary>
public class GameManager : MonoBehaviour, IMatch3GameHandler
{    
    public static GameManager Instance { get; private set; } //Singleton instance.

    public GameObject UnitPrefab;
    public Transform MapTransform;
    public Text ScoreText;
    public TextTweener EarnedScoreText;

    [Header("Map Size")]
    public int Rows;
    public int Columns;

    [Header("Variation")]
    public int ColorsCount;

    [Header("Unit Options")]
    public float UnitSize = 50f;
    public float UnitSpeed = 200f;    
    
    private Match3Game _game;
    private IMatch3GameState _state; //Current game state.
    private UnitInfo[,] _unitsArray; //Array of links to unit management scripts.
    private Color[] _colors; //Colors for units, assigned by id.
    private UnitInfo _selectedUnit; //Selected by player current unit.        

    //Lists of units that need to be moved in FixedUpdate.
    public List<UnitInfo> UnitsToMove { get; private set; } = new List<UnitInfo>();    
    public List<UnitInfo> UnitsToDieAndReborn { get; private set; } = new List<UnitInfo>();    

    //Coordinates where changes occurred in the units, and where we need to check for new matches.
    private List<Position> _unitsPossToCheckMatches = new List<Position>();

    private void Awake()
    {
        //Singleton implementation.
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
            Destroy(gameObject);
    }

    /// <summary>
    /// Validation of the entered data.
    /// </summary>
    private void OnValidate()
    {
        if (Rows < 3)
            Rows = 3;

        if (Columns < 3)
            Columns = 3;

        if (ColorsCount < 3)
            ColorsCount = 3;

        if (UnitSize < 0)
            UnitSize = 1f;

        if (UnitSpeed < 0)
            UnitSpeed = 1f;
    }

    private void Start()
    {
        //Shift the playing field to the center of the screen.
        MapTransform.localPosition = new Vector3(Columns * -1 * UnitSize / 2f, Rows * -1 * UnitSize / 2f, 0f);

        if (_colors == null)
            SetupColors();

        InitNewGame();
    }

    /// <summary>
    /// Creating an array of random colors for units.
    /// </summary>
    private void SetupColors()
    {
        _colors = new Color[ColorsCount];

        for (int i = 0; i < ColorsCount; i++)
            _colors[i] = new Color(Random.value, Random.value, Random.value, 1.0f);
    }

    /// <summary>
    /// Creating a new Match3-game.
    /// </summary>
    private void InitNewGame()
    {        
        _game?.Dispose();
        _game = new Match3Game(Rows, Columns, ColorsCount, this);
        SetGameState(Match3GameStates.Wait);        
        ScoreText.text = "0";
        EarnedScoreText.UpdateText("", withMoving: false);
    }

    /// <summary>
    /// Accept new or updated game map after it has been built and update units according to the map.
    /// </summary>
    /// <param name="map"></param>
    public void MapUpdateHandler(int[,] map)
    {
        if (map == null) return;

        int x = map.GetLength(0);
        int y = map.GetLength(1);

        if (_unitsArray == null)
            _unitsArray = new UnitInfo[x, y];

        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                int id = map[i, j];

                if (id == -1)
                {
                    if (_unitsArray[i, j] != null)
                        Destroy(_unitsArray[i, j].gameObject);
                }
                else
                {
                    if (_unitsArray[i, j] == null)
                    {
                        GameObject go = Instantiate(Instance.UnitPrefab, Vector3.zero, Quaternion.identity, MapTransform);
                        _unitsArray[i, j] = go.GetComponent<UnitInfo>();
                        _unitsArray[i, j].InitUnit(new Position(i, j), UnitSize);
                    }

                    _unitsArray[i, j].UpdateUnitIdAndColor(id, _colors[id]);
                }
            }
        }
    }

    /// <summary>
    /// Swap of two units.
    /// </summary>
    /// <param name="units"></param>
    private void SwapTwoUnits(params UnitInfo[] units)
    {
        if (units == null || units.Length != 2) return;

        SetGameState(Match3GameStates.Swap);        

        for (int i = 0; i < 2; i++)
        {
            UnitsToMove.Add(units[i]);
            _unitsPossToCheckMatches.Add(units[i].MPos);

            //Replace links for swapped units in the common array of units.
            _unitsArray[units[i].MPos.X, units[i].MPos.Y] = units[i == 0 ? 1 : 0];
        }

        //Replace positions for swapped units.
        Position temp = units[0].MPos;
        units[0].SetNewPosition(units[1].MPos);
        units[1].SetNewPosition(temp);
    }

    /// <summary>
    /// Player's selection of units.
    /// In the case of the selection of the second unit, check the possibility of their swapping. If it leads to the match, swap selected units.
    /// </summary>
    /// <param name="ci"></param>
    public void UnitClickHandler(UnitInfo ci)
    {
        if (ci == null || !(_state is StateWait)) return;

        if (_selectedUnit == null)
        {
            _selectedUnit = ci;
            _selectedUnit.ShowShadow();
        }
        else
        {
            if (_selectedUnit != ci && _game.TrySwapUnits(_selectedUnit.MPos, ci.MPos))
                SwapTwoUnits(_selectedUnit, ci);

            _selectedUnit.HideShadow();
            _selectedUnit = null;
        }
    }

    /// <summary>
    /// Performing all unit movements, according to the current game state and formed lists of units that need to be moved (UnitsToMove, UnitsToDieAndReborn).
    /// </summary>
    private void FixedUpdate()
    {   
        _state.MoveUnits(this);        
    }

    /// <summary>
    /// After delay switch to the next game state.
    /// </summary>
    public void NextState()
    {
        _state.NextGameState(this);
    }

    /// <summary>
    /// Set new current game state.
    /// </summary>
    /// <param name="state"></param>
    internal void SetGameState(IMatch3GameState state)
    {
        _state = state;
    }

    /// <summary>
    /// Check for matches.
    /// If successful, start displaying the received changes in the units, by performing sequentially game states.
    /// </summary>
    /// <returns></returns>
    public bool CheckForMatches()
    {
        //Exit if there are no places to check new matches.
        if (_unitsPossToCheckMatches.Count == 0) return false;

        //Received changes in the units.
        List<Position> matchedUnits;
        List<PositionVector> fallingUnits;
        List<UnitData> newUnits;

        _game.DoMatchesPipeline(out matchedUnits, out fallingUnits, out newUnits, _unitsPossToCheckMatches.ToArray());

        UnitsToMove.Clear();
        _unitsPossToCheckMatches.Clear();

        //Exit if there are no new matches found.
        if (matchedUnits == null || matchedUnits.Count == 0) return false;

        //We keep the places of the matched units and units that are going to fall, to check them for new matches in the future.
        foreach (PositionVector posv in fallingUnits)
            _unitsPossToCheckMatches.Add(posv.From);
        foreach (Position pos in matchedUnits)
            _unitsPossToCheckMatches.Add(pos);

        //Falling units keep their target positions and are added to the list of units to be moved.
        foreach (PositionVector pos in fallingUnits)
        {
            UnitInfo unit = _unitsArray[pos.From.X, pos.From.Y];
            unit.SetNewPosition(pos.To);
            UnitsToMove.Add(unit);
        }

        //Matched units are highlighted and take on new positions and colors for rebirth. And they are added to the list of units to be moved (die and reborn).
        for (int i = 0; i < newUnits.Count; i++)
        {
            Position pos = matchedUnits[i];
            UnitInfo unit = _unitsArray[pos.X, pos.Y];
            unit.SetNewPosition(newUnits[i].Pos);
            unit.SetNewUnitIdAndColor(newUnits[i].Id, _colors[newUnits[i].Id]);
            unit.ShowShadow();
            UnitsToDieAndReborn.Add(unit);
        }

        //Replacing links to units according to their new positions (in which they have to be after their movements).
        foreach (UnitInfo unit in UnitsToDieAndReborn)
            _unitsArray[unit.MPos.X, unit.MPos.Y] = unit;

        foreach (UnitInfo unit in UnitsToMove)
            _unitsArray[unit.MPos.X, unit.MPos.Y] = unit;

        return true;
    }

    /// <summary>
    /// Accept updates of earned points.
    /// </summary>
    /// <param name="currentScore"></param>
    /// <param name="earnedScore"></param>
    public void ScoreUpdateHandler(int currentScore, int earnedScore)
    {
        ScoreText.text = currentScore.ToString();
        EarnedScoreText.UpdateText("+" + earnedScore);
    }

    /// <summary>
    /// Reset progress and start a new game.
    /// </summary>
    public void ResetGame()
    {
        InitNewGame();
    }

    /// <summary>
    /// Creating a new array of random colors and applying it for units.
    /// </summary>
    public void ResetColors()
    {
        SetupColors();

        foreach (UnitInfo unit in _unitsArray)
            unit.UpdateUnitColor(_colors[unit.Id]);
    }
}
    
