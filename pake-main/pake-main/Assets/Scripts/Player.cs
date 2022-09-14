using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class Player : NetworkBehaviour
{
    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] isEnabled;
    [SyncVar]
    private bool _isDead = false;
    public bool isDead { get { return _isDead; } protected set { _isDead = value; } }
    [SerializeField]
    private int maxHp;

    [SerializeField]
    private CharacterController cc;

    [SyncVar]
    private int health;
    [SerializeField]
    private Text healthText;

    public GameObject DeadText;
    public Animator animator;
    public GameObject crosshair;

    [SerializeField] private Camera cam;
    [SerializeField] private float crouchUpAmount;

    public float startDamageTimer;
    private float damageTimer;
    private bool damageBool;
    public GameObject damageCanvas;

    public void Setup()
    {
        isEnabled = new bool[disableOnDeath.Length];
        for (int i = 0; i < isEnabled.Length; i++)
        {
            isEnabled[i] = disableOnDeath[i].enabled;
        }
        SetDefaults();
    }

    private void SetDefaults()
    {
        if (GetComponent<PlayerMovement>().isCrouching == true)
        {
            cam.transform.localPosition += new Vector3(0, crouchUpAmount, 0);
            GetComponent<PlayerMovement>().Speed = GetComponent<PlayerMovement>().StartSpeed;
            GetComponent<PlayerMovement>().isCrouching = false;
        }
        animator.SetBool("IsCrouching", false);
        crosshair.SetActive(true);
        healthText.text = health.ToString();
        health = maxHp;
        isDead = false;
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = isEnabled[i];
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = true;
        }
    }

    private void Update()
    {
        if (isLocalPlayer)
        {
            healthText.text = health.ToString();
            if (Input.GetKeyDown(KeyCode.J))
                RpcTakeDamage(1);
            if (damageBool)
            {
                damageTimer -= Time.deltaTime;

                if (damageTimer <= 0)
                {
                    damageCanvas.SetActive(false);
                    damageBool = false;
                    damageTimer = startDamageTimer;
                }
            }
        }
    }


    [ClientRpc]
    public void RpcTakeDamage(int dmg)
    {
        if (isDead)
            return;

        health -= dmg;
        CoolDamageEffect();
        AudioManager.instance.Play("Hit");
        healthText.text = health.ToString();

        if (health <= 0)
        {
            GetComponent<MouseLook>().isZoomed = false;
            Die();
            DeadText.SetActive(true);
        }
    }

    private void CoolDamageEffect()
    {
        if (isLocalPlayer)
        {
            damageTimer = startDamageTimer;
            damageCanvas.SetActive(true);
            damageBool = true;
        }
    }
    private void Die()
    {
        damageCanvas.SetActive(false);
        isDead = true;
        cc.enabled = false;
        animator.SetBool("isDead", true);
        crosshair.SetActive(false);
        AudioManager.instance.Play("Death");
        AudioManager.instance.StopPlay("Walk");
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }
        Collider col = GetComponent<Collider>();
        GetComponent<FlagCapture>().CheckDead();
        if (col != null)
            col.enabled = false;
        StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        Debug.Log("Respawning...");
        transform.position = GetComponent<PlayerManager>().spawnPoint.position;
        transform.rotation = GetComponent<PlayerManager>().spawnPoint.rotation;
        yield return new WaitForSeconds(10f);
        SetDefaults();
        GetComponent<PlayerWeapon>().currentAmmo = 30;
        cc.enabled = true;
        DeadText.SetActive(false);
        Debug.Log("Respawn complete!");
        animator.SetBool("isDead", false);
    }


    [Client]
    public void HealPlayer(int _hp)
    {
        CmdHealPlayer(_hp);
    }

    [Command]
    private void CmdHealPlayer(int _hp)
    {
        RpcHealPlayer(_hp);
    }

    [ClientRpc]
    private void RpcHealPlayer(int _hp)
    {
        if (isLocalPlayer)
        {
            health += _hp;
            if (health >= maxHp)
            {
                health = maxHp;
                StopPlayHealSound();
            }
            healthText.text = health.ToString();
        }
    }

    public bool IsHpFull()
    {
        if (health == maxHp)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void StartPlayHealSound()
    {
        if (isLocalPlayer)
        {
            AudioManager.instance.Play("HealSound");
        }
    }
    public void StopPlayHealSound()
    {
        if (isLocalPlayer)
        {
            AudioManager.instance.StopPlay("HealSound");            
        }
    }

}
