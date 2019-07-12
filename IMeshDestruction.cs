using System.Collections.Generic;
using UnityEngine;

namespace DamageSystem
{
    public interface IMeshDestruction
    {
        void DeformMesh(Mesh mesh, Vector3[] originalMesh, Collision collision, float cos, Transform meshTransform, Quaternion rot);
        void RepairMeshData();
        List<MeshFilter> DeformableMeshFilters {get;}
        MeshDestruction.OriginalMeshVerts[] OriginalMeshVert {get;}
    }
}