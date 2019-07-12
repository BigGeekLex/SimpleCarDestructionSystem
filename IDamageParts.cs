using UnityEngine;

namespace DamageSystem
{
    public interface IDamageParts
    {
            float CalculateCurrentPartHealth(Collision col, float damageFactor, float massFactor, float colVelocityMagnitude);
            void DetachPart(HingeJoint j);
            GameObject PartObj { get; set; } //Reference to current GameObject
            float PartMult{get;}
            float PartHealth{get; set;}
            void SetUpHingeJoint(HingeJoint joint);
    }
}