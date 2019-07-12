using UnityEngine;

namespace DamageSystem
{
    public interface IDamageSystem
    {
        void TakeDamage(Collision col, bool useDamage);
    }
}