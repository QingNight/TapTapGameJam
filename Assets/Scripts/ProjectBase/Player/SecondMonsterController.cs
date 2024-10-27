using UnityEngine;

public class SecondMonsterController: MonsterController
{
    private SpriteRenderer spriteRenderer;

    [Range(0f, 1f)]
    public float invisibleAlpha = 0.0f; // 隐身时的透明度
    [Range(0f, 1f)]
    public float visibleAlpha = 1.0f; // 现形时的透明度

    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetInvisible(); // 初始状态为隐身
    }

    public override void ByWead()
    {
        base.ByWead(); // 调用父类的方法
        // 在虚弱状态时现形
        SetVisible();
    }

    private void SetInvisible()
    {
        Color color = spriteRenderer.color;
        color.a = invisibleAlpha; // 使用暴露的透明度值
        spriteRenderer.color = color;
        Visible = false;
    }

    private void SetVisible()
    {
        Color color = spriteRenderer.color;
        color.a = visibleAlpha; // 使用暴露的透明度值
        spriteRenderer.color = color;
        Visible = true;
    }

    public override void Update()
    {
        base.Update();
        if (state == MonsterState.Idel || state == MonsterState.Attack)
        {
            SetInvisible(); // 其他状态保持隐身
        }
    }
}
