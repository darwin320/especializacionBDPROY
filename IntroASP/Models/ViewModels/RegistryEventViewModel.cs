using System.Collections.Generic;

namespace IntroASP.Models.ViewModels
{
    public class RegistryEventViewModel
    {
        // La lista de registros de eventos (como la que ya estás pasando a tu vista actualmente)
        public List<RegistryEvent> RegistryEvents { get; set; }

        // Aquí, puedes agregar propiedades para representar los datos del gráfico, por ejemplo:

        // Número de estudiantes que asistieron a todas las clases
        public int AttendedAllClasses { get; set; }

        // Número de estudiantes que asistieron a 3 clases
        public int AttendedThreeClasses { get; set; }

        // Número de estudiantes que asistieron a 2 clases
        public int AttendedTwoClasses { get; set; }

        // Número de estudiantes que asistieron a 1 clase
        public int AttendedOneClass { get; set; }

        // Número de estudiantes que no asistieron a ninguna clase
        public int AttendedNone { get; set; }
    }
}
