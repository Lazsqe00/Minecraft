using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SinhMap : MonoBehaviour
{
    //public GameObject block;

    public GameObject player;
    public GameObject MAP;
    int chieuDai = 16, chieuRong = 16, doCao = 255;

    private FastNoiseLite doCaoMap;
    private FastNoiseLite hangDong;

    public const int chieuDaiChunk = 16;
    public const int doCaoChunk = 64;
    private FastNoiseLite doCaoDa;
    List<ViTri> ViTriCanPhaiTao = new List<ViTri>();
    List<ViTri> ViTriDaBiXoa = new List<ViTri>();
    // Lưu danh sách chunk đã tạo 

    // Phải hiểu là 1 cái chunk là 1 cái gameObject nó là mảng chứa các chunk
    public static Dictionary<ViTri, Chunk> chunks = new Dictionary<ViTri, Chunk>();
    void Start()
    {
        //Tạo độ cao gồ ghề của MAP
        doCaoMap = new FastNoiseLite();
        doCaoMap.SetSeed(12345);
        doCaoMap.SetNoiseType(FastNoiseLite.NoiseType.Perlin); // Perlin
        ////set frequency nhỏ để noise thay đổi chậm
        doCaoMap.SetFrequency(0.1f);


        /*Sinh hang động*/
        hangDong = new FastNoiseLite();
        hangDong.SetSeed(12345);
        hangDong.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
        hangDong.SetFrequency(0.08f);

        // độ cao của ĐÁ
        doCaoDa = new FastNoiseLite();
        doCaoDa.SetSeed(54321); // seed khác với doCaoMap
        doCaoDa.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
        doCaoDa.SetFrequency(0.05f);


        KhoiTaoMap();

        //LoadChunk(true);
    }

    private void Update()
    {
        LoadChunk(false);
    }
    // HÀM KHỞI TẠO MAP THAM KHẢO
    void KhoiTaoMap()
    {
        /* 
         sinh ra tọa độ cho những vị trí liền nhau
         Vector3 cần tham số x, y, z trong đó x là chiều ngang, y chiều cao, z chiều sâu
        */
        for (int x = 0; x < chieuDai; x++)
        {
            for (int z = 0; z < chieuRong; z++)
            {
                /*
                 - Chuyển về độ cao của noise VD: 0.13, 0.12,... với từng tọa độ [x, y] 
                 - Các giá trị độ cao của noise (float) này chạy từ [-1, 1]
                 - noise là mức độ cao thấp tại điểm [x, z]
                */
                float noise = doCaoMap.GetNoise(x, z);

                // Chuyển giá trị của nó về [0, 1] chứ k dùng [-1, 1]
                float chuyenDoiNoise = (noise + 1f) / 2f;

                /*
                 Sinh ra chiều cao dựa trên khoảng chạy và làm tròn lên
                 Nếu noise = 0 => chuyển đổi là 0.5 => độ cao sẽ là 3                
                */

                int chieuCao = Mathf.RoundToInt(chuyenDoiNoise * doCao);

                /*
                  - Mathf.RoundToInt(0.5f) sẽ làm tròn xuống 0 -> phải lấy max với 1
                */
                chieuCao = Mathf.Max(chieuCao, 1);

                for (int y = 0; y < chieuCao; y++)
                {
                    /*Khi tạo hang cần tọa độ không gian 3D. Ta sẽ tạo mức độ tiếng ồn 3D
                    (nếu tiếng ồn càng nhỏ hang sẽ càng lớn) do đó ta chọn mức độ vừa phải 0.4. 
                    Và ràng buộc thêm ít nhất để có được hang động thì độ cao phải > 4
                    */
                    float mucDoTiengOn = hangDong.GetNoise(x, y, z);
                    if (mucDoTiengOn > 0.4f && y > 4) continue; // bỏ qua khối này



                    // vi tri phai lien tiep nhau
                    Vector3 viTri = new Vector3(x, y, z);
                    //Instantiate(block, viTri, Quaternion.identity);
                    // TẠM KHÓA 
                }
            }
        }
    }


    /*
     - Hàm quản lý việc tải MAP liên tục và tháo dở mấy phần MAP không cần thiết
        Logic: 
         + Tính vùng hiện tại người chơi đang đứng
         + Xét các vùng lân cận trong bán kính cho phép, nếu chưa tồn tại 
           để thêm vào danh sách cần sinh hoặc sinh ngay.
         + Xét các vùng quá xa => đưa vào danh sách cho biến mất.
         + Lưu ý khi cho biến mất các vùng quá xa, thì vẫn phải lưu lại các vùng đó để tái sử dụng lại.
        
        CÁCH SẼ LÀM
        - Ta sẽ vẽ theo chunk, mỗi chunk được định nghĩa là vẽ từ vị trí x đến vị trí z. Ví dụ 
        x = 2, z = 5 => Bắt đầu từ 2 ta vẽ 5 ô
        - Đầu tiên, ta tính vị trí của player đang đứng ở chunk nào theo x, và theo z làm tròn xuống.
        Ví dụ x = 34.2f và z  = 10.8f => chunk theo x = FLOOR(34 / 16) = 2, chunk theo z = 0
        
        - Vẽ các chunk xung quanh người chơi trong bán kính phía trước người chơi là 16 chunk,
        phía sau người chơi cũng 16 chunk

        - Khi vẽ chunk chú ý rằng, ngay ban đầu các chunk xung quanh người chơi phải vẽ ngay lập tức 
        nên biến veNgayLapTuc = True, còn lại thì cứ cho vào mảng để từ từ vẽ
        
        CÓ THỂ VIẾT HÀM RIÊNG 
           - Thêm những chunk nằm quá xa người chơi và mảng để xóa => xóa nó đi
           - Gỡ các chunk đang chờ tạo nằm trong mảng nhưng quá xa
    */

    // hàm build sẽ vẽ từ vị trí x đến vị trí z
    void BuildMap(int posX, int posZ)
    {
        Chunk chunkXZ;
        GameObject chunkGO = Instantiate(MAP, new Vector3(posX, 0, posZ), Quaternion.identity);
        chunkXZ = chunkGO.GetComponent<Chunk>();

        chunkXZ.Khois = new tenKhoi[chieuDaiChunk, doCaoChunk, chieuDaiChunk];

        for (int x = 0; x < chieuDaiChunk + 2; x++)
        {
            for (int z = 0; z < chieuDaiChunk + 2; z++)
            {
                for (int y = 0; y < doCaoChunk; y++)
                {
                    chunkXZ.Khois[x, y, z] = getTenKhoi(posX + x, y, posZ + z);
                }
            }
        }

        chunkXZ.DrawChunk();
        // lưu lại chunk vừa tạo
        chunks[new ViTri(posX, posZ)] = chunkXZ;
    }

    /* HÀM tìm độ cao maximum (nhỏ hơn hoặc bằng độ cao của chunk = 64) */
    int timDoCaoDiaHinh2D(FastNoiseLite doCaoDiaHinh, int x, int z)
    {
        /*
          - Chuyển về độ cao của noise VD: 0.13, 0.12,... với từng tọa độ [x, z] 
          - Các giá trị độ cao của noise (float) này chạy từ [-1, 1]
          - noise là mức độ cao thấp tại điểm [x, z]
          - Ta cần phải lấy độ cao MAX tại điểm [x, z] đó, bằng cách lấy độ cao
          khoảng chạy * độ cao tối đa của chunk
       */
        float noise = doCaoDiaHinh.GetNoise(x, z);

        // Chuyển giá trị của nó về [0, 1] chứ k dùng [-1, 1]
        float chuyenDoiNoise = (noise + 1f) / 2f;

        /*
         Sinh ra chiều cao dựa trên khoảng chạy và làm tròn lên
         Nếu noise = 0 => chuyển đổi là 0.5 => độ cao sẽ là 3                
        */

        int chieuCaoDiaHinh = Mathf.RoundToInt(chuyenDoiNoise * doCao);
        /*
          - Mathf.RoundToInt(0.5f) sẽ làm tròn xuống 0 -> phải lấy max với 1
        */
        chieuCaoDiaHinh = Mathf.Max(chieuCaoDiaHinh, 1); // y max khi ở tọa độ [x, z]

        return chieuCaoDiaHinh;
    }
    tenKhoi getTenKhoi(int x, int y, int z)
    {
        int chieuCaoMapTaiViTriXZ = timDoCaoDiaHinh2D(doCaoMap, x, z);
        int chieuCaoDaTaiViTriXZ = timDoCaoDiaHinh2D(doCaoDa, x, z);

        tenKhoi nameBlock = tenKhoi.Air;

        if (y < chieuCaoDaTaiViTriXZ) nameBlock = tenKhoi.Stone;
        else if (y <= chieuCaoMapTaiViTriXZ)
        {
            nameBlock = tenKhoi.Dirt;
            if (y == chieuCaoMapTaiViTriXZ) nameBlock = tenKhoi.Grass;
        }

        /*Khi tạo hang cần tọa độ không gian 3D. Ta sẽ tạo mức độ tiếng ồn 3D => đặt là mucDoHangDong
            (nếu tiếng ồn càng nhỏ hang sẽ càng lớn) do đó ta chọn mức độ vừa phải 0.4. 
            Và ràng buộc thêm ít nhất để có được hang động thì độ cao phải > 4
        */
        float mucDoHangDong = hangDong.GetNoise(x, y, z);
        if (mucDoHangDong > 0.4 && y > 0.4) nameBlock = tenKhoi.Air;
        return nameBlock;
    }
    ViTri getViTriPlayer()
    {
        int ViTriPlayerTheoX = Mathf.FloorToInt(player.transform.position.x / 16f);
        int ViTriPlayerTheoZ = Mathf.FloorToInt(player.transform.position.z / 16f);
        return new ViTri(ViTriPlayerTheoX, ViTriPlayerTheoZ);
    }


    void LoadChunk(bool loadNgayLapTuc)
    {
        ViTri viTriPlayer = getViTriPlayer();

        int ViTriPlayerTheoX = viTriPlayer.x;
        int ViTriPlayerTheoZ = viTriPlayer.y;

        // chieudaiMapQuyDinh phía trước và phía sau của nhân vật, là 1 chunk [16, 64, 16]
        int chieuDaiMapQuyDinh = (chieuDaiChunk * 16);
        //SINH MAP
        for (int i = (ViTriPlayerTheoX * 16) - chieuDaiMapQuyDinh; i <= (ViTriPlayerTheoX * 16) + chieuDaiMapQuyDinh; i += 16)
        {
            for (int j = (ViTriPlayerTheoZ * 16) - chieuDaiMapQuyDinh; j <= (ViTriPlayerTheoZ * (16)) + chieuDaiMapQuyDinh; j += 16)
            {
                if (loadNgayLapTuc)
                {
                    BuildMap(i, j);
                }
                else
                {
                    ViTri viTri = new ViTri(i, j);
                    ViTriCanPhaiTao.Add(viTri);
                }
            }
        }

        DeleteChunkCanPhaiTaoTooFar();
        DeleteChunksTooFar();

        StartCoroutine(buildChunkCanTao());
    }

    void DeleteChunkCanPhaiTaoTooFar()
    {
        ViTri viTriPlayer = getViTriPlayer();

        int ViTriPlayerTheoX = viTriPlayer.x;
        int ViTriPlayerTheoZ = viTriPlayer.y;

        List<ViTri> tmp = new List<ViTri>();
        foreach (var vt in ViTriCanPhaiTao)
        {
            if (Mathf.Abs(ViTriPlayerTheoX - vt.x) * 16 > 16 * (chieuDaiChunk + 3)
                || Mathf.Abs(ViTriPlayerTheoZ - vt.y) * 16 > 16 * (chieuDaiChunk + 3))
            {
                tmp.Add(vt); 
            }
        }
        foreach (var vt in tmp) ViTriCanPhaiTao.Remove(vt);
    }
    void DeleteChunksTooFar()
    {
        ViTri viTriPlayer = getViTriPlayer();

        int ViTriPlayerTheoX = viTriPlayer.x;
        int ViTriPlayerTheoZ = viTriPlayer.y;

        List<ViTri> ViTriCanXoa = new List<ViTri>();
        foreach (var kv in chunks)
        {
            ViTri vt = kv.Key;
            if (Mathf.Abs(ViTriPlayerTheoX - vt.x) * 16 > 16 * (chieuDaiChunk + 3)
                || Mathf.Abs(ViTriPlayerTheoZ - vt.y) * 16 > 16 * (chieuDaiChunk + 3))
            {
                ViTriCanXoa.Add(vt);
            }
        }

        foreach (var viTri in ViTriCanXoa)
        {
            chunks[viTri].gameObject.SetActive(false);
            ViTriDaBiXoa.Add(viTri);
            chunks.Remove(viTri);
        }
    }

    IEnumerator buildChunkCanTao()
    {
        while (ViTriCanPhaiTao.Count > 0)
        {
            BuildMap(ViTriCanPhaiTao[0].x, ViTriCanPhaiTao[0].y);
            ViTriCanPhaiTao.RemoveAt(0);

            yield return new WaitForSeconds(.2f);
        }

    }
}
