using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum tenMatKhoi { Dirt, Grass, GrassSide, Stone, TreeCX, TreeSide, Leaves }

/*TreeCX là hình khúc gõ có vòng tròn ở giữa, còn TreeSide là thân gỗ bên ngoài*/
public class MatKhoi
{
    /*
     * Khi khởi tạo ta viết luôn tính tọa độ thật 4 đỉnh của ô dựa trên vị trí lý thuyết trong atlas
     thay vì mình viết 1 hàm và gọi nó tính tọa độ.
     Lưu tọa độ 4 đỉnh vào uvs. 
    */
    float atlasSizeX = 16f, atlasSizeY = 16f;
    Vector2[] uvs = new Vector2[24];
    public MatKhoi(int x, int y)
    {
        float chieuNgang = 1f / atlasSizeX;
        float chieuDoc = 1f / atlasSizeY;

        /*
         Tìm được chiều ngang và dọc của ô, ta sẽ đi tìm 4 đỉnh tạo thành ô đó
         Mình cần 4 ô trên hình vuông thế này: 
         (left,bottom), (right,bottom), (left,top), (right,top) 
         - Ta tính từng vị trí left, right, top, bot thôi
        */
        float trai = x * chieuNgang;
        float phai = trai + chieuNgang;

        float duoi = y * chieuDoc;
        float tren = duoi + chieuDoc;


        /*
         * Xuất hiện lỗi “đường chéo đen” - tất cả các mặt đều có đường chéo màu đen 
         * Thường xảy ra khi texture atlas có nhiều ô liền nhau và bạn dùng UV chính xác 0–1 của ô mà không bù offset.
         - Để đảm bảo an toàn k bị lấy 1 phần thừa bên ngoài thì:
            + trái và dưới phải cộng offset 
            + phải và trên phải trừ ofset
        */
        float offset = 0.001f;

         uvs = new Vector2[]
        {
                new Vector2(trai + offset, duoi + offset),
                new Vector2(phai - offset, duoi + offset),
                new Vector2(phai - offset, tren - offset),
                new Vector2(trai + offset, tren - offset)
        };
    }

    public Vector2[] getUVS()
    {
        return uvs;
    }

    public static Dictionary<tenMatKhoi, MatKhoi> KhoiTaoMatKhoi = new Dictionary<tenMatKhoi, MatKhoi>()
    {
        { tenMatKhoi.Dirt, new MatKhoi(0, 0)},
        { tenMatKhoi.Grass, new MatKhoi(1, 0)},
        { tenMatKhoi.GrassSide, new MatKhoi(0, 1)},
        { tenMatKhoi.Stone, new MatKhoi(0, 2)},
        { tenMatKhoi.TreeCX, new MatKhoi(0, 3)},
        { tenMatKhoi.TreeSide, new MatKhoi(0, 4)},
        { tenMatKhoi.Leaves, new MatKhoi(0, 5)},
    };
}
