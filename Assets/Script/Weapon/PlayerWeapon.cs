using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

interface Iweapon
{
    GameObject weapon { get; }
    bool IsEquip {  get; }
    bool IsAiming { get; }

    void Fire(Vector3 _firePoint, Vector3 _fireDerection);
    void Equip();
    void Unequip();
    void OnAim();

}

public class PlayerWeapon : Iweapon
{
    public int weaponNum {  get; private set; }
    public GameObject weapon { get; private set; }
    public bool IsEquip { get; private set; }
    public bool IsAiming { get; private set; }
    LayerMask layerWall;
    LayerMask layerEnemy;
    float rayDistance;
    Image crossHair;
    public PlayerWeapon(GameObject _weapon ,int _weaponNum, float _rayDistance, 
        LayerMask _layerWall,LayerMask _layerEnemy,Image _crossHair)
    {
        weapon = _weapon;
        rayDistance = _rayDistance;
        weaponNum = _weaponNum;
        layerWall = _layerWall;
        layerEnemy = _layerEnemy;
        crossHair = _crossHair;
        weapon.SetActive(false);
    }
    public void Equip()
    {
        //IsEquip �� �ʿ�� �����Ű��� �� �Լ��� Ż ���� ��ġ �������ִ� ���ҷ� ����
        IsEquip = true;
        weapon.SetActive(true);
        Debug.Log($"{weaponNum} ����");
    }
    public void Unequip()
    {
        IsEquip = false;
        IsAiming = false;
        weapon.SetActive(false);
        Debug.Log($"{weaponNum} Ż��");

    }
    public void OnAim()
    {
        IsAiming = true;
        crossHair.enabled = true;
    }
    public void OffAim()
    {
        IsAiming = false;
        crossHair.enabled = false;

    }

    public void Fire(Vector3 _firePoint,Vector3 _fireDerection)
    {
        if (IsAiming)
        {
            //��
            // ���� ��ġ�� ������ ���� �������� ���̸� ���ϴ�.
            Ray ray = new Ray(_firePoint, _fireDerection); ;
            RaycastHit hitInfo;
            Debug.Log("�߻�");
            // ���� ���̰� � ��ü�� �浹�Ѵٸ�
            if (Physics.Raycast(ray, out hitInfo, rayDistance, layerEnemy))
            {
                // �浹�� ��ü�� ���� ���
                Debug.Log("Hit Monster : " + hitInfo.collider.name);

                hitInfo.transform.GetComponent<Enemy>().HitEnemy();
                // ���⼭ �߰����� ������ ������ �� �ֽ��ϴ�.
                // ���� ���, �浹�� ��ü�� ���� ó���� �����ϰų� Ư�� ������ ������ �� �ֽ��ϴ�.
            }
            else if (Physics.Raycast(ray, out hitInfo, rayDistance, layerWall))
            {
                // �浹�� ��ü�� ���� ���
                Debug.Log("Hit Wall: ");

                // ���⼭ �߰����� ������ ������ �� �ֽ��ϴ�.
                // ���� ���, �浹�� ��ü�� ���� ó���� �����ϰų� Ư�� ������ ������ �� �ֽ��ϴ�.
            }
            else
            {
                // ���̰� �浹���� �ʾ��� ���� ����
            }
            // ���̸� �ð������� �׸��� ���� �ּ� ������ �� �ֽ��ϴ�.
            Debug.DrawRay(_firePoint, _fireDerection * rayDistance, Color.green);
        }
        
    }
}
