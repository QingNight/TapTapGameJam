 using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

public class PlayerController :SingletonMono<PlayerController>
{
    // 定义私有变量rb，类型为Rigidbody2D，用于存储刚体组件的引用
    private Rigidbody2D rb;
    // 定义私有变量coll，类型为BoxCollider2D，用于存储碰撞体组件的引用
    private BoxCollider2D coll;
    // 定义私有变量anim，类型为Animator，用于存储动画控制器组件的引用
    private Animator anim;
    // 定义公共变量JumpSpeed，类型为float，用于存储跳跃速度
    public float JumpSpeed = 8.0f;
    // 定义公共变量MoveSpeed，类型为float，用于存储移动速度
    public float MoveSpeed = 0.0f;
    float MoveChangeSpeed = 1.0f;
    public float MinMoveSpeed = 2.0f;
    public float MaxMoveSpeed = 8.0f;
    // 定义公共变量Level，类型为float，用于存储水平输入值
    public float Level = 0f;
    // 定义公共变量Level，类型为float，用于存储水平输入值
    float Dir = 0f;

    // 定义私有变量JumpableGround，类型为LayerMask，用于存储可以跳跃的地面层
    [SerializeField] private LayerMask JumpableGround;

    // 定义一个名为MoveState的枚举，包含idle（静止）、run（跑步）、jump（跳跃）、fall（下落）四个状态
    private enum MoveState { idle, run, jump, fall }

    // Start方法在脚本实例化后、第一帧更新前被调用
    private void Start()
    {
        // 获取并赋值当前GameObject上的Rigidbody2D组件到rb变量
        rb = GetComponent<Rigidbody2D>();
        // 获取并赋值当前GameObject上的BoxCollider2D组件到coll变量
        coll = GetComponent<BoxCollider2D>();
        // 获取并赋值当前GameObject上的Animator组件到anim变量
        //anim = GetComponent<Animator>();
        MoveChangeSpeed = (MaxMoveSpeed - MinMoveSpeed) / (5.0f * 60.0f);
    }

    public float _time = 1.0f;

    float _MonsterCDTime = 5.0f;
    bool _CanUP = true;
    bool haveMonster = false;


    // Update方法每帧调用一次
    private void Update()
    {
        _time -= Time.deltaTime;
        _MonsterCDTime -= Time.deltaTime;

        if (_time < 0.0f)
        {
            _time = 0.5f;
        }
        if (_MonsterCDTime < 0.0f)
        {
            _CanUP = true;
            _MonsterCDTime = 5.0f;
        }

        // 获取水平方向的输入值，不进行平滑处理
        Level = Input.GetAxisRaw("Horizontal");

        if (Level == 0)
        {
            //MoveSpeed = 0.0f;
            MoveSpeed  -= (MoveChangeSpeed * 1.20f);
            if (MoveSpeed < 0)
            {
                MoveSpeed = 0.0f;
            }
        }
        else 
        {
            Dir = Level;
            MoveSpeed += MoveChangeSpeed;
            MoveSpeed = Mathf.Clamp(MoveSpeed, MinMoveSpeed, MaxMoveSpeed);
        }


        // 设置角色的水平速度为Level乘以MoveSpeed，垂直速度保持不变
        rb.velocity = new Vector2(Dir * (IsWall() ? 0 : MoveSpeed), rb.velocity.y);
        //rb.velocity = new Vector2(Dir * MoveSpeed, rb.velocity);

        // 如果按下跳跃按钮并且角色在地面上
        if (Input.GetButtonDown("Jump") && IsGround())
        {
            // 设置角色的垂直速度为JumpSpeed，水平速度保持不变
            rb.velocity = new Vector2(rb.velocity.x, JumpSpeed);
        }
        bool fire = false;
        if (Input.GetKeyDown(KeyCode.E) && haveMonster)
        {
            haveMonster = false;
            fire = true;
            MonsterController.GetInstance().ThrowItOut(Dir * MoveSpeed, JumpSpeed);
        }
        // 
        if (Input.GetKeyDown(KeyCode.E) && _CanUP && !haveMonster && !fire)
        {
            _CanUP = false;
            haveMonster = true;
            MonsterController.GetInstance().PickUp(this.transform.GetChild(0));
        }
        // 调用UpdateStates方法来更新动画状态
        UpdateStates();
    }
    // 定义一个MoveState类型的变量state
    MoveState state;

    // UpdateStates方法用于更新角色的动画状态
    private void UpdateStates()
    {

        // 根据水平输入值翻转角色
        if (Level > 0f)
        {
            // 如果输入值大于0，设置状态为run（跑步），并设置角色不翻转
            state = MoveState.run;
            this.gameObject.GetComponent<SpriteRenderer>().flipX = false;
        }
        else if (Level < 0f)
        {
            // 如果输入值小于0，设置状态为run（跑步），并设置角色翻转
            state = MoveState.run;
            this.gameObject.GetComponent<SpriteRenderer>().flipX = true;
        }
        else
        {
            // 如果没有水平输入，设置状态为idle（静止）
            state = MoveState.idle;
        }
        // 根据角色的垂直速度设置跳跃或下落状态
        if (rb.velocity.y > .1f)
        {
            // 如果垂直速度大于0.1，设置状态为jump（跳跃）
            state = MoveState.jump;
        }
        else if (rb.velocity.y < -.1f)
        {
            // 如果垂直速度小于-0.1，设置状态为fall（下落）
            state = MoveState.fall;
        }
        // 使用Animator组件设置名为"state"的整数参数，值为state的枚举整数值
        //anim.SetInteger("state", (int)state);
    }

    // IsGround方法用于检测角色是否在地面上
    private bool IsGround()
    {
        // 使用Physics2D.BoxCast方法进行射线投射，检测角色下方是否有可以跳跃的地面层
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .075f, JumpableGround);
    }

    // IsWall方法用于检测角色是否碰墙
    private bool IsWall()
    {
        var Center = coll.bounds.center;
        var Size = coll.bounds.size;
        var group = Physics2D.BoxCast(Center, Size, 0f, Dir * Vector2.right, .01f, JumpableGround);




        return group;
    }
}