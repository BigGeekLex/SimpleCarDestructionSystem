using UnityEngine;

namespace DamageSystem
{
    public interface IDamageEffectManager
    {
        void ActivateEffect(Vector3 pos,Quaternion rot, EffectsType type);
    }
}