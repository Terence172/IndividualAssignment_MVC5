//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace IndividualAssignment_MVC5.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class proposal
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public proposal()
        {
            this.comments = new HashSet<comment>();
        }
    
        public int prop_id { get; set; }
        public string prop_title { get; set; }
        public string prop_type { get; set; }
        public string prop_file { get; set; }
        public string prop_status { get; set; }
        public int stu_id { get; set; }
    
        public virtual student student { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<comment> comments { get; set; }
    }
}
