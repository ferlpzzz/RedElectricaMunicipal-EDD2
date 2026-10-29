using System;
using System.Globalization;

namespace RedElectricaMunicipal
{
    // =========================================================================
    //ARCHIVO: Program.cs
    //AUTORES: José Antonio (Menú e Interfaz) y Ferry López (Flujo Integrado)
    //DESCRIPCIÓN: Menú interactivo de consola con operaciones individuales y
    //              el flujo coordinado de las 4 estructuras de datos.
    // =========================================================================
    public class Program
    {
        const int MaximoSubestaciones = 100;

        static Grafo grafo = new Grafo(MaximoSubestaciones);
        static UnionFind unionFind = new UnionFind(MaximoSubestaciones);
        static TablaHash tablaHash = new TablaHash(11);
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
                    case 14:
                        EjecutarFlujoIntegrado();
                        break;
                    case 15:
                        CargarArchivosPersonalizados();
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
            Console.WriteLine("\n================ RED ELECTRICA MUNICIPAL ================");
            Console.WriteLine("1. Ver topologia de la red");
            Console.WriteLine("2. Recorrido en anchura");
            Console.WriteLine("3. Recorrido en profundidad");
            Console.WriteLine("4. Ruta de menor costo por codigo directo");
            Console.WriteLine("5. Ver zonas electricas conectadas");
            Console.WriteLine("6. Agregar subestacion manualmente");
            Console.WriteLine("7. Agregar linea de distribucion manualmente");
            Console.WriteLine("8. Eliminar linea de distribucion");
            Console.WriteLine("9. Buscar subestacion por codigo");
            Console.WriteLine("10. Autocompletar nombre de subestacion");
            Console.WriteLine("11. Imprimir estructura de la Tabla Hash");
            Console.WriteLine("12. Eliminar codigo de la Tabla Hash");
            Console.WriteLine("13. Eliminar nombre del Trie");
            Console.WriteLine("14. Asistente de restablecimiento");
            Console.WriteLine("15. Cargar nuevos archivos .csv desde ruta especifica");
            Console.WriteLine("0. Salir");
            Console.WriteLine("=========================================================");
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

        //Usa la Tabla Hash en O(1) como método principal para obtener el IdInterno
        static int PedirId(string mensaje)
        {
            string codigo = LeerTexto(mensaje);
            Subestacion sub = tablaHash.ObtenerSubestacion(codigo);

            if (sub != null)
            {
                return sub.IdInterno;
            }

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

            if (!unionFind.EstanEnMismaZona(idOrigen, idDestino))
            {
                Console.WriteLine("Las subestaciones pertenecen a zonas electricas desconectadas. No hay ruta posible.");
                return;
            }

            grafo.DijkstraRutaMenorCosto(idOrigen, idDestino);
        }

        static void EjecutarFlujoIntegrado()
        {
            Console.WriteLine("\n=== FLUJO INTEGRADO DE RESTABLECIMIENTO ELECTRICO ===");

            // PASO 1 y 2 PARA ORIGEN (Trie + Tabla Hash)
            Console.WriteLine("\n--- 1. SELECCION DE SUBESTACION DE ORIGEN ---");
            string prefijoOrigen = LeerTexto("Escribe el inicio del nombre de la subestacion de ORIGEN (ej. 'que', 'sal'): ");
            int sugerenciasOrigen = trie.BuscarPorPrefijo(prefijoOrigen);

            if (sugerenciasOrigen == 0)
            {
                return;
            }

            string codOrigen = LeerTexto("Ingresa el codigo exacto de ORIGEN de la lista anterior: ");
            Subestacion subOrigen = tablaHash.ObtenerSubestacion(codOrigen);

            if (subOrigen == null)
            {
                Console.WriteLine("El codigo ingresado no se encontro en la Tabla Hash.");
                return;
            }
            Console.WriteLine("Registro localizado: " + subOrigen.ToString());

            // PASO 1 y 2 PARA DESTINO (Trie + Tabla Hash)
            Console.WriteLine("\n--- 2. SELECCION DE SUBESTACION DE DESTINO ---");
            string prefijoDestino = LeerTexto("Escribe el inicio del nombre de la subestacion de DESTINO (ej. 'la', 'san', 'can'): ");
            int sugerenciasDestino = trie.BuscarPorPrefijo(prefijoDestino);

            if (sugerenciasDestino == 0)
            {
                return;
            }

            string codDestino = LeerTexto("Ingresa el codigo exacto de DESTINO de la lista anterior: ");
            Subestacion subDestino = tablaHash.ObtenerSubestacion(codDestino);

            if (subDestino == null)
            {
                Console.WriteLine("El codigo ingresado no se encontro en la Tabla Hash.");
                return;
            }
            Console.WriteLine("Registro localizado: " + subDestino.ToString());

            // PASO 3: VALIDACIÓN DE ZONA ELÉCTRICA CON UNION-FIND
            Console.WriteLine("\n--- 3. VERIFICACION DE CONECTIVIDAD (UNION-FIND) ---");
            bool mismaZona = unionFind.EstanEnMismaZona(subOrigen.IdInterno, subDestino.IdInterno);

            if (!mismaZona)
            {
                Console.WriteLine("ALERTA: [" + subOrigen.Codigo + "] y [" + subDestino.Codigo + "] estan en zonas electricas aisladas.");
                Console.WriteLine("No es posible restablecer el suministro sin antes construir una linea que una ambas zonas.");
                return;
            }

            int idRaiz = unionFind.Find(subOrigen.IdInterno);
            Console.WriteLine("[Union-Find O(α(n))] Conectividad confirmada. Ambas pertenecen a la zona principal liderada por [" + grafo.Vertices[idRaiz].Codigo + "].");

            // PASO 4: CÁLCULO DE RUTA DE MENOR COSTO CON DIJKSTRA EN EL GRAFO
            Console.WriteLine("\n--- 4. CALCULO DE RUTA DE MENOR COSTO ---");
            grafo.DijkstraRutaMenorCosto(subOrigen.IdInterno, subDestino.IdInterno);
        }

        static void CargarArchivosPersonalizados()
        {
            Console.WriteLine("\n=== CARGA DINAMICA DE ARCHIVOS CSV ===");
            string rutaSub = LeerTexto("Ruta del archivo de subestaciones (Enter para 'subestaciones.csv'): ");
            if (rutaSub == "") rutaSub = "subestaciones.csv";

            string rutaLin = LeerTexto("Ruta del archivo de lineas (Enter para 'lineas.csv'): ");
            if (rutaLin == "") rutaLin = "lineas.csv";

            GestorArchivos.CargarSubestaciones(rutaSub, grafo, unionFind, tablaHash, trie);
            GestorArchivos.CargarLineas(rutaLin, grafo, unionFind);
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
            tablaHash.Insertar(codigo, grafo.Vertices[id]);

            if (!trie.Insertar(nombre, codigo))
            {
                Console.WriteLine("Aviso: el nombre no se agrego al autocompletado (repetido o con caracteres no validos).");
            }

            Console.WriteLine("Subestacion agregada en las 4 estructuras: " + grafo.Vertices[id].ToString());
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

            if (unionFind.EstanEnMismaZona(idOrigen, idDestino))
            {
                Console.WriteLine("Aviso: Ambas subestaciones ya pertenecen a la misma zona electrica conectada (se agregara como linea redundante).");
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
            Subestacion sub = tablaHash.ObtenerSubestacion(codigo);

            if (sub != null)
            {
                Console.WriteLine("Registro localizado directamente en la Tabla Hash O(1):");
                Console.WriteLine(sub.ToString());
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