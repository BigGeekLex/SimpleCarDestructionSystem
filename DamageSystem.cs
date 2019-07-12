using System.Collections.Generic;
using UnityEngine;

namespace DamageSystem
{
    
        public class DamageSystem : MonoBehaviour, IDamageSystem
        {
            #region variables
            
            private IHealth _health;
            private IDamagePartsListCreator _damagePartCreator;
            private IDamageEffectManager _damageEffectManager;
            private IMeshDestruction _meshDestruction;

            private RCC_CarControllerV3 _controllerV3;
            
            [SerializeField]private List<DamageParts> damageParts;
            
            [Header("Damage Factor")]
            [Range(0.1f, 5.0f)] [SerializeField] private float damageFactor;

            [SerializeField] private float maxCollisionMagnitude = 100.0f;
            // Damage.
            public bool useDamage = true;
            
            #endregion
            
            private void Start()
            {
                _health = GetComponent<IHealth>();
                 damageParts = new List<DamageParts>();
                _meshDestruction = GetComponent<IMeshDestruction>();
                _damagePartCreator = GetComponent<IDamagePartsListCreator>();
                _damageEffectManager = GetComponent<IDamageEffectManager>();

                _damagePartCreator.SetParts(damageParts);
                _health.CreateHealthParamList(damageParts);
            }
            
            public void TakeDamage(Collision col, bool isUseDamage)
            {
                if (col.relativeVelocity.magnitude > 10.0f && isUseDamage)
                {
                    Vector3 dirOfCollision = transform.position - col.contacts[0].point;
                    dirOfCollision = -dirOfCollision.normalized;
                    var massFactor = 1.0f;
                    var rb = gameObject.GetComponent<Rigidbody>();
                    
                    float cos = Mathf.Abs (Vector3.Dot (col.contacts [0].normal, col.relativeVelocity.normalized));
                
                    float colMag = Mathf.Min(col.relativeVelocity.magnitude, maxCollisionMagnitude);
                    float clampedColMag = Mathf.Pow(Mathf.Sqrt(colMag) * 0.5f, 1.5f);
                
                    if (col.collider.attachedRigidbody)
                    {
                       massFactor = Mathf.Clamp01(col.contacts[0].otherCollider.attachedRigidbody.mass / rb.mass);
                    }

                    //Car mesh destruction here
                    for (int i = 0; i < _meshDestruction.DeformableMeshFilters.Count; i++)
                    {
                            _meshDestruction.DeformMesh(_meshDestruction.DeformableMeshFilters[i].mesh,
                            _meshDestruction.OriginalMeshVert[i].meshVerts, col, cos,
                            _meshDestruction.DeformableMeshFilters[i].transform, Quaternion.identity);
                    }

                    foreach (ContactPoint points in col.contacts)
                    {
                        Vector3 contactPoint = new Vector3(points.point.x, points.point.y,  points.point.z);
                        //Calculate motor health here
                        _health.CalculateMotorHealth(colMag, massFactor, clampedColMag, points);
                        //Activate sparks effects here
                        _damageEffectManager.ActivateEffect(contactPoint, Quaternion.identity, EffectsType.Sparks);
                        //Calculate Transmission health here
                    }
                    
                    foreach (var part in damageParts)
                    {
                        part.CalculateCurrentPartHealth(col, damageFactor, massFactor, colMag);
                    }
                }
            }
            private void OnCollisionEnter(Collision col)
            {
                TakeDamage(col, useDamage);
            }
            private void Update()
            {
                if (_health != null)
                {
                    StartCoroutine(_health.UpdateHealthInfo(damageParts, _health.PartsHealth));
                }
            }
        }
}

