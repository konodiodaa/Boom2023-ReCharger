using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAfterImage : MonoBehaviour
{

    [SerializeField]
    private float activeTime;
    private float timeActivated;
    private float alpha;
    [SerializeField]
    private float alphaSet;
    [SerializeField]
    private float alphaMultipler;

    private Transform player;

    private SpriteRenderer SR;
    private SpriteRenderer playerSR;

    private Color color;
    private bool isInit;

    private void OnEnable()
    {
        SR = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerSR = player.GetComponent<SpriteRenderer>();

        alpha = alphaSet;
        SR.sprite = playerSR.sprite;
        transform.position = player.position;
        transform.rotation = player.rotation;
        timeActivated = Time.time;
    }

    private void Update()
    {
        alpha *= alphaMultipler;
        color = new Color(1f, 1f, 1f, alpha);
        SR.color = color;

        if (Time.time >= (activeTime + timeActivated) || !isInit)
        {
            PlayerAfterImagePool.Instance.Add2Pool(gameObject);
            isInit = true;
        }
    }
}
