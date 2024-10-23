using UnityEngine;

public enum MonsterState
{
    None = 0,//��״̬ 
    Idel,//Ѳ��
    Attack,//����or׷��
    Weak,//����
    ThrowItOut,//���ӳ���
    Die,//����
}




public class MonsterController : MonoBehaviour
{
    public MonsterState state = MonsterState.None;


    private Rigidbody2D rb;

    private BoxCollider2D coll;
    private SpriteRenderer sRender;
    public float speed = 1.0f;
    float MaxDistance = 10.0f;
    public float weadTimeCD = 1.0f; //����ʱ��
    float _weadTime = 1.0f; //����ʱ��

    public GameObject go_flashLightMask;

    Vector3 startPos ;
    int Dir = 1;

    // ����˽�б���JumpableGround������ΪLayerMask�����ڴ洢������Ծ�ĵ����
    [SerializeField] private LayerMask JumpableGround;

    // Start�����ڽű�ʵ�����󡢵�һ֡����ǰ������

    bool isInit = false;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        sRender = GetComponent<SpriteRenderer>();
        //anim = GetComponent<Animator>();

        MonManager.instance.monsters.Add(this);

        state = MonsterState.Idel;
        startPos = transform.position;

        isInit = true;

    }

    void Update()
    {
        if (!isInit) return;

        var playerDir = PlayerController.instance.Dir;
        if (go_flashLightMask != null)
        {
            var MaskPos = go_flashLightMask.transform.localPosition;
            go_flashLightMask.transform.localPosition = new Vector3(Mathf.Abs(MaskPos.x) * playerDir, MaskPos.y, MaskPos.x);
            if (playerDir < 0)
            {
                go_flashLightMask.gameObject.SetActive(PlayerController.instance.transform.position.x > this.transform.position.x);
            }
            else if (playerDir > 0)
            {
                go_flashLightMask.gameObject.SetActive(PlayerController.instance.transform.position.x < this.transform.position.x);
            }
        }

        switch (state)
        {
            case MonsterState.Idel:
                {
                    coll.isTrigger = false;
                    rb.gravityScale = 1.0f;
                    var distance =  Vector3.Distance(transform.position, startPos);
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
                    coll.isTrigger = true;
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
                    coll.isTrigger = false;
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

    }


    public void ByWead()
    {
        state = MonsterState.Weak;
        _weadTime = weadTimeCD;

    }


    // IsWall�������ڼ���ɫ�Ƿ���ǽ
    private bool IsWall()
    {
        var Center = coll.bounds.center;
        var Size = coll.bounds.size;
        var group = Physics2D.BoxCast(Center, Size, 0f, Dir * Vector2.right, .01f, JumpableGround);




        return group;
    }
    // IsGround�������ڼ���ɫ�Ƿ��ڵ�����
    private bool IsGround()
    {
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .075f, JumpableGround);
    }

    //��ץס
    public void PickUp( Transform transform)
    {
        //this.transform.parent = transform;
        rb.simulated = false;

        state = MonsterState.ThrowItOut;
        //this.transform.localPosition = Vector3.zero;
    }

    //�ӳ�
    public void ThrowItOut(float HSpeed,float VSpeed)
    {
        rb.simulated = true;
        rb.velocity = new Vector2(HSpeed, VSpeed);

    }
    //void OnCollisionEnter2D(Collision2D collision)
    //{
    //    Debug.LogError("!!!!!!");
    //}
    // ����ҽ�ɫ�뿪��������Χʱ����
}