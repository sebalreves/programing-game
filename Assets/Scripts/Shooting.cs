using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

//monobehavior pun for rpc
public class Shooting : MonoBehaviourPun {
    public GameObject bulletPrefab;
    public Transform firePosition;
    public Camera playerCamera;
    public DeathRacePlayer playerProperties;

    public bool useLaser;
    public LineRenderer lineRenderer;

    private float fireRate;
    private float fireTimer;

    void Start() {
        fireRate = playerProperties.fireRate;
        if (playerProperties.weaponName == "Laser Gun") useLaser = true;
        else useLaser = false;
    }

    void Update() {
        if (!photonView.IsMine) return;
        if (Input.GetKey("space")) {
            if (fireTimer > fireRate) {
                photonView.RPC("fire", RpcTarget.All, firePosition.position);
                fireTimer = 0f;
            }
        }

        if (fireTimer < fireRate) {
            fireTimer += Time.deltaTime;
        }
    }

    [PunRPC]
    public void fire(Vector3 _firePosition) {
        if (useLaser) {
            RaycastHit _hit;
            Ray _ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
            if (Physics.Raycast(_ray, out _hit, 200)) {
                if (!lineRenderer.enabled) {
                    lineRenderer.enabled = true;
                }

                lineRenderer.startWidth = 0.3f;
                lineRenderer.endWidth = 0.1f;

                lineRenderer.SetPosition(0, _firePosition);
                lineRenderer.SetPosition(1, _hit.point);

                //si se dispara a otro jugador
                if (_hit.collider.gameObject.CompareTag("Player")) {
                    if (_hit.collider.gameObject.GetComponent<PhotonView>().IsMine)
                        _hit.collider.gameObject.GetComponent<PhotonView>().RPC("Dodamage", RpcTarget.All, playerProperties.damage);
                }
                StopAllCoroutines();
                StartCoroutine(DisableLaser(0.3f));
            }
        } else {
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
            GameObject bulletGameobject = Instantiate(bulletPrefab, _firePosition, Quaternion.identity);
            bulletGameobject.GetComponent<BulletScript>().initialize(ray.direction, playerProperties.bulletSpeed, playerProperties.damage);
        }
    }

    IEnumerator DisableLaser(float _time) {
        yield return new WaitForSeconds(_time);
        lineRenderer.enabled = false;
    }
}
