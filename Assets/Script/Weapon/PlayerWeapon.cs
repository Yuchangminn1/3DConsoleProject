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
        //IsEquip 은 필요는 없을거같고 이 함수는 탈 부착 위치 조정해주는 역할로 쓰자
        IsEquip = true;
        weapon.SetActive(true);
        Debug.Log($"{weaponNum} 장착");
    }
    public void Unequip()
    {
        IsEquip = false;
        IsAiming = false;
        weapon.SetActive(false);
        Debug.Log($"{weaponNum} 탈착");

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
            //총
            // 현재 위치와 전방을 향한 방향으로 레이를 쏩니다.
            Ray ray = new Ray(_firePoint, _fireDerection); ;
            RaycastHit hitInfo;
            Debug.Log("발사");
            // 만약 레이가 어떤 물체와 충돌한다면
            if (Physics.Raycast(ray, out hitInfo, rayDistance, layerEnemy))
            {
                // 충돌한 물체의 정보 출력
                Debug.Log("Hit Monster : " + hitInfo.collider.name);

                hitInfo.transform.GetComponent<Enemy>().HitEnemy();
                // 여기서 추가적인 동작을 수행할 수 있습니다.
                // 예를 들어, 충돌한 물체에 대한 처리를 진행하거나 특정 동작을 수행할 수 있습니다.
            }
            else if (Physics.Raycast(ray, out hitInfo, rayDistance, layerWall))
            {
                // 충돌한 물체의 정보 출력
                Debug.Log("Hit Wall: ");

                // 여기서 추가적인 동작을 수행할 수 있습니다.
                // 예를 들어, 충돌한 물체에 대한 처리를 진행하거나 특정 동작을 수행할 수 있습니다.
            }
            else
            {
                // 레이가 충돌하지 않았을 때의 동작
            }
            // 레이를 시각적으로 그리기 위해 주석 해제할 수 있습니다.
            Debug.DrawRay(_firePoint, _fireDerection * rayDistance, Color.green);
        }
        
    }
}
