using System;

namespace RedElectricaMunicipal
{
    // =========================================================================
    //ESTRUCTURA: Árbol de Prefijos (Trie) con Arreglo Puro ASCII (256)
    //RESPONSABLE: José Sulecio
    //JUSTIFICACIÓN TÉCNICA: Permite autocompletar nombres de subestaciones
    //y recuperar sus códigos asociados en tiempo O(L), sin colecciones nativas.
    // =========================================================================

    public class NodoTrie
    {
        public NodoTrie[] Hijos;
        public bool EsFinal;
        public string Nombre;
        public string Codigo;

        public NodoTrie()
        {
            Hijos = new NodoTrie[256];
            EsFinal = false;
            Nombre = "";
            Codigo = "";

            for (int i = 0; i < 256; i++)
            {
                Hijos[i] = null;
            }
        }
    }

    public class Trie
    {
        private NodoTrie raiz;
        private int cantidad;

        public int Cantidad
        {
            get { return cantidad; }
        }

        public Trie()
        {
            raiz = new NodoTrie();
            cantidad = 0;
        }

        private string Normalizar(string texto)
        {
            if (texto == null)
            {
                return "";
            }

            return texto.Trim().ToLower();
        }

        public bool Insertar(string nombre, string codigo)
        {
            string clave = Normalizar(nombre);

            if (clave == "")
            {
                return false;
            }

            for (int i = 0; i < clave.Length; i++)
            {
                if (clave[i] >= 256)
                {
                    return false;
                }
            }

            NodoTrie actual = raiz;

            for (int i = 0; i < clave.Length; i++)
            {
                int indice = clave[i];

                if (actual.Hijos[indice] == null)
                {
                    actual.Hijos[indice] = new NodoTrie();
                }

                actual = actual.Hijos[indice];
            }

            if (actual.EsFinal)
            {
                return false;
            }

            actual.EsFinal = true;
            actual.Nombre = nombre.Trim();
            actual.Codigo = codigo;
            cantidad++;

            return true;
        }

        private NodoTrie BuscarNodo(string clave)
        {
            NodoTrie actual = raiz;

            for (int i = 0; i < clave.Length; i++)
            {
                if (clave[i] >= 256)
                {
                    return null;
                }

                actual = actual.Hijos[clave[i]];

                if (actual == null)
                {
                    return null;
                }
            }

            return actual;
        }

        private bool TieneHijos(NodoTrie nodo)
        {
            for (int i = 0; i < 256; i++)
            {
                if (nodo.Hijos[i] != null)
                {
                    return true;
                }
            }

            return false;
        }

        public bool Eliminar(string nombre)
        {
            string clave = Normalizar(nombre);

            if (clave == "")
            {
                return false;
            }

            NodoTrie nodo = BuscarNodo(clave);

            if (nodo == null || !nodo.EsFinal)
            {
                return false;
            }

            nodo.EsFinal = false;
            nodo.Nombre = "";
            nodo.Codigo = "";
            cantidad--;

            LimpiarRama(raiz, clave, 0);

            return true;
        }

        private bool LimpiarRama(NodoTrie nodo, string clave, int nivel)
        {
            if (nivel == clave.Length)
            {
                return !nodo.EsFinal && !TieneHijos(nodo);
            }

            int indice = clave[nivel];
            NodoTrie hijo = nodo.Hijos[indice];

            if (hijo == null)
            {
                return false;
            }

            if (LimpiarRama(hijo, clave, nivel + 1))
            {
                nodo.Hijos[indice] = null;
            }

            return !nodo.EsFinal && !TieneHijos(nodo);
        }

        public int BuscarPorPrefijo(string prefijo)
        {
            string clave = Normalizar(prefijo);

            if (clave == "")
            {
                Console.WriteLine("Escribe al menos una letra para buscar.");
                return 0;
            }

            NodoTrie nodo = BuscarNodo(clave);

            Console.WriteLine("\nResultados para \"" + prefijo.Trim() + "\":");

            if (nodo == null)
            {
                Console.WriteLine("No hay subestaciones con ese prefijo.");
                return 0;
            }

            int encontrados = MostrarDesde(nodo);

            if (encontrados == 0)
            {
                Console.WriteLine("No hay subestaciones con ese prefijo.");
            }

            return encontrados;
        }

        private int MostrarDesde(NodoTrie nodo)
        {
            int total = 0;

            if (nodo.EsFinal)
            {
                Console.WriteLine(nodo.Nombre + " (" + nodo.Codigo + ")");
                total++;
            }

            for (int i = 0; i < 256; i++)
            {
                if (nodo.Hijos[i] != null)
                {
                    total = total + MostrarDesde(nodo.Hijos[i]);
                }
            }

            return total;
        }
    }
}