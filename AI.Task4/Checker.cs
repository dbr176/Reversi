using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AI.Task4
{

    public sealed class Checker : BoardCell
    {
        public Checker(bool isBlack)
        {
            IsBlack = isBlack;
        }
        bool isBlack;

        public bool IsBlack
        {
            get
            {
                return isBlack;
            }

            set
            {
                isBlack = value;
                ColorChanged?.Invoke(this, new CheckerColorChangedEventArgs(value));
            }
        }

        public bool IsWhite => !IsBlack;

        public event EventHandler<CheckerColorChangedEventArgs> ColorChanged;
    }

}