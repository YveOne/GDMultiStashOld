﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D3DHook.Overlay.Scrolling
{

    public class ScrollHandler
    {

        ScrollOrientation Orientation => _orientation;
        private readonly ScrollOrientation _orientation;

        public ScrollHandler(ScrollOrientation orientation)
        {
            _orientation = orientation;
        }

        #region VisibleUnits

        public event EventHandler<EventArgs> VisibleUnitsXChanged;
        public event EventHandler<EventArgs> VisibleUnitsYChanged;

        private int _visibleUnitsX = 0;
        private int _visibleUnitsY = 0;

        public int VisibleUnitsX
        {
            get => _visibleUnitsX;
            set
            {
                bool changed = value != _visibleUnitsX;
                _visibleUnitsX = value;
                if (changed)
                {
                    VisibleUnitsXChanged?.Invoke(this, EventArgs.Empty);
                    ScrollPositionX = _scrollPositionX;
                }
            }
        }

        public int VisibleUnitsY
        {
            get => _visibleUnitsY;
            set
            {
                bool changed = value != _visibleUnitsY;
                _visibleUnitsY = value;
                if (changed)
                {
                    VisibleUnitsYChanged?.Invoke(this, EventArgs.Empty);
                    ScrollPositionY = _scrollPositionY;
                }
            }
        }

        #endregion

        #region TotalUnits

        public event EventHandler<EventArgs> TotalUnitsXChanged;
        public event EventHandler<EventArgs> TotalUnitsYChanged;

        private int _totalUnitsX = 0;
        private int _totalUnitsY = 0;

        public int TotalUnitsX
        {
            get => _totalUnitsX;
            set
            {
                bool changed = value != _totalUnitsX;
                _totalUnitsX = value;
                if (changed)
                {
                    TotalUnitsXChanged?.Invoke(this, EventArgs.Empty);
                    ScrollPositionX = _scrollPositionX;
                }
            }
        }

        public int TotalUnitsY
        {
            get => _totalUnitsY;
            set
            {
                bool changed = value != _totalUnitsY;
                _totalUnitsY = value;
                if (changed)
                {
                    TotalUnitsYChanged?.Invoke(this, EventArgs.Empty);
                    ScrollPositionY = _scrollPositionY;
                }
            }
        }

        #endregion

        #region ScrollPosition

        public event EventHandler<EventArgs> ScrollPositionXChanged;
        public event EventHandler<EventArgs> ScrollPositionYChanged;

        private int _scrollPositionX = 0;
        private int _scrollPositionY = 0;

        public int ScrollPositionX
        {
            get => _scrollPositionX;
            set
            {
                int max = _totalUnitsX - _visibleUnitsX;
                if (value > max) value = max;
                if (value < 0) value = 0;
                if (_visibleUnitsX >= _totalUnitsX) value = 0;
                bool changed = value != _scrollPositionX;
                _scrollPositionX = value;
                if (changed) ScrollPositionXChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public int ScrollPositionY
        {
            get => _scrollPositionY;
            set
            {
                int max = _totalUnitsY - _visibleUnitsY;
                if (value > max) value = max;
                if (value < 0) value = 0;
                if (_visibleUnitsY >= _totalUnitsY) value = 0;
                bool changed = value != _scrollPositionY;
                _scrollPositionY = value;
                if (changed) ScrollPositionYChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion

    }

}
