﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using Def.Enum;


public class LineRenderController : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer_ = default;
    [SerializeField] private List<RectTransform> listPos_ = default;
    [SerializeField] private Material mat_ = default;

    public bool isEditMode_ = false;
    public float width_ = 0.2f;

    public float time_ = 0f;
    private Color baseColor_ = default;
    private Color targetColor_ = default;
    private enumFadeType enumFade;
    private Sequence seqPosition_ = default;
    private Sequence seqColor_ = default;

    private void Awake()
    {
        mat_ = lineRenderer_.material;
        seqPosition_ = DOTween.Sequence();
        seqColor_ = DOTween.Sequence();

        seqPosition_.Pause();
        seqColor_.Pause();
    }

    private void Update()
    {
        SetLineRenderer();
    }

    private void SetLineRenderer()
    {
        lineRenderer_.startWidth = width_;
        lineRenderer_.endWidth = width_;

        for (int index = 0; index < listPos_.Count; index++)
        {
            lineRenderer_.SetPosition(index, listPos_[index].position);
        }
    }

    public void SetLines(List<Vector2> listVec2, Color32 color32)
    {
        mat_.color = color32;

        for (int index = 0; index < listPos_.Count; index++)
        {
            listPos_[index].anchoredPosition = listVec2[index];
        }
    }

    /// <summary> 트윈 애니메이션을 추가한다.</summary>
    public void AddTweenPositionLines(List<Vector2> listVec2Target, float duration)
    {
        seqPosition_.Append(listPos_[0].DOAnchorPos(listVec2Target[0], duration));

        if(listPos_.Count >1)
        {
            for (int index = 1; index < listPos_.Count; index++)
            {
                seqPosition_.Join(listPos_[index].DOAnchorPos(listVec2Target[index], duration));
            }
        }
    }

    /// <summary> 트윈 애니메이션을 추가한다.</summary>
    public void AddTweenColorLines(Color32 color32, float duration)
    {
        seqColor_.Append(mat_.DOColor(color32, duration));
    }


    /// <summary>트윈 애니메이션으로 점을 움직인다.</summary>
    /// <param name="fadeType">페이드 유형</param>
    /// <param name="listVec2"></param>
    /// <param name="onDone"></param>
    public void TweenLines(string fadeType, List<Vector2> listVec2, Action onDone)
    {
        //투명화를 위한 임시 color
        Color tmpColor = mat_.color;

        Enum.TryParse(fadeType, out enumFade);

        Sequence sequence;
        sequence = DOTween.Sequence();

        for (int index = 0; index < listPos_.Count; index++)
        {
            sequence.Join(listPos_[index].DOAnchorPos(listVec2[index], time_));
        }

        switch (enumFade)
        {
            case enumFadeType.None:
                break;

            case enumFadeType.FadeIn:
                mat_.color = new Color(tmpColor.r, tmpColor.g, tmpColor.b, 0);
                sequence.Join(mat_.DOColor(tmpColor, time_));
                break;
            case enumFadeType.FadeOut:
                tmpColor.a = 0f;
                sequence.Join(mat_.DOColor(tmpColor, time_));
                break;
        }
        sequence.OnComplete(() => { onDone?.Invoke(); });
    }

    public void PlayTweenAnimation()
    {
        if(seqPosition_ != null)
            seqPosition_.Play();

        if (seqColor_ != null)
            seqColor_.Play();
    }
}
