using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
    const float viewerMoveThresholdForChunkUpdate = 25f;
    const float sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;
    const float scale = 10f;

    public static float maxViewDistance;
    public LODInfo[] detailLevels;

    public GameObject viewer;
    bool viewerPlaced = false;

    public Material mapMaterial;
    public GameObject water;
    public GameObject grass;
    public GameObject boundingBox;
    public GameObject flock;
    public GameObject axolotl;

    public static Vector2 viewerPosition;
    Vector2 viewerPositionOld;

    float waterLevel;
    static MapGenerator mapGenerator;

    int chunkSize;
    int chunksVisibleInViewDistance;



    Dictionary<Vector2, TerrainChunk> terrainChunkDict = new Dictionary<Vector2, TerrainChunk>();
    static List<TerrainChunk> terrainChunkVisibleLastUpdate = new List<TerrainChunk>();
    private void Start()
    {
        maxViewDistance = detailLevels[detailLevels.Length - 1].visibleDstThreshold;
        mapGenerator = FindObjectOfType<MapGenerator>();
        chunkSize = MapGenerator.mapChunkSize - 1;
        chunksVisibleInViewDistance = Mathf.RoundToInt(maxViewDistance / chunkSize);
        waterLevel = mapGenerator.meshHeightCurve.Evaluate((float).45) * mapGenerator.meshHeightMultiplier;
        water.SetActive(true);
        UpdateVisibleChunks();
    }
    private void Update()
    {
        viewerPosition = new Vector2(viewer.transform.position.x, viewer.transform.position.z) / scale;
        water.transform.position = new Vector3(viewerPosition.x, waterLevel, viewerPosition.y) * scale;
        boundingBox.transform.position = new Vector3(viewerPosition.x, 0, viewerPosition.y) * scale;

        if ((viewerPositionOld-viewerPosition).sqrMagnitude > sqrViewerMoveThresholdForChunkUpdate)
        {
            UpdateVisibleChunks();
            viewerPositionOld = viewerPosition;
        }

        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);
        Vector2 viewerCoords = new Vector2(currentChunkCoordX, currentChunkCoordY);

        if (!viewerPlaced &&
            terrainChunkDict.ContainsKey(viewerCoords) &&
            terrainChunkDict[viewerCoords].mapDataRecieved)
        {
            RaycastHit hit;
            float maxHeight = mapGenerator.meshHeightMultiplier * scale;
            Ray ray = new Ray(new Vector3(viewer.transform.position.x, maxHeight, viewer.transform.position.z), Vector3.down);
            if (terrainChunkDict[viewerCoords].meshCollider.Raycast(ray, out hit, 2.0f * maxHeight))
            {
                flock.transform.position = viewer.transform.position + new Vector3(0, 150, 0);
                flock.SetActive(true);
                boundingBox.SetActive(true);
                Vector3 point3D = new Vector3(viewer.transform.position.x, hit.point.y + 0.1f, viewer.transform.position.z);
                viewer.transform.position = point3D;
                viewer.SetActive(true);
                viewerPlaced = true;

                // axolotl spawn
                // calculate height
                //GameObject newAxolotl = Instantiate(axolotl, point3D, Quaternion.Euler(0, Random.Range(0, 360), 0));
                axolotl.transform.position = point3D;
                axolotl.SetActive(true);
            }
        }
    }


    void UpdateVisibleChunks()
    {
        for(int i=0; i < terrainChunkVisibleLastUpdate.Count; i++)
        {
            terrainChunkVisibleLastUpdate[i].SetVisible(false);
        }
        terrainChunkVisibleLastUpdate.Clear();
        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);

        for(int yOffset = -chunksVisibleInViewDistance; yOffset <= chunksVisibleInViewDistance; yOffset++)
        {
            for (int xOffset = -chunksVisibleInViewDistance; xOffset <= chunksVisibleInViewDistance; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);
                if (terrainChunkDict.ContainsKey(viewedChunkCoord))
                {
                    terrainChunkDict[viewedChunkCoord].UpdateTerrainChunk();
                }
                else
                {
                    terrainChunkDict.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, detailLevels, transform, mapMaterial, grass, axolotl));
                }
            }

        }
    }

    public class TerrainChunk
    {
        Vector2 position;
        GameObject meshObject;
        Bounds bounds;

        MeshRenderer meshRenderer;
        MeshFilter meshFilter;
        public MeshCollider meshCollider;
        
        LODInfo[] detailLevels;
        LODMesh[] lodMeshes;

        MapData mapData;
        public bool mapDataRecieved;

        List<Vector2> grassPoints;
        GameObject grass;

        List<Vector2> axolotls;
        GameObject axolotl;

        int prevLODIndex = -1;

        public TerrainChunk(Vector2 coord, int size, LODInfo[] detailLevels, Transform parent, Material material, GameObject grass, GameObject axolotl)
        {
            this.detailLevels = detailLevels;
            position = coord * size;
            bounds = new Bounds(position, Vector2.one * size);
            Vector3 positionV3 = new Vector3(position.x, 0, position.y);

            meshObject = new GameObject("Terrain chunk");
            this.grass = grass;
            this.axolotl = axolotl;
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshCollider = meshObject.AddComponent<MeshCollider>();
            meshRenderer.material = material;
            meshObject.transform.position = positionV3 * scale;
            meshObject.transform.localScale = Vector3.one * scale;
            meshObject.transform.parent = parent;
            SetVisible(false);

            lodMeshes = new LODMesh[detailLevels.Length];
            for(int i =0; i < detailLevels.Length; i++)
            {
                lodMeshes[i] = new LODMesh(detailLevels[i].lod, UpdateTerrainChunk);
            }

            mapGenerator.RequestMapData(position, OnMapDataRecieved);
        }

        void OnMapDataRecieved(MapData m)
        {
            this.mapData = m;
            mapDataRecieved = true;
            UpdateTerrainChunk();
        }

        public void UpdateTerrainChunk()
        {
            if (mapDataRecieved) {
                float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
                bool visible = viewerDstFromNearestEdge <= maxViewDistance;

            if (visible)
                {
                    int lodIndex = 0;
                    for (int i = 0; i < detailLevels.Length - 1; i++)
                    {
                        if (viewerDstFromNearestEdge > detailLevels[i].visibleDstThreshold)
                        {
                            lodIndex = i + 1;
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (lodIndex != prevLODIndex)
                    {
                        LODMesh lodMesh = lodMeshes[lodIndex];
                        if (lodMesh.hasmesh)
                        {
                            prevLODIndex = lodIndex;
                            meshFilter.mesh = lodMesh.mesh;
                            meshCollider.sharedMesh = meshFilter.mesh;

                            // place grass
                            if(lodIndex == 0 && grassPoints == null)
                            {
                                float grassRadius = 30f;
                                float grassSize = 1000;
                                int sampleRejection = 30;
                                grassPoints = PoissonDiskSampling.GeneratePoints(grassRadius, Vector2.one * grassSize, sampleRejection);
                                float grassMin = mapGenerator.meshHeightCurve.Evaluate((float).55) * mapGenerator.meshHeightMultiplier * scale;
                                float grassMax = mapGenerator.meshHeightCurve.Evaluate((float).7) * mapGenerator.meshHeightMultiplier * scale;
                                foreach (Vector2 point in grassPoints)
                                {
                                    // calculate height
                                    RaycastHit hit;
                                    float maxHeight = mapGenerator.meshHeightMultiplier * scale;
                                    Ray ray = new Ray(new Vector3(point.x - (grassSize / 2), maxHeight, point.y - (grassSize / 2)), Vector3.down);
                                    Vector3 point3D = Vector3.zero;
                                    if (meshCollider.Raycast(ray, out hit, 2.0f * maxHeight))
                                    {
                                        point3D = new Vector3(point.x - (grassSize / 2), hit.point.y, point.y - (grassSize / 2));
                                    }
                                    if(point3D.y > grassMin &&
                                        point3D.y < grassMax)
                                    {
                                        GameObject newGrass = Instantiate(grass, point3D, Quaternion.Euler(0, Random.Range(0, 360), 0), meshObject.transform.parent);
                                        newGrass.transform.localScale = Vector3.one * .5f;
                                        newGrass.SetActive(true);
                                    }
                                }

                                
                                
                            }
                            
                        }
                        else if(!lodMesh.hasRequestedMesh)
                        {
                            lodMesh.RequestMesh(mapData);
                        }
                    }
                    
                        terrainChunkVisibleLastUpdate.Add(this);
                    
                }
                SetVisible(visible);
            }
        }

        public void SetVisible(bool visible)
        {
            meshObject.SetActive(visible);
        }
        public bool IsVisible()
        {
            return meshObject.activeSelf;
        }
    }

    class LODMesh
    {
        public Mesh mesh;
        public bool hasRequestedMesh;
        public bool hasmesh;
        int lod;

        System.Action updateCallback;

        public LODMesh(int lod, System.Action updateCallback)
        {
            this.lod = lod;
            this.updateCallback = updateCallback;
        }

        void OnMeshDataRecieved(MeshData meshdata)
        {
            mesh = meshdata.CreateMesh();
            hasmesh = true;
            updateCallback();
        }

        public void RequestMesh(MapData mapData)
        {
            hasRequestedMesh = true;
            mapGenerator.RequestMeshData(mapData, lod, OnMeshDataRecieved);
        }
    }
    [System.Serializable]
    public struct LODInfo
    {
        public int lod;
        public float visibleDstThreshold;

    }
}
