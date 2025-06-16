//ENUM 값이 음수면 테이블에서 Parsing하지 않는 타입
public enum Status
{
    NONE,
    ATTACK,
    COOLDOWN,
    PROJECTILE_NUM,
    SPEED,
    MAX_HP,
    RANGE,
    THROUGH,
    PROJECTILE_SPEED,
    SKILL_DURATION,
    AVOID,
    RESURRECTION,
    BLOOD_ABSORBING,
    EXP,
    MONEY,
    CURRENT_HP = -1,
    LEVEL = -2,
    NEXT_EXP = -3
}