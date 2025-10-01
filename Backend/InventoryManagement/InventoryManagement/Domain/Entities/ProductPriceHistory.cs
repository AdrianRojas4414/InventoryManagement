using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagement.Domain.Entities;

[Table("Product_Price_History")]
public class ProductPriceHistory
{
    public short ProductId { get; set; }
    public short SupplierId { get; set; }
    public int PurchaseId { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal PurchasePrice { get; set; }
    public DateTime PurchaseDate { get; set; }
    
    // Propiedades de navegación
    public required virtual Product Product { get; set; }
    public required virtual Supplier Supplier { get; set; }
    public required virtual Purchase Purchase { get; set; }
}