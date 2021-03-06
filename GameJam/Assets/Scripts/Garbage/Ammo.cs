﻿using UnityEngine;
using UnityEngine.UI;

public class Ammo : PoolObject
{
    public int player;
    public int damage;
    public Rigidbody2D Rigidbody2D;
    public Image Image;
    public Sprite[] Sprites;
    public GameObject garbage;
    public RectTransform transform;

    private void Awake()
    {
        Initialize(Players.Player1, Robots.Robot1);
    }

    public void Initialize(Players _player, Robots whichRobot)
    {
        player = (int) _player;
        GameManager.RobotParameter rp = GameManager.Instance.RobotParameters[whichRobot];
        damage = rp.AmmoDamage;
        Rigidbody2D.mass = rp.AmmoMass;
        Rigidbody2D.drag = rp.AmmoDrag;
        transform.localScale *= rp.AmmoScale;
        Image.sprite = Sprites[Random.Range(0, 3)];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerBody pb = collision.gameObject.GetComponent<PlayerBody>();
        if (collision.transform.CompareTag("Player") && player != (int) pb.WhichPlayer && !pb.Lying)
        {
            pb.Hitted(damage);
            pb.ShowEmoji(PlayerBody.Emojis.Hitted, 0.3f);

            /*int temp = (int)Random.Range(6, 9);
            collision.gameObject.GetComponent<PlayerBody>().Loss_Garbage(temp);
            */
            Vector2 vector2 = new Vector2(-transform.up.x, -transform.up.y);
            for (int i = 0; i < damage;i++)
            {
                Drop(vector2);
            }
            
            SoundPlay("sfx/ShootHit");
            Rigidbody2D.velocity = Vector3.zero;
            PoolRecycle();
        }

        if (collision.transform.CompareTag("Wall"))
        {
            Vector2 vector2 = Vector2.zero;
            switch (collision.name)
            {
                case "Wall_left":
                    vector2.x = 1;
                    break;
                case "Wall_right":
                    vector2.x = -1;
                    break;
                case "Wall_up":
                    vector2.y = -1;
                    break;
                case "Wall_down":
                    vector2.y = 1;
                    break;
                default: break;
            }

            for (int i = 0; i < damage; i++)
            {
                Drop(vector2);
            }
            Rigidbody2D.velocity = Vector3.zero;
            PoolRecycle();
        }
    }

    void Drop(Vector2 vector2)
    {
        GarbageMain am = GameObjectPoolManager.Instance.Pool_Garbage.AllocateGameObject<GarbageMain>(GameBoardManager.Instance.GameBoardGarbagesCanvas.transform);
        am.Initialize();
        am.CanPick = true;
        am.transform.position = transform.position;




        am.Rigidbody2D.velocity = RotateVector2(vector2.normalized, Random.Range(-60, 60)) * Random.Range(100, 200);
        
        am.Initialize();
    }

    Vector2 RotateVector2(Vector2 v,float angle)
    {
        float x = v.x;
        float y = v.y;
        float sin = Mathf.Sin(Mathf.PI * angle / 180);
        float cos = Mathf.Cos(Mathf.PI * angle / 180);
        float newX = x * cos + y * sin;
        float newY = x * -sin + y * cos;
        return new Vector2((float)newX, (float)newY);
    }
}