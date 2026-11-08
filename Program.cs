using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace GestionDeFicheros2
{
    public class Program
    {
        // Rutas por defecto (relativas, como recomienda el temario)
        static string rutaCsv = "./NetflixOriginals.csv";
        static string rutaXml = "./NetflixOriginals.xml";

        public static void Main(string[] args)
        {
            int opcion;
            do
            {
                Console.WriteLine("\n---- MENÚ ----");
                Console.WriteLine("1_Exportar todo el documento (CSV → XML).");
                Console.WriteLine("2_Exportar por género.");
                Console.WriteLine("3_Exportar por año (y superiores).");
                Console.WriteLine("4_Exportar por lenguaje.");
                Console.WriteLine("5_Exportar XML → CSV (todo).");
                Console.WriteLine("6_Exportar XML → CSV por género.");
                Console.WriteLine("7_Exportar XML → CSV por año.");
                Console.WriteLine("8_Exportar XML → CSV por lenguaje.");
                Console.WriteLine("9_Cambiar rutas de ficheros.");
                Console.WriteLine("0_Salir.");
                Console.Write("Opción: ");
                opcion = int.Parse(Console.ReadLine());

                switch (opcion)
                {
                    case 1: ExportarTodo(); break;
                    case 2: ExportarPorGenero(); break;
                    case 3: ExportarPorAnio(); break;
                    case 4: ExportarPorLenguaje(); break;
                    case 5: XmlACsvTodo(); break;
                    case 6: XmlACsvPorGenero(); break;
                    case 7: XmlACsvPorAnio(); break;
                    case 8: XmlACsvPorLenguaje(); break;
                    case 9: CambiarRutas(); break;
                    case 0: Console.WriteLine("Cerrando aplicación..."); break;
                    default: Console.WriteLine("Opción no válida."); break;
                }
            } while (opcion != 0);
        }

        // ============================================================
        //  PARTE 1: CSV → XML
        // ============================================================

        public static void ExportarTodo()
        {
            string[] lineas = File.ReadAllLines(rutaCsv);
            EscribirXml(rutaXml, lineas, 1, lineas.Length - 1);
            Console.WriteLine($"Exportado todo a {rutaXml}");
        }

        public static void ExportarPorGenero()
        {
            string[] lineas = File.ReadAllLines(rutaCsv);

            // Primera pasada: sacar géneros únicos (ignorando cabecera)
            HashSet<string> generos = new HashSet<string>();
            for (int i = 1; i < lineas.Length; i++)
            {
                string[] campos = lineas[i].Split(',');
                if (campos.Length > 1)
                    generos.Add(campos[1].Trim());
            }

            Console.WriteLine("Géneros disponibles:");
            foreach (var g in generos)
                Console.WriteLine($" - {g}");

            Console.Write("Introduce el género a exportar: ");
            string generoElegido = Console.ReadLine().Trim();

            // Segunda pasada: filtrar
            List<string> filtradas = new List<string>();
            for (int i = 1; i < lineas.Length; i++)
            {
                string[] campos = lineas[i].Split(',');
                if (campos.Length > 1 && campos[1].Trim().Equals(generoElegido, StringComparison.OrdinalIgnoreCase))
                    filtradas.Add(lineas[i]);
            }

            if (filtradas.Count == 0)
            {
                Console.WriteLine("No se encontraron películas con ese género.");
                return;
            }

            string salida = $"./NetflixOriginals_{generoElegido.Replace(" ", "_")}.xml";
            string[] nuevasLineas = new string[filtradas.Count + 1];
            nuevasLineas[0] = lineas[0]; // cabecera
            filtradas.CopyTo(nuevasLineas, 1);

            EscribirXml(salida, nuevasLineas, 1, nuevasLineas.Length - 1);
            Console.WriteLine($"Exportadas {filtradas.Count} películas a {salida}");
        }

        public static void ExportarPorAnio()
        {
            string[] lineas = File.ReadAllLines(rutaCsv);

            Console.Write("Introduce el año (se exportarán ese año y superiores): ");
            int anio = int.Parse(Console.ReadLine());

            List<string> filtradas = new List<string>();
            for (int i = 1; i < lineas.Length; i++)
            {
                string[] campos = lineas[i].Split(',');
                if (campos.Length > 2)
                {
                    // Premiere suele venir como "2020-01-01" o "January 1, 2020"
                    // Ajusta este parseo según el formato real del CSV
                    string fecha = campos[2].Trim();
                    int anioPeli;
                    if (int.TryParse(fecha.Substring(0, 4), out anioPeli) && anioPeli >= anio)
                        filtradas.Add(lineas[i]);
                }
            }

            if (filtradas.Count == 0)
            {
                Console.WriteLine("No se encontraron películas de ese año o posteriores.");
                return;
            }

            string salida = $"./NetflixOriginals_desde_{anio}.xml";
            string[] nuevasLineas = new string[filtradas.Count + 1];
            nuevasLineas[0] = lineas[0];
            filtradas.CopyTo(nuevasLineas, 1);

            EscribirXml(salida, nuevasLineas, 1, nuevasLineas.Length - 1);
            Console.WriteLine($"Exportadas {filtradas.Count} películas a {salida}");
        }

        public static void ExportarPorLenguaje()
        {
            string[] lineas = File.ReadAllLines(rutaCsv);

            HashSet<string> idiomas = new HashSet<string>();
            for (int i = 1; i < lineas.Length; i++)
            {
                string[] campos = lineas[i].Split(',');
                if (campos.Length > 5)
                    idiomas.Add(campos[5].Trim());
            }

            Console.WriteLine("Idiomas disponibles:");
            foreach (var idi in idiomas)
                Console.WriteLine($" - {idi}");

            Console.Write("Introduce el idioma a exportar: ");
            string idiomaElegido = Console.ReadLine().Trim();

            List<string> filtradas = new List<string>();
            for (int i = 1; i < lineas.Length; i++)
            {
                string[] campos = lineas[i].Split(',');
                if (campos.Length > 5 && campos[5].Trim().Equals(idiomaElegido, StringComparison.OrdinalIgnoreCase))
                    filtradas.Add(lineas[i]);
            }

            if (filtradas.Count == 0)
            {
                Console.WriteLine("No se encontraron películas en ese idioma.");
                return;
            }

            string salida = $"./NetflixOriginals_{idiomaElegido.Replace(" ", "_")}.xml";
            string[] nuevasLineas = new string[filtradas.Count + 1];
            nuevasLineas[0] = lineas[0];
            filtradas.CopyTo(nuevasLineas, 1);

            EscribirXml(salida, nuevasLineas, 1, nuevasLineas.Length - 1);
            Console.WriteLine($"Exportadas {filtradas.Count} películas a {salida}");
        }

        /// <summary>
        /// Escribe un XML a partir de las líneas del CSV (desde 'desde' hasta 'hasta').
        /// </summary>
        private static void EscribirXml(string rutaSalida, string[] lineas, int desde, int hasta)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = new UTF8Encoding(false);

            using (XmlWriter xmlWriter = XmlWriter.Create(rutaSalida, settings))
            {
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("Peliculas");

                for (int i = desde; i <= hasta; i++)
                {
                    string[] campos = lineas[i].Split(',');
                    if (campos.Length < 6) continue; // línea mal formada

                    xmlWriter.WriteStartElement("Pelicula");

                    xmlWriter.WriteElementString("Title",       campos[0].Trim());
                    xmlWriter.WriteElementString("Genre",       campos[1].Trim());
                    xmlWriter.WriteElementString("Premiere",    campos[2].Trim());
                    xmlWriter.WriteElementString("Runtime",     campos[3].Trim());
                    xmlWriter.WriteElementString("IMDB_Score",  campos[4].Trim());
                    xmlWriter.WriteElementString("Language",    campos[5].Trim());

                    xmlWriter.WriteEndElement(); // Pelicula
                }

                xmlWriter.WriteEndElement(); // Peliculas
                xmlWriter.WriteEndDocument();
            }
        }

        // ============================================================
        //  PARTE 2: XML → CSV
        // ============================================================

        public static void XmlACsvTodo()
        {
            string[] lineas = LeerXmlComoLineasCsv(rutaXml, null, null, null);
            File.WriteAllLines("./NetflixOriginals_exportado.csv", lineas);
            Console.WriteLine("Exportado XML → CSV completo.");
        }

        public static void XmlACsvPorGenero()
        {
            string[] lineas = LeerXmlComoLineasCsv(rutaXml, "Genre", null, null);
            string genero = PreguntarFiltro(lineas, "Genre", 1);
            if (genero == null) return;

            string[] filtradas = FiltrarPorColumna(lineas, 1, genero);
            string salida = $"./NetflixOriginals_XML2CSV_{genero.Replace(" ", "_")}.csv";
            File.WriteAllLines(salida, filtradas);
            Console.WriteLine($"Exportadas {filtradas.Length - 1} películas a {salida}");
        }

        public static void XmlACsvPorAnio()
        {
            string[] lineas = LeerXmlComoLineasCsv(rutaXml, "Premiere", null, null);
            Console.Write("Introduce el año (se exportarán ese año y superiores): ");
            int anio = int.Parse(Console.ReadLine());

            List<string> filtradas = new List<string> { lineas[0] };
            for (int i = 1; i < lineas.Length; i++)
            {
                string[] campos = lineas[i].Split(',');
                if (campos.Length > 2)
                {
                    int anioPeli;
                    if (int.TryParse(campos[2].Substring(0, 4), out anioPeli) && anioPeli >= anio)
                        filtradas.Add(lineas[i]);
                }
            }

            string salida = $"./NetflixOriginals_XML2CSV_desde_{anio}.csv";
            File.WriteAllLines(salida, filtradas);
            Console.WriteLine($"Exportadas {filtradas.Count - 1} películas a {salida}");
        }

        public static void XmlACsvPorLenguaje()
        {
            string[] lineas = LeerXmlComoLineasCsv(rutaXml, "Language", null, null);
            string idioma = PreguntarFiltro(lineas, "Language", 5);
            if (idioma == null) return;

            string[] filtradas = FiltrarPorColumna(lineas, 5, idioma);
            string salida = $"./NetflixOriginals_XML2CSV_{idioma.Replace(" ", "_")}.csv";
            File.WriteAllLines(salida, filtradas);
            Console.WriteLine($"Exportadas {filtradas.Length - 1} películas a {salida}");
        }

        /// <summary>
        /// Lee el XML y lo convierte en un array de líneas CSV (con cabecera).
        /// </summary>
        private static string[] LeerXmlComoLineasCsv(string rutaXml, string colFiltro, string valFiltro, object _)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(rutaXml);

            List<string> lineas = new List<string>();
            lineas.Add("Title,Genre,Premiere,Runtime,IMDB_Score,Language"); // cabecera

            XmlNodeList peliculas = doc.SelectNodes("//Peliculas/Pelicula");
            foreach (XmlNode p in peliculas)
            {
                string title    = p.SelectSingleNode("Title")?.InnerText    ?? "";
                string genre    = p.SelectSingleNode("Genre")?.InnerText    ?? "";
                string premiere = p.SelectSingleNode("Premiere")?.InnerText ?? "";
                string runtime  = p.SelectSingleNode("Runtime")?.InnerText  ?? "";
                string score    = p.SelectSingleNode("IMDB_Score")?.InnerText ?? "";
                string language = p.SelectSingleNode("Language")?.InnerText ?? "";

                lineas.Add($"{title},{genre},{premiere},{runtime},{score},{language}");
            }

            return lineas.ToArray();
        }

        private static string PreguntarFiltro(string[] lineas, string nombreColumna, int indiceColumna)
        {
            HashSet<string> valores = new HashSet<string>();
            for (int i = 1; i < lineas.Length; i++)
            {
                string[] campos = lineas[i].Split(',');
                if (campos.Length > indiceColumna)
                    valores.Add(campos[indiceColumna].Trim());
            }

            Console.WriteLine($"{nombreColumna} disponibles:");
            foreach (var v in valores)
                Console.WriteLine($" - {v}");

            Console.Write($"Introduce {nombreColumna}: ");
            string elegido = Console.ReadLine().Trim();

            if (!valores.Contains(elegido))
            {
                Console.WriteLine("Valor no encontrado.");
                return null;
            }
            return elegido;
        }

        private static string[] FiltrarPorColumna(string[] lineas, int indice, string valor)
        {
            List<string> resultado = new List<string> { lineas[0] };
            for (int i = 1; i < lineas.Length; i++)
            {
                string[] campos = lineas[i].Split(',');
                if (campos.Length > indice && campos[indice].Trim().Equals(valor, StringComparison.OrdinalIgnoreCase))
                    resultado.Add(lineas[i]);
            }
            return resultado.ToArray();
        }

        // ============================================================
        //  CAMBIO DE RUTAS
        // ============================================================

        public static void CambiarRutas()
        {
            Console.Write($"Ruta CSV actual: {rutaCsv}\nNueva ruta CSV (Enter para mantener): ");
            string nuevaCsv = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(nuevaCsv)) rutaCsv = nuevaCsv;

            Console.Write($"Ruta XML actual: {rutaXml}\nNueva ruta XML (Enter para mantener): ");
            string nuevaXml = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(nuevaXml)) rutaXml = nuevaXml;

            Console.WriteLine("Rutas actualizadas.");
        }
    }
}