﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    public PlayerBody PlayerBody;
    public Transform shoot_point;
    public float shoot_speed;
    public int damage;

    public void Initialize()
    {
        GameManager.RobotParameter rp = GameManager.Instance.RobotParameters[(Robots) PlayerBody.WhichRobot];
        shoot_speed = rp.Shoot_Speed;
        damage = rp.AmmoDamage;
        ShootCD = rp.Shoot_CD;
    }

    public float ShootCD = 1.0f;
    private float shootTick = 0;

    void Update()
    {
        if (GameBoardManager.Instance.M_StateMachine.GetState() == GameBoardManager.StateMachine.States.Hide)
        {
            return;
        }

        shootTick += Time.deltaTime;
        if (Input.GetButtonDown(PlayerBody.Index_name + "fire") && !PlayerBody.Lying)
        {
            if (shootTick > ShootCD)
            {
                shootTick = 0f;
                if (PlayerBody.Trash >= damage)
                {
                    Fire();
                }
            }
        }
    }

    void Fire()
    {
        PlayerBody.Loss_Garbage(damage);
        Vector3 dir = shoot_point.position - transform.position;
        dir.Normalize();
        int sfx_index = Random.Range(0, 2);
        PlayerBody.SoundPlay("sfx/HitFromFar_" + sfx_index, 0.5f);
        Ammo am = GameObjectPoolManager.Instance.Pool_GarbageLitter.AllocateGameObject<Ammo>(GameBoardManager.Instance.GameBoardGarbagesCanvas.transform);
        am.transform.position = shoot_point.transform.position;
        am.transform.rotation = shoot_point.transform.rotation;
        am.Rigidbody2D.velocity = dir * shoot_speed;
        am.Initialize(PlayerBody.WhichPlayer, PlayerBody.WhichRobot);
        PlayerBody.ShootParticleSystem.gameObject.SetActive(true);
        PlayerBody.ShootParticleSystem.Play();
    }
}