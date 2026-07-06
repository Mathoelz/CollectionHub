using System;
using System.ComponentModel.DataAnnotations;

public class Game
{
    public Guid Id {get; set; }

    [Required]
    [StringLength(100)]
    public string? Title { get; set; }

    [Required]
    public CollectionStatus Status { get; set; }

    [Range(0, 10)]
    public int? Rating { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }

}
