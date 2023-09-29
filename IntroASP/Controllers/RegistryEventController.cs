using IntroASP.Models;
using IntroASP.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;

namespace IntroASP.Controllers
{
    public class RegistryEventController : Controller
    {
        private readonly ProjectDbContext _context;

        public RegistryEventController(ProjectDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var registryEvents = await GetRegistryEventsWithAttendances();
            return View(registryEvents);
        }

        public async Task<IActionResult> ExportToCSV()
        {
            var registryEvents = await GetRegistryEventsWithAttendances();

            var sb = new StringBuilder();

            var uniqueDatesList = (List<string>)ViewBag.UniqueDates;

            if (uniqueDatesList != null)
            {
                sb.AppendLine("Nombre;Correo Institucional;Programa Académico;Asignatura;" + string.Join(";", uniqueDatesList.Select(d => $"Asistió-{d}")));
                foreach (var registryEvent in registryEvents)
                {
                    sb.AppendLine($"{registryEvent.Nombre};{registryEvent.CorreoInstitucional};{registryEvent.ProgramaAcademico};{registryEvent.Asignatura};" + string.Join(";", uniqueDatesList.Select(d => ViewBag.AttendanceRecords[registryEvent.CorreoInstitucional][d])));
                }
            }

            return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "Asistencias.csv");
        }

        private async Task<List<RegistryEvent>> GetRegistryEventsWithAttendances()
        {
            var registryEvents = await _context.RegistryEvents.Include(r => r.Attendances).ToListAsync();

            foreach (var registryEvent in registryEvents)
            {
                if (string.IsNullOrWhiteSpace(registryEvent.Nombre))
                {
                    var relatedAttendance = registryEvent.Attendances.FirstOrDefault();
                    if (relatedAttendance != null)
                    {
                        registryEvent.Nombre = $"{relatedAttendance.Nombre} {relatedAttendance.Apellido}";
                    }
                }

                // Si el nombre sigue estando vacío después de la primera transformación:
                if (string.IsNullOrWhiteSpace(registryEvent.Nombre) && registryEvent.CorreoInstitucional.Contains('.'))
                {
                    var parts = registryEvent.CorreoInstitucional.Split('@')[0].Split('.');
                    if (parts.Length >= 2) // Asegurarse de que haya al menos un nombre y un apellido
                    {
                        registryEvent.Nombre = $"{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(parts[0])} {CultureInfo.CurrentCulture.TextInfo.ToTitleCase(parts[1])}";
                    }
                }

                // Remover números del campo Nombre:
                registryEvent.Nombre = new string(registryEvent.Nombre.Where(c => !char.IsDigit(c)).ToArray());
            }

            // Extraer todas las fechas únicas en los registros de attendance:
            var allDates = await _context.Attendances.Select(a => a.Fecha).ToListAsync();

            var uniqueDates = allDates
                                    .Where(f => !string.IsNullOrEmpty(f))
                                    .Select(f => new string(f.Where(c => char.IsDigit(c) || c == '-').ToArray()))
                                    .Distinct()
                                    .ToList();




            // uniqueDates = uniqueDates.Select(date => new string(date.Where(char.IsDigit).ToArray())).ToList();

            // Crear un diccionario para guardar las asistencias:
            var attendanceRecords = new Dictionary<string, Dictionary<string, string>>();

            foreach (var registryEvent in registryEvents)
            {
                var studentAttendances = new Dictionary<string, string>();

                foreach (var date in uniqueDates)
                {
                    var attended = DidStudentAttendOnDate(registryEvent, date);
                    studentAttendances[date] = attended ? "Sí" : "No";
                }

                attendanceRecords[registryEvent.CorreoInstitucional] = studentAttendances;
            }

            // Pasar las fechas únicas y los registros de asistencia a la vista
            ViewBag.UniqueDates = uniqueDates;
            ViewBag.AttendanceRecords = attendanceRecords;


            return registryEvents;
        }

        private bool DidStudentAttendOnDate(RegistryEvent registryEvent, string date)
        {
            if (string.IsNullOrEmpty(date))
                return false;

            return registryEvent.Attendances.Any(a => a.Fecha == date);
        }

