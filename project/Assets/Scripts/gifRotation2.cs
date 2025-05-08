using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gifRotation2 : MonoBehaviour
{
    public float rotationSpeed = 40f; // 旋转速度，可在 Inspector 面板中调整

    void Update()
    {
        // 绕 Y 轴旋转，你可以根据需求修改旋转轴
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}
