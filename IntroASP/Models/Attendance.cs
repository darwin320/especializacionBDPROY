using System;
using System.Collections.Generic;

namespace IntroASP.Models;

public partial class Attendance
{
    public int IdAttendance { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public string CorreoElectronico { get; set; } = null!;

    public string Duracion { get; set; } = null!;

    public string HoraUnio { get; set; } = null!;

    public string HoraSalio { get; set; } = null!;

    public int? IdEvent { get; set; }

    public string? Fecha { get; set; }

    public virtual RegistryEvent? IdEventNavigation { get; set; }
}
