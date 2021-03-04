namespace Lus.TextClient
{
    /// <summary>
    /// Identifier of a game screen.
    /// A game screen is a game state.
    /// </summary>
    internal enum GameScreen
    {
        /// <summary>
        /// Undefined screen may be if error in the code.
        /// </summary>
        Undefinded,

        /// <summary>
        /// Globe settings and generation screen.
        /// </summary>
        GlobeGeneration,

        /// <summary>
        /// Global fraction management screen.
        /// </summary>
        Globe,

        /// <summary>
        /// Battle screen.
        /// </summary>
        Battle,
    }
}
