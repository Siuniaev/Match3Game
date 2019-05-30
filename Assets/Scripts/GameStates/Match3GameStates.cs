namespace GameStates
{
    /// <summary>
    /// Pre-created game states.
    /// </summary>
    public static class Match3GameStates
    {
        public static StateWait Wait = new StateWait();
        public static StateSwap Swap = new StateSwap();
        public static StateShowMatch ShowMatch = new StateShowMatch();
        public static StateBurn Burn = new StateBurn();
        public static StateFall Fall = new StateFall();
        public static StateFill Fill = new StateFill();
    }
}
