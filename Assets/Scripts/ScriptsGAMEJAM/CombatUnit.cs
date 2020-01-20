using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Random = UnityEngine.Random;

namespace GJ18
{
    public class CombatUnit : MonoBehaviour
    {
        [SerializeField] private Image _unitImg;
        [SerializeField] private Text _amountText;
        [SerializeField] private GameObject _amountBackground;
        [SerializeField] private GameObject _deadSkullPrefab;
        [SerializeField] private GameObject _deadSkullPlaceholder;
        private UnitConfig _config;

        public bool IsDark = false;

        private int _unitsAmount;
        public int UnitsAmount
        {
            get { return _unitsAmount; }
            set
            {
                _unitsAmount = Mathf.Max(value, 0);
                bool alive = _unitsAmount > 0;
                
                _amountText.text = "x" + _unitsAmount;
                ShowAmountUnits(alive);
                _unitImg.gameObject.SetActive(alive);
            }
        }

        public void ShowAmountUnits(bool show)
        {
            _amountBackground.SetActive(show);
            _amountText.gameObject.SetActive(show);
        }

        public bool IsAlive()
        {
            return _unitsAmount > 0;
        }

        public UnitConfig __debugConfig;
        public bool __debugIsEnemy;

        public UnitConfig GetConfig()
        {
            return _config;
        }

        public bool IsEmpty()
        {
            return _config == null || UnitsAmount == 0;
        }

        public int GetPower()
        {
            if (IsEmpty() || !IsAlive())
            {
                return 0;
            }

            return GetConfig().unitPower * UnitsAmount;
        }

        public void SetUnitConfig(UnitConfig config, bool isEnemy, bool animate = false)
        {
         
            _config = config;
            
            if (config != null)
            {
                gameObject.name = config.type.ToString();
                
                _unitImg.gameObject.SetActive(true);
                _unitImg.sprite = isEnemy ? config.stickerSpriteDark : config.stickerSprite;
                IsDark = isEnemy;

                if (animate)
                {
                    var initialPosition = _unitImg.transform.position;
                    
                    _unitImg.transform.position += Vector3.up * 100.0f;
                    _unitImg.color = new Color(1,1,1,0);
                    
                    _unitImg.transform.DOMove(initialPosition, 0.3f).SetEase(Ease.InQuint);
                    _unitImg.DOFade(1.0f, 0.15f).SetEase(Ease.InQuint);
                }
            }
            else
            {
                _unitImg.gameObject.SetActive(false);
            }
        }

        public BoardObjectType GetObjType()
        {
            return _config.type;
        }

        private bool _keepPlayingRandomAttacks = false;
        private float _playRandomAttacks_minDelay;
        private float _playRandomAttacks_maxDelay;
        
        public void PlayRandomAttacks(GameObject randomAttacksPlaceholders, GameObject randomAttakcsBackPlaceholders, bool startPlayRandomAttack, float minDelay, float maxDelay)
        {
            float delay = Random.Range(minDelay, maxDelay);
            StartCoroutine(WaitPlayRandomAttacks(randomAttacksPlaceholders, randomAttakcsBackPlaceholders, startPlayRandomAttack, delay));
        }

        private IEnumerator WaitPlayRandomAttacks(GameObject randomAttacksPlaceholders, GameObject randomAttakcsBackPlaceholders, bool startPlayRandomAttack, float delay)
        {
            yield return new WaitForSeconds(delay);
            ShowAmountUnits(false);
            PlayRandomAttacks(randomAttacksPlaceholders, randomAttakcsBackPlaceholders, startPlayRandomAttack);
        }

        private bool _isBack = false;
        private Vector3 _initialPosition;
        private void PlayRandomAttacks(GameObject randomAttacksPlaceholders, GameObject randomAttakcsBackPlaceholders, bool startPlayRandomAttack)
        {
            if (startPlayRandomAttack)
            {
                _keepPlayingRandomAttacks = true;
                _initialPosition = _unitImg.transform.position;
            }
            
            if (!_keepPlayingRandomAttacks)
            {
                if (!_isBack)
                {
                    return;    
                }
            }

            _isBack = _keepPlayingRandomAttacks;
            
            var indexAttackPoint = Random.Range(0, randomAttacksPlaceholders.transform.childCount);
            var goAttackPosition = randomAttacksPlaceholders.transform.GetChild(indexAttackPoint);
            
            Vector3 returnPositon;
            
            if (_isBack)
            {
                var indexAttackBackPoint = Random.Range(0, randomAttakcsBackPlaceholders.transform.childCount);
                var attackBackPoint = randomAttakcsBackPlaceholders.transform.GetChild(indexAttackBackPoint);
                
                returnPositon = attackBackPoint.transform.position;
            }
            else
            {
                returnPositon = _initialPosition;
            }
            
            var seq = DOTween.Sequence();
            seq.Append(_unitImg.transform.DOMove(goAttackPosition.transform.position, 0.1f).SetEase(Ease.OutQuint));
            seq.Append(_unitImg.transform.DOMove(returnPositon, 0.1f).SetEase(Ease.InQuint));
            seq.OnComplete(() =>
            {
                if (!_isBack)
                {
                    ShowAmountUnits(true);
                }
                PlayRandomAttacks(randomAttacksPlaceholders, randomAttakcsBackPlaceholders, false);
            });
        }

        public void StopRandomAttacks()
        {
            _keepPlayingRandomAttacks = false;
        }

        private int _pendingDeadsToPlay;
        private int _deadsAnimating;
        public void PlayDeads(int amount, Transform skullsTransform, Action onSkullCallback, Action onSkullsAnimationsEndedCallback)
        {
            _pendingDeadsToPlay = amount;
            StartCoroutine(PlayDeads_coroutine(skullsTransform, onSkullCallback, onSkullsAnimationsEndedCallback));
        }
        
        private IEnumerator PlayDeads_coroutine(Transform skullsTransform, Action onSkullCallback, Action onSkullsAnimationsEndedCallback)
        {
            _deadsAnimating = 0;
            while (_pendingDeadsToPlay > 0)
            {
                _pendingDeadsToPlay--;
                _deadsAnimating++;
                
                var pos = _deadSkullPlaceholder.transform.position;
                var go = Instantiate(_deadSkullPrefab, pos, Quaternion.identity, skullsTransform);
                go.GetComponent<DeadSkull>().Init(gameObject, () =>
                {
                    _deadsAnimating--;
                    if (_deadsAnimating == 0)
                    {
                        onSkullsAnimationsEndedCallback();
                    }
                });
                
                onSkullCallback();
                
                yield return new WaitForSeconds(0.05f);    
            }
        }

        public void PlayHappyAnimation()
        {
            StartCoroutine(PlayHappyAnimation_coroutine());
        }

        private IEnumerator PlayHappyAnimation_coroutine()
        {
            yield return new WaitForSeconds(Random.Range(0, 0.3f));
            
            var posDown = _unitImg.transform.position;
            var posUp = posDown + Vector3.up * 25;
            var seq = DOTween.Sequence();
            seq.Append(_unitImg.transform.DOMove(posUp, 0.15f).SetEase(Ease.OutQuint));
            seq.Append(_unitImg.transform.DOMove(posDown, 0.15f).SetEase(Ease.OutQuint));
            seq.SetLoops(-1);
        }
    }
}