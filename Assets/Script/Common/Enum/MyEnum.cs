[System.Serializable]
public enum GameFlowState
{
    None,
    Inisialize,
    Title,
    InGame,
    Result,
}
[System.Serializable]
public enum PlayerCondition
{
    None,
    Waiting,
    Alive,
    Dead,
}
[System.Serializable]
public enum ObstacleType
{
    None,
    Enemy,
    Item,
}