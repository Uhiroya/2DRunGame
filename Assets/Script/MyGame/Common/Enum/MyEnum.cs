//ステートマシンを利用することでよりスマートに書くことが出来るはず。
public enum GameFlowState
{
    None,
    Initialize,
    Title,
    GameInitialize,
    GameInitializeEnd,
    InGame,
    Pause,
    Waiting,
    Result,
}
public enum PlayerCondition
{
    None,
    Initialize,
    Alive,
    Pause,
    Dying,
    Dead,
}
public enum CollisionTag
{
    None,
    Player,
    Enemy,
    Item,
    OutField,
}
