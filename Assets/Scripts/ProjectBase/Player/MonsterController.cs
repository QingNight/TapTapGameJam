using UnityEngine;

public enum MonsterState
{
    Idel,//巡逻
    Attack,//攻击or追击
    Weak,//虚弱
    ThrowItOut,//被扔出中
    Die,//死亡
}




public class MonsterController : MonoBehaviour
{
    public MonsterState state = MonsterState.Idel;


    protected Rigidbody2D rb;
    protected bool Visible = true;
    protected BoxCollider2D coll;
    private Animator anim;
    protected SpriteRenderer sRender;
    public float speed = 1.0f;
    float MaxDistance = 10.0f;
    public float weadTimeCD = 3.0f; //虚弱时间
    float _weadTime = 1.0f; //虚弱时间


    Vector3 startPos;
    int Dir = 1;

    // 定义私有变量JumpableGround，类型为LayerMask，用于存储可以跳跃的地面层
    [SerializeField] protected LayerMask JumpableGround;

    // Start方法在脚本实例化后、第一帧更新前被调用

    bool isInit = false;
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        sRender = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        MonManager.Instance.monsters.Add(this);

        state = MonsterState.Idel;
        startPos = transform.position;

        isInit = true;

    }

    public virtual void Update()
    {
        if (!isInit) return;

        var playerDir = PlayerController.instance.Dir;

        switch (state)
        {
            case MonsterState.Idel:
                {
                    coll.isTrigger = !Visible;
                    rb.gravityScale = 1.0f;
                    var distance = Vector3.Distance(transform.position, startPos);
                    if (distance > MaxDistance || IsWall())
                    {
                        Dir = this.transform.position.x > startPos.x ? -1 : 1;
                    }

                    this.transform.position =
                        this.transform.position + speed * new Vector3(1.0f, 0, 0) * Dir * Time.deltaTime;
                }
                break;
            case MonsterState.Attack:
                {
                    coll.isTrigger = false;
                    rb.gravityScale = 1.0f;
                }
                break;
            case MonsterState.Weak:
                {
                    coll.isTrigger = !Visible;
                    rb.gravityScale = 0.0f;
                    _weadTime -= Time.deltaTime;
                    if (_weadTime < 0)
                    {
                        state = MonsterState.Idel;
                        sRender.color = new Color(sRender.color.r, sRender.color.g, sRender.color.b, (0.5f + (weadTimeCD - _weadTime) / weadTimeCD));
                    }
                }
                break;
            case MonsterState.ThrowItOut:
                {
                    coll.isTrigger = !Visible;
                    rb.gravityScale = 1.0f;
                    if (IsGround())
                    {
                        state = MonsterState.Idel;
                    }
                }
                break;
            case MonsterState.Die:
                {
                    coll.isTrigger = false;
                    rb.gravityScale = 0.0f;
                }
                break;
        }

        this.gameObject.GetComponent<SpriteRenderer>().flipX = Dir > 0f ? true : false;
        anim.SetInteger("state", (int)state);

    }


    public virtual void ByWead()
    {
        state = MonsterState.Weak;
        _weadTime = weadTimeCD;

    }


    // IsWall方法用于检测角色是否碰墙
    protected bool IsWall()
    {
        var Center = coll.bounds.center;
        var Size = coll.bounds.size;
        var group = Physics2D.BoxCast(Center, Size, 0f, Dir * Vector2.right, .01f, JumpableGround);




        return group;
    }
    // IsGround方法用于检测角色是否在地面上
    protected bool IsGround()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .075f, JumpableGround);
    }

    //被抓住
    public void PickUp(Transform transform)
    {
        //this.transform.parent = transform;
        rb.simulated = false;

        state = MonsterState.ThrowItOut;
        //this.transform.localPosition = Vector3.zero;
    }

    //扔出
    public void ThrowItOut(float HSpeed, float VSpeed)
    {
        rb.simulated = true;
        rb.velocity = new Vector2(HSpeed, VSpeed);

    }
    //void OnCollisionEnter2D(Collision2D collision)
    //{
    //    Debug.LogError("!!!!!!");
    //}
    // 当玩家角色离开触发器范围时调用
}
