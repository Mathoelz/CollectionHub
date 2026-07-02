using System;

public class Game
{
    public Guid Id {get; set; }

    public string? Title { get; set; }

    public CollectionStatus Status { get; set; }

    public int? Rating { get; set; }

    public string? Notes { get; set; }

}
