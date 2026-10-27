using System;

namespace RedElectricaMunicipal
{
    // =========================================================================
    // ESTRUCTURA: Conjunto Disjunto (Union-Find)
    // AUTOR / RESPONSABLE: Ferry de Jesús López del Cid
    // JUSTIFICACIÓN TÉCNICA: Permite verificar en tiempo casi constante O(α(n))
    // si dos subestaciones pertenecen a la misma zona eléctrica conectada antes
    // de habilitar una nueva línea o ejecutar el algoritmo de Dijkstra.
    // =========================================================================
    public class UnionFind
    {
        private int[] padre;
        private int[] rango;
        private int capacidadMaxima;
        private int cantidadZonas;

        public UnionFind(int maxSubestaciones)
        {
            capacidadMaxima = maxSubestaciones;
            padre = new int[capacidadMaxima];
            rango = new int[capacidadMaxima];
            cantidadZonas = 0;

            for (int i = 0; i < capacidadMaxima; i++)
            {
                padre[i] = -1;
                rango[i] = 0;
            }
        }

        public void RegistrarElemento(int idInterno)
        {
            if (idInterno >= 0 && idInterno < capacidadMaxima && padre[idInterno] == -1)
            {
                padre[idInterno] = idInterno;
                rango[idInterno] = 0;
                cantidadZonas++;
            }
        }

        // Búsqueda con Compresión de Camino (Path Compression) - O(α(n))
        public int Find(int i)
        {
            if (i < 0 || i >= capacidadMaxima || padre[i] == -1)
                return -1;

            if (padre[i] != i)
            {
                padre[i] = Find(padre[i]);
            }
            return padre[i];
        }

        // Unión por Rango - O(α(n))
        public bool Union(int idA, int idB)
        {
            int raizA = Find(idA);
            int raizB = Find(idB);

            if (raizA == -1 || raizB == -1 || raizA == raizB)
                return false;

            if (rango[raizA] < rango[raizB])
            {
                padre[raizA] = raizB;
            }
            else if (rango[raizA] > rango[raizB])
            {
                padre[raizB] = raizA;
            }
            else
            {
                padre[raizB] = raizA;
                rango[raizA]++;
            }

            cantidadZonas--;
            return true;
        }

        public bool EstanEnMismaZona(int idA, int idB)
        {
            int raizA = Find(idA);
            int raizB = Find(idB);
            if (raizA == -1 || raizB == -1) return false;
            return raizA == raizB;
        }

        public void ImprimirZonas(Subestacion[] catalogo, int totalSubestaciones)
        {
            Console.WriteLine("\n=== ESTADO DE ZONAS ELÉCTRICAS (UNION-FIND) ===");
            Console.WriteLine("Zonas eléctricas independientes activas: " + cantidadZonas);
            Console.WriteLine("------------------------------------------------");

            for (int i = 0; i < totalSubestaciones; i++)
            {
                if (catalogo[i] != null && padre[i] != -1)
                {
                    int raiz = Find(i);
                    Console.WriteLine("Subestación [" + catalogo[i].Codigo + "] " + catalogo[i].Nombre +
                                      " --> Zona Raíz: [" + catalogo[raiz].Codigo + "] " + catalogo[raiz].Nombre);
                }
            }
        }
    }
}