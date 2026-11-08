using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers;

public class LibraryController : Controller
{
    private static readonly IReadOnlyList<LibraryBook> Books = new List<LibraryBook>
    {
        new() { Id = 1, Title = "Altorių šešėly", Author = "Vincas Mykolaitis-Putinas", Genre = "Lietuvių klasika", Year = "1933", Isbn = "978-609-01-1234-5", Shelf = "A-12", AvailableCopies = 3, TotalCopies = 5, CoverClass = "cover-gold", Description = "Psichologinis romanas apie kunigo Liudo Vasario vidinius išgyvenimus ir savęs paieškas." },
        new() { Id = 2, Title = "1984", Author = "George Orwell", Genre = "Distopija", Year = "1949", Isbn = "978-609-01-2234-6", Shelf = "B-04", AvailableCopies = 0, TotalCopies = 4, CoverClass = "cover-red", Description = "Klasikinė distopija apie visuomenę, kurioje kiekvieną žmogaus žingsnį stebi Didysis Brolis." },
        new() { Id = 3, Title = "Mažasis princas", Author = "Antoine de Saint-Exupéry", Genre = "Grožinė literatūra", Year = "1943", Isbn = "978-609-01-3234-7", Shelf = "C-18", AvailableCopies = 2, TotalCopies = 3, CoverClass = "cover-blue", Description = "Poetiška pasaka apie draugystę, atsakomybę ir tai, kas iš tiesų svarbu." },
        new() { Id = 4, Title = "Dievų miškas", Author = "Balys Sruoga", Genre = "Memuarai", Year = "1957", Isbn = "978-609-01-4234-8", Shelf = "A-07", AvailableCopies = 1, TotalCopies = 2, CoverClass = "cover-green", Description = "Autoriaus atsiminimai iš Štuthofo koncentracijos stovyklos, parašyti savitu ironijos stiliumi." },
        new() { Id = 5, Title = "Sapiens", Author = "Yuval Noah Harari", Genre = "Istorija", Year = "2011", Isbn = "978-609-01-5234-9", Shelf = "D-11", AvailableCopies = 4, TotalCopies = 4, CoverClass = "cover-orange", Description = "Žmonijos istorija nuo pirmųjų Homo sapiens iki šiuolaikinio pasaulio." },
        new() { Id = 6, Title = "Balta drobulė", Author = "Antanas Škėma", Genre = "Lietuvių klasika", Year = "1958", Isbn = "978-609-01-6234-0", Shelf = "A-15", AvailableCopies = 1, TotalCopies = 3, CoverClass = "cover-violet", Description = "Modernistinis romanas apie Antaną Garšvą ir jo trapią vidinę būseną." }
    };

    private static readonly IReadOnlyList<LoanViewModel> LoanData = new List<LoanViewModel>
    {
        new() { Reader = "Ieva Petrauskaitė", ReaderInitials = "IP", BookTitle = "Altorių šešėly", Author = "V. Mykolaitis-Putinas", DueDate = "2026-11-12", Status = "Grąžinti netrukus" },
        new() { Reader = "Mantas Jankauskas", ReaderInitials = "MJ", BookTitle = "Mažasis princas", Author = "A. de Saint-Exupéry", DueDate = "2026-11-18", Status = "Aktyvus" },
        new() { Reader = "Gabija Kazlauskė", ReaderInitials = "GK", BookTitle = "Dievų miškas", Author = "B. Sruoga", DueDate = "2026-11-05", Status = "Pavėluota" },
        new() { Reader = "Lukas Vaitkus", ReaderInitials = "LV", BookTitle = "1984", Author = "G. Orwell", DueDate = "2026-11-21", Status = "Aktyvus" }
    };

    private static readonly IReadOnlyList<ReservationViewModel> ReservationData = new List<ReservationViewModel>
    {
        new() { Reader = "Ugnė Rimkutė", ReaderInitials = "UR", BookTitle = "1984", RequestedOn = "2026-11-06", Queue = "1 eilėje", Status = "Laukia" },
        new() { Reader = "Tadas Mikutis", ReaderInitials = "TM", BookTitle = "Balta drobulė", RequestedOn = "2026-11-04", Queue = "2 eilėje", Status = "Laukia" },
        new() { Reader = "Emilija Žukaitė", ReaderInitials = "EŽ", BookTitle = "Sapiens", RequestedOn = "2026-11-02", Queue = "Paruošta atsiimti", Status = "Paruošta" }
    };

    private static readonly IReadOnlyList<ReaderViewModel> ReaderData = new List<ReaderViewModel>
    {
        new() { Name = "Ieva Petrauskaitė", Initials = "IP", Email = "ieva.petrauskaite@email.lt", Phone = "+370 612 12345", ActiveLoans = 2, Status = "Aktyvus" },
        new() { Name = "Mantas Jankauskas", Initials = "MJ", Email = "mantas.jankauskas@email.lt", Phone = "+370 614 56789", ActiveLoans = 1, Status = "Aktyvus" },
        new() { Name = "Gabija Kazlauskė", Initials = "GK", Email = "gabija.kazlauske@email.lt", Phone = "+370 687 32109", ActiveLoans = 1, Status = "Turi skolą" },
        new() { Name = "Lukas Vaitkus", Initials = "LV", Email = "lukas.vaitkus@email.lt", Phone = "+370 699 45678", ActiveLoans = 1, Status = "Aktyvus" }
    };

    public IActionResult Dashboard()
    {
        ViewBag.TotalBooks = Books.Sum(book => book.TotalCopies);
        ViewBag.ActiveLoans = LoanData.Count;
        ViewBag.Reservations = ReservationData.Count;
        ViewBag.OverdueLoans = LoanData.Count(loan => loan.Status == "Pavėluota");
        ViewBag.RecentLoans = LoanData.Take(3);
        ViewBag.WaitingReservations = ReservationData.Take(2);
        return View();
    }

    public IActionResult Catalog() => View(Books);

    public IActionResult Book(int id)
    {
        var book = Books.FirstOrDefault(book => book.Id == id);
        return book is null ? NotFound() : View(book);
    }

    public IActionResult Loans() => View(LoanData);
    public IActionResult Reservations() => View(ReservationData);
    public IActionResult Readers() => View(ReaderData);
}
