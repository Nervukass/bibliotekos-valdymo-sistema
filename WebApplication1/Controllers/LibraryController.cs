using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers;

public class LibraryController : Controller
{
    private const string RoleSessionKey = "LibraryRole";

    private static readonly IReadOnlyList<LibraryBook> Books =
    [
        new() { Id = 1, Title = "Altorių šešėly", Author = "Vincas Mykolaitis-Putinas", Genre = "Lietuvių klasika", Year = "1933", Isbn = "978-609-01-1234-5", Shelf = "A-12", AvailableCopies = 3, TotalCopies = 5, CoverClass = "cover-gold", Description = "Psichologinis romanas apie Liudo Vasario vidinius išgyvenimus ir savęs paieškas." },
        new() { Id = 2, Title = "1984", Author = "George Orwell", Genre = "Distopija", Year = "1949", Isbn = "978-609-01-2234-6", Shelf = "B-04", AvailableCopies = 0, TotalCopies = 4, CoverClass = "cover-red", Description = "Klasikinė distopija apie visuomenę, kurioje kiekvieną žmogaus žingsnį stebi Didysis Brolis." },
        new() { Id = 3, Title = "Mažasis princas", Author = "Antoine de Saint-Exupéry", Genre = "Grožinė literatūra", Year = "1943", Isbn = "978-609-01-3234-7", Shelf = "C-18", AvailableCopies = 2, TotalCopies = 3, CoverClass = "cover-blue", Description = "Poetiška pasaka apie draugystę, atsakomybę ir tai, kas iš tiesų svarbu." },
        new() { Id = 4, Title = "Dievų miškas", Author = "Balys Sruoga", Genre = "Memuarai", Year = "1957", Isbn = "978-609-01-4234-8", Shelf = "A-07", AvailableCopies = 1, TotalCopies = 2, CoverClass = "cover-green", Description = "Autoriaus atsiminimai iš Štuthofo koncentracijos stovyklos." },
        new() { Id = 5, Title = "Sapiens", Author = "Yuval Noah Harari", Genre = "Istorija", Year = "2011", Isbn = "978-609-01-5234-9", Shelf = "D-11", AvailableCopies = 4, TotalCopies = 4, CoverClass = "cover-orange", Description = "Žmonijos istorija nuo pirmųjų Homo sapiens iki šiuolaikinio pasaulio." },
        new() { Id = 6, Title = "Balta drobulė", Author = "Antanas Škėma", Genre = "Lietuvių klasika", Year = "1958", Isbn = "978-609-01-6234-0", Shelf = "A-15", AvailableCopies = 1, TotalCopies = 3, CoverClass = "cover-violet", Description = "Modernistinis romanas apie Antaną Garšvą ir jo trapią vidinę būseną." }
    ];

    private static readonly IReadOnlyList<LoanViewModel> LoanData =
    [
        new() { Reader = "Ieva Petrauskaitė", ReaderInitials = "IP", BookTitle = "Altorių šešėly", Author = "V. Mykolaitis-Putinas", DueDate = "2026-11-12", Status = "Grąžinti netrukus" },
        new() { Reader = "Mantas Jankauskas", ReaderInitials = "MJ", BookTitle = "Mažasis princas", Author = "A. de Saint-Exupéry", DueDate = "2026-11-18", Status = "Aktyvus" },
        new() { Reader = "Gabija Kazlauskė", ReaderInitials = "GK", BookTitle = "Dievų miškas", Author = "B. Sruoga", DueDate = "2026-11-05", Status = "Pavėluota" },
        new() { Reader = "Lukas Vaitkus", ReaderInitials = "LV", BookTitle = "1984", Author = "G. Orwell", DueDate = "2026-11-21", Status = "Aktyvus" }
    ];

