using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    float hp = 10;
    [SerializeField]
    ParticleSystem hitEffect;
    [SerializeField]
    ParticleSystem deathEffect;

    public bool isDead { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        
    }
    public void HitEnemy(int Damage = 1)
    {
        Debug.Log("Enemy Hit");
        if(hp - Damage < 1)
        {
            Debug.Log("����");
            isDead = true;
            StartCoroutine(Dead());
        }
        hp -= Damage;
        hitEffect.Play();
        Debug.Log($"Hp = {hp}");

        //����Ʈ ���� ��� 
    }
    IEnumerator Dead()
    {
        deathEffect.Play();
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);

    }
}
