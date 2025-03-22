namespace ManagementCafe.Models.FillModels
{
    public class FillOrder
    {

        public FillOrder(List<Category> categories, List<Product> products)
        {
            Categories = categories;
            Products = products;
        }

        public List<Category> Categories { get; set; }
        public List<Product> Products { get; set; }

    }
}
