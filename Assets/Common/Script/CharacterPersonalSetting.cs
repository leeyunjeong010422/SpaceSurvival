using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 부모 프리펩이 네트워크 오브젝트일 것이기 때문에 Photon View가 있을 것을 전제로 작성함
/// </summary>
public class CharacterPersonalSetting : MonoBehaviourPun
{
    [SerializeField] Renderer meshRenderer;

    private void Start()
    {
        meshRenderer.material.color = new Color(
            photonView.Owner.GetColorR(),
            photonView.Owner.GetColorG(),
            photonView.Owner.GetColorB());
    }
}