    private static readonly IReadOnlyList<ReservationViewModel> ReservationData =
    [
        new() { Reader = "Ugnė Rimkutė", ReaderInitials = "UR", BookTitle = "1984", RequestedOn = "2026-11-06", Queue = "1 eilėje", Status = "Laukia" },
        new() { Reader = "Tadas Mikutis", ReaderInitials = "TM", BookTitle = "Balta drobulė", RequestedOn = "2026-11-04", Queue = "2 eilėje", Status = "Laukia" },
        new() { Reader = "Emilija Žukaitė", ReaderInitials = "EŽ", BookTitle = "Sapiens", RequestedOn = "2026-11-02", Queue = "Paruošta atsiimti", Status = "Paruošta" }
    ];

    private static readonly IReadOnlyList<ReaderViewModel> ReaderData =
    [
        new() { Name = "Ieva Petrauskaitė", Initials = "IP", Email = "ieva.petrauskaite@email.lt", Phone = "+370 612 12345", ActiveLoans = 2, Status = "Aktyvus" },
        new() { Name = "Mantas Jankauskas", Initials = "MJ", Email = "mantas.jankauskas@email.lt", Phone = "+370 614 56789", ActiveLoans = 1, Status = "Aktyvus" },
        new() { Name = "Gabija Kazlauskė", Initials = "GK", Email = "gabija.kazlauske@email.lt", Phone = "+370 687 32109", ActiveLoans = 1, Status = "Turi skolą" },
        new() { Name = "Lukas Vaitkus", Initials = "LV", Email = "lukas.vaitkus@email.lt", Phone = "+370 699 45678", ActiveLoans = 1, Status = "Aktyvus" }
    ];

    private static readonly IReadOnlyDictionary<string, PortalPageViewModel> Pages = CreatePages();

    public IActionResult Dashboard()
    {
        var denied = Require(LibraryRoles.Librarian);
        if (denied is not null) return denied;

        ViewBag.TotalBooks = Books.Sum(book => book.TotalCopies);
        ViewBag.ActiveLoans = LoanData.Count;
        ViewBag.Reservations = ReservationData.Count;
        ViewBag.OverdueLoans = LoanData.Count(loan => loan.Status == "Pavėluota");
        ViewBag.RecentLoans = LoanData.Take(3);
        ViewBag.WaitingReservations = ReservationData.Take(2);
        return View();
    }

    public IActionResult Page(string id)
    {
        if (!Pages.TryGetValue(id, out var page)) return NotFound();

        var denied = Require(page.Role);
        if (denied is not null) return denied;

        return View("PortalPage", page);
    }

    public IActionResult Catalog()
    {
        var denied = Require(LibraryRoles.Librarian, LibraryRoles.Client);
        return denied ?? View(Books);
    }

    public IActionResult Book(int id)
    {
        var denied = Require(LibraryRoles.Librarian, LibraryRoles.Client);
        if (denied is not null) return denied;
        var book = Books.FirstOrDefault(book => book.Id == id);
        return book is null ? NotFound() : View(book);
    }

    public IActionResult Loans()
    {
        var denied = Require(LibraryRoles.Librarian);
        return denied ?? View(LoanData);
    }

    public IActionResult Reservations()
    {
        var denied = Require(LibraryRoles.Librarian);
        return denied ?? View(ReservationData);
    }

    public IActionResult Readers()
    {
        var denied = Require(LibraryRoles.Librarian);
        return denied ?? View(ReaderData);
    }

    private IActionResult? Require(params string[] roles)
    {
        var role = HttpContext.Session.GetString(RoleSessionKey);
        if (role is not null && roles.Contains(role)) return null;

        TempData["RoleMessage"] = "Šis langas nepasiekiamas su pasirinkta role.";
        return RedirectToAction("Index", "Home");
    }

