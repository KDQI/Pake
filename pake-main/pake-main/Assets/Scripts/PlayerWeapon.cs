using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
public class PlayerWeapon : NetworkBehaviour
{
    [SerializeField] private GameObject canvas;
    [SerializeField] private Text AmmoCounterText;
    [SerializeField] private GameObject cam;
    [SerializeField] private LayerMask mask;
    [SerializeField] private int range;
    [SerializeField] private int dmg;
    [SerializeField] private GameObject gun;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private Renderer rend;
    [SerializeField] private Animation shoot;
    private Shader shader;
  
    private Collider cl;

    public GameObject bulletPrefab;

    public Transform bulletSpawner;

    public float bulletSpeed = 30;

    private float timer;
    public float startTimer = 2;
    public float reloadTimer;

    public float maxAmmo;
    public float currentAmmo;

    private float lerp = 0;

    private bool isReloading;


    void Start()
    {
        
        shader = Shader.Find("Particles/Standard Unlit");
        rend.material.shader = shader;
        if(isLocalPlayer)
        {
            
            cl = this.GetComponent<CapsuleCollider>();
            isReloading = false;
            timer = startTimer;
            currentAmmo = maxAmmo;
            gun.layer = LayerMask.NameToLayer("Gun");
            foreach (Transform trans in gun.GetComponentsInChildren<Transform>(true))
            {
                trans.gameObject.layer = LayerMask.NameToLayer("Gun");
            }
        } else
        {
            canvas.SetActive(false);
        }
    }
    void Update()
    {
        if(isLocalPlayer)
        {
            HandleFiring();
        }
    }
    [Client]
    private void HandleFiring()
    {
        AmmoCounterText.text = currentAmmo.ToString() + " / ∞ ";
        if (!isReloading)
        {
            if (timer <= 0)
            {
                if (Input.GetMouseButtonDown(0) && currentAmmo >= 1)
                {
                    CmdShootAudio();                   
                    timer = startTimer;
                    Shoot();
                    currentAmmo--;
                }
            }
            else
            {
                timer -= Time.deltaTime;
            }
            if (Input.GetKeyDown(KeyCode.R) && currentAmmo <= 29 || currentAmmo <= 0)
            {
                StartCoroutine(ReloadGun());
            }
        }
    }

    IEnumerator ReloadGun()
    {
        isReloading = true;
        AudioManager.instance.Play("Reload");
        while(currentAmmo != maxAmmo)
        {
            currentAmmo++;
            AmmoCounterText.text = currentAmmo.ToString();
            yield return new WaitForSeconds(0.7f);
        }
        AudioManager.instance.StopPlay("Reload");
        isReloading = false;
    }
    [Command]
    void CmdShootAudio()
    {
        RpcShootAudio();
    }

    [ClientRpc]
    void RpcShootAudio()
    {
        AudioManager.instance.Play("Shooting");
    }
    [Command]
    void CmdOnShoot()
    {
        RpcDoShotEffects();
    }

    [ClientRpc]
    void RpcDoShotEffects()
    {
        muzzleFlash.Play();
    }

    [Command]
    void CmdOnHit(Vector3 pos, Vector3 normal)
    {
        RpcDoHitEffect(pos, normal);
    }

    [ClientRpc]
    void RpcDoHitEffect(Vector3 pos, Vector3 normal)
    {
        GameObject hitFx = Instantiate(hitEffect, pos, Quaternion.LookRotation(normal));
        Destroy(hitFx, 1);
    }

    [Client]
    private void Shoot()
    {
        if(!isLocalPlayer)
        {
            return;
        }
        shoot.Play("Shoot");

        CmdOnShoot();
        RaycastHit hit;
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range, mask))
        {
            if(hit.collider.gameObject.CompareTag("Red") || hit.collider.gameObject.CompareTag("Blue")) //TODO: Jos on oma tiimiläinen, älä tee damagea.
            {
                CmdPlayerShoot(hit.collider.gameObject, dmg, transform.position, hit.point);
            }
            CmdOnHit(hit.point, hit.normal);
        }
    }

    [Command]
    void CmdPlayerShoot(GameObject id, int _dmg, Vector3 originPos, Vector3 hitPos)
    {
        Debug.Log(id + " has been shot");
        id.GetComponent<Player>().RpcTakeDamage(_dmg);
    }
}

