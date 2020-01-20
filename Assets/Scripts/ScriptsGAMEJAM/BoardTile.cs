using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GJ18
{
    public class BoardTile : MonoBehaviour
    {
        [SerializeField] private Image _floorImg;
        [SerializeField] private Image _unitImg;

        [HideInInspector]
        public int _i;
        [HideInInspector]
        public int _j;
        
        private UnitConfig _config;

        //private Vector3 _initialLocalPosition;

        void Awake()
        {
            //_initialLocalPosition = _unitImg.transform.localPosition;
        }
        

        public UnitConfig GetConfig()
        {
            return _config;
        }

        public bool IsEmpty()
        {
            return _config == null;
        }
    
        public void Init(int i, int j, float sizex, float sizey)
        {
            _i = i;
            _j = j;
            var col1 = new Color(1,1,1,0.2f);
            var col2 = new Color(0.5f,0.5f,0.5f,0.5f);
        
            _floorImg.color = (i % 2 + j % 2) % 2 ==  0? col1 : col2;
        
        
            Utils.SetSize(GetComponent<RectTransform>(), sizex, sizey);
            Utils.SetSize(_floorImg.GetComponent<RectTransform>(), sizex, sizey);
        }

        public void SetUnitConfig(UnitConfig config, bool animate = false)
        {
            _config = config;
            
            if (config != null)
            {
                _unitImg.gameObject.SetActive(true);
                _unitImg.sprite = config.sprite;

                float s = 0.5f;
                float width = config.sprite.rect.width * s;
                float height = config.sprite.rect.height * s;
                Utils.SetSize(_unitImg.GetComponent<RectTransform>(), width, height);
                
                gameObject.name = gameObject.name + "__" + config.type;                

                if (animate)
                {
                    var initialPosition = _unitImg.transform.position;
                    //var initialScale = _unitImg.transform.localScale;
                    _unitImg.transform.position += Vector3.up * 100.0f;
                    _unitImg.color = new Color(1,1,1,0);
                    //_unitImg.transform.localScale = Vector3.one * 1.5f;
                    
                    _unitImg.transform.DOMove(initialPosition, 0.3f).SetEase(Ease.InQuint);
                    _unitImg.DOFade(1.0f, 0.15f).SetEase(Ease.InQuint);
                    //_unitImg.transform.DOScale(initialScale, 0.2f).SetEase(Ease.InQuint);
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

        public void PlayMergeEffect()
        {
            _unitImg.transform.localScale = Vector3.one * 0.8f;
            _unitImg.transform.DOScale(1.0f, 1.0f).SetEase(Ease.OutElastic);
            
            GetComponentInChildren<ParticleSystem>().Play();
        }
    }
}