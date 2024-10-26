using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayerController : SingletonMono<PlayerController>
{

    private Rigidbody2D rb;
    private BoxCollider2D coll;
    private Animator anim;

    public float JumpSpeed = 8.0f;
    float MoveSpeed = 0.0f;
    float MoveChangeSpeed = 1.0f;
    public float MinMoveSpeed = 2.0f;
    public float MaxMoveSpeed = 8.0f;

    public float Level = 0f;

    public float Dir = 1.0f;

    public GameObject go_flashLight;
    public GameObject go_flashLightMask;
    Vector3 go_flashLightPos;
    Vector3 go_flashLightScale;
    [SerializeField] private LayerMask JumpableGround;
    [SerializeField] private LayerMask MonSterLayer;



    public float viewRadius = 5f;//视野距离
    [Range(0, 360)]
    public float viewAngle = 270f;//视野角度



    // 定义一个名为MoveState的枚举，包含idle（静止）、run（跑步）、jump（跳跃）、fall（下落）四个状态
    private enum PlayerState { idle, run, jump, fall, die }

    // Start方法在脚本实例化后、第一帧更新前被调用
    private void Start()
    {
        // 获取并赋值当前GameObject上的Rigidbody2D组件到rb变量
        rb = GetComponent<Rigidbody2D>();
        // 获取并赋值当前GameObject上的BoxCollider2D组件到coll变量
        coll = GetComponent<BoxCollider2D>();
        // 获取并赋值当前GameObject上的Animator组件到anim变量
        anim = GetComponent<Animator>();
        MoveChangeSpeed = (MaxMoveSpeed - MinMoveSpeed) / (5.0f * 60.0f);
        go_flashLightPos = go_flashLight.transform.localPosition;

        go_flashLightScale = go_flashLight.transform.localScale;
        go_flashLight.transform.localPosition = new Vector3((0.5f + viewRadius / 2) * Dir, go_flashLightPos.y, go_flashLightPos.z);
        go_flashLightScale = go_flashLight.transform.localScale;


        go_flashLight.transform.localScale = new Vector3(viewRadius, go_flashLightScale.y, go_flashLightScale.z);
        go_flashLightScale = go_flashLight.transform.localScale;

        go_flashLightMask.transform.GetChild(0).transform.localScale = new Vector3(viewRadius + 0.5f, 1, 1);
        go_flashLightMask.transform.GetChild(0).transform.localPosition = new Vector3((viewRadius + 0.5f) / 2 - 0.5f, 0, 0);

    }

    public float _time = 1.0f;

    bool haveMonster = false;
    MonsterController weadMonster;

    // Update方法每帧调用一次
    private void Update()
    {
        _time -= Time.deltaTime;

        if (_time < 0.0f)
        {
            _time = 0.5f;
        }

        if (state == PlayerState.die) return;



        // 获取水平方向的输入值，不进行平滑处理
        Level = Input.GetAxisRaw("Horizontal");
        if (Level == 0)
        {
            MoveSpeed -= (MoveChangeSpeed * 1.20f);
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
        //rb.velocity = new Vector2(Dir * MoveSpeed, rb.velocity.y);

        if (Input.GetButtonDown("Jump") && (IsGround() || IsMonster()))
        {
            rb.velocity = new Vector2(rb.velocity.x, JumpSpeed);
        }
        if (go_flashLight != null)
        {
            if (Input.GetKey(KeyCode.C))
            {
                if (Dir != 0)
                {
                    go_flashLight.gameObject.SetActive(true);
                    var pos = go_flashLight.transform.localPosition;
                    go_flashLight.transform.localPosition = new Vector3(3 * Dir, go_flashLightPos.y, go_flashLightPos.z);
                }
            }
            //else
            //{
            //    go_flashLight.gameObject.SetActive(false);
            //}
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (haveMonster)
            {
                if (weadMonster != null)
                {
                    haveMonster = false;
                    weadMonster.ThrowItOut(Dir * MoveSpeed * 1.1f, JumpSpeed);
                    weadMonster = null;
                }
            }
            else
            {
                weadMonster = MonManager.instance.GetWeadMonster(this.transform);
                if (weadMonster != null)
                {
                    weadMonster.PickUp(this.transform.GetChild(0));
                    haveMonster = true;
                }
            }
        }
        if (weadMonster != null)
        {
            weadMonster.transform.position = this.transform.GetChild(0).position;
        }
        if (Input.GetKey(KeyCode.F))
        {
            DrawFieldOfView();
        }

        DrawFieldOfView();

        // 调用UpdateStates方法来更新动画状态
        UpdateStates();
    }
    // 定义一个MoveState类型的变量state
    PlayerState state;

    private void UpdateStates()
    {
        if (Level == 0f)
        {
            state = PlayerState.idle;
        }
        else
        {
            state = PlayerState.run;
            this.gameObject.GetComponent<SpriteRenderer>().flipX = Level > 0f ? false : true;
        }
        if (rb.velocity.y > 0.1f)
        {
            state = PlayerState.jump;
        }
        else if (rb.velocity.y < -0.1f)
        {
            state = PlayerState.fall;
        }
        anim.SetInteger("state", (int)state);
    }

    // IsGround方法用于检测角色是否在地面上
    private bool IsGround()
    {
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
    private bool IsMonster()
    {
        // 使用Physics2D.BoxCast方法进行射线投射，检测角色下方是否有死掉的怪物
        var obj = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .075f, MonSterLayer);
        if (obj.collider.transform.tag == "Monster")
        {
            Debug.LogError(obj.collider.transform.tag);
            var monster = obj.collider.GetComponent<MonsterController>();
            return monster.state == MonsterState.Die;
        }
        return false;
    }
    void DrawFieldOfView()
    {
        var Center = coll.bounds.center + new Vector3(Dir * 1.0f, 0.05f, 0);
        var sub = viewAngle / 2;
        List<Transform> hitList = new List<Transform>();
        for (int i = 0; i < viewAngle; i++)
        {
            var quaternion = Quaternion.Euler(0, 0, i * 1.0f - sub);// new Vector2(, 1.0f) * new Vector2(1.0f, 1.0f) * 3.0f
            Debug.DrawRay(Center, quaternion * new Vector3(Dir * 1.0f, 0.0f, 0.0f) * viewRadius, Color.red, 0.05f);
            var hit2D = Physics2D.Raycast(Center, quaternion * new Vector3(Dir * 1.0f, 0.0f, 0.0f), viewRadius, MonSterLayer);
            if (hit2D.transform != null)
            {
                if (!hitList.Contains(hit2D.transform))
                    hitList.Add(hit2D.transform);
            }
        }
        int index = 0;
        var maskCount = go_flashLight.transform.childCount - 1;
        for (int i = 0; i < MathF.Max(hitList.Count, maskCount); i++)
        {
            if (i < hitList.Count && i < maskCount)
            {
                var hit = hitList[index];
                var obj = go_flashLight.transform.GetChild(index + 1);
                obj.position = hit.position;
                var scale = go_flashLightMask.transform.localScale;
                obj.gameObject.SetActive(true);
                //obj.SetParent(hit.transform);
                //obj.localPosition = Vector3.zero;
                //go_flashLight.transform.GetChild(index).localScale = new(scale.x, scale.y, scale.z);
                //obj.SetParent(go_flashLight.transform);
            }
            if (i < hitList.Count && i >= maskCount)
            {
                var hit = hitList[index];
                var obj = GameObject.Instantiate(go_flashLightMask.transform, go_flashLight.transform);
                obj.position = hit.position;
                var scale = go_flashLightMask.transform.localScale;
                obj.localScale = new Vector3(scale.x / go_flashLightScale.x, scale.y / go_flashLightScale.y, scale.z / go_flashLightScale.z);
                obj.gameObject.SetActive(true);
                //obj.SetParent(hit.transform);
                //obj.localPosition = Vector3.zero;
                //var scale = go_flashLight.transform.GetChild(index).localScale;
                //go_flashLight.transform.GetChild(index).localScale = new(1, scale.y, scale.z);
                //obj.SetParent(go_flashLight.transform);
            }
            index++;
            if (i >= hitList.Count && i < maskCount)
            {
                go_flashLight.transform.GetChild(i + 1).gameObject.SetActive(false);
            }

        }
        go_flashLight.gameObject.SetActive(true);
        go_flashLight.transform.localPosition = new Vector3((1.0f + viewRadius / 2) * Dir, go_flashLightPos.y + 0.05f, go_flashLightPos.z);
        go_flashLight.transform.localScale = new Vector3(viewRadius * Dir, go_flashLightScale.y, go_flashLightScale.z);
        //if (index != 0)
        //{
        //}
        //else
        //{
        //    go_flashLight.SetActive(false);
        //}
        //Debug.LogError($"{hitList.Count}");
    }



    public void Die(Collision2D collision)
    {
        Coroutine coroutine = StartCoroutine(DieAnim(1.8f));

    }
    IEnumerator DieAnim(float _time = 1.0f)
    {
        //死了
        coll.isTrigger = true;
        rb.velocity = new Vector2(rb.velocity.x, 5.0f);
        UIMainView.instance.DieShow(true);
        CameraController.instance.SetTform(null);
        state = PlayerState.die;
        MoveSpeed = 0.0f;

        yield return new WaitForSeconds(_time);

        //复活
        this.transform.localPosition = Vector3.zero;
        coll.isTrigger = false;
        UIMainView.instance.DieShow(false);
        CameraController.instance.SetTform(this.transform);
        state = PlayerState.idle;
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Monster")
        {
            var monster = collision.transform.GetComponent<MonsterController>();
            if (monster.state == MonsterState.Weak || monster.state == MonsterState.Die)
            {

                return;
            }
            Die(collision);
            Debug.LogError("YOU Die");
        }
    }

}
