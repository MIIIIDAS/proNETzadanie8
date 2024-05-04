using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

class Zadanie
{
    public int Id { get; set; }
    public string Nazwa { get; set; }
    public string? Opis { get; set; }
    public DateTime DataZakonczenia { get; set; }
    public bool CzyWykonane { get; set; }

    public Zadanie(int id, string nazwa, string? opis, DateTime dataZakonczenia)
    {
        Id = id;
        Nazwa = nazwa ?? throw new ArgumentNullException(nameof(nazwa));
        Opis = opis;
        DataZakonczenia = dataZakonczenia;
        CzyWykonane = false;
    }
}

class ManagerZadan
{
    public List<Zadanie> ListaZadan = new List<Zadanie>();

    public void DodajZadanie(Zadanie zadanie)
    {
        ListaZadan.Add(zadanie);
    }

    public void UsunZadanie(int id)
    {
        var zadanie = ListaZadan.FirstOrDefault(z => z.Id == id);
        if (zadanie != null)
            ListaZadan.Remove(zadanie);
        else
            Console.WriteLine("Nie znaleziono zadania o podanym ID.");
    }

    public void WyswietlZadania()
    {
        if (ListaZadan.Count == 0)
        {
            Console.WriteLine("Lista zadań jest pusta.");
            return;
        }

        Console.WriteLine("Lista zadań:");
        foreach (var zadanie in ListaZadan)
        {
            Console.WriteLine($"ID: {zadanie.Id}, Nazwa: {zadanie.Nazwa}, Data zakończenia: {zadanie.DataZakonczenia}, Wykonane: {(zadanie.CzyWykonane ? "Tak" : "Nie")}");
        }
    }

    public void ZapiszDoPliku(string sciezka)
    {
        var opcje = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        var json = JsonSerializer.Serialize(ListaZadan, opcje);
        File.WriteAllText(sciezka, json);
    }

    public void WczytajZPliku(string sciezka)
    {
        if (File.Exists(sciezka))
        {
            var json = File.ReadAllText(sciezka);
            ListaZadan = JsonSerializer.Deserialize<List<Zadanie>>(json);
        }
        else
        {
            Console.WriteLine("Plik nie istnieje.");
        }
    }

    public void EdytujZadanie(int id, string nazwa, string? opis, DateTime dataZakonczenia)
    {
        var zadanie = ListaZadan.FirstOrDefault(z => z.Id == id);
        if (zadanie != null)
        {
            zadanie.Nazwa = nazwa ?? throw new ArgumentNullException(nameof(nazwa));
            zadanie.Opis = opis;
            zadanie.DataZakonczenia = dataZakonczenia;
        }
        else
        {
            Console.WriteLine("Nie znaleziono zadania o podanym ID.");
        }
    }

    public void OznaczJakoWykonane(int id)
    {
        var zadanie = ListaZadan.FirstOrDefault(z => z.Id == id);
        if (zadanie != null)
        {
            zadanie.CzyWykonane = true;
        }
        else
        {
            Console.WriteLine("Nie znaleziono zadania o podanym ID.");
        }
    }

    public void SortujZadaniaPoNazwie()
    {
        ListaZadan = ListaZadan.OrderBy(z => z.Nazwa).ToList();
    }

    public void SortujZadaniaPoDacie()
    {
        ListaZadan = ListaZadan.OrderBy(z => z.DataZakonczenia).ToList();
    }
}

class Program
{
    static void Main(string[] args)
    {
        ManagerZadan manager = new ManagerZadan();

        while (true)
        {
            Console.WriteLine("1. Dodaj zadanie");
            Console.WriteLine("2. Usuń zadanie");
            Console.WriteLine("3. Wyświetl zadania");
            Console.WriteLine("4. Zapisz listę zadań do pliku");
            Console.WriteLine("5. Wczytaj listę zadań z pliku");
            Console.WriteLine("6. Edytuj zadanie");
            Console.WriteLine("7. Oznacz zadanie jako wykonane");
            Console.WriteLine("8. Sortuj zadania po nazwie");
            Console.WriteLine("9. Sortuj zadania po dacie");
            Console.WriteLine("0. Wyjdź");

            string? input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    Console.WriteLine("Podaj nazwę zadania:");
                    string? nazwa = Console.ReadLine();
                    Console.WriteLine("Podaj opis zadania:");
                    string? opis = Console.ReadLine();
                    Console.WriteLine("Podaj datę zakończenia zadania (w formacie YYYY-MM-DD HH:MM):");
                    if (DateTime.TryParse(Console.ReadLine(), out DateTime dataZakonczenia))
                    {
                        int id = manager.ListaZadan.Count > 0 ? manager.ListaZadan.Max(z => z.Id) + 1 : 1;
                        Zadanie noweZadanie = new Zadanie(id, nazwa, opis, dataZakonczenia);
                        manager.DodajZadanie(noweZadanie);
                        Console.WriteLine("Dodano nowe zadanie.");
                    }
                    else
                    {
                        Console.WriteLine("Nieprawidłowy format daty.");
                    }
                    break;
                case "2":
                    Console.WriteLine("Podaj ID zadania do usunięcia:");
                    if (int.TryParse(Console.ReadLine(), out int idUsun))
                    {
                        manager.UsunZadanie(idUsun);
                    }
                    else
                    {
                        Console.WriteLine("Nieprawidłowy format ID.");
                    }
                    break;
                case "3":
                    manager.WyswietlZadania();
                    break;
                case "4":
                    manager.ZapiszDoPliku("lista_zadan.json");
                    break;
                case "5":
                    manager.WczytajZPliku("lista_zadan.json");
                    break;
                case "6":
                    Console.WriteLine("Podaj ID zadania do edycji:");
                    if (int.TryParse(Console.ReadLine(), out int idEdytuj))
                    {
                        Console.WriteLine("Podaj nową nazwę zadania:");
                        string? nowaNazwa = Console.ReadLine();
                        Console.WriteLine("Podaj nowy opis zadania:");
                        string? nowyOpis = Console.ReadLine();
                        Console.WriteLine("Podaj nową datę zakończenia zadania (w formacie YYYY-MM-DD HH:MM):");
                        if (DateTime.TryParse(Console.ReadLine(), out DateTime nowaDataZakonczenia))
                        {
                            manager.EdytujZadanie(idEdytuj, nowaNazwa, nowyOpis, nowaDataZakonczenia);
                            Console.WriteLine("Zadanie zostało zaktualizowane.");
                        }
                        else
                        {
                            Console.WriteLine("Nieprawidłowy format daty.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Nieprawidłowy format ID.");
                    }
                    break;
                case "7":
                    Console.WriteLine("Podaj ID zadania do oznaczenia jako wykonane:");
                    if (int.TryParse(Console.ReadLine(), out int idOznacz))
                    {
                        manager.OznaczJakoWykonane(idOznacz);
                        Console.WriteLine("Zadanie zostało oznaczone jako wykonane.");
                    }
                    else
                    {
                        Console.WriteLine("Nieprawidłowy format ID.");
                    }
                    break;
                case "8":
                    manager.SortujZadaniaPoNazwie();
                    Console.WriteLine("Zadania zostały posortowane po nazwie.");
                    break;
                case "9":
                    manager.SortujZadaniaPoDacie();
                    Console.WriteLine("Zadania zostały posortowane po dacie.");
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Nieprawidłowy wybór.");
                    break;
            }
            Console.WriteLine();
        }
    }
}
