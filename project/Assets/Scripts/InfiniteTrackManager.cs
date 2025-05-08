using UnityEngine;

public class InfiniteTrackGenerator : MonoBehaviour
{
    public GameObject mainCamera; // 相机对象
    public GameObject trackPrefab; // 原点在起点的预制体
    public float trackLength = 500f; // 跑道实际长度
    private float lastTrackEndX;      // 当前最后一个跑道的终点X坐标
    private GameObject[] trackInstances = new GameObject[2];
    private int currentTrackIndex = 0;
    

    void Start()
    {
        lastTrackEndX = 0; // 初始起点在原点
        GenerateInitialTracks();
    }

    void GenerateInitialTracks()
    {
        // 生成第一个跑道：起点在原点 (0,0,0)
        trackInstances[0] = Instantiate(trackPrefab, new Vector3(lastTrackEndX, 0, 0), Quaternion.identity);
        lastTrackEndX += trackLength; // 更新终点到第一个跑道的右侧末端
        //Debug.Log($"当前末端X：{lastTrackEndX}");

        // 生成第二个跑道：起点在第一个跑道的终点
        trackInstances[1] = Instantiate(trackPrefab, new Vector3(lastTrackEndX, 0, 0), Quaternion.identity);
        lastTrackEndX += trackLength; // 更新终点到第二个跑道的右侧末端
        //Debug.Log($"当前末端X：{lastTrackEndX}");
        //Debug.Log($"要超过X：{lastTrackEndX - trackLength}");
    }

    void Update()
    {
        float cameraX = mainCamera.transform.position.x;

        // 当相机位置超过当前终点的左侧时，生成新跑道（向X轴正向延伸）
        if (cameraX > lastTrackEndX - trackLength)
        {
            //Debug.Log($"玩家位置超过lastTrackEndX - trackLength");
            SpawnTrack();
        }
    }

    void SpawnTrack()
    {
        //Debug.Log($"进入销毁跑道函数");
        // 销毁最旧的跑道
        if (trackInstances[currentTrackIndex] != null)
        {
            Destroy(trackInstances[currentTrackIndex]);
        }

        // 生成新跑道：起点对齐当前终点
        trackInstances[currentTrackIndex] = Instantiate(trackPrefab, new Vector3(lastTrackEndX, 0, 0), Quaternion.identity);
        lastTrackEndX += trackLength; // 更新终点
        //Debug.Log($"当前末端X：{lastTrackEndX}");

        // 循环索引
        currentTrackIndex = (currentTrackIndex + 1) % 2;

        //Debug.Log($"新跑道起点：{lastTrackEndX - trackLength}，终点：{lastTrackEndX}");
    }
}