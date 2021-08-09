using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Drawing;
using System.Configuration;
using System.Windows;
using System.Runtime.InteropServices;

namespace EditorCore
{
    public static class Constants
    {
        public const string LinkedListName = "____EditorInternalList___";
    }

    public static class CursorHelper
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public static implicit operator Point(POINT point)
            {
                return new Point(point.X, point.Y);
            }
        }

        /// <summary>
        /// Retrieves the cursor's position, in screen coordinates.
        /// </summary>
        /// <see>See MSDN documentation for further information.</see>
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);
        public static Point GetCursorPosition()
        {
            POINT lpPoint;
            GetCursorPos(out lpPoint);
            //bool success = User32.GetCursorPos(out lpPoint);
            // if (!success)

            return lpPoint;
        }
    }


    public static class DeepCloneDictArr
    {
        public static Dictionary<string, dynamic> DeepClone(Dictionary<string, dynamic> d)
        {
            var res = new Dictionary<string, dynamic>();
            foreach (string k in d.Keys)
            {
                if (d[k] is Dictionary<string, dynamic> || d[k] is List<dynamic>)
                    res.Add(k, DeepClone(d[k]));
                else if (d[k] is ICloneable)
                    res.Add(k, d[k].Clone());
                else res.Add(k, d[k]);
            }
            return res;
        }

        public static List<dynamic> DeepClone(List<dynamic> l)
        {
            var res = new List<dynamic>();
            foreach (var o in l)
            {
                if (o is Dictionary<string, dynamic> || o is List<dynamic>)
                    res.Add(DeepClone(o));
                else if (o is ICloneable)
                    res.Add(o.Clone());
                else res.Add(o);
            }
            return res;
        }
    }

    //public class CustomStringWriter : System.IO.StringWriter
    //   {
    //       private readonly Encoding encoding;

    //       public CustomStringWriter(Encoding encoding)
    //       {
    //           this.encoding = encoding;
    //       }

    //       public override Encoding Encoding
    //       {
    //           get { return encoding; }
    //       }
    //   }

    public class CustomStack<T> : IEnumerable<T>
    {
        private List<T> items = new List<T>();
        public int MaxItems = 50;

        public int Count
        { get { return items.Count(); } }

        public void Remove(int index)
        {
            items.RemoveAt(index);
        }

        public void Push(T item)
        {
            items.Add(item);
            if (items.Count > MaxItems)
            {
                for (int i = MaxItems; i < items.Count; i++) Remove(0);
            }
        }

        public T Pop()
        {
            if (items.Count > 0)
            {
                T tmp = items[items.Count - 1];
                items.RemoveAt(items.Count - 1);
                return tmp;
            }
            else return default(T);
        }

        public void RemoveAt(int index) => items.RemoveAt(index);

        public T Peek() { return items[items.Count - 1]; }

        public T[] ToArray()
        {
            return items.ToArray();
        }

        public void Clear()
        {
            items.Clear();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator();
        }
    }

    public class UndoAction
    {
        public string actionName;
        public Action<dynamic> _act;
        public dynamic _arg;

        public void Undo()
        {
            _act.Invoke(_arg);
        }

        public override string ToString()
        {
            return actionName;
        }

        public UndoAction(string name, Action<dynamic> Act, dynamic arg)
        {
            actionName = name;
            _act = Act;
            _arg = arg;
        }

    }


    namespace ExtensionMethods
    {
        public static class Extensions
        {


            public static bool Matches(this byte[] arr, string magic) =>
                arr.Matches(0, magic.ToCharArray());
            public static bool Matches(this byte[] arr, uint startIndex, string magic) =>
                arr.Matches(startIndex, magic.ToCharArray());

            public static bool Matches(this byte[] arr, uint startIndex, params char[] magic)
            {
                if (arr.Length < magic.Length + startIndex) return false;
                for (uint i = 0; i < magic.Length; i++)
                {
                    if (arr[i + startIndex] != magic[i]) return false;
                }
                return true;
            }

            public static int AddIfNotContins(this IList l, dynamic obj)
            {
                int index = l.IndexOf(obj);
                if (index == -1)
                {
                    l.Add(obj);
                    return l.Count - 1;
                }
                else
                    return index;
            }
        }
    }
}
