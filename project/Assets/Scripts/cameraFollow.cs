using UnityEngine;

public class MixamoController : MonoBehaviour
{
    public Transform cameraTransform;      // 拖入主相机
    public Vector3 cameraOffset = new Vector3(0, 5f, -7f); // 相机偏移（人物的局部后方）
    public float smoothSpeed = 5f;         // 跟随平滑度

    void LateUpdate()
    {
        if (cameraTransform != null)
        {
            // 关键修改：将偏移量乘以人物自身的朝向
            Vector3 targetPosition = transform.position + transform.rotation * cameraOffset;  // 相机位置

            cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetPosition, smoothSpeed * Time.deltaTime);
            
            // 摄像机看向人物的上半身
            cameraTransform.LookAt(transform.position + Vector3.up * 1.5f);
        }
    }
}
