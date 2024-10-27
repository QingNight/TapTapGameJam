using UnityEngine;

public class SecondMonsterController: MonsterController
{
    private SpriteRenderer spriteRenderer;

    [Range(0f, 1f)]
    public float invisibleAlpha = 0.0f; // ����ʱ��͸����
    [Range(0f, 1f)]
    public float visibleAlpha = 1.0f; // ����ʱ��͸����

    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetInvisible(); // ��ʼ״̬Ϊ����
    }

    public override void ByWead()
    {
        base.ByWead(); // ���ø���ķ���
        // ������״̬ʱ����
        SetVisible();
    }

    private void SetInvisible()
    {
        Color color = spriteRenderer.color;
        color.a = invisibleAlpha; // ʹ�ñ�¶��͸����ֵ
        spriteRenderer.color = color;
        Visible = false;
    }

    private void SetVisible()
    {
        Color color = spriteRenderer.color;
        color.a = visibleAlpha; // ʹ�ñ�¶��͸����ֵ
        spriteRenderer.color = color;
        Visible = true;
    }

    public override void Update()
    {
        base.Update();
        if (state == MonsterState.Idel || state == MonsterState.Attack)
        {
            SetInvisible(); // ����״̬��������
        }
    }
}
