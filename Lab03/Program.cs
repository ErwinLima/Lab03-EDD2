using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;

namespace Lab03;

public class Program
{
    public static void Main()
    {
        try
        {
            AVLTree<Persona> arbolPersonas = new AVLTree<Persona>(); //Árbol AVL para almacenar las personas
            string route = @"C:\Users\AndresLima\Desktop\input.csv"; //Ruta del archivo a leer

            if (!File.Exists(route))
            {
                Console.WriteLine("No existe el archivo");
                return;
            }
            string[] FileData = File.ReadAllLines(route);
            foreach (var item in FileData)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    string[] fila = item.Split(";");
                    Persona? persona = JsonSerializer.Deserialize<Persona>(fila[1]);
                    string ruta = @"C:\Users\AndresLima\Desktop\inputs\";
                    string patron = "REC-" + persona.dpi + "*";
                    persona.cartas = Directory.GetFiles(ruta, patron);
                    if (fila[0] == "INSERT")
                    {
                        arbolPersonas.Add(persona!, Delegates.DPIComparison);
                    }
                    else if (fila[0] == "DELETE")
                    {
                        arbolPersonas.Delete(persona!, Delegates.DPIComparison);
                    }
                    else if (fila[0] == "PATCH")
                    {
                        arbolPersonas.Patch(persona!, Delegates.DPIComparison);
                    }
                }
            }



            //Operaciones
            Console.WriteLine("Ingrese el dpi de la persona que quiere buscar: ");
            string? dpi = Console.ReadLine();
            Persona persona1 = new Persona();
            persona1.dpi = dpi!;
            Nodo<Persona> temporal = arbolPersonas.Search(persona1, Delegates.DPIComparison);
            if (temporal == null)
            {
                Console.WriteLine("No se encontraron resultados que coincidan con el DPI");
                return;
            }
            Persona p1 = temporal.Value;
            string path = @"C:\Users\AndresLima\Desktop\compressed";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            int i = 1;
            foreach (var item in p1.cartas)
            {
                try
                {
                    string content = File.ReadAllText(item);
                    List<int>? code = encode(content);
                    string text = "";
                    foreach (var num in code)
                    {
                        if (num == code.Last())
                        {
                            text += num.ToString();
                        }
                        else
                        {
                            text += num.ToString() + ",";
                        }
                    }
                    string fileName = "compressed-REC-" + p1.dpi + "-" + i.ToString() + ".txt";
                    fileName = path + "\\" + fileName;
                    File.WriteAllText(fileName, text);
                }
                catch (Exception e)
                {

                    throw new Exception("Sucedio un error inesperado");
                }
                i++;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Ha ocurrido un error inesperado");
            Console.WriteLine(e.Message);            
        }
    }

    public static List<int> encode (string text)
    {
        Dictionary<string, int> dictionary = new Dictionary<string, int>();
        for (int i = 0; i < 256; i++)
        {
            dictionary.Add(((char)i).ToString(), i);
        }
        string w = "";
        List<int> resul = new List<int>();

        foreach (var c in text)
        {
            string temp = w + c;
            if (dictionary.ContainsKey(temp))
            {
                w = temp;
            }
            else
            {
                resul.Add(dictionary[w]);
                dictionary.Add(temp, dictionary.Count);
                w = c.ToString();
            }
        }

        if (!string.IsNullOrEmpty(text))
        {
            resul.Add(dictionary[w]);
        }

        return resul;
    }

    public static string decompress (List<int> lista)
    {
        Dictionary<int, string> dictionary = new Dictionary<int, string>();
        for (int i = 0; i < 256; i++)
        {
            dictionary.Add(i, ((char)i).ToString());
        }
        string w = dictionary[lista[0]];
        lista.RemoveAt(0);
        StringBuilder resul = new StringBuilder(w);

        foreach (var item in lista)
        {
            string entry = "";
            if (dictionary.ContainsKey(item))
            {
                entry = dictionary[item];
            }else if(item == dictionary.Count)
            {
                entry = w + w[0];
            }

            resul.Append(entry);

            dictionary.Add(dictionary.Count, w + entry[0]);

            w = entry;
        }

        return resul.ToString();
    }
}