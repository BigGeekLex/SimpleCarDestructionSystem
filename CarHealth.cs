using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DamageSystem
{
   
        /// <summary>
        ///  Health script provide all necessary calculations.
        /// </summary>
        internal class CarHealth : MonoBehaviour, IHealth
        {
            [Header("Current car health")]
            [SerializeField]
            private float mainCarHealth = 100.0f; //Main Car health
            [Space(1)]
            
            [Header("Motor Health")]
            [SerializeField] private float motorHealth = 100.0f;
            
            [Header("Motor Factor")]
            [SerializeField] [Range(0.1f, 0.85f)] private float motorFactor;
            private float _maxFactor = 1.0f;
            private float _partFactor;
            
            public Transform motorTr; //motor position
            
            public bool repairNow = false;											
            public bool repaired = true;
            
            [Header("Parts health")]
            [SerializeField]
            private List<float> damageHealthParams; //List of damage parts health parametr
            
            public float Health
            {
                get { return mainCarHealth; }
            }
            
            private IMeshDestruction _meshDestruction;
            private IDamageEffectManager _effectManager;
            public List<float> PartsHealth
            {
                get { return damageHealthParams; }
                set { damageHealthParams = value; }
            }
            
            public void CreateHealthParamList(List<DamageParts> damageParts)
            {
                //var iterator = 0;
                if (damageParts.Count > 1)
                {
                    damageHealthParams = new List<float>(damageParts.Count);
                    damageHealthParams.AddRange(damageParts.Select(i => i.PartHealth));
                }
            }

            private void Start()
            {
                _meshDestruction = GetComponent<IMeshDestruction>();
                _effectManager = GetComponent<IDamageEffectManager>();
                
                _partFactor = _maxFactor - motorFactor;
            }
            
            #region calculation-health-functions
           
            public float CalculateHealth(List<DamageParts> damageParts, List<float> damageHealthParam) 
            {
                var partHealth = 0.0f;
                
                for (int i = 0; i < damageParts.Count; i++)
                {
                    for (int j = 0; j < damageHealthParam.Count; j++)
                    {
                        if (i == j)
                        {
                            partHealth += damageHealthParam[j] * damageParts[i].PartMult;
                            
                            if (damageParts.Count - 1 == i) //Calculate mainHealth only if damageParts list ended
                            {
                                mainCarHealth = motorHealth * motorFactor + (partHealth / damageParts.Count) * _partFactor;

                                if (mainCarHealth <= 0.0f)
                                {
                                    Die();
                                }
                            }
                        }
                    } 
                }
                return mainCarHealth;
            }
            
            public IEnumerator UpdateHealthInfo(List<DamageParts> damageParts, List<float> damageHealthParam)
            {
                for (int i = 0; i < damageParts.Count; i++)
                {
                    var damagePartCurrent = damageParts[i].PartHealth;
                    
                    for (int j = 0; j < damageHealthParams.Count; j++)
                    {
                        var damageHealthParamCurrent = damageHealthParams[j];
                        
                        if (damagePartCurrent < damageHealthParamCurrent) //If any changes register between two param we called CalculateHealth function 
                        {
                            if (i == j)
                            {
                                damageHealthParams[j] = damagePartCurrent;

                                if (damagePartCurrent <= 0.0f)
                                {
                                    damageHealthParams[j] = 0.0f;
                                    
                                    damageParts[i].DetachPart(damageParts[i].gameObject.GetComponent<HingeJoint>());
                                }
                                CalculateHealth(damageParts, damageHealthParam);
                            }
                        }
                    }
                }
                return new WaitForSecondsRealtime(0.2f);
            }
            
            #endregion
           
            public void Die()
            {
                gameObject.SetActive(false);
                StartCoroutine(DieTimer());
                //Add more complex effects system
                _effectManager.ActivateEffect(gameObject.transform.position, Quaternion.identity,
                    EffectsType.Explosion);
                //Calling respawn function here
            }
            
            public float CalculateMotorHealth(float colMag, float massFactor, float clampedColMag, ContactPoint p)
            {
                motorHealth -= colMag * massFactor *
                               Mathf.Min(clampedColMag * 0.05f, (clampedColMag * 0.005f) /
                                                                Mathf.Pow(Vector3.Distance(motorTr.position, p.point),
                                                                    clampedColMag));
                if (motorHealth < 25.0f)
                {
                    //Activate motor smoke effect
                }
                return motorHealth;
            }
            
            //Could calling from bonus gameObject, for example
            public void Repair(bool repaired, bool repairNow)
            {
                if (!repaired && repairNow)
                {
                    motorHealth = 100.0f;
                    repaired = true;
                    
                    //RepairMeshData
                    _meshDestruction.RepairMeshData();
                    
                    if (repaired)
                        repairNow = false;
                }
            }

            private IEnumerator DieTimer()
            {
                yield return new WaitForSeconds(2);
            } 
        }
}

