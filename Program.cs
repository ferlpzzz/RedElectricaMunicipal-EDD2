using System;
using System.Globalization;

namespace RedElectricaMunicipal
{

    public class Program
    {
        const int MaximoSubestaciones = 100;

        static Grafo grafo = new Grafo(MaximoSubestaciones);
        static UnionFind unionFind = new UnionFind(MaximoSubestaciones);
        static TablaHash tablaHash = new TablaHash(5);
        static Trie trie = new Trie();

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            string rutaSubestaciones = "subestaciones.csv";
            string rutaLineas = "lineas.csv";

            GestorArchivos.AsegurarArchivosPorDefecto(rutaSubestaciones, rutaLineas);
            GestorArchivos.CargarSubestaciones(rutaSubestaciones, grafo, unionFind, tablaHash, trie);
            GestorArchivos.CargarLineas(rutaLineas, grafo, unionFind);

            int opcion = -1;

            while (opcion != 0)
            {
                MostrarMenu();
                opcion = LeerEntero("Elige una opcion: ");

                switch (opcion)
                {
                    case 1:
                        grafo.ImprimirGrafo();
                        break;
                    case 2:
                        HacerBFS();
                        break;
                    case 3:
                        HacerDFS();
                        break;
                    case 4:
                        CalcularRuta();
                        break;
                    case 5:
                        unionFind.ImprimirZonas(grafo.Vertices, grafo.TotalSubestaciones);
                        break;
                    case 6:
                        AgregarSubestacion();
                        break;
                    case 7:
                        AgregarLinea();
                        break;
                    case 8:
                        EliminarLinea();
                        break;
                    case 9:
                        BuscarPorCodigo();
                        break;
                    case 10:
                        AutocompletarNombre();
                        break;
                    case 11:
                        tablaHash.Imprimir();
                        break;
                    case 12:
                        EliminarDeTablaHash();
                        break;
                    case 13:
                        EliminarDelTrie();
                        break;
                    case 0:
                        Console.WriteLine("Saliendo del programa.");
                        break;
                    default:
                        Console.WriteLine("Opcion no valida.");
                        break;
                }

                if (opcion != 0)
                {
                    Console.WriteLine("\nPresiona Enter para continuar");
                    Console.ReadLine();
                }
            }
        }

        static void MostrarMenu()
        {
            Console.WriteLine("\nRED ELECTRICA MUNICIPAL");
            Console.WriteLine("1. Ver topologia de la red");
            Console.WriteLine("2. Recorrido en anchura (BFS)");
            Console.WriteLine("3. Recorrido en profundidad (DFS)");
            Console.WriteLine("4. Ruta de menor costo (Dijkstra)");
            Console.WriteLine("5. Ver zonas electricas (Union-Find)");
            Console.WriteLine("6. Agregar subestacion");
            Console.WriteLine("7. Agregar linea de distribucion");
            Console.WriteLine("8. Eliminar linea de distribucion");
            Console.WriteLine("9. Buscar subestacion por codigo (tabla hash)");
            Console.WriteLine("10. Autocompletar nombre de subestacion (trie)");
            Console.WriteLine("11. Imprimir tabla hash");
            Console.WriteLine("12. Eliminar codigo de la tabla hash");
            Console.WriteLine("13. Eliminar nombre del trie");
            Console.WriteLine("0. Salir");
        }

        static string LeerTexto(string mensaje)
        {
            Console.Write(mensaje);
            string texto = Console.ReadLine();

            if (texto == null)
            {
                return "";
            }

            return texto.Trim();
        }

        static int LeerEntero(string mensaje)
        {
            int numero;
            string texto = LeerTexto(mensaje);

            while (!int.TryParse(texto, out numero))
            {
                Console.WriteLine("Escribe un numero entero valido.");
                texto = LeerTexto(mensaje);
            }

            return numero;
        }

        static double LeerDecimal(string mensaje)
        {
            double numero;
            string texto = LeerTexto(mensaje).Replace(',', '.');

            while (!double.TryParse(texto, NumberStyles.Float, CultureInfo.InvariantCulture, out numero)
                   || double.IsNaN(numero) || double.IsInfinity(numero))
            {
                Console.WriteLine("Escribe un numero valido.");
                texto = LeerTexto(mensaje).Replace(',', '.');
            }

            return numero;
        }

        static bool LeerSiNo(string mensaje)
        {
            string respuesta = LeerTexto(mensaje).ToLower();

            while (respuesta != "s" && respuesta != "n")
            {
                Console.WriteLine("Responde s o n.");
                respuesta = LeerTexto(mensaje).ToLower();
            }

            return respuesta == "s";
        }

        static int PedirId(string mensaje)
        {
            string codigo = LeerTexto(mensaje);
            int id = grafo.BuscarIdPorCodigo(codigo);

            if (id == -1)
            {
                Console.WriteLine("No existe una subestacion con el codigo " + codigo + ".");
            }

            return id;
        }

