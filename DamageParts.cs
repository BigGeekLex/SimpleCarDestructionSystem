using UnityEngine;

namespace DamageSystem
{
    
        /// <summary>
        /// Damage Parts contain all info about current car part.
        /// </summary>
        public class DamageParts : MonoBehaviour, IDamageParts
        {
            [SerializeField] private float partHealth = 100.0f;
 
            [Header("Using Limit")]
            [SerializeField] private bool isLimited = false;
            [Space(1)]
            [Header("Using Motor")]
            [SerializeField] private bool isUseMotor = false;
            [Space(1)]
            [Header("Using Spring")]
            [SerializeField] private bool isUseSpring = false;
            [Space(1)]
            [Header("Anchor")]
            [SerializeField] private Vector3 anchor;
            [Space(1)]
            [Header("Axis")]
            [SerializeField] private Vector3 axis;

            private GameObject _partObj;


            [Space(1)]
            public JointLimit[] jointLimit;
            [Space(1)]
            [Header("Enable collisions")]
            [SerializeField]
            private bool isActivateCollision;
            [Space(1)]
            [Header("Break Force")]
            [SerializeField] private float breakForce;
            [Space(1)] 
            [Header("Break Torque")] [SerializeField]
            private float breakTorque;

            [Header("Connected body")] [SerializeField]
            private Rigidbody connectedBody;

            [Range(0.1f, 1.0f)] [SerializeField] private float partMult;

            private Rigidbody _partRb;

            public float PartMult
            {
                get
                {
                    return partMult;
                }
            }
            
            public float PartHealth
            {
                get { return partHealth; }
                set { partHealth = value; }
            }

            public GameObject PartObj
            {
                get { return _partObj; }
                set { _partObj = value; }
            }

            /// <summary>
            /// This function called on Awake and set hinge joint to damage part. RCC_CarController does not allow set hinge joint in inspector, this is why i use this function.
            /// </summary>
            public void SetUpHingeJoint(HingeJoint joint)
            {
                if (joint != null)
                {
                    joint.useLimits = isLimited;
                    joint.useMotor = isUseMotor; 
                    joint.connectedAnchor = gameObject.transform.position;
                    joint.autoConfigureConnectedAnchor = true;
                    joint.anchor = anchor;
                    joint.useSpring = isUseSpring;
                    joint.axis = axis;
                    joint.enableCollision = isActivateCollision;
                    joint.breakForce = breakForce;
                    joint.breakTorque = breakTorque;
                    joint.connectedBody = connectedBody;
                    
                    if (jointLimit.Length > 0)
                    {
                        JointLimit chosenJoint = jointLimit[Mathf.RoundToInt(Random.Range(0, jointLimit.Length))];
                        
                        joint.limits = new JointLimits
                        {
                            bounceMinVelocity = chosenJoint.bounceMinVelocity,
                            bounciness = chosenJoint.bounciness,
                            min = chosenJoint.min,
                            max = chosenJoint.max,
                            contactDistance = chosenJoint.contactDistance
                        };
                    }
                }
            }
            
            #region calculation-CurrentPartHealth-function
            
            public float CalculateCurrentPartHealth(Collision col, float damageFactor, float massFactor, float colVelocityMagnitude)
            {
                
                    foreach (ContactPoint contactPoint in col.contacts)
                    {
                        //if (col.gameObject.GetComponent<Bullet>() != null)
                        //{
                        //    var bullet = col.gameObject.GetComponent<Bullet>();
                        //    damageFactor *= bullet.Damage; 
                        //}
                        partHealth -= ((massFactor * colVelocityMagnitude) * damageFactor);  //Add more complex calculation
                    }
                    
                    return partHealth;
            }
            
            #endregion

            public void DetachPart(HingeJoint j)
            {
                if (j != null)
                {
                    j.breakForce = 10.0f;
                    j.breakTorque = 10.0f;
                }
            }
            
            private void Awake()
            {
                _partObj = gameObject;
                
                if (_partRb == null)
                {
                    _partRb = gameObject.AddComponent<Rigidbody>();
                }
                
                else
                {
                    _partRb = gameObject.GetComponent<Rigidbody>();
                }
            }
        }

        [System.Serializable]
    public class JointLimit
    {
        public float max;
        public float min;
        public float contactDistance;
        public float bounciness;
        public float bounceMinVelocity;
    }
    
}


