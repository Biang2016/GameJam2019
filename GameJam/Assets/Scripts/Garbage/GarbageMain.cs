﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class GarbageMain : PoolObject
{
    private bool can_Pick;

    public bool CanPick
    {
        get { return can_Pick; }
        set
        {
            can_Pick = value;
            circle.isTrigger = can_Pick;
        }
    }

    public int num_;
    public CircleCollider2D circle;
    public Rigidbody2D Rigidbody2D;
    public Image Image;
    public SpriteAtlas GarbageSpriteAtlas;
    public static Sprite[] Sprites;
    float time = 0;

    public override void PoolRecycle()
    {
        Rigidbody2D.drag = 0;
        CanPick = false;
        base.PoolRecycle();
    }

    private void Awake()
    {
        Sprites = new Sprite[GarbageSpriteAtlas.spriteCount];
        GarbageSpriteAtlas.GetSprites(Sprites);
        CanPick = false;
    }

    void Start()
    {
    }

    void Update()
    {
        if (GameBoardManager.Instance.M_StateMachine.GetState() == GameBoardManager.StateMachine.States.Hide) return;
        if (!CanPick && Rigidbody2D.velocity.magnitude < GameManager.Instance.GarbageBulletBeLitterSpeedThreshold)
        {
            Rigidbody2D.drag = 100000;
            CanPick = true;
            SoundPlay("sfx/ShootMiss", 0.3f);
        }

        if (!CanPick)
        {
            time += Time.deltaTime;
            if (time > 2f)
            {
                CanPick = true;
            }
        }
    }

    public void Initialize()
    {
        Rigidbody2D.drag = 0.5f;
        int index = Random.Range(0, Sprites.Length);
        Image.sprite = Sprites[index];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerBody>().Pick_Garbage(num_);
            PoolRecycle();
        }
        if(collision.CompareTag("Wall"))
        {
            Rigidbody2D.velocity = Vector2.zero;
        }
    }
}