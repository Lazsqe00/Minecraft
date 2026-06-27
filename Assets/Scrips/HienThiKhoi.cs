using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HienThiKhoi : MonoBehaviour
{
    /*
     MeshFilter là nơi chứa dữ liệu hình học của đối tượng 3D.
     Ví dụ dữ liệu trong Mesh: 
        - vertices là các đỉnh của mesh 
        - triangles là danh sách các tam giác nối các đỉnh
        - uv là tọa độ texture
        - normal là hướng mặt để tính sáng

    MeshRenderer là component chịu trách nhiệm hiển thị mesh lên màn hình.

    - Ta cần cung cấp cho mesh là ds đỉnh của mặt, tam giác của của mặt và tọa độ 4 đỉnh của mặt đó
    trong atlas

    Trước hết, hình ảnh trong atlas luôn đi từ dưới lên, đi từ trên xuống ;)) bó tay 

    Để xác định ô:
    float tileW = 1f / atlasSizeX;  // chiều rộng mỗi ô
    float tileH = 1f / atlasSizeY;  // chiều cao mỗi ô
    
    tính cạnh ô, Giả sử tilePos = (x, y) là cột và hàng của ô:
    float left   = tilePos.x * tileW;       // cạnh trái của ô
    float right  = left + tileW;            // cạnh phải của ô
    float bottom = tilePos.y * tileH;       // cạnh dưới của ô
    float top    = bottom + tileH;          // cạnh trên của ô

   
    */

    public int atlasSizeX = 16;
    public int atlasSizeY = 16;

    //public Vector2Int matTren = new Vector2Int(1, 0);    // Cỏ
    //public Vector2Int matDuoi = new Vector2Int(0, 0);    // Đất
    //public Vector2Int matXungQuanh = new Vector2Int(0, 1);    // Đất - cỏ


    void Start()
    {
        if (GetComponent<MeshFilter>() == null)
            gameObject.AddComponent<MeshFilter>();

        if (GetComponent<MeshRenderer>() == null)
            gameObject.AddComponent<MeshRenderer>();


        TaoKhoi(tenMatKhoi.Grass, tenMatKhoi.Dirt, tenMatKhoi.GrassSide);

    }

    /*
     Xác định 8 đỉnh của Cube.
     Xác định 12 tam giác (6 mặt × 2 tam giác/mặt).
     Tính UV để gán đúng texture cho từng mặt
     Gán mesh vào MeshFilter, gán Material, và tính normals để ánh sáng chiếu đẹp


      Tạo khối truyền vào tên tham chiếu các mặt tương ứng với từng tọa độ, 
       trong static class MatKhoi => truy xuất tọa độ MatKhoi.ToaDo[TÊN MẶT]
    */

    public void TaoKhoi(tenMatKhoi matTren, tenMatKhoi matDuoi, tenMatKhoi matXungQuanh)
    {
        /*
         * tạo mesh: điểm, Triangles (chỉ số các đỉnh tạo thành tam giác) ,UVs (tọa độ texture)
          - Normals (hướng mặt để ánh sáng phản chiếu đúng)
          - Bounds (kích thước khối, để Unity render đúng)
        */
        Mesh mesh = new Mesh();

        Vector3[] dinh = new Vector3[24];


        // Các tọa độ góc, khi mình tăng kích thước thì vẫn đảm bảo sửa được
        float x0 = 0f, y0 = 0f, z0 = 0f;  
        float x1 = 1f, y1 = 1f, z1 = 1f;

        /*Xác định 4 đỉnh của tất cả các mặt*/
        // Top face 
        dinh[0]  = new Vector3(x0, y1, z0);  
        dinh[1]  = new Vector3(x1, y1, z0);  
        dinh[2]  = new Vector3(x1, y1, z1); 
        dinh[3]  = new Vector3(x0, y1, z1);   

        // Bottom face 
        dinh[4]  = new Vector3(x0, y0, z0);
        dinh[5]  = new Vector3(x1, y0, z0);
        dinh[6]  = new Vector3(x1, y0, z1);
        dinh[7]  = new Vector3(x0, y0, z1);

        // Front face 
        dinh[8]  = new Vector3(x0, y0, z1);
        dinh[9]  = new Vector3(x1, y0, z1);
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


        // lưu mảng đỉnh
        mesh.vertices = dinh;


        /*
         * tạo Tam giác dựa trên ds đỉnh đã nối ví dụ có 4 đỉnh thì đây là mặt trên
           3 ------- 2
           |         |
           |         |
           0 ------- 1 
         có 2 tam giác là (0, 1, 2), (0, 2, 3) . Tương tự với các mặt còn lại

            
           4 ------- 5
           |         |
           |         |
           7 ------- 6 
        */

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
        mesh.triangles = dsTamGiac;

        /*
        Vector2[] uvs = tọa độ texture atlas cho mỗi đỉnh, tương ứng vị trí trong atlas

        - Tìm chiều ngang, chiều dọc của ô
        - Dựa trên tọa độ  mặt trên, mặt dưới, mặt xung quanh(lý thuyết), ta sẽ 
        tính tọa độ uv của ô thật trong atlas
        */
        Vector2[] uvs = new Vector2[24];

        // Hàm tính tọa độ thật 4 đỉnh của ô dựa trên vị trí lý thuyết trong atlas

        Vector2[] getToaDo4DinhTrongAtlas(Vector2 toaDoTrongHinh)
        {
            float chieuNgang = 1f / atlasSizeX;
            float chieuDoc = 1f / atlasSizeY;

            /*
             Tìm được chiều ngang và dọc của ô, ta sẽ đi tìm 4 đỉnh tạo thành ô đó
             Mình cần 4 ô trên hình vuông thế này: 
             (left,bottom), (right,bottom), (left,top), (right,top) 
             - Ta tính từng vị trí left, right, top, bot thôi
            */
            float trai = toaDoTrongHinh.x * chieuNgang;
            float phai = trai + chieuNgang;

            float duoi = toaDoTrongHinh.y * chieuDoc;
            float tren = duoi + chieuDoc;


            /*
             * Xuất hiện lỗi “đường chéo đen” - tất cả các mặt đều có đường chéo màu đen 
             * Thường xảy ra khi texture atlas có nhiều ô liền nhau và bạn dùng UV chính xác 0–1 của ô mà không bù offset.
             - Để đảm bảo an toàn k bị lấy 1 phần thừa bên ngoài thì:
                + trái và dưới phải cộng offset 
                + phải và trên phải trừ ofset
            */
            float offset = 0.001f; 

            return new Vector2[]
            {
                new Vector2(trai + offset, duoi + offset),   
                new Vector2(phai - offset, duoi + offset),  
                new Vector2(phai - offset, tren - offset),  
                new Vector2(trai + offset, tren - offset)   
            };
        }

        /*
         ĐÂY LÀ CODE THAM KHẢO, PHẦN DƯỚI DO SỬA CODE NÊN BỊ LỖI
        */

        //// Lấy ra 4 đỉnh tương ứng với 1 mặt trong atlas, lấy 3 mặt
        //var matTop = getToaDo4DinhTrongAtlas(MatKhoi.ToaDo[matTren]);
        //var matBottom = getToaDo4DinhTrongAtlas(MatKhoi.ToaDo[matDuoi]);
        //var matSide = getToaDo4DinhTrongAtlas(MatKhoi.ToaDo[matXungQuanh]);


        ///*
        // * Dán vào các mặt, cú pháp:
        // * System.Array.Copy(Mảng nguồn, sourceIndex, Mảng đích, destIndex, length);
        //  sourceIndex là bắt đầu copy từ chỉ số này trong mảng nguồn
        //  destIndex là copy vào bắt đầu từ chỉ số này trong mảng đích
        //  length là số phần tử cần copy
        //*/

        //System.Array.Copy(matTop, 0, uvs, 0, 4);
        //System.Array.Copy(matBottom, 0, uvs, 4, 4);
        //System.Array.Copy(matSide, 0, uvs, 8, 4);
        //System.Array.Copy(matSide, 0, uvs, 12, 4);
        //System.Array.Copy(matSide, 0, uvs, 16, 4);
        //System.Array.Copy(matSide, 0, uvs, 20, 4);

        // lưu uvs
        mesh.uv = uvs;

        // tính toán ánh sáng cho đúng
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();


        GetComponent<MeshFilter>().mesh = mesh;

    }
}
