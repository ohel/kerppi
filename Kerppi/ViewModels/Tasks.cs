﻿/*
    Copyright 2015, 2017, 2018, 2024 Olli Helin / GainIT
    This file is part of Kerppi, a free software released under the terms of the
    GNU General Public License v3: http://www.gnu.org/licenses/gpl-3.0.en.html
*/

using System;
using System.ComponentModel;

namespace Kerppi.ViewModels
{
    class Tasks : INotifyPropertyChanged, IKerppiRefreshable
    {
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        void IKerppiRefreshable.Refresh()
        {
        }
    }
}
