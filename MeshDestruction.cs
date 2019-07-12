using System.Collections.Generic;
using UnityEngine;

namespace DamageSystem
{
    public class MeshDestruction : MonoBehaviour, IMeshDestruction
    {
        #region variables
        public struct OriginalMeshVerts
        {
            public Vector3[] meshVerts;
        }

        private OriginalMeshVerts[] _originalMeshData;// Array for struct above.
        public List<MeshFilter> deformableMeshFilters; // Deformable Meshes.
        public float randomizeVertices = 1f; // Randomize Verticies on Collisions for more complex deforms.
        public float damageRadius = .5f;	// Verticies in this radius will be effected on collisions.
        private float minimumVertDistanceForDamagedMesh = .002f;
        public float maximumDamage = .5f; // Maximum Vert Distance for Limiting Damage. 0 Value Will Disable The Limit.
        private RCC_CarControllerV3 _carController;

        public List<MeshFilter> DeformableMeshFilters => deformableMeshFilters;

        public OriginalMeshVerts[] OriginalMeshVert => _originalMeshData;

        #endregion
        public void DeformMesh(Mesh mesh, Vector3[] originalMesh, Collision collision, float cos, Transform meshTransform, Quaternion rot){
		
            Vector3[] vertices = mesh.vertices;
		
            foreach (ContactPoint contact in collision.contacts){
			
                Vector3 point = meshTransform.InverseTransformPoint(contact.point);
			
                for (int i = 0; i < vertices.Length; i++){

                    if ((point - vertices[i]).magnitude < damageRadius){
					
                        vertices[i] += rot * ((_carController.localVector * (damageRadius - (point - vertices[i]).magnitude) / 
                                               damageRadius) * cos + (new Vector3(Mathf.Sin(vertices[i].y * 1000), 
                                                                          Mathf.Sin(vertices[i].z * 1000), 
                                                                          Mathf.Sin(vertices[i].x * 100)).normalized * (randomizeVertices / 500f)));
					
                        if (maximumDamage > 0 && ((vertices[i] - originalMesh[i]).magnitude) > maximumDamage){
                            vertices[i] = originalMesh[i] + (vertices[i] - originalMesh[i]).normalized * (maximumDamage);
                        }
                    }
                }
            }
		
            mesh.vertices = vertices;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
        }
            
        void LoadOriginalMeshData(){

            _originalMeshData = new OriginalMeshVerts[deformableMeshFilters.Count];

            for (int i = 0; i < deformableMeshFilters.Count; i++)
                _originalMeshData[i].meshVerts = deformableMeshFilters[i].mesh.vertices;

        }

        public void RepairMeshData()
        {
            int k;
            for (k = 0; k < deformableMeshFilters.Count; k++)
            {
                Vector3[] vertices = deformableMeshFilters[k].mesh.vertices;
                if (_originalMeshData == null)
                    LoadOriginalMeshData();

                for (int i = 0; i < vertices.Length; i++)
                {
                    vertices[i] += (_originalMeshData[k].meshVerts[i] - vertices[i]) * (Time.deltaTime * 2f);
                    if ((_originalMeshData[k].meshVerts[i] - vertices[i]).magnitude >=
                        minimumVertDistanceForDamagedMesh) ;
                }

                deformableMeshFilters[k].mesh.vertices = vertices;
                deformableMeshFilters[k].mesh.RecalculateNormals();
                deformableMeshFilters[k].mesh.RecalculateBounds();
            }
        }

        private void Awake()
        {
            //Load mesh data
            _carController = GetComponent<RCC_CarControllerV3>();
            //LoadOriginalMeshData();
            DamageMeshInitialize();
        }
        
        void DamageMeshInitialize ()
        {
            if (deformableMeshFilters.Count == 0 || deformableMeshFilters == null){

                MeshFilter[] allMeshFilters = GetComponentsInChildren<MeshFilter>();
                List <MeshFilter> properMeshFilters = new List<MeshFilter>();
                foreach(MeshFilter mf in allMeshFilters){
				
                    if(!mf.transform.IsChildOf(_carController.FrontLeftWheelTransform) && !mf.transform.IsChildOf(_carController.FrontRightWheelTransform) && 
                       !mf.transform.IsChildOf(_carController.RearLeftWheelTransform) && !mf.transform.IsChildOf(_carController.RearRightWheelTransform))
                        properMeshFilters.Add(mf);
                }
                deformableMeshFilters = properMeshFilters;
            }
            LoadOriginalMeshData();
        }
    }
}