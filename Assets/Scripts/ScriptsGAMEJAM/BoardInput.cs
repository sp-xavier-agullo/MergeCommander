using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GJ18
{
    public class BoardInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        private BoardConfig _boardConfig;
        [SerializeField] private Camera _gameplayCamera;
        [SerializeField] private bool __showMousePointer = false;
        [SerializeField] private Transform _mousePointer;

        private Vector2 _startMousePos;
        private int _dragginICoord;
        private int _dragginJCoord;
        private int _draggedIDir;
        private int _draggedJDir;
        private bool _dragDetected;
        private bool _inputCanceled = false;
        
        public Action<int, int, int, int> OnSwipedCallback;
        public Action<Coord> OnClickCallback;
        
        public void Init(BoardConfig config)
        {
            _boardConfig = config;
            _mousePointer.gameObject.SetActive(__showMousePointer);
        }

        public struct Coord
        {
            public int i;
            public int j;
        }

        private Coord mouseToCoord(BoardConfig boardConfig, Vector2 mousePos)
        {
            _startMousePos = mousePos;
            var coord = new Coord();
            
            var rectTransform = transform.GetComponent<RectTransform>();
            
            Vector2 localPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, mousePos, _gameplayCamera, out localPos);

            if (__showMousePointer)
            {
                _mousePointer.GetComponent<RectTransform>().localPosition = localPos;    
            }
            
            var tileSizeX = rectTransform.rect.width / boardConfig.SizeX;
            var tileSizeY = rectTransform.rect.height / boardConfig.SizeY;
            var tileOffsetX = -rectTransform.rect.width * 0.5f;
            var tileOffsetY = -rectTransform.rect.height * 0.5f;
            
            localPos -= new Vector2(tileOffsetX, tileOffsetY);
            coord.i = (int)(localPos.x / tileSizeX);
            coord.j = (int)(localPos.y / tileSizeY);
            
            return coord;
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            var coord = mouseToCoord(_boardConfig, eventData.position);
            _dragDetected = false;
            
            _dragginICoord = coord.i;
            _dragginJCoord = coord.j;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            var endMousePos = eventData.position;

            var screenDistance = (_startMousePos - endMousePos).magnitude;
            var dpiDistance = screenDistance / Screen.dpi;
            if (dpiDistance > 0.02f)
            {
                return;
            }
            
            var coord = mouseToCoord(_boardConfig, eventData.position);
            
            bool iChanged = coord.i != _dragginICoord;
            bool jChanged = coord.j != _dragginJCoord;

            bool click = !iChanged && !jChanged && !_dragDetected;
            if (click)
            {
                OnClickCallback(coord);
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_dragDetected)
            {
                return;
            }
            
            var coord = mouseToCoord(_boardConfig, eventData.position);
            
            
            bool iChanged = coord.i != _dragginICoord;
            bool jChanged = coord.j != _dragginJCoord;

            _dragDetected = iChanged || jChanged;

            if(!(iChanged ^ jChanged))
            {
                return;
            }

            int correctedDragginIcoord = _dragginICoord;
            int correctedDragginJcoord = _dragginJCoord;
            if(iChanged)
            {
                correctedDragginIcoord = Mathf.Clamp(_dragginICoord + (coord.i > _dragginICoord ? +1 : -1), 0, _boardConfig.SizeX);
            }
            else // jChanged == true.
            {
                correctedDragginJcoord = Mathf.Clamp(_dragginJCoord + (coord.j > _dragginJCoord ? +1 : -1), 0, _boardConfig.SizeY);
            }

            bool iValidAndChanged = correctedDragginIcoord != _dragginICoord;
            bool jValidAndChanged = correctedDragginJcoord != _dragginJCoord; 
            
            // second test. if not valid coordinates, cancel dragging.
            if(!(iValidAndChanged ^ jValidAndChanged))
            {
                return;
            }
            
            _draggedIDir = correctedDragginIcoord - _dragginICoord;
            _draggedJDir = correctedDragginJcoord - _dragginJCoord;
            
            OnSwipedCallback(_dragginICoord, _dragginJCoord, _draggedIDir, _draggedJDir);
        }
    }
}