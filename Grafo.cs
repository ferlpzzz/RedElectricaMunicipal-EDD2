using System;
using System.Collections.Generic; // Permitido únicamente para Queue<int> en el recorrido BFS

namespace RedElectricaMunicipal
{
    // =========================================================================
    // ESTRUCTURA: Grafo Dirigido y Ponderado (Lista de Adyacencia Manual)
    // AUTOR / RESPONSABLE: Ferry de Jesús López del Cid
    // JUSTIFICACIÓN TÉCNICA: Modela las subestaciones y líneas de distribución
    // mediante arreglos y listas enlazadas propias, permitiendo recorridos
    // BFS/DFS en O(V + E) y cálculo de ruta óptima con Dijkstra en O(V^2).
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

        // 4. Recorrido en Anchura (BFS) - O(V + E)
        public void RecorridoBFS(int idInicio)
        {
            if (idInicio < 0 || idInicio >= totalSubestaciones || vertices[idInicio] == null)
            {
                Console.WriteLine("Subestación de inicio inválida.");
                return;
            }

            bool[] visitado = new bool[totalSubestaciones];
            Queue<int> cola = new Queue<int>();

            visitado[idInicio] = true;
            cola.Enqueue(idInicio);

            Console.WriteLine("\n=== RECORRIDO EN ANCHURA (BFS) DESDE [" + vertices[idInicio].Codigo + "] ===");
            int paso = 1;

            while (cola.Count > 0)
            {
                int actual = cola.Dequeue();
                Console.WriteLine(paso + ". [" + vertices[actual].Codigo + "] " + vertices[actual].Nombre + " (" + vertices[actual].Capacidad + " MW)");
                paso++;

                NodoArista arista = listaAdyacencia[actual];
                while (arista != null)
                {
                    int vecino = arista.IdDestino;
                    if (!visitado[vecino])
                    {
                        visitado[vecino] = true;
                        cola.Enqueue(vecino);
                    }
                    arista = arista.Siguiente;
                }
            }
        }

        // 5. Recorrido en Profundidad (DFS) - O(V + E)
        public void RecorridoDFS(int idInicio)
        {
            if (idInicio < 0 || idInicio >= totalSubestaciones || vertices[idInicio] == null)
            {
                Console.WriteLine("Subestación de inicio inválida.");
                return;
            }

            bool[] visitado = new bool[totalSubestaciones];
            Console.WriteLine("\n=== RECORRIDO EN PROFUNDIDAD (DFS) DESDE [" + vertices[idInicio].Codigo + "] ===");
            int contador = 1;
            DFSRecursivo(idInicio, visitado, ref contador);
        }

        private void DFSRecursivo(int actual, bool[] visitado, ref int contador)
        {
            visitado[actual] = true;
            Console.WriteLine(contador + ". [" + vertices[actual].Codigo + "] " + vertices[actual].Nombre + " (" + vertices[actual].Capacidad + " MW)");
            contador++;

            NodoArista arista = listaAdyacencia[actual];
            while (arista != null)
            {
                int vecino = arista.IdDestino;
                if (!visitado[vecino])
                {
                    DFSRecursivo(vecino, visitado, ref contador);
                }
                arista = arista.Siguiente;
            }
        }

        // 6. Algoritmo de Dijkstra (Ruta de Menor Costo de Mantenimiento) - O(V^2)
        public void DijkstraRutaMenorCosto(int idOrigen, int idDestino)
        {
            if (idOrigen < 0 || idOrigen >= totalSubestaciones || idDestino < 0 || idDestino >= totalSubestaciones)
            {
                Console.WriteLine("Subestaciones inválidas para el cálculo de ruta.");
                return;
            }

            double[] costoMinimo = new double[totalSubestaciones];
            double[] longitudAcumulada = new double[totalSubestaciones];
            int[] previo = new int[totalSubestaciones];
            bool[] visitado = new bool[totalSubestaciones];

            for (int i = 0; i < totalSubestaciones; i++)
            {
                costoMinimo[i] = double.MaxValue;
                longitudAcumulada[i] = 0;
                previo[i] = -1;
                visitado[i] = false;
            }

            costoMinimo[idOrigen] = 0;

            for (int count = 0; count < totalSubestaciones - 1; count++)
            {
                int u = -1;
                double min = double.MaxValue;

                for (int v = 0; v < totalSubestaciones; v++)
                {
                    if (!visitado[v] && costoMinimo[v] < min)
                    {
                        min = costoMinimo[v];
                        u = v;
                    }
                }

                if (u == -1 || u == idDestino)
                    break;

                visitado[u] = true;

                NodoArista arista = listaAdyacencia[u];
                while (arista != null)
                {
                    int v = arista.IdDestino;
                    double pesoCosto = arista.CostoMantenimiento;

                    if (!visitado[v] && costoMinimo[u] != double.MaxValue &&
                        costoMinimo[u] + pesoCosto < costoMinimo[v])
                    {
                        costoMinimo[v] = costoMinimo[u] + pesoCosto;
                        longitudAcumulada[v] = longitudAcumulada[u] + arista.LongitudKm;
                        previo[v] = u;
                    }
                    arista = arista.Siguiente;
                }
            }

            Console.WriteLine("\n=== RUTA ÓPTIMA DE RESTABLECIMIENTO (DIJKSTRA) ===");
            if (costoMinimo[idDestino] == double.MaxValue)
            {
                Console.WriteLine("No existe una ruta dirigida disponible desde [" +
                                  vertices[idOrigen].Codigo + "] hacia [" + vertices[idDestino].Codigo + "].");
                return;
            }

            int[] camino = new int[totalSubestaciones];
            int tope = 0;
            int rastreo = idDestino;

            while (rastreo != -1)
            {
                camino[tope] = rastreo;
                tope++;
                rastreo = previo[rastreo];
            }

            Console.WriteLine("Origen:  [" + vertices[idOrigen].Codigo + "] " + vertices[idOrigen].Nombre);
            Console.WriteLine("Destino: [" + vertices[idDestino].Codigo + "] " + vertices[idDestino].Nombre);
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("Secuencia de subestaciones a energizar:");

            for (int i = tope - 1; i >= 0; i--)
            {
                int idPaso = camino[i];
                string flecha = (i > 0) ? "  ===>  " : " (SUMINISTRO RESTABLECIDO)";
                Console.WriteLine("  Paso " + (tope - i) + ": [" + vertices[idPaso].Codigo + "] " + vertices[idPaso].Nombre + flecha);
            }

            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("Costo Total de Mantenimiento: Q" + costoMinimo[idDestino].ToString("F2"));
            Console.WriteLine("Longitud Física Total:        " + longitudAcumulada[idDestino].ToString("F2") + " km");
        }

        // 7. Imprimir Topología de la Red
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