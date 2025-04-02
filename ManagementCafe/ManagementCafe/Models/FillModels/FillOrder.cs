namespace ManagementCafe.Models.FillModels
{
    public class FillOrder
    {

        public FillOrder(List<Category> categories, List<Product> products, List<PartyTable> partyTables)
        {
            Categories = categories;
            Products = products;
            PartyTables = partyTables;
        }

        public List<Category> Categories { get; set; }
        public List<Product> Products { get; set; }
        public List<PartyTable> PartyTables { get; set; }

    }
}
