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
    
    public partial class comment
    {
        public int cmt_id { get; set; }
        public Nullable<int> lect_id { get; set; }
        public Nullable<int> stud_id { get; set; }
        public string cmt_content { get; set; }
        public int prop_id { get; set; }
    
        public virtual lecturer lecturer { get; set; }
        public virtual proposal proposal { get; set; }
        public virtual student student { get; set; }
    }
}
