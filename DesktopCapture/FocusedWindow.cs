﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Xml;
using System.Xml.Linq;

namespace DesktopCapture
{
    public class FocusedWindow : IEquatable<FocusedWindow>
    {
        //private static List<string> acceptablePrograms;

        public string WindowTitle
        {
            get; private set;
        }

        public string ProgramName
        {
            get; set;
        }

        public string FileName
        {
            get; private set;
        }

        public static List<string> acceptablePrograms
        {
            get; private set;
        }

        public static void SetupPrograms()
        {
            /*List<string> popList = new List<string>();
            popList.Add("soffice.bin");
            popList.Add("WINWORD");
            //popList.Add("Skype");
            popList.Add("EXCEL");
            popList.Add("POWERPNT");
            //popList.Add("AcroRd32");
            popList.Add("wmplayer");

            acceptablePrograms = popList;
            WriteToXML();*/
            LoadFromXML();
        }

        private int _windowHandle;
        public int WindowHandle {get { return _windowHandle; }}

        public FocusedWindow(string name, int _handle)
        {
            WindowTitle = name;
            this._windowHandle = _handle;
        }

        public bool Equals(FocusedWindow other)
        {
            if (other.WindowHandle == WindowHandle)
            {
                return true;
            }
            return false;
        }


        public bool IsALearningActivity()
        {
            bool programInList = (from program in acceptablePrograms
                                 where program == ProgramName
                                 select program).Any();

            //bool programInList2 = acceptablePrograms.Where(x => x == ProgramName).Any();
            return programInList;

        }

        public DictionaryEntry GetProgramNameAndFileName()
        {
            for (int i = WindowTitle.Length - 1; i > 0; i--)
            {
                if (WindowTitle[i].Equals('-'))
                {
                    FileName = "with " + WindowTitle.Remove(i - 1);
                    break;
                }
                else
                {
                    FileName = WindowTitle;
                }
            }
            return new DictionaryEntry(FileName, ProgramName);
        }

        public static void AddToProgramList(string programName)
        {
            if (!acceptablePrograms.Contains(programName))
            {
                acceptablePrograms.Add(programName);
            }
            WriteToXML();
        }

        public static void RemoveFromProgramList(string programName)
        {
            if (acceptablePrograms.Contains(programName))
            {
                acceptablePrograms.Remove(programName);
            }
            WriteToXML();
        }

        private static void WriteToXML()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            XmlWriter writer = XmlWriter.Create(@"Programs.xml", settings);

            writer.WriteStartDocument();

            writer.WriteComment("XML Document for storage of Acceptable Learning Programs");
            writer.WriteStartElement("ProgramList");
            foreach (string process in acceptablePrograms)
            {
                writer.WriteStartElement("Program");
                writer.WriteElementString("ProcessName", process);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            writer.Close();

        }

        private static void LoadFromXML()
        {
            List<string> popList = new List<string>();

            XmlDocument doc = new XmlDocument();
            doc.Load("Programs.xml");

            foreach (XmlNode n in doc.DocumentElement.ChildNodes)
            {
                string pass = n.InnerText;
                popList.Add(pass);
            }

            acceptablePrograms = popList;
        }
    }
}