        static void HacerBFS()
        {
            int idInicio = PedirId("Codigo de la subestacion de inicio: ");

            if (idInicio == -1)
            {
                return;
            }

            grafo.RecorridoBFS(idInicio);
        }

        static void HacerDFS()
        {
            int idInicio = PedirId("Codigo de la subestacion de inicio: ");

            if (idInicio == -1)
            {
                return;
            }

            grafo.RecorridoDFS(idInicio);
        }

        static void CalcularRuta()
        {
            int idOrigen = PedirId("Codigo de origen: ");

            if (idOrigen == -1)
            {
                return;
            }

            int idDestino = PedirId("Codigo de destino: ");

            if (idDestino == -1)
            {
                return;
            }

            grafo.DijkstraRutaMenorCosto(idOrigen, idDestino);
        }

        static void AgregarSubestacion()
        {
            string codigo = LeerTexto("Codigo (ejemplo SUB-08): ");
            string nombre = LeerTexto("Nombre: ");

            if (codigo == "" || nombre == "")
            {
                Console.WriteLine("El codigo y el nombre no pueden estar vacios.");
                return;
            }

            double capacidad = LeerDecimal("Capacidad en MW: ");

            if (capacidad <= 0)
            {
                Console.WriteLine("La capacidad debe ser mayor que cero.");
                return;
            }

            int id = grafo.RegistrarSubestacion(codigo, nombre, capacidad);

            if (id == -1)
            {
                Console.WriteLine("No se pudo agregar, el codigo ya existe o el grafo esta lleno.");
                return;
            }

            unionFind.RegistrarElemento(id);
            tablaHash.Insertar(codigo);

            if (!trie.Insertar(nombre, codigo))
            {
                Console.WriteLine("Aviso: el nombre no se agrego al autocompletado (repetido o con caracteres no validos).");
            }

            Console.WriteLine("Subestacion agregada: " + grafo.Vertices[id].ToString());
        }

        static void AgregarLinea()
        {
            int idOrigen = PedirId("Codigo de origen: ");

            if (idOrigen == -1)
            {
                return;
            }

            int idDestino = PedirId("Codigo de destino: ");

            if (idDestino == -1)
            {
                return;
            }

            double longitud = LeerDecimal("Longitud en km: ");
            double costo = LeerDecimal("Costo de mantenimiento: ");

            if (longitud <= 0 || costo < 0)
            {
                Console.WriteLine("La longitud debe ser mayor que cero y el costo no puede ser negativo.");
                return;
            }

            bool bidireccional = LeerSiNo("Es bidireccional? (s/n): ");

            if (grafo.AgregarLinea(idOrigen, idDestino, longitud, costo, bidireccional))
            {
                unionFind.Union(idOrigen, idDestino);
                Console.WriteLine("Linea agregada o actualizada.");
            }
            else
            {
                Console.WriteLine("No se pudo agregar la linea, el origen y el destino son iguales.");
            }
        }

        static void EliminarLinea()
        {
            int idOrigen = PedirId("Codigo de origen: ");

            if (idOrigen == -1)
            {
                return;
            }

            int idDestino = PedirId("Codigo de destino: ");

            if (idDestino == -1)
            {
                return;
            }

            bool bidireccional = LeerSiNo("Quitar tambien la linea de regreso? (s/n): ");

            // el Union-Find no puede separar zonas, asi que no se modifica aqui
            if (grafo.EliminarLinea(idOrigen, idDestino, bidireccional))
            {
                Console.WriteLine("Linea eliminada.");
            }
            else
            {
                Console.WriteLine("No existe una linea del origen al destino.");
            }
        }

        static void BuscarPorCodigo()
        {
            string codigo = LeerTexto("Codigo a buscar: ");

            if (tablaHash.Buscar(codigo))
            {
                Console.WriteLine("Codigo encontrado en la tabla hash.");

                int id = grafo.BuscarIdPorCodigo(codigo);

                if (id != -1)
                {
                    Console.WriteLine(grafo.Vertices[id].ToString());
                }
            }
            else
            {
                Console.WriteLine("El codigo no esta en la tabla hash.");
            }
        }

        static void AutocompletarNombre()
        {
            string prefijo = LeerTexto("Escribe el inicio del nombre: ");
            trie.BuscarPorPrefijo(prefijo);
        }

        static void EliminarDeTablaHash()
        {
            string codigo = LeerTexto("Codigo a quitar de la tabla hash: ");

            if (tablaHash.Eliminar(codigo))
            {
                Console.WriteLine("Codigo eliminado de la tabla hash. La subestacion sigue en el grafo.");
            }
            else
            {
                Console.WriteLine("Ese codigo no estaba en la tabla hash.");
            }
        }

        static void EliminarDelTrie()
        {
            string nombre = LeerTexto("Nombre a quitar del trie: ");

            if (trie.Eliminar(nombre))
            {
                Console.WriteLine("Nombre eliminado del trie. La subestacion sigue en el grafo.");
            }
            else
            {
                Console.WriteLine("Ese nombre no estaba en el trie.");
            }
        }
    }
}