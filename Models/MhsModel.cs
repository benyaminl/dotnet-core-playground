using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoApi.Models {
    public class MhsModel {
        [Key][Required]
        public string nrp {get; set;}
        public string? nama {get; set;}
        public string? alamat {get; set;}
        public string? telepon {get; set;}
        
        // [InverseProperty("mhs")]
        [ForeignKey("nrp")]
        public List<NilaiModel> nilais {get; set;} = null!;
    }
}