using UnityEngine;

public enum ElementalAttribute
{
    None,
    Lightning,
    Fire,
    Water
}

[CreateAssetMenu(fileName = "MonsterDataSO", menuName = "Scriptable Objects/MonsterDataSO")]
public class MonsterDataSO : ScriptableObject
{
    public int monsterHP;
    public int monsterAttackPower;
    public float monsterSpeed;
    public ElementalAttribute weakElementalAttribute;
    public ElementalAttribute strengthElementalAttribute;
    public bool isBoss;
    
    

}
