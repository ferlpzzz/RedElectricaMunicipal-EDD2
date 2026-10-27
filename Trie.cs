using System;

namespace RedElectricaMunicipal
{

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
    }
}