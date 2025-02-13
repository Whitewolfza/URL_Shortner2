using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
public class short_urls
{
    [Key]
    public long urlid { get; set; }
    public string long_url { get; set; }
    public string shortcode { get; set; }
    public DateTime createdate { get; set; }
    public DateTime expiry_time { get; set; }
}

