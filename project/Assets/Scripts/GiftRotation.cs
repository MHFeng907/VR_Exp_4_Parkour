using UnityEngine;

public class GiftRotation : MonoBehaviour
{
    public float rotationSpeed = 30f; // 旋转速度，可在 Inspector 面板中调整

    void Update()
    {
        // 绕 Y 轴旋转，你可以根据需求修改旋转轴
        transform.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);
    }
}    