using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : SingletonMono<CameraController>
{   // 公共变量，用于在编辑器中指定玩家角色的Transform和摄像机平滑移动的速度
    public Transform tform; // 玩家角色的Transform
    public float smoothing; // 摄像机平滑移动的速度

    // 公共变量，用于设置摄像机移动的边界限制
    public Vector2 minPosition; // 摄像机移动的最小边界
    public Vector2 maxPosition; // 摄像机移动的最大边界

    void Start()
    {
        // 在游戏开始时查找并获取CameraShake组件的引用
        //GameController.cameraShake = GameObject.FindGameObjectWithTag("CameraShake").GetComponent<CameraShake>();
    }

    // Update is called once per frame
    void Update()
    {
        // LateUpdate函数将在Update函数之后调用，用于处理摄像机的跟随逻辑
    }

    void LateUpdate()
    {
        // 如果指定了玩家角色的Transform
        if (tform != null)
        {
            // 如果摄像机的位置不等于玩家角色的位置
            if (transform.position != tform.position)
            {
                // 计算目标位置
                Vector3 targetPos = tform.position;
                // 使用Mathf.Clamp函数限制目标位置的x和y值，使其在设定的最小和最大边界内
                targetPos.x = Mathf.Clamp(targetPos.x, minPosition.x, maxPosition.x);
                targetPos.y = Mathf.Clamp(targetPos.y, minPosition.y, maxPosition.y);

                // 使用Vector3.Lerp函数平滑地移动摄像机到目标位置
                transform.position = Vector3.Lerp(transform.position, targetPos, smoothing) + new Vector3(0, 0, -20.0f);
            }
        }
    }

    // 公共方法，用于设置摄像机的移动边界
    public void SetCamPosLimit(Vector2 minPos, Vector2 maxPos)
    {
        // 更新最小和最大边界
        minPosition = minPos;
        maxPosition = maxPos;
    }

    public void SetTform(Transform trans)
    {
        tform = trans;
    }

}
