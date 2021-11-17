public enum ElementState
{
    Covered,
    Uncovered,
    Marked
}

public enum ElementType
{
    SingleCovered,
    DoubleCovered,
    CantCovered
}

public enum ElementContent
{
    Number,
    Trap,
    Tool,
    Gold,
    Enemy,
    Door,
    BigWall,
    SmallWall,
    Exit
}

public enum ToolType
{
    Hp,
    Armor,
    Sword,
    Map,
    Arrow,
    Key,
    Tnt,
    Hoe,
    Grass
}

public enum GoldType
{
    One,
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven
}

public enum WeaponType
{
    None,
    Arrow,
    Sword
}

public enum EffectType
{
    None,
    SmokeEffect,
    UncoveredEffect,
    GoldEffect
}