using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AI.Task4
{

    public class UIChecker
    {
        ResourceDictionary _res;
        Button _btn;
        Checker _checker;

        public Checker Checker
        {
            get
            {
                return _checker;
            }

            set
            {
                _checker = value;
            }
        }

        public UIChecker(int row, int col, bool isBlack, ResourceDictionary r, Grid g)
        {
            _res = r;

            var style = (isBlack ? r["BlackChecker"] : r["WhiteChecker"]) as Style;
            var b = new Button {
                Style = style
            };
            g.Children.Add(b);

            Grid.SetRow(b, row);
            Grid.SetColumn(b, col);

            _btn = b;

            var checker = new Checker(isBlack);
            checker.ColorChanged += Checker_ColorChanged;

            _checker = checker;
        }

        private void Checker_ColorChanged(object sender, CheckerColorChangedEventArgs e)
        {
            _btn.Style = (e.IsBlack ? _res["BlackChecker"] : _res["WhiteChecker"]) as Style;
            _btn.InvalidateVisual();
        }
    }

}