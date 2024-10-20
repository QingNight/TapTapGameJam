using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : SingletonMono<CameraController>
{   // ���������������ڱ༭����ָ����ҽ�ɫ��Transform�������ƽ���ƶ����ٶ�
    public Transform tform; // ��ҽ�ɫ��Transform
    public float smoothing; // �����ƽ���ƶ����ٶ�

    // ������������������������ƶ��ı߽�����
    public Vector2 minPosition; // ������ƶ�����С�߽�
    public Vector2 maxPosition; // ������ƶ������߽�

    void Start()
    {
        // ����Ϸ��ʼʱ���Ҳ���ȡCameraShake���������
        //GameController.cameraShake = GameObject.FindGameObjectWithTag("CameraShake").GetComponent<CameraShake>();
    }

    // Update is called once per frame
    void Update()
    {
        // LateUpdate��������Update����֮����ã����ڴ���������ĸ����߼�
    }

    void LateUpdate()
    {
        // ���ָ������ҽ�ɫ��Transform
        if (tform != null)
        {
            // ����������λ�ò�������ҽ�ɫ��λ��
            if (transform.position != tform.position)
            {
                // ����Ŀ��λ��
                Vector3 targetPos = tform.position;
                // ʹ��Mathf.Clamp��������Ŀ��λ�õ�x��yֵ��ʹ�����趨����С�����߽���
                targetPos.x = Mathf.Clamp(targetPos.x, minPosition.x, maxPosition.x);
                targetPos.y = Mathf.Clamp(targetPos.y, minPosition.y, maxPosition.y);

                // ʹ��Vector3.Lerp����ƽ�����ƶ��������Ŀ��λ��
                transform.position = Vector3.Lerp(transform.position, targetPos, smoothing) + new Vector3(0, 0, -20.0f);
            }
        }
    }

    // ��������������������������ƶ��߽�
    public void SetCamPosLimit(Vector2 minPos, Vector2 maxPos)
    {
        // ������С�����߽�
        minPosition = minPos;
        maxPosition = maxPos;
    }

    public void SetTform(Transform trans)
    {
        tform = trans;
    }

}
