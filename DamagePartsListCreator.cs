using System.Collections.Generic;
using UnityEngine;

namespace DamageSystem
{
    public class DamagePartsListCreator : MonoBehaviour, IDamagePartsListCreator
    {
        public void SetParts(List<DamageParts> parts)
        {
            DamageParts[] p = gameObject.GetComponentsInChildren<DamageParts>();
            //parts = new List<DamageParts>(p.Length);
            foreach (var pt in p)
            {
                parts.Add(pt);
                HingeJoint j = pt.PartObj.AddComponent<HingeJoint>();
                var mesh = pt.PartObj.gameObject.AddComponent<MeshCollider>();
                mesh.convex = true;
                pt.SetUpHingeJoint(j);
            }
        }
    }
}