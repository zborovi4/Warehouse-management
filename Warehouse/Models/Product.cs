//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Warehouse.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Web.Mvc;

    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            this.In_warehouse = new HashSet<In_warehouse>();
        }
    
        public int Id { get; set; }

        [Required]
        [Remote("IsItemNoAvailable", "Products", AdditionalFields = "Id")]
        [Editable(true)]
        public string Name { get; set; }

        [DisplayFormat(DataFormatString = "{0:F2}")]
        [Display(Name = "Base price USD")]
        public decimal Price_base { get; set; }

        [DisplayFormat(DataFormatString = "{0:F2}")]
        public decimal Price { get; set; }

        [Display(Name = "Category")]
        public int Category_id { get; set; }

        [Display(Name = "Currency")]
        public int Currency_id { get; set; }
        public int Barcode { get; set; }

        [NotMapped]
        [DisplayFormat(DataFormatString = "{0:F2}")]
        public decimal Quantity { get; set; }

        [NotMapped]
        public int? Warehouse_id{ get; set; }

        public virtual Category Category { get; set; }
        public virtual Currency Currency { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<In_warehouse> In_warehouse { get; set; }
    }
}
