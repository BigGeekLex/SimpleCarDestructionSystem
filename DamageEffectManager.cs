using UnityEngine;

namespace DamageSystem
{
    public enum EffectsType
    {
        Smoke = 0,
        Sparks = 1,
        Explosion = 2
    }

    internal class DamageEffectManager : MonoBehaviour, IDamageEffectManager
    {
        //Motor dependent variables
        public ParticleSystem motorSmokeEffect;
        //public ParticleSystem motorFireEffect;
        public ParticleSystem explosionEffect;
        //Sparks for damage parts
        public ParticleSystem sparksEffect;

        public void ActivateEffect(Vector3 pos, Quaternion rot, EffectsType type)
        {
            switch (type)
            {
                case EffectsType.Smoke:
                    if (motorSmokeEffect != null)
                    {
                        motorSmokeEffect.gameObject.transform.position = pos;
                        motorSmokeEffect.gameObject.SetActive(true);
                    }
                    break;
                case EffectsType.Sparks:
                    if (sparksEffect != null)
                    {
                        sparksEffect.gameObject.transform.position = pos;
                        sparksEffect.gameObject.SetActive(true);   
                    }
                    break;
                case EffectsType.Explosion:
                    
                    if (explosionEffect != null)
                    {
                        explosionEffect.gameObject.transform.position = pos;
                        explosionEffect.gameObject.SetActive(true);   
                    }
                    break;
            }
        }

        public void SpawnEffect()
        {
            var motorEfObj = Instantiate(motorSmokeEffect, Vector3.zero, Quaternion.identity);
            var explositionEfObj = Instantiate(explosionEffect, Vector3.zero, Quaternion.identity);
            var sparksEfObj = Instantiate(sparksEffect, Vector3.zero, Quaternion.identity);

            motorSmokeEffect = motorEfObj;
            explosionEffect = explositionEfObj;
            sparksEffect = sparksEfObj;
                
            motorEfObj.gameObject.SetActive(false);
            explositionEfObj.gameObject.SetActive(false);
            sparksEfObj.gameObject.SetActive(false);
        }

        public void Start()
        {
            SpawnEffect();
        }
    }
}