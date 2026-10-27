using System;

namespace RedElectricaMunicipal
{
    // =========================================================================
    // ESTRUCTURA: Grafo Dirigido y Ponderado (Lista de Adyacencia Manual)
    // AUTOR / RESPONSABLE: Ferry de Jesús López del Cid
    // JUSTIFICACIÓN TÉCNICA: Modela las subestaciones y líneas de distribución
    // mediante arreglos y listas enlazadas propias sin colecciones nativas.
    // =========================================================================

    public class NodoArista
    {
        public int IdDestino;
        public double LongitudKm;
        public double CostoMantenimiento;
        public NodoArista Siguiente;

        public NodoArista(int idDestino, double longitudKm, double costoMantenimiento)
        {
            IdDestino = idDestino;
            LongitudKm = longitudKm;
            CostoMantenimiento = costoMantenimiento;
            Siguiente = null;
        }
    }

    public class Grafo
    {
        private Subestacion[] vertices;
        private NodoArista[] listaAdyacencia;
        private int capacidadMaxima;
        private int totalSubestaciones;

        public int TotalSubestaciones
        {
            get { return totalSubestaciones; }
        }

        public Subestacion[] Vertices
        {
            get { return vertices; }
        }

        public Grafo(int maxSubestaciones)
        {
            capacidadMaxima = maxSubestaciones;
            vertices = new Subestacion[capacidadMaxima];
            listaAdyacencia = new NodoArista[capacidadMaxima];
            totalSubestaciones = 0;
        }

        // 1. Insertar Subestación (Vértice)
        public int RegistrarSubestacion(string codigo, string nombre, double capacidad)
        {
            if (totalSubestaciones >= capacidadMaxima)
                return -1;

            if (BuscarIdPorCodigo(codigo) != -1)
                return -1;

            int nuevoId = totalSubestaciones;
            vertices[nuevoId] = new Subestacion(codigo, nombre, capacidad, nuevoId);
            listaAdyacencia[nuevoId] = null;
            totalSubestaciones++;
            return nuevoId;
        }

        // Buscar ID interno de una subestación por su código
        public int BuscarIdPorCodigo(string codigo)
        {
            codigo = (codigo ?? "").Trim().ToUpper();
            for (int i = 0; i < totalSubestaciones; i++)
            {
                if (vertices[i] != null && vertices[i].Codigo == codigo)
                    return i;
            }
            return -1;
        }

        // 2. Insertar Línea de Distribución (Arista dirigida o bidireccional)
        public bool AgregarLinea(int idOrigen, int idDestino, double longitudKm, double costo, bool bidireccional)
        {
            if (idOrigen < 0 || idOrigen >= totalSubestaciones || idDestino < 0 || idDestino >= totalSubestaciones)
                return false;

            if (idOrigen == idDestino)
                return false;

            InsertarAristaUnidireccional(idOrigen, idDestino, longitudKm, costo);

            if (bidireccional)
            {
                InsertarAristaUnidireccional(idDestino, idOrigen, longitudKm, costo);
            }

            return true;
        }

        private void InsertarAristaUnidireccional(int origen, int destino, double longitudKm, double costo)
        {
            NodoArista actual = listaAdyacencia[origen];
            while (actual != null)
            {
                if (actual.IdDestino == destino)
                {
                    actual.LongitudKm = longitudKm;
                    actual.CostoMantenimiento = costo;
                    return;
                }
                actual = actual.Siguiente;
            }

            NodoArista nueva = new NodoArista(destino, longitudKm, costo);
            nueva.Siguiente = listaAdyacencia[origen];
            listaAdyacencia[origen] = nueva;
        }

        // 3. Eliminar Línea de Distribución
        public bool EliminarLinea(int idOrigen, int idDestino, bool bidireccional)
        {
            bool borrado = EliminarAristaUnidireccional(idOrigen, idDestino);
            if (bidireccional)
            {
                EliminarAristaUnidireccional(idDestino, idOrigen);
            }
            return borrado;
        }

        private bool EliminarAristaUnidireccional(int origen, int destino)
        {
            if (origen < 0 || origen >= totalSubestaciones) return false;

            NodoArista actual = listaAdyacencia[origen];
            NodoArista anterior = null;

            while (actual != null)
            {
                if (actual.IdDestino == destino)
                {
                    if (anterior == null)
                        listaAdyacencia[origen] = actual.Siguiente;
                    else
                        anterior.Siguiente = actual.Siguiente;
                    return true;
                }
                anterior = actual;
                actual = actual.Siguiente;
            }
            return false;
        }

        // 4. Imprimir Topología de la Red
        public void ImprimirGrafo()
        {
            Console.WriteLine("\n=== TOPOLOGÍA DE LA RED ELÉCTRICA (LISTA DE ADYACENCIA) ===");
            if (totalSubestaciones == 0)
            {
                Console.WriteLine("No hay subestaciones registradas.");
                return;
            }

            for (int i = 0; i < totalSubestaciones; i++)
            {
                Console.WriteLine("\nSubestación " + vertices[i].ToString() + ":");
                NodoArista actual = listaAdyacencia[i];

                if (actual == null)
                {
                    Console.WriteLine("   └── (Sin líneas de salida)");
                }
                else
                {
                    while (actual != null)
                    {
                        Subestacion dest = vertices[actual.IdDestino];
                        Console.WriteLine("   └──> Hacia [" + dest.Codigo + "] " + dest.Nombre +
                                          " | Longitud: " + actual.LongitudKm + " km | Costo: Q" + actual.CostoMantenimiento);
                        actual = actual.Siguiente;
                    }
                }
            }
        }
    }
}