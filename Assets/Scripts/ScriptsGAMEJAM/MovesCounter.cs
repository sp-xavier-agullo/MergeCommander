using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MovesCounter : MonoBehaviour
{
	[SerializeField] private Text _movesText;
	[SerializeField] private TextMeshProUGUI _titleText;

	private static string movesStr = "MOVES";
	private static string turnStr = "TURN";

	public void Awake()
	{
		_titleText.text = movesStr;
	}
	
	public void SetMoves(int moves, bool animIfDifferent = true)
	{
		if (_titleText.text != movesStr && animIfDifferent)
		{
			ChangeAnimation(() => { SetMoves(moves, false); });
			return;
		}
		
		_titleText.text = movesStr;
		_movesText.text = moves.ToString();
		//AnimateCounter();
	}
	
	public void SetTurn(int turn, bool animIfDifferent = true)
	{
		if (_titleText.text != turnStr && animIfDifferent)
		{
			ChangeAnimation(() => { SetTurn(turn, false); });
			return;
		}
		
		_titleText.text = turnStr;
		_movesText.text = turn.ToString();
		//AnimateCounter();
	}

	public void AnimateCounter()
	{
		var seq = DOTween.Sequence();
		seq.Append(_movesText.transform.DOScale(1.2f, 0.2f).SetEase(Ease.InQuint));
		seq.Append(_movesText.transform.DOScale(1.0f, 0.2f).SetEase(Ease.OutQuint));
		seq.Play();
	}

	void ChangeAnimation(Action onHidedCallback)
	{
		var initialPosition = transform.position;
		var pos1 = initialPosition + Vector3.up * 150;
		var seq = DOTween.Sequence();
		seq.Append(transform.DOMove(pos1, 1).SetEase(Ease.Linear).OnComplete(() =>
		{
			onHidedCallback();
		}));
		
		seq.Append(transform.DOMove(pos1 , 1.0f));
		
		seq.Append(transform.DOMove(initialPosition , 1.0f).SetEase(Ease.OutBounce));
		
		seq.Play();
	}
}
