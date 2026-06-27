using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Khoi : MonoBehaviour
{
    public tenMatKhoi matTren, matDuoi, matXungQuanh;

    public MatKhoi toaDoMatTren, toaDoMatDuoi, toaDoMatXungQuanh;
    /*
     * Hàm khởi tạo nếu truyền vào 1 thì tất cả 1 mặt đều vẽ giống nhau
     * Nếu truyền vào 3 thì vẽ 6 mặt theo lý thuyết
     * 
     * Khi khởi tạo mặt khối sẽ trả về vector uvs, truy xuất biến toaDoMat.getUVS của
      mặt đó

    Đừng quan tâm vào hàm khởi tạo, quan tâm khi gọi ta sẽ gọi dictionary để khởi tạo riêng
    khối đó
    */
    public Khoi(tenMatKhoi nameBlock)
    {
        matTren = matDuoi = matXungQuanh = nameBlock;
        LayToaDoLyThuyetAtlas(matTren, matDuoi, matXungQuanh);
    }

    public Khoi(tenMatKhoi tren, tenMatKhoi duoi, tenMatKhoi xungQuanh)
    {
        matTren = tren;
        matDuoi = duoi;
        matXungQuanh = xungQuanh;
        LayToaDoLyThuyetAtlas(matTren, matDuoi, matXungQuanh);
    }
    /*toaDoMatTren.getuvs*/
    void LayToaDoLyThuyetAtlas(tenMatKhoi matTren, tenMatKhoi matDuoi, tenMatKhoi matXungQuanh)
    {
        toaDoMatTren = MatKhoi.KhoiTaoMatKhoi[matTren];
        toaDoMatDuoi = MatKhoi.KhoiTaoMatKhoi[matDuoi];
        toaDoMatXungQuanh = MatKhoi.KhoiTaoMatKhoi[matXungQuanh];
    }

    /*
     * Dùng luôn dictionary đểu lưu lại tên và các khởi tạo của block đó luôn
     * Truy xuất: Khoi.getKhoi[TenKhoi.ABC]
    */

    public static Dictionary<tenKhoi, Khoi> getKhoi = new Dictionary<tenKhoi, Khoi>()
    {
        {tenKhoi.Dirt,   new Khoi(tenMatKhoi.Dirt)},
        {tenKhoi.Stone,  new Khoi(tenMatKhoi.Stone)},
        {tenKhoi.Leaves, new Khoi(tenMatKhoi.Leaves)},
        {tenKhoi.Grass,  new Khoi(tenMatKhoi.Grass, tenMatKhoi.Dirt, tenMatKhoi.GrassSide)},
        {tenKhoi.Trunk,  new Khoi(tenMatKhoi.TreeCX, tenMatKhoi.TreeCX, tenMatKhoi.TreeSide)},

    };

}

/*Trunk la than cay go
 Nếu tại vị trí đó k có khối thì nó là không khí : Air
*/
public enum tenKhoi
{
    Dirt, Grass, Stone, Trunk, Leaves, Air
}