using System;
using System.ComponentModel.DataAnnotations;

namespace BusinessObject.Entities;

public class TarotCard
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string ImageUrl { get; set; } = null!;
}
