using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BulletScript : MonoBehaviour {
    public float bulletDamage;

    void Start() {


    }

    // Update is called once per frame
    void Update() {

    }

    void OnTriggerEnter(Collider other) {
        Destroy(gameObject);
        //si no se verifica que es el player, todos los jugadores ejecutaran do damage cuando choque una bala
        if (other.gameObject.GetComponent<PhotonView>().IsMine) {
            if (other.gameObject.CompareTag("Player")) {
                //llamada RPC desde otro objeto
                other.gameObject.GetComponent<PhotonView>().RPC("DoDamage", RpcTarget.All, bulletDamage);
            }
        }
    }

    public void initialize(Vector3 _direction, float speed, float damage) {
        bulletDamage = damage;
        transform.forward = _direction;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = _direction * speed;
    }
}
