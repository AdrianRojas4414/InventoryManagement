using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagement.Domain.Entities;

[Table("Product_Price_History")]
public class ProductPriceHistory
{
    [Column("id_product")]
    [ForeignKey("Product")]
    public short ProductId { get; set; }
    [Column("id_supplier")]
    [ForeignKey("Supplier")]
    public short SupplierId { get; set; }
    [Column("id_purchase")]
    [ForeignKey("Purchase")]
    public int PurchaseId { get; set; }

    [Column("purchase_price", TypeName = "decimal(10, 2)")]
    public decimal PurchasePrice { get; set; }
    [Column("purchase_date")]
    public DateTime PurchaseDate { get; set; }
    
    // Propiedades de navegación
    public required virtual Product Product { get; set; }
    public required virtual Supplier Supplier { get; set; }
    public required virtual Purchase Purchase { get; set; }
}