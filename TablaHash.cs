using System;

namespace RedElectricaMunicipal
{

    public class NodoHash
    {
        public string Codigo;
        public NodoHash Siguiente;

        public NodoHash(string codigo)
        {
            Codigo = codigo;
            Siguiente = null;
        }
    }

    public class TablaHash
    {
        private NodoHash[] tabla;
        private int tamano;

        public TablaHash(int tamano)
        {
            this.tamano = tamano;
            tabla = new NodoHash[tamano];

            for (int i = 0; i < tamano; i++)
            {
                tabla[i] = null;
            }
        }

        private int Hash(string codigo)
        {
            int valor = 0;

            for (int i = 0; i < codigo.Length; i++)
            {
                valor = (valor * 31 + codigo[i]) % tamano;
            }

            return valor;
        }
    }
}