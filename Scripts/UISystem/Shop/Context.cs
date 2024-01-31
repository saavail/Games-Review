using System;

namespace UISystem.Shop
{
    public class Context
    {
        public int SelectedIndex = -1;
        public Action<int> OnCellClicked;
    }
}