using System;
using System.Collections.Generic;

namespace IntroASP.Models;

public partial class RegistryEvent
{
    public int IdEvent { get; set; }

    public string Nombre { get; set; } = null!;

    public string CorreoInstitucional { get; set; } = null!;

    public string ProgramaAcademico { get; set; } = null!;

    public string Asignatura { get; set; } = null!;

    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
}
