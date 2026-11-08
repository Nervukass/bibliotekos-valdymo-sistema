namespace WebApplication1.Models;

public sealed class LibraryBook
{
    public int Id { get; init; }
    public required string Title { get; init; }
    public required string Author { get; init; }
    public required string Genre { get; init; }
    public required string Year { get; init; }
    public required string Isbn { get; init; }
    public required string Shelf { get; init; }
    public required string Description { get; init; }
    public int AvailableCopies { get; init; }
    public int TotalCopies { get; init; }
    public required string CoverClass { get; init; }
}

public sealed class LoanViewModel
{
    public required string Reader { get; init; }
    public required string ReaderInitials { get; init; }
    public required string BookTitle { get; init; }
    public required string Author { get; init; }
    public required string DueDate { get; init; }
    public required string Status { get; init; }
}

public sealed class ReservationViewModel
{
    public required string Reader { get; init; }
    public required string ReaderInitials { get; init; }
    public required string BookTitle { get; init; }
    public required string RequestedOn { get; init; }
    public required string Queue { get; init; }
    public required string Status { get; init; }
}

public sealed class ReaderViewModel
{
    public required string Name { get; init; }
    public required string Initials { get; init; }
    public required string Email { get; init; }
    public required string Phone { get; init; }
    public int ActiveLoans { get; init; }
    public required string Status { get; init; }
}
