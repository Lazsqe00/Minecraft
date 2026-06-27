using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class BlockMesh : MonoBehaviour
{
    [Header("Atlas Settings")]
    public int atlasSizeX = 2;          // Số ô ngang trong texture atlas
    public int atlasSizeY = 2;          // Số ô dọc trong texture atlas

    [Header("Texture positions in atlas (x,y) - chỉnh theo atlas thật của bạn")]
    public Vector2Int topTexture = new Vector2Int(0, 1);    // grass_top
    public Vector2Int bottomTexture = new Vector2Int(1, 1);    // dirt
    public Vector2Int sideTexture = new Vector2Int(0, 0);    // grass_side

    void Start()
    {
        CreateCubeMesh();
    }

    void CreateCubeMesh()
    {
        Mesh mesh = new Mesh();
        mesh.name = "CubeMesh";

        // ── 24 vertices ── (mỗi mặt 4 đỉnh, không share vertex để UV khác nhau)
        Vector3[] vertices = new Vector3[24];

        // Các tọa độ góc cube (0→1)
        float x0 = 0f, x1 = 1f;
        float y0 = 0f, y1 = 1f;
        float z0 = 0f, z1 = 1f;

        // Top face (+Y)
        vertices[0] = new Vector3(x0, y1, z0);
        vertices[1] = new Vector3(x1, y1, z0);
        vertices[2] = new Vector3(x1, y1, z1);
        vertices[3] = new Vector3(x0, y1, z1);

        // Bottom face (-Y)
        vertices[4] = new Vector3(x0, y0, z0);
        vertices[5] = new Vector3(x1, y0, z0);
        vertices[6] = new Vector3(x1, y0, z1);
        vertices[7] = new Vector3(x0, y0, z1);

        // Front face (+Z)
        vertices[8] = new Vector3(x0, y0, z1);
        vertices[9] = new Vector3(x1, y0, z1);
        vertices[10] = new Vector3(x1, y1, z1);
        vertices[11] = new Vector3(x0, y1, z1);

        // Back face (-Z)
        vertices[12] = new Vector3(x0, y0, z0);
        vertices[13] = new Vector3(x1, y0, z0);
        vertices[14] = new Vector3(x1, y1, z0);
        vertices[15] = new Vector3(x0, y1, z0);

        // Left face (-X)
        vertices[16] = new Vector3(x0, y0, z0);
        vertices[17] = new Vector3(x0, y0, z1);
        vertices[18] = new Vector3(x0, y1, z1);
        vertices[19] = new Vector3(x0, y1, z0);

        // Right face (+X)
        vertices[20] = new Vector3(x1, y0, z0);
        vertices[21] = new Vector3(x1, y0, z1);
        vertices[22] = new Vector3(x1, y1, z1);
        vertices[23] = new Vector3(x1, y1, z0);

        mesh.vertices = vertices;

        // ── Triangles ── (counter-clockwise khi nhìn từ ngoài vào)
        int[] triangles = new int[36]
        {
            // Top
            0, 1, 2,    0, 2, 3,
            // Bottom - đảo chiều để normals hướng xuống (nhưng vẫn counter-clockwise khi nhìn từ dưới)
            4, 6, 5,    4, 7, 6,
            // Front
            8, 9, 10,   8, 10, 11,
            // Back
            12, 14, 13, 12, 15, 14,
            // Left
            16, 17, 18, 16, 18, 19,
            // Right
            20, 22, 21, 20, 23, 22
        };
        mesh.triangles = triangles;

        // ── UVs ──
        Vector2[] uvs = new Vector2[24];

        Vector2[] GetFaceUVs(Vector2Int tilePos)
        {
            float tileW = 1f / atlasSizeX;
            float tileH = 1f / atlasSizeY;

            float left = tilePos.x * tileW;
            float right = left + tileW;
            float bottom = tilePos.y * tileH;
            float top = bottom + tileH;

            // Thứ tự: bottom-left → bottom-right → top-right → top-left
            // (phù hợp với Unity UV origin bottom-left)
            return new Vector2[]
            {
                new Vector2(left,  bottom),     // 0
                new Vector2(right, bottom),     // 1
                new Vector2(right, top),        // 2
                new Vector2(left,  top)         // 3
            };
        }

        // Gán UV cho từng mặt
        var topUV = GetFaceUVs(topTexture);
        var bottomUV = GetFaceUVs(bottomTexture);
        var sideUV = GetFaceUVs(sideTexture);

        // Top
        System.Array.Copy(topUV, 0, uvs, 0, 4);
        // Bottom
        System.Array.Copy(bottomUV, 0, uvs, 4, 4);
        // Front
        System.Array.Copy(sideUV, 0, uvs, 8, 4);
        // Back
        System.Array.Copy(sideUV, 0, uvs, 12, 4);
        // Left
        System.Array.Copy(sideUV, 0, uvs, 16, 4);
        // Right
        System.Array.Copy(sideUV, 0, uvs, 20, 4);

        mesh.uv = uvs;

        // Normals + bounds
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        GetComponent<MeshFilter>().mesh = mesh;
    }
}