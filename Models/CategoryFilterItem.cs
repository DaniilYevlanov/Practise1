namespace MagazinWPF.Models
{
    public class CategoryFilterItem
    {
        public int? Id { get; }

        public string Name { get; }

        public CategoryFilterItem(int? id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
