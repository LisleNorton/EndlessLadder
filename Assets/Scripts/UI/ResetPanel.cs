﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class ResetPanel : MonoBehaviour
{
    private Button btn_Reset;
    private Button btn_NoReset;
    private Image img_Bg;
    private GameObject dialog;
    private ManagerVars vars;

    private void Awake()
    {
        EventCenter.AddListener(EventDefine.ShowResetPanel, ShowPanel);
        img_Bg = transform.Find("bg").GetComponent<Image>();
        btn_Reset = transform.Find("Dialog/btn_Reset").GetComponent<Button>();
        btn_Reset.onClick.AddListener(OnYesButtonClick);
        btn_NoReset = transform.Find("Dialog/btn_NoReset").GetComponent<Button>();
        btn_NoReset.onClick.AddListener(OnNoButtonClick);
        dialog = transform.Find("Dialog").gameObject;

        img_Bg.color = new Color(img_Bg.color.r, img_Bg.color.g, img_Bg.color.b, 0);
        dialog.transform.localScale = Vector3.zero;
        gameObject.SetActive(false);
    }
    private void HidePanel()
    {
        gameObject.SetActive(false);
    }
        private void OnDestroy()
    {
        EventCenter.RemoveListener(EventDefine.ShowResetPanel, ShowPanel);
    }
    private void ShowPanel()
    {
        gameObject.SetActive(true);
        img_Bg.DOColor(new Color(img_Bg.color.r, img_Bg.color.g, img_Bg.color.b, 0.3f), 0.3f);
        dialog.transform.DOScale(Vector3.one, 0.3f);
    }
    /// <summary>
    /// 是按钮点击
    /// </summary>
    private void OnYesButtonClick()
    {
        EventCenter.Broadcast(EventDefine.PlayClikAudio);
        GameManager.Instance.ResetData();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    /// <summary>
    /// 否按钮点击
    /// </summary>
    private void OnNoButtonClick()
    {
        EventCenter.Broadcast(EventDefine.PlayClikAudio);
        img_Bg.DOColor(new Color(img_Bg.color.r, img_Bg.color.g, img_Bg.color.b, 0), 0.3f);
        dialog.transform.DOScale(Vector3.zero, 0.3f).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }
}
