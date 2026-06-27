using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Chunk : MonoBehaviour
{
    /*
     * Đây là class sẽ chứa code vẽ chunk, đưa danh sách tọa độ đỉnh, tam giác và uvs để vẽ
     * 1 mảng 3D ký hiệu [,,] để biết loại khối đó tại vị trí [x, y, z], 
     * Chunk có thể có 16×64×16 (x, y, z) block.
     * Cần tách riêng ra 1 chunk đại diện để sau này mình xóa chunk đó đi thì lưu lại vào 1 bộ nhớ 
     * Khi vẽ các khối k vẽ toàn bộ mà phải vẽ từng mặt để tối ưu hiệu suất
     */
    public const int chieuDaiChunk = 16;
    public const int doCaoChunk = 64;
    // gán giá trị, truy xuất tên khối tại vị trí [x,y,z] là khoi[x,y,z]
    public tenKhoi[,,] Khois = new tenKhoi[chieuDaiChunk + 5, doCaoChunk + 5, chieuDaiChunk + 5];

   
    public void DrawChunk()
    {
        Mesh mesh = new Mesh();

        /*Tọa độ đỉnh, tam giác, uvs*/
        Vector3[] dinh = new Vector3[24];


        // Ghi thế này khi mình tăng kích thước thì vẫn đảm bảo sửa được
        float x0 = 0f, y0 = 0f, z0 = 0f;
        float x1 = 1f, y1 = 1f, z1 = 1f;

        /*Xác định 4 đỉnh của tất cả các mặt*/
        // Top face 
        dinh[0] = new Vector3(x0, y1, z0);
        dinh[1] = new Vector3(x1, y1, z0);
        dinh[2] = new Vector3(x1, y1, z1);
        dinh[3] = new Vector3(x0, y1, z1);

        // Bottom face 
        dinh[4] = new Vector3(x0, y0, z0);
        dinh[5] = new Vector3(x1, y0, z0);
        dinh[6] = new Vector3(x1, y0, z1);
        dinh[7] = new Vector3(x0, y0, z1);

        // Front face 
        dinh[8] = new Vector3(x0, y0, z1);
        dinh[9] = new Vector3(x1, y0, z1);
        dinh[10] = new Vector3(x1, y1, z1);
        dinh[11] = new Vector3(x0, y1, z1);

        // Back face 
        dinh[12] = new Vector3(x0, y0, z0);
        dinh[13] = new Vector3(x1, y0, z0);
        dinh[14] = new Vector3(x1, y1, z0);
        dinh[15] = new Vector3(x0, y1, z0);

        // Left face 
        dinh[16] = new Vector3(x0, y0, z0);
        dinh[17] = new Vector3(x0, y0, z1);
        dinh[18] = new Vector3(x0, y1, z1);
        dinh[19] = new Vector3(x0, y1, z0);

        // Right face 
        dinh[20] = new Vector3(x1, y0, z0);
        dinh[21] = new Vector3(x1, y0, z1);
        dinh[22] = new Vector3(x1, y1, z1);
        dinh[23] = new Vector3(x1, y1, z0);


        int[] dsTamGiac = new int[36]
        {
                // Top
                3, 2, 1, 3, 1, 0,    
                //Bot
                4, 5, 6, 4, 6, 7,
                // Front
                8, 9, 10, 8, 10, 11,
                // Back
                12, 14, 13, 12, 15, 14,
                // Left
                16, 17, 18, 16, 18, 19,
                // Right
                20, 22, 21, 20, 23, 22
        };
        /*Phía trên là data*/

        List<Vector3> vert = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        int index = 0;
        for (int x = 0; x < chieuDaiChunk; x++)
        {
            for (int z = 0; z < chieuDaiChunk; z++)
            {
                for (int y = 0; y < doCaoChunk; y++)
                {
                    if (Khois[x, y, z] == tenKhoi.Air) continue;

                    Vector3 pos = new Vector3(x, y, z);

                    /*
                     * Logic là nếu khối tại vị trí phía trên, phía dưới hay xung quanh 
                     * khối hiện tại là không khí thì mặt đó người khác thấy được và ta phải vẽ
                     * mặt đó
                
                    Tọa độ (x,y,z) của đỉnh là tọa độ được lưu riêng trong đỉnh, tọa độ x,y,z của 
                    khối là tọa độ được lưu riêng trong khối

                    Một cube đặt tâm ở (0,0,0) thì các đỉnh sẽ là 
                    v0 = (0,0,0)
                    v1 = (1,0,0)
                    v2 = (1,1,0)
                    v3 = (0,1,0)
                    Chỉ đúng trong khối đó, chưa biết nó sẽ đứng ở đâu trong thế giới lớn.
                    Để vẽ khối đó đúng trong thế giới lớn thì phải lấy tọa độ đỉnh của mặt đó cộng cho
                    vị trí của khối cube đó trong thế giới lớn.


                    - LOGIC thêm tam giác
                    Tam giác thì dự trên chỉ số đỉnh trong danh sách đỉnh (của vert) nằm trong mặt đó
                    cứ 1 lần ta xét 4 đỉnh nên phải cộng thêm 4

                    - Đỉnh thứ 1 mình test nó phải đi theo dãy 3, 2, 1, 3, 1, 0 mới k bị lỗi
                    còn bình thường nó sẽ tuân theo dãy 0, 1, 2, 0, 2, 3. Lấy mỗi số + 4 sẽ
                    ra được dãy tiếp theo
                    */

                    // TOP FACE
                    if (Khois[x, y + 1, z] == tenKhoi.Air)
                    {
                        // DINH
                        vert.Add(pos + dinh[0]);
                        vert.Add(pos + dinh[1]);
                        vert.Add(pos + dinh[2]);
                        vert.Add(pos + dinh[3]);

                        /*
                         * TAM GIAC
                         * Dãy đầu tiên phải tuân theo 3, 2, 1, 3, 1, 0 mới k bị lỗi hiển thị
                        */
                        triangles.Add(index + 3);
                        triangles.Add(index + 2);
                        triangles.Add(index + 1);
                        triangles.Add(index + 3);
                        triangles.Add(index + 1);
                        triangles.Add(index + 0);

                        index += 4;

                        /*
                         * để vẽ UVS cần phải biết loại khối là gì để dán đúng hình 
                         * Dùng hàm addRange để thêm nhiều phần tử vào cùng 1 lúc
                         * Khoi.getKhoi[TÊN KHỐI].toado.uvs
                        */
                        uvs.AddRange(Khoi.getKhoi[Khois[x, y, z]].toaDoMatTren.getUVS());

                    }
                    // BOTTOM FACE
                    if (Khois[x, y - 1, z] == tenKhoi.Air)
                    {
                        vert.Add(pos + dinh[4]);
                        vert.Add(pos + dinh[5]);
                        vert.Add(pos + dinh[6]);
                        vert.Add(pos + dinh[7]);

                        triangles.Add(index + 0);
                        triangles.Add(index + 1);
                        triangles.Add(index + 2);
                        triangles.Add(index + 0);
                        triangles.Add(index + 2);
                        triangles.Add(index + 3);

                        index += 4;

                        uvs.AddRange(Khoi.getKhoi[Khois[x, y, z]].toaDoMatDuoi.getUVS());
                    }
                    // FRONT FACE
                    if (Khois[x, y, z + 1] == tenKhoi.Air)
                    {
                        vert.Add(pos + dinh[8]);
                        vert.Add(pos + dinh[9]);
                        vert.Add(pos + dinh[10]);
                        vert.Add(pos + dinh[11]);

                        triangles.Add(index + 0);
                        triangles.Add(index + 1);
                        triangles.Add(index + 2);
                        triangles.Add(index + 0);
                        triangles.Add(index + 2);
                        triangles.Add(index + 3);

                        index += 4;
                        uvs.AddRange(Khoi.getKhoi[Khois[x, y, z]].toaDoMatXungQuanh.getUVS());
                    }
                    // BACK FACE
                    if (Khois[x, y, z - 1] == tenKhoi.Air)
                    {
                        vert.Add(pos + dinh[12]);
                        vert.Add(pos + dinh[13]);
                        vert.Add(pos + dinh[14]);
                        vert.Add(pos + dinh[15]);

                        triangles.Add(index + 0);
                        triangles.Add(index + 1);
                        triangles.Add(index + 2);
                        triangles.Add(index + 0);
                        triangles.Add(index + 2);
                        triangles.Add(index + 3);

                        index += 4;
                        uvs.AddRange(Khoi.getKhoi[Khois[x, y, z]].toaDoMatXungQuanh.getUVS());
                    }
                    // LEFT FACE
                    if (Khois[x - 1, y, z] == tenKhoi.Air)
                    {
                        vert.Add(pos + dinh[16]);
                        vert.Add(pos + dinh[17]);
                        vert.Add(pos + dinh[18]);
                        vert.Add(pos + dinh[19]);

                        triangles.Add(index + 0);
                        triangles.Add(index + 1);
                        triangles.Add(index + 2);
                        triangles.Add(index + 0);
                        triangles.Add(index + 2);
                        triangles.Add(index + 3);

                        index += 4;
                        uvs.AddRange(Khoi.getKhoi[Khois[x, y, z]].toaDoMatXungQuanh.getUVS());
                    }
                    //  RIGHT FACE
                    if (Khois[x + 1, y, z] == tenKhoi.Air)
                    {
                        vert.Add(pos + dinh[20]);
                        vert.Add(pos + dinh[21]);
                        vert.Add(pos + dinh[22]);
                        vert.Add(pos + dinh[23]);

                        triangles.Add(index + 0);
                        triangles.Add(index + 1);
                        triangles.Add(index + 2);
                        triangles.Add(index + 0);
                        triangles.Add(index + 2);
                        triangles.Add(index + 3);

                        index += 4;
                        uvs.AddRange(Khoi.getKhoi[Khois[x, y, z]].toaDoMatXungQuanh.getUVS());
                    }              
                }
            }       
        }

        /*ToArray() chuyển List thành mảng, từ List<Vector3> thành Vector3[] */
        mesh.vertices = vert.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

        // tính toán ánh sáng cho đúng
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();

        //gán mesh vừa tạo vào MeshFilter
        GetComponent<MeshFilter>().mesh = mesh;

        // bọc khối đó trong collider
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }
}
