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
        private int cantidad;

        public int Cantidad
        {
            get { return cantidad; }
        }

        public TablaHash(int tamano)
        {
            this.tamano = tamano;
            tabla = new NodoHash[tamano];
            cantidad = 0;

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

        private string Normalizar(string codigo)
        {
            if (codigo == null)
            {
                return "";
            }

            return codigo.Trim().ToUpper();
        }

        public bool Insertar(string codigo)
        {
            codigo = Normalizar(codigo);

            if (codigo == "")
            {
                return false;
            }

            if (Buscar(codigo))
            {
                return false;
            }

            int posicion = Hash(codigo);

            NodoHash nuevo = new NodoHash(codigo);
            nuevo.Siguiente = tabla[posicion];
            tabla[posicion] = nuevo;
            cantidad++;

            return true;
        }

        public bool Buscar(string codigo)
        {
            codigo = Normalizar(codigo);

            if (codigo == "")
            {
                return false;
            }

            int posicion = Hash(codigo);
            NodoHash actual = tabla[posicion];

            while (actual != null)
            {
                if (actual.Codigo == codigo)
                {
                    return true;
                }
                actual = actual.Siguiente;
            }

            return false;
        }

        public bool Eliminar(string codigo)
        {
            codigo = Normalizar(codigo);

            if (codigo == "")
            {
                return false;
            }

            int posicion = Hash(codigo);
            NodoHash actual = tabla[posicion];
            NodoHash anterior = null;

            while (actual != null)
            {
                if (actual.Codigo == codigo)
                {
                    if (anterior == null)
                    {
                        tabla[posicion] = actual.Siguiente;
                    }
                    else
                    {
                        anterior.Siguiente = actual.Siguiente;
                    }

                    cantidad--;
                    return true;
                }

                anterior = actual;
                actual = actual.Siguiente;
            }

            return false;
        }

        public void Imprimir()
        {
            Console.WriteLine("\nTABLA HASH DE SUBESTACIONES");

            if (cantidad == 0)
            {
                Console.WriteLine("La tabla esta vacia.");
                return;
            }

            for (int i = 0; i < tamano; i++)
            {
                Console.Write("Posicion " + i + ": ");

                NodoHash actual = tabla[i];

                if (actual == null)
                {
                    Console.WriteLine("vacia");
                }
                else
                {
                    while (actual != null)
                    {
                        Console.Write(actual.Codigo);

                        if (actual.Siguiente != null)
                        {
                            Console.Write(", ");
                        }

                        actual = actual.Siguiente;
                    }
                    Console.WriteLine();
                }
            }

            Console.WriteLine("Total de codigos: " + cantidad);
        }
    }
}