using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pixelplacement.XRTools
{
    public class RoomMapperBuildGeometry : RoomMapperPhase
    {
        //Public Variables:
        public LineRenderer wireframe;
        
        //Private Variables:
        private Vector3 _ceilingCenter;
        private float _windingDirection;

        //Startup:
        protected override void Awake()
        {
            base.Awake();
            wireframe.transform.parent = RoomAnchor.Instance.transform;
        }

        private void OnEnable()
        {
            //sets:
            wireframe.positionCount = 0;
            
            //calls:
            SetCeilingCenter();
            SetWindingDirection();
            BuildWalls();
            BuildHorizontalSurfaces();
            
            //calls:
            RoomMapper.Instance.HideGeometry();
            
            //activate:
            wireframe.gameObject.SetActive(true);
            
            //continue:
            Next();
        }

        //Private Methods:
        private void BuildHorizontalSurfaces()
        {
            //gameobject creation:
            GameObject ceiling = new GameObject("(Ceiling)");
            GameObject floor = new GameObject("(Floor)");
            ceiling.transform.parent = RoomAnchor.Instance.transform;
            floor.transform.parent = RoomAnchor.Instance.transform;
            
            //mesh creation:
            Mesh floorMesh = new Mesh();
            Mesh ceilingMesh = new Mesh();

            //placement:
            ceiling.transform.SetPositionAndRotation(_ceilingCenter, RoomAnchor.Instance.transform.rotation);
            ceiling.transform.Rotate(Vector3.forward * 180);
            floor.transform.SetPositionAndRotation(_ceilingCenter, RoomAnchor.Instance.transform.rotation);
            
            //get all local points:
            List<Vector3> ceilingVerts3D = new List<Vector3>();
            List<Vector2> ceilingVerts2D = new List<Vector2>();
            List<Vector3> floorVerts3D = new List<Vector3>();
            List<Vector2> floorVerts2D = new List<Vector2>();
            for (int i = 0; i < RoomMapper.Instance.CeilingCorners.Length - 1; i++)
            {
                //ceiling:
                Vector3 ceilingVert = ceiling.transform.InverseTransformPoint(RoomAnchor.Instance.transform.TransformPoint(RoomMapper.Instance.CeilingCorners[i]));
                ceilingVerts3D.Add(ceilingVert);
                ceilingVerts2D.Add(new Vector2(ceilingVert.x, ceilingVert.z));
                
                //floor:
                Vector3 floorVert = floor.transform.InverseTransformPoint(RoomAnchor.Instance.transform.TransformPoint(RoomMapper.Instance.CeilingCorners[i]));
                floorVerts3D.Add(floorVert);
                floorVerts2D.Add(new Vector2(floorVert.x, floorVert.z));
            }
            
            //set vers:
            ceilingMesh.vertices = ceilingVerts3D.ToArray();
            floorMesh.vertices = floorVerts3D.ToArray();
            
            //triangles:
            int[] ceilingTriangles = new Triangulator(ceilingVerts2D.ToArray()).Triangulate();
            int[] floorTriangles = new Triangulator(floorVerts2D.ToArray()).Triangulate();
            if (_windingDirection == 1)
            {
                ceilingTriangles.Reverse();
            }
            else
            {
                floorTriangles.Reverse();
            }
            ceilingMesh.triangles = ceilingTriangles;
            floorMesh.triangles = floorTriangles;

            //uvs:
            Vector2[] uvs = new Vector2[floorVerts2D.Count];
            for (int i = 0; i < uvs.Length; i++)
            {
                uvs [i] = new Vector2 (floorVerts2D[i].x, floorVerts2D[i].y);
            }
            ceilingMesh.uv = uvs;
            floorMesh.uv = uvs;

            //rendering components:
            ceiling.AddComponent<MeshFilter>().mesh = ceilingMesh;
            floor.AddComponent<MeshFilter>().mesh = floorMesh;
            
            //materials:
            MeshRenderer ceilingRenderer = ceiling.AddComponent<MeshRenderer>();
            if (RoomMapper.Instance.ceilingMaterial)
            {
                ceilingRenderer.material = RoomMapper.Instance.ceilingMaterial;
            }
            else
            {
                ceilingRenderer.enabled = false;
            }

            MeshRenderer floorRenderer = floor.AddComponent<MeshRenderer>();
            if (RoomMapper.Instance.floorMaterial)
            {
                floorRenderer.material = RoomMapper.Instance.floorMaterial;
            }
            else
            {
                floorRenderer.enabled = false;
            }
            
            //calculate:
            ceilingMesh.RecalculateNormals();
            ceilingMesh.RecalculateBounds();
            floorMesh.RecalculateNormals();
            floorMesh.RecalculateBounds();
            
            //colliders (no need for mesh colliders - simplify with boxes):
            ceiling.AddComponent<BoxCollider>();
            floor.AddComponent<BoxCollider>();

            //push floor down:
            floor.transform.Translate(Vector3.down * RoomMapper.Instance.RoomHeight);
            
            //cache:
            RoomMapper.Instance.Ceiling = ceiling;
            RoomMapper.Instance.Floor = floor;
        }
        
        private void SetCeilingCenter()
        {
            // find bounds:
            Bounds bounds = new Bounds(RoomAnchor.Instance.transform.TransformPoint(RoomMapper.Instance.CeilingCorners[0]), Vector3.zero);
            foreach (var corner in RoomMapper.Instance.CeilingCorners)
            {
                bounds.Encapsulate(RoomAnchor.Instance.transform.TransformPoint(corner));
            }
            
            //sets:
            _ceilingCenter = bounds.center;
        }
        
        private void SetWindingDirection()
        {
            //discover winding direction:
            Vector3 centerToFirst = Vector3.Normalize(RoomAnchor.Instance.transform.TransformPoint(RoomMapper.Instance.CeilingCorners[0]) - _ceilingCenter);
            Vector3 centerToLast = Vector3.Normalize(RoomAnchor.Instance.transform.TransformPoint(RoomMapper.Instance.CeilingCorners[RoomMapper.Instance.CeilingCorners.Length - 2]) - _ceilingCenter);
            float windingAngle = Vector3.SignedAngle(centerToLast, centerToFirst, Vector3.up);

            //1 = clockwise, -1 = counterclockwise
            _windingDirection = Mathf.Sign(windingAngle); 
        }

        private void BuildWalls()
        {
            List<GameObject> walls = new List<GameObject>();
            
            for (int i = 0; i < RoomMapper.Instance.CeilingCorners.Length - 1; i++)
            {
                //create:
                GameObject wall = new GameObject("(Walls)");
                wall.transform.parent = RoomAnchor.Instance.transform;
                
                //orientation discovery:
                Vector3 crossPointA = _windingDirection == 1 ? RoomAnchor.Instance.transform.TransformPoint(RoomMapper.Instance.CeilingCorners[i]) : RoomAnchor.Instance.transform.TransformPoint(RoomMapper.Instance.CeilingCorners[1]);
                Vector3 crossPointB = _windingDirection == 1 ? RoomAnchor.Instance.transform.TransformPoint(RoomMapper.Instance.CeilingCorners[i + 1]) : RoomAnchor.Instance.transform.TransformPoint(RoomMapper.Instance.CeilingCorners[0]);
                Vector3 wallForward = Vector3.Cross(Vector3.Normalize(crossPointA - crossPointB), Vector3.up);

                //orient:
                Vector3 left = RoomAnchor.Instance.transform.TransformPoint(RoomMapper.Instance.CeilingCorners[i]) + Vector3.down * (RoomMapper.Instance.RoomHeight * .5f);
                Vector3 right = RoomAnchor.Instance.transform.TransformPoint(RoomMapper.Instance.CeilingCorners[i + 1]) + Vector3.down * (RoomMapper.Instance.RoomHeight * .5f);
                wall.transform.position = Vector3.Lerp(left, right, .5f);
                wall.transform.rotation = Quaternion.LookRotation(wallForward);
                wall.transform.localScale = new Vector3(Vector3.Distance(RoomMapper.Instance.CeilingCorners[i], RoomMapper.Instance.CeilingCorners[i + 1]), RoomMapper.Instance.RoomHeight, 1);
                
                //lists:
                List<Vector3> verts = new List<Vector3>();
                List<int> tris = new List<int>();
                
                //quad corners:
                Vector3 lowerLeft = RoomAnchor.Instance.transform.TransformPoint(RoomMapper.Instance.CeilingCorners[i]) + Vector3.down * RoomMapper.Instance.RoomHeight;
                Vector3 upperLeft = RoomAnchor.Instance.transform.TransformPoint(RoomMapper.Instance.CeilingCorners[i]);
                Vector3 upperRight = RoomAnchor.Instance.transform.TransformPoint(RoomMapper.Instance.CeilingCorners[i + 1]);
                Vector3 lowerRight = RoomAnchor.Instance.transform.TransformPoint(RoomMapper.Instance.CeilingCorners[i + 1]) + Vector3.down * RoomMapper.Instance.RoomHeight;
                
                //set vertices (in local space of wall):
                verts.Add(wall.transform.InverseTransformPoint(lowerLeft));
                verts.Add(wall.transform.InverseTransformPoint(upperLeft));
                verts.Add(wall.transform.InverseTransformPoint(upperRight));
                verts.Add(wall.transform.InverseTransformPoint(lowerRight));
                
                //build/extend wireframe:
                if (i == 0)
                {
                    wireframe.positionCount += 5;
                    wireframe.SetPosition(0, wireframe.transform.InverseTransformPoint(lowerLeft));
                    wireframe.SetPosition(1, wireframe.transform.InverseTransformPoint(upperLeft));
                    wireframe.SetPosition(2, wireframe.transform.InverseTransformPoint(upperRight));
                    wireframe.SetPosition(3, wireframe.transform.InverseTransformPoint(lowerRight));
                    wireframe.SetPosition(4, wireframe.transform.InverseTransformPoint(lowerLeft));
                }
                else
                {
                    wireframe.positionCount += 4;
                    wireframe.SetPosition(wireframe.positionCount - 4, wireframe.transform.InverseTransformPoint(upperLeft));
                    wireframe.SetPosition(wireframe.positionCount - 3, wireframe.transform.InverseTransformPoint(upperRight));
                    wireframe.SetPosition(wireframe.positionCount - 2, wireframe.transform.InverseTransformPoint(lowerRight));
                    wireframe.SetPosition(wireframe.positionCount - 1, wireframe.transform.InverseTransformPoint(lowerLeft));
                }

                if (i == RoomMapper.Instance.CeilingCorners.Length - 2)
                {
                    wireframe.positionCount += 1;
                    wireframe.SetPosition(wireframe.positionCount - 1, wireframe.transform.InverseTransformPoint(upperRight));
                }
                
                //set triangles:
                tris.Add(verts.Count - 4);
                tris.Add(verts.Count - 3);
                tris.Add(verts.Count - 2);
                tris.Add(verts.Count - 2);
                tris.Add(verts.Count - 1);
                tris.Add(verts.Count - 4);
                
                if (_windingDirection == -1)
                {
                    tris.Reverse();
                }
                
                //uvs:
                Vector2[] uvs = new Vector2[]
                {
                    new Vector2(0,0),
                    new Vector2(0,verts[2].y),
                    new Vector2(verts[2].x,verts[2].y),
                    new Vector2(verts[2].x,0)
                };

                //populate mesh:
                Mesh mesh = new Mesh();
                mesh.vertices = verts.ToArray();
                mesh.triangles = tris.ToArray();
                mesh.uv = uvs;
                mesh.RecalculateNormals();
                mesh.RecalculateBounds();
                
                //rendering:
                wall.AddComponent<MeshFilter>().mesh = mesh;
                
                //collider:
                wall.AddComponent<BoxCollider>().size = new Vector3(1, 1, .01f);
                
                //material:
                MeshRenderer wallRenderer = wall.AddComponent<MeshRenderer>();
                if (RoomMapper.Instance.wallMaterial)
                {
                    wallRenderer.material = RoomMapper.Instance.wallMaterial;
                }
                else
                {
                    wallRenderer.enabled = false;
                }

                //cache:
                walls.Add(wall);
            }

            //cache:
            RoomMapper.Instance.Walls = SortWalls(walls).ToArray(); //largest to smallest wall
        }

        private List<GameObject> SortWalls(List<GameObject> unsortedWalls)
        {
            int min;
            GameObject temp;

            for (int i = 0; i < unsortedWalls.Count; i++)
            {
                min = i;

                for (int j = i + 1; j < unsortedWalls.Count; j++)
                {
                    if (unsortedWalls[j].transform.localScale.x < unsortedWalls[min].transform.localScale.x)
                    {
                        min = j;
                    }
                }

                if (min != i)
                {
                    temp = unsortedWalls[i];
                    unsortedWalls[i] = unsortedWalls[min];
                    unsortedWalls[min] = temp;
                }
            }

            //make the list be largest to smallest:
            unsortedWalls.Reverse();
            return unsortedWalls;
        }
    }
}