    private static IReadOnlyDictionary<string, PortalPageViewModel> CreatePages()
    {
        var pages = new List<PortalPageViewModel>
        {
            new()
            {
                Id = "profile", Role = LibraryRoles.Client, Section = "KLIENTO PASKYRA", Title = "Mano profilis",
                Description = "Kliento paskyros pagrindinis langas. Iš čia pasieksite visas savo paslaugas.",
                Details = [("Vardas", "Austėja Petrauskaitė"), ("El. paštas", "austeja@email.lt"), ("Statusas", "Aktyvus klientas")],
                Actions = [
                    Action("Redaguoti profilį", "Pakeiskite savo kontaktinius duomenis.", "profile-edit"),
                    Action("Mano rezervacijos", "Peržiūrėkite rezervuotų knygų sąrašą.", "my-reservations"),
                    Action("Apmokėti baudą", "Atidarykite savo baudos apmokėjimo langą.", "fine-payment"),
                    Action("Rekomendacijos", "Atraskite jums siūlomas knygas.", "recommendations"),
                    Action("Skolinimosi istorija", "Peržiūrėkite ankstesnius išdavimus.", "loan-history"),
                    Action("Knygų peržiūra", "Raskite knygą ir palikite atsiliepimą.", "catalog")
                ]
            },
            new()
            {
                Id = "profile-edit", Role = LibraryRoles.Client, Section = "KLIENTO PASKYRA", Title = "Profilio redagavimas",
                Description = "Demonstracinė forma – duomenys jau užpildyti ir gali būti laikomi hardcode.", BackPageId = "profile",
                Details = [("Telefonas", "+370 600 00000"), ("Miestas", "Vilnius"), ("Pranešimai", "Įjungti")]
            },
            new()
            {
                Id = "my-reservations", Role = LibraryRoles.Client, Section = "KLIENTO PASKYRA", Title = "Rezervacijų sąrašas",
                Description = "Jūsų laukiančios ir paruoštos atsiimti knygos.", BackPageId = "profile",
                Details = [("1984", "1 vieta eilėje"), ("Balta drobulė", "Paruošta atsiimti iki lapkričio 14 d."), ("Mokėtina bauda", "2,00 €")],
                Actions = [Action("Apmokėti baudą", "Atidarykite baudos apmokėjimo langą.", "fine-payment")]
            },
            new()
            {
                Id = "fine-payment", Role = LibraryRoles.Client, Section = "KLIENTO PASKYRA", Title = "Baudos apmokėjimas",
                Description = "Demonstraciniame lange rodoma mokėtina suma ir apmokėjimo būsena.", BackPageId = "my-reservations",
                Details = [("Bauda", "2,00 €"), ("Priežastis", "Pavėluotas grąžinimas"), ("Būsena", "Laukia apmokėjimo")]
            },
            new()
            {
                Id = "recommendations", Role = LibraryRoles.Client, Section = "KLIENTO PASKYRA", Title = "Rekomendacijos",
                Description = "Knygos, parinktos pagal jūsų ankstesnius skolinimus.", BackPageId = "profile",
                Details = [("Metai", "Alvydas Šlepikas"), ("Keliautojo laiku žmona", "Audrey Niffenegger"), ("Sapiens", "Yuval Noah Harari")]
            },
            new()
            {
                Id = "loan-history", Role = LibraryRoles.Client, Section = "KLIENTO PASKYRA", Title = "Knygų skolinimosi istorija",
                Description = "Ankstesni ir dabar aktyvūs jūsų knygų išdavimai.", BackPageId = "profile",
                Details = [("Mažasis princas", "Grąžinta 2026-10-16"), ("Dievų miškas", "Grąžinta 2026-09-21"), ("Altorių šešėly", "Grąžinti iki 2026-11-12")]
            },
            new()
            {
                Id = "books-overview", Role = LibraryRoles.Client, Section = "KLIENTO PASKYRA", Title = "Knygų peržiūra",
                Description = "Knygų katalogo peržiūros langas klientui.", BackPageId = "profile",
                Details = [("Mažasis princas", "Laisvi 2 iš 3 egzempliorių"), ("Sapiens", "Laisvi 4 iš 4 egzempliorių"), ("1984", "Šiuo metu nėra laisvų")],
                Actions = [Action("Kurti atsiliepimą", "Palikite trumpą atsiliepimą apie perskaitytą knygą.", "review-create")]
            },
            new()
            {
                Id = "review-create", Role = LibraryRoles.Client, Section = "KLIENTO PASKYRA", Title = "Atsiliepimo kūrimas",
                Description = "Demonstracinis knygos vertinimo ir komentaro langas.", BackPageId = "catalog",
                Details = [("Knyga", "Mažasis princas"), ("Įvertinimas", "5 / 5"), ("Komentaras", "Jautri ir įkvepianti knyga.")]
            },

            new()
            {
                Id = "admin-dashboard", Role = LibraryRoles.Administrator, Section = "ADMINISTRATORIAUS POSISTEMĖ", Title = "Administratoriaus darbo vieta",
                Description = "Pasirinkite, kurią bibliotekos administravimo funkciją norite atlikti.",
                Details = [("Klientai", "124 aktyvūs"), ("Bibliotekininkai", "6 darbuotojai"), ("Šio mėnesio ataskaita", "Paruošta")],
                Actions = [
                    Action("Apriboti kliento profilį", "Sustabdykite kliento paskyros prieigą.", "restrict-client"),
                    Action("Priskirti baudą", "Sukurkite baudą konkrečiam klientui.", "assign-fine"),
                    Action("Priminimai apie grąžinimą", "Siųskite priminimą el. paštu.", "due-reminder"),
                    Action("Pranešti apie rezervaciją", "Siųskite SMS apie paruoštą knygą.", "reservation-notice"),
                    Action("Redaguoti kliento profilį", "Atnaujinkite kliento paskyros duomenis.", "client-profiles"),
                    Action("Redaguoti bibliotekininko profilį", "Tvarkykite darbuotojų paskyras.", "librarian-profiles"),
                    Action("Sudaryti bibliotekos statistiką", "Atidarykite bibliotekos statistikos langą.", "statistics")
                ]
            },
            new() { Id = "restrict-client", Role = LibraryRoles.Administrator, Section = "ADMINISTRATORIAUS POSISTEMĖ", Title = "Kliento profilio apribojimas", Description = "Pasirinkite klientą ir nurodykite prieigos apribojimo priežastį.", BackPageId = "admin-dashboard", Details = [("Klientas", "Gabija Kazlauskė"), ("Priežastis", "Neapmokėta bauda"), ("Būsena", "Apribojimas neaktyvus")] },
            new() { Id = "assign-fine", Role = LibraryRoles.Administrator, Section = "ADMINISTRATORIAUS POSISTEMĖ", Title = "Baudos priskyrimas", Description = "Naujos baudos sukūrimo langas.", BackPageId = "admin-dashboard", Details = [("Klientas", "Gabija Kazlauskė"), ("Suma", "2,00 €"), ("Terminas", "2026-11-20")] },
            new() { Id = "due-reminder", Role = LibraryRoles.Administrator, Section = "ADMINISTRATORIAUS POSISTEMĖ", Title = "Priminimas apie grąžinimo terminą", Description = "Pranešimas bus išsiųstas pasirinktam klientui el. paštu.", BackPageId = "admin-dashboard", Details = [("Gavėjas", "ieva.petrauskaite@email.lt"), ("Knyga", "Altorių šešėly"), ("Grąžinti iki", "2026-11-12")] },
            new() { Id = "reservation-notice", Role = LibraryRoles.Administrator, Section = "ADMINISTRATORIAUS POSISTEMĖ", Title = "Pranešimas apie knygos rezervaciją", Description = "SMS pranešimo klientui paruošimo langas.", BackPageId = "admin-dashboard", Details = [("Gavėjas", "+370 600 00000"), ("Knyga", "Balta drobulė"), ("Žinutė", "Knyga jau laukia jūsų bibliotekoje.")] },
            new() { Id = "client-profiles", Role = LibraryRoles.Administrator, Section = "ADMINISTRATORIAUS POSISTEMĖ", Title = "Klientų profiliai", Description = "Klientų sąrašo ir pasirinkto profilio redagavimo langas.", BackPageId = "admin-dashboard", Details = [("Ieva Petrauskaitė", "Aktyvi"), ("Mantas Jankauskas", "Aktyvus"), ("Gabija Kazlauskė", "Turi baudą")] },
            new() { Id = "librarian-profiles", Role = LibraryRoles.Administrator, Section = "ADMINISTRATORIAUS POSISTEMĖ", Title = "Bibliotekininkų profiliai", Description = "Bibliotekininkų paskyrų redagavimo langas.", BackPageId = "admin-dashboard", Details = [("Birutė Kazlauskė", "Katalogavimas"), ("Tomas Jonaitis", "Išdavimai"), ("Rasa Vaitkutė", "Grąžinimai")] },
            new() { Id = "statistics", Role = LibraryRoles.Administrator, Section = "ADMINISTRATORIAUS POSISTEMĖ", Title = "Bibliotekos statistika", Description = "Bibliotekos naudojimo suvestinė ir ataskaitos duomenys.", BackPageId = "admin-dashboard", Details = [("Išduota knygų", "348 šį mėnesį"), ("Naujos rezervacijos", "76"), ("Populiariausia knyga", "Mažasis princas")] },

            new()
            {
                Id = "issue-book", Role = LibraryRoles.Librarian, Section = "BIBLIOTEKININKO POSISTEMĖ", Title = "Knygos išdavimas",
                Description = "Užregistruokite knygos išdavimą klientui.",
                Details = [("Skaitytojas", "Mantas Jankauskas"), ("Knyga", "Mažasis princas"), ("Grąžinti iki", "2026-11-23")]
            },
            new() { Id = "renew-book", Role = LibraryRoles.Librarian, Section = "BIBLIOTEKININKO POSISTEMĖ", Title = "Knygos atnaujinimas", Description = "Pratęskite pasirinktos knygos skolinimosi terminą.", Details = [("Skaitytojas", "Ieva Petrauskaitė"), ("Knyga", "Altorių šešėly"), ("Naujas terminas", "2026-11-26")] },
            new() { Id = "add-book", Role = LibraryRoles.Librarian, Section = "BIBLIOTEKININKO POSISTEMĖ", Title = "Naujos knygos pridėjimas", Description = "Katalogo papildymo langas.", Details = [("Pavadinimas", "Nauja knyga"), ("Autorius", "Autoriaus vardas"), ("Lentyna", "E-03")] },
            new() { Id = "remove-book", Role = LibraryRoles.Librarian, Section = "BIBLIOTEKININKO POSISTEMĖ", Title = "Knygos pašalinimas", Description = "Pašalinkite nurašytą arba prarastą katalogo egzempliorių.", Details = [("Knyga", "Nurašytas egzempliorius"), ("Priežastis", "Sugadinta"), ("Būsena", "Laukia patvirtinimo")] },
            new() { Id = "book-condition", Role = LibraryRoles.Librarian, Section = "BIBLIOTEKININKO POSISTEMĖ", Title = "Knygos būklės peržiūra", Description = "Patikrinkite pasirinktos knygos fizinę būklę.", Details = [("Knyga", "Dievų miškas"), ("Būklė", "Gera"), ("Paskutinė apžiūra", "2026-10-29")], Actions = [Action("Vertinti knygos būklę", "Užregistruokite naują būklės įvertinimą.", "assess-condition")] },
            new() { Id = "assess-condition", Role = LibraryRoles.Librarian, Section = "BIBLIOTEKININKO POSISTEMĖ", Title = "Knygos būklės vertinimas", Description = "Demonstracinė knygos būklės įvertinimo forma.", BackPageId = "book-condition", Details = [("Knyga", "Dievų miškas"), ("Įvertinimas", "Gera"), ("Pastaba", "Viršelis šiek tiek nusidėvėjęs")] }
        };

        return pages.ToDictionary(page => page.Id);
    }

    private static PortalActionViewModel Action(string title, string description, string pageId) =>
        new() { Title = title, Description = description, PageId = pageId };
}