        public async Task<IActionResult> Index1()
        {
            var registryEvents = await GetRegistryEventsWithAttendances();
            var attendanceCounts = new int[5];

            var attendanceRecords = ViewBag.AttendanceRecords as Dictionary<string, Dictionary<string, string>>;

            foreach (var studentAttendances in attendanceRecords.Values)
            {
                int attendanceCount = studentAttendances.Values.Count(v => v == "Sí");
                attendanceCounts[attendanceCount]++;
            }



            ViewBag.AttendedAll = attendanceCounts[4];
            ViewBag.AttendedThree = attendanceCounts[3];
            ViewBag.AttendedTwo = attendanceCounts[2];
            ViewBag.AttendedOne = attendanceCounts[1];
            ViewBag.AttendedNone = attendanceCounts[0];

            return View();
        }


        private string NormalizeSubjectName(string subjectName)
        {
            if (string.IsNullOrEmpty(subjectName))
                return subjectName;

            subjectName = subjectName.ToUpper().Trim();
            subjectName = subjectName.Normalize(NormalizationForm.FormD); // Para remover tildes
            subjectName = new string(subjectName.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray());

            if (subjectName.StartsWith("COMUNICACIONES 2")
                || subjectName.StartsWith("COMUNICACIONES II"))
            {
                return "COMUNICACIONES 2";
            }

            if (subjectName.Contains("COMUNICACIONES - INVESTIGACION DE OPERACIONES")
                || subjectName.Contains("COMUNICA")
                || subjectName.Contains("COMUICACIONES") // versión mal escrita
                || subjectName.Contains("OPERACIONES"))
            {
                return "COMUNICACIONES";
            }

            if (subjectName.Contains("ELECTIVA III") || subjectName.Contains("GESTION DE REDES"))
            {
                return "ELECTIVA III - GESTION DE REDES";
            }

            if (subjectName.Contains("TRANSMISION")
                || subjectName.Contains("TRANSMISIÓN")
                || subjectName.Contains("TRASMISIÓN")
                || subjectName.Contains("TRANSMISIONES")
                || subjectName.Contains("TRANSMISIÓN DE DATOS - SEMINARIO DE GRADO"))
            {
                return "TRANSMISION DE DATOS";
            }

            if (subjectName.Contains("TELEMATICA")
                || subjectName.Contains("TELEMATICS")
                || subjectName.Contains("TELEMÁTICA"))
            {
                return "TELEMATICA";
            }

            return subjectName;
        }




        public async Task<IActionResult> AcademicProgramsWithSubjects()
        {
            var registryEvents = await _context.RegistryEvents.ToListAsync();

            Dictionary<string, HashSet<string>> programWithSubjects = new Dictionary<string, HashSet<string>>();

            foreach (var registryEvent in registryEvents)
            {
                string normalizedSubject = NormalizeSubjectName(registryEvent.Asignatura);

                if (!programWithSubjects.ContainsKey(registryEvent.ProgramaAcademico))
                {
                    programWithSubjects[registryEvent.ProgramaAcademico] = new HashSet<string>();
                }

                programWithSubjects[registryEvent.ProgramaAcademico].Add(normalizedSubject);
            }

            ViewBag.ProgramWithSubjects = programWithSubjects;

            return View();
        }

        public async Task<IActionResult> ExportAcademicProgramsWithSubjectsToCSV()
        {
            var registryEvents = await _context.RegistryEvents.ToListAsync();
            Dictionary<string, HashSet<string>> programWithSubjects = new Dictionary<string, HashSet<string>>();

            foreach (var registryEvent in registryEvents)
            {
                string normalizedSubject = NormalizeSubjectName(registryEvent.Asignatura);
                if (!programWithSubjects.ContainsKey(registryEvent.ProgramaAcademico))
                {
                    programWithSubjects[registryEvent.ProgramaAcademico] = new HashSet<string>();
                }
                programWithSubjects[registryEvent.ProgramaAcademico].Add(normalizedSubject);
            }

            var sb = new StringBuilder();
            sb.AppendLine("Programa Académico;Asignaturas");
            foreach (var program in programWithSubjects)
            {
                sb.AppendLine($"{program.Key};{string.Join(", ", program.Value)}");
            }

            return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "ProgramasConAsignaturas.csv");
        }





    }


}
