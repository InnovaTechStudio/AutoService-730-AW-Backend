namespace AutoServiceAW.API.StaffCoordination.Domain.Model.Aggregates;

public class Mechanic
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Specialty { get; set; }
    public int MaxCapacity { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }

    public Mechanic()
    {
        FullName = string.Empty;
        Specialty = string.Empty;
        Email = string.Empty;
        Password = string.Empty;
    }

    public Mechanic(string fullName, string specialty, int maxCapacity, string email, string password)
    {
        FullName = fullName;
        Specialty = specialty;
        MaxCapacity = maxCapacity;
        Email = email;
        Password = password;
    }
}