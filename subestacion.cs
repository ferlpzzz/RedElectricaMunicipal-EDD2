using System;

namespace RedElectricaMunicipal
{
    // =========================================================================
    // ARCHIVO: Subestacion.cs
    // AUTOR: Ferry de Jesús López del Cid (Modelo compartido con Jose Antonio)
    // DESCRIPCIÓN: Representa una subestación eléctrica con código único,
    //              nombre, capacidad y su índice interno para Grafo y Union-Find.
    // =========================================================================
    public class Subestacion
    {
        public string Codigo { get; set; }      // Código único (Ej: "SUB-01") - Clave para Tabla Hash
        public string Nombre { get; set; }      // Nombre (Ej: "Xela Centro") - Clave para Trie
        public double Capacidad { get; set; }   // Capacidad en MW
        public int IdInterno { get; set; }      // Índice (0 a N-1) para arreglos de Grafo y Union-Find

        public Subestacion(string codigo, string nombre, double capacidad, int idInterno)
        {
            Codigo = (codigo ?? "").Trim().ToUpper();
            Nombre = (nombre ?? "").Trim();
            Capacidad = capacidad;
            IdInterno = idInterno;
        }

        public override string ToString()
        {
            return "[" + Codigo + "] " + Nombre + " | Capacidad: " + Capacidad + " MW (ID: " + IdInterno + ")";
        }
    }
}