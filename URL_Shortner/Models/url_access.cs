using System;
using System.ComponentModel.DataAnnotations;
public class url_access
{
    [Key]
    public long accesid { get; set; }
    public long urlid { get; set; }
    public DateTime accessdate { get; set; }
}
