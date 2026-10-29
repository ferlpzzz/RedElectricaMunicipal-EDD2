using System;

namespace RedElectricaMunicipal
{
    // =========================================================================
    //ESTRUCTURA: Tabla Hash con Encadenamiento Separado (Listas Enlazadas)
    //RESPONSABLE: José Sulecio
    //JUSTIFICACIÓN TÉCNICA: Permite localizar el registro completo de una
    //subestación a partir de su código único en tiempo constante promedio O(1),
    //resolviendo colisiones mediante nodos enlazados propios.
    // =========================================================================

    public class NodoHash
    {
        public string Codigo;
        public Subestacion Registro; // Referencia directa al registro completo
        public NodoHash Siguiente;

        public NodoHash(string codigo, Subestacion registro = null)
        {
            Codigo = codigo;
            Registro = registro;
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

        public bool Insertar(string codigo, Subestacion registro = null)
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

            NodoHash nuevo = new NodoHash(codigo, registro);
            nuevo.Siguiente = tabla[posicion];
            tabla[posicion] = nuevo;
            cantidad++;

            return true;
        }

        public bool Buscar(string codigo)
        {
            return ObtenerSubestacion(codigo) != null || BuscarNodo(codigo) != null;
        }

        private NodoHash BuscarNodo(string codigo)
        {
            codigo = Normalizar(codigo);

            if (codigo == "")
            {
                return null;
            }

            int posicion = Hash(codigo);
            NodoHash actual = tabla[posicion];

            while (actual != null)
            {
                if (actual.Codigo == codigo)
                {
                    return actual;
                }
                actual = actual.Siguiente;
            }

            return null;
        }

        // Localiza y devuelve el registro completo de la subestación en O(1)
        public Subestacion ObtenerSubestacion(string codigo)
        {
            NodoHash nodo = BuscarNodo(codigo);
            if (nodo != null)
            {
                return nodo.Registro;
            }
            return null;
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
            Console.WriteLine("\n=== TABLA HASH DE SUBESTACIONES ===");

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
                        if (actual.Registro != null)
                        {
                            Console.Write("[" + actual.Codigo + ": " + actual.Registro.Nombre + "]");
                        }
                        else
                        {
                            Console.Write(actual.Codigo);
                        }

                        if (actual.Siguiente != null)
                        {
                            Console.Write(" -> ");
                        }

                        actual = actual.Siguiente;
                    }
                    Console.WriteLine();
                }
            }

            Console.WriteLine("Total de subestaciones indexadas: " + cantidad);
        }
    }
}