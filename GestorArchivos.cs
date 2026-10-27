using System;
using System.IO;
using System.Globalization;

namespace RedElectricaMunicipal
{
    // =========================================================================
    // MÓDULO: Gestor de Archivos CSV
    // AUTOR / RESPONSABLE: Ferry de Jesús López del Cid
    // DESCRIPCIÓN: Gestiona la ubicación, existencia y carga dinámica de los
    //              archivos subestaciones.csv y lineas.csv hacia las estructuras.
    // =========================================================================
    public static class GestorArchivos
    {
        // Genera los archivos CSV base en la raíz del proyecto si no existen
        public static void AsegurarArchivosPorDefecto(string rutaSubestaciones, string rutaLineas)
        {
            if (!File.Exists(rutaSubestaciones))
            {
                string[] subestacionesIniciales = {
                    "Codigo,Nombre,Capacidad",
                    "SUB-01,Quetzaltenango Centro,500",
                    "SUB-02,Quetzaltenango Norte,350",
                    "SUB-03,La Esperanza,250",
                    "SUB-04,Salcaja,300",
                    "SUB-05,San Mateo,200",
                    "SUB-06,Cantel Industrial,450",
                    "SUB-07,Zunil Hidro,400"
                };
                File.WriteAllLines(rutaSubestaciones, subestacionesIniciales);
            }

            if (!File.Exists(rutaLineas))
            {
                string[] lineasIniciales = {
                    "CodigoOrigen,CodigoDestino,LongitudKm,Costo,Bidireccional",
                    "SUB-01,SUB-02,8.5,1200,true",
                    "SUB-01,SUB-03,12.0,1800,true",
                    "SUB-02,SUB-03,5.0,450,true",
                    "SUB-01,SUB-04,15.0,2100,true",
                    "SUB-04,SUB-06,10.5,950,false",
                    "SUB-03,SUB-05,9.0,800,true",
                    "SUB-06,SUB-07,7.5,700,true"
                };
                File.WriteAllLines(rutaLineas, lineasIniciales);
            }
        }

        // Valida existencia y carga subestaciones desde archivo CSV
        public static int CargarSubestaciones(string rutaArchivo, Grafo grafo, UnionFind unionFind)
        {
            string rutaCompleta = Path.GetFullPath(rutaArchivo);

            if (!File.Exists(rutaCompleta))
            {
                Console.WriteLine("[ERROR DE ARCHIVO] No existe el archivo en la ubicación: " + rutaCompleta);
                return 0;
            }

            int cargadas = 0;
            string[] lineas = File.ReadAllLines(rutaCompleta);

            for (int i = 0; i < lineas.Length; i++)
            {
                string linea = lineas[i].Trim();
                if (linea == "" || linea.StartsWith("Codigo", StringComparison.OrdinalIgnoreCase))
                    continue;

                string[] partes = linea.Split(',');
                if (partes.Length >= 3)
                {
                    string codigo = partes[0].Trim();
                    string nombre = partes[1].Trim();

                    if (double.TryParse(partes[2].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out double capacidad))
                    {
                        int idAsignado = grafo.RegistrarSubestacion(codigo, nombre, capacidad);
                        if (idAsignado != -1)
                        {
                            unionFind.RegistrarElemento(idAsignado);
                            // NOTA PARA TOÑO: Aquí puedes enlazar directamente:
                            // tablaHash.Insertar(grafo.Vertices[idAsignado]);
                            // trie.Insertar(nombre, codigo);
                            cargadas++;
                        }
                    }
                }
            }

            Console.WriteLine("[OK] Subestaciones leídas desde: " + rutaCompleta + " (" + cargadas + " registros)");
            return cargadas;
        }

        // Valida existencia y carga líneas de distribución desde archivo CSV
        public static int CargarLineas(string rutaArchivo, Grafo grafo, UnionFind unionFind)
        {
            string rutaCompleta = Path.GetFullPath(rutaArchivo);

            if (!File.Exists(rutaCompleta))
            {
                Console.WriteLine("[ERROR DE ARCHIVO] No existe el archivo en la ubicación: " + rutaCompleta);
                return 0;
            }

            int cargadas = 0;
            string[] lineas = File.ReadAllLines(rutaCompleta);

            for (int i = 0; i < lineas.Length; i++)
            {
                string linea = lineas[i].Trim();
                if (linea == "" || linea.StartsWith("CodigoOrigen", StringComparison.OrdinalIgnoreCase))
                    continue;

                string[] partes = linea.Split(',');
                if (partes.Length >= 5)
                {
                    string codOrigen = partes[0].Trim();
                    string codDestino = partes[1].Trim();

                    int idOrigen = grafo.BuscarIdPorCodigo(codOrigen);
                    int idDestino = grafo.BuscarIdPorCodigo(codDestino);

                    if (idOrigen != -1 && idDestino != -1 &&
                        double.TryParse(partes[2].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out double longitud) &&
                        double.TryParse(partes[3].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out double costo) &&
                        bool.TryParse(partes[4].Trim(), out bool bidireccional))
                    {
                        if (grafo.AgregarLinea(idOrigen, idDestino, longitud, costo, bidireccional))
                        {
                            unionFind.Union(idOrigen, idDestino);
                            cargadas++;
                        }
                    }
                }
            }

            Console.WriteLine("[OK] Líneas leídas desde: " + rutaCompleta + " (" + cargadas + " registros)");
            return cargadas;
        }
    }
}