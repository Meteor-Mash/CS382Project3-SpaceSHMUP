using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Hero : MonoBehaviour
{
    static public Hero S { get; private set; }

    [Header("Inscribed")]
    public float speed = 30;
    public float rollMult = -45;
    public float pitchMult = 30;
    public GameObject projectilePrefab;
    public float projectileSpeed = 40;
    public Weapon[] weapons;
    private Color[] rainbow = {Color.red, Color.yellow, Color.green, Color.blue};

    [Header("Dynamic")] [Range(0, 4)]
    private float _shieldLevel = 1;
    private GameObject lastTriggerGo = null;
    public delegate void WeaponFireDelegate();
    public event WeaponFireDelegate fireEvent;
    private bool starUp = false;
    private Material[] materials;
    private Color[] originalColors;
    private int currentColor = 0;

    public float shieldLevel
    {
        get { return _shieldLevel; }
        private set
        {
            _shieldLevel = Mathf.Min(value, 4);
            if (value < 0)
            {
                Destroy(this.gameObject);
                Main.HERO_DIED();
            }
        }
    }

    void Awake()
    {
        if (S == null)
        {
            S = this;
        }
        else
        {
            Debug.LogError("Hero.Awake() - Attempted to assign second Hero.S");
        }

        ClearWeapons();
        weapons[0].SetType(eWeaponType.blaster);

        materials = Utils.GetAllMaterials(gameObject);
        originalColors = new Color[materials.Length];
        for (int i = 0; i < materials.Length; ++i)
        {
            originalColors[i] = materials[i].color;
        }
    }


    void Update()
    {
        float hAxis = Input.GetAxis("Horizontal");
        float vAxis = Input.GetAxis("Vertical");

        Vector3 pos = transform.position;
        pos.x += hAxis * speed * Time.deltaTime;
        pos.y += vAxis * speed * Time.deltaTime;
        transform.position = pos;

        transform.rotation = Quaternion.Euler(vAxis * pitchMult, hAxis * rollMult, 0);

        if (Input.GetAxis("Jump") == 1 && fireEvent != null)
        {
            fireEvent();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;

        if (go == lastTriggerGo) return;
        lastTriggerGo = go;

        Enemy enemy = go.GetComponent<Enemy>();
        if (enemy != null)
        {
            if (!starUp) {
                --shieldLevel;
            }
            Destroy(go);
            return;
        }
        PowerUp pUp = go.GetComponent<PowerUp>();
        if (pUp != null)
        {
            AbsorbPowerUp(pUp);
            return;
        }
        Debug.LogWarning("Shield trigger hi by non-Enemy, non-PowerUp: " + go.name);
        
    }

    public void AbsorbPowerUp(PowerUp pUp)
    {
//        Debug.Log("Absorbed PowerUp:" + pUp.type);
        switch (pUp.type)
        {
            case eWeaponType.shield:
                ++shieldLevel;
                break;

            case eWeaponType.star:
            StarStart();
                break;

            default:
                if (pUp.type == weapons[0].type)
                {
                    Weapon weap = GetEmptyWeaponSlot();
                    if (weap != null)
                    {
                        weap.SetType(pUp.type);
                    }
                }
                else
                {
                    ClearWeapons();
                    weapons[0].SetType(pUp.type);
                }
                break;
        }
        pUp.AbsorbedBy(this.gameObject);
    }

    void StarStart()
    {
        starUp = true;

        speed = 60;

        Invoke(nameof(StarColor), 0.2f);

        Invoke(nameof(StarEnd), 5f);
    }

    void StarColor()
    {
        if (!starUp) return;
        foreach (Material m in materials)
        {
            m.color = rainbow[currentColor];
        }
        currentColor = (currentColor + 1) % rainbow.Length;
        Invoke(nameof(StarColor), 0.2f);
    }

    void StarEnd()
    {
        starUp = false;

        speed = 30;

        for (int i = 0; i < materials.Length; ++i)
        {
            materials[i].color = originalColors[i];
        }
    }

    Weapon GetEmptyWeaponSlot()
    {
        for (int i = 0; i < weapons.Length; ++i)
        {
            if(weapons[i].type == eWeaponType.none)
            {
                return weapons[i];
            }
        }
        return null;
    }

    void ClearWeapons()
    {
        foreach (Weapon w in weapons)
        {
            w.SetType(eWeaponType.none);
        }
    }
/*
    void TempFire()
    {
        GameObject projGO = Instantiate<GameObject>(projectilePrefab);
        projGO.transform.position = transform.position;
        Rigidbody rigidB = projGO.GetComponent<Rigidbody>();

        ProjectileHero proj = projGO.GetComponent<ProjectileHero>();
        proj.type = eWeaponType.blaster;
        float tSpeed = Main.GET_WEAPON_DEFINITION(proj.type).velocity;
        rigidB.velocity = Vector3.up * tSpeed;
    }
*/
}
