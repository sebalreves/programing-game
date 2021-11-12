using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class TakeDamage : MonoBehaviourPun {
    public float startHealth = 100f;
    private float health;
    public Image healthBar;
    public GameObject deathPanelPrefab;
    public GameObject deathPanelGameObject;
    Rigidbody rb;

    void Start() {
        health = startHealth;
        healthBar.fillAmount = health / startHealth;
        rb = GetComponent<Rigidbody>();

    }

    void Update() {

    }

    [PunRPC]
    public void DoDamage(float _damage) {
        health -= _damage;
        healthBar.fillAmount = health / startHealth;
        if (health <= 0) {
            Die();
        }
    }

    public void Die() {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        if (photonView.IsMine) {
            //respawn
            StartCoroutine(respawn());
        }
    }

    IEnumerator respawn() {
        GameObject canvas = GameObject.Find("Canvas");
        if (deathPanelGameObject == null)
            deathPanelGameObject = Instantiate(deathPanelPrefab, canvas.transform);
        else
            deathPanelGameObject.SetActive(true);

        Text respawnTimeText = deathPanelGameObject.transform.Find("RespawnTimeText").GetComponent<Text>();
        float respawnTime = 8f;

        respawnTimeText.text = respawnTime.ToString(".00");
        while (respawnTime > 0f) {
            yield return new WaitForSeconds(1f);
            respawnTime -= 1f;
            respawnTimeText.text = respawnTime.ToString(".00");
            GetComponent<CarMovement>().enabled = false;
            GetComponent<Shooting>().enabled = false;
        }

        deathPanelGameObject.SetActive(false);
        GetComponent<CarMovement>().enabled = true;
        GetComponent<Shooting>().enabled = true;

        int randomPoint = Random.Range(-20, 20);
        transform.position = new Vector3(randomPoint, 0f, randomPoint);

        photonView.RPC("reborn", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void reborn() {
        health = startHealth;
        healthBar.fillAmount = health / startHealth;

    }
}
