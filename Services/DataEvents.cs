using System;

namespace MagazinWPF.Services
{
    public static class DataEvents
    {
        public static event Action? ProductsChanged;
        public static event Action? CategoriesChanged;

        public static void RaiseProductsChanged() => ProductsChanged?.Invoke();

        public static void RaiseCategoriesChanged() => CategoriesChanged?.Invoke();
    }
}
