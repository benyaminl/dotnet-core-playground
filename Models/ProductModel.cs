using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoApi.Models {
    [Table("Product", Schema = "Production")]
    public class ProductModel {
        [Key]
        public int ProductID {get;set;}
        public string Name {get;set;} = null!;
        public string ProductNumber{get;set;} = null!;
        public Int16 SafetyStockLevel {get;set;}
    }   
}