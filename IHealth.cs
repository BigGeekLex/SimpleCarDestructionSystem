using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamageSystem
{
        public interface IHealth
        {
            void Repair(bool isRepaired, bool isRepairingNow);
            void Die();
            void CreateHealthParamList(List<DamageParts> damageParts);
            float CalculateMotorHealth(float colMag, float massFactor, float clampedColMag, ContactPoint p);
            IEnumerator UpdateHealthInfo(List<DamageParts> damageParts, List<float> healhList );
            List<float> PartsHealth{get; set;}
        }   
    
}