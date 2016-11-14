using Converter_Braille.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Converter_Braille.Translater
{
    public class AppSettings
    {
        private string filePath = "settings.config";
        private XmlDocument configFile = new XmlDocument();


        public bool Load()
        {
            if (File.Exists(filePath))
            {
                configFile.LoadXml(File.ReadAllText(filePath));
                foreach (XmlNode childnode in configFile.DocumentElement)
                {
                    if (childnode.Name == "CharInRow")
                        Settings.GetInstance().letterCount = Convert.ToInt32(childnode.Attributes.GetNamedItem("value").Value);

                    if (childnode.Name == "LineInPage")
                        Settings.GetInstance().lineCount = Convert.ToInt32(childnode.Attributes.GetNamedItem("value").Value);

                    if (childnode.Name == "PreView")
                        Settings.GetInstance().preView = Convert.ToBoolean(childnode.Attributes.GetNamedItem("value").Value);
                }
                return true;
            }
            else
            {
                Settings.GetInstance().letterCount = 30;
                Settings.GetInstance().lineCount = 25;
                Settings.GetInstance().preView = true;
                Create();
                return false;
            }

        }

        public void Save()
        {
            foreach (XmlNode childnode in configFile.DocumentElement)
            {
                if (childnode.Name == "CharInRow")
                     childnode.Attributes.GetNamedItem("value").Value = Settings.GetInstance().letterCount.ToString();

                if (childnode.Name == "LineInPage")
                    childnode.Attributes.GetNamedItem("value").Value = Settings.GetInstance().lineCount.ToString();

                if (childnode.Name == "PreView")
                    childnode.Attributes.GetNamedItem("value").Value = Settings.GetInstance().preView.ToString();
            }
            configFile.Save(filePath);
        }

        public void Create()
        {
            XmlElement root, node;
            XmlAttribute attr;

            root = configFile.CreateElement("AppSetting");

            node = configFile.CreateElement("CharInRow");
            attr = configFile.CreateAttribute("value");
            node.Attributes.Append(attr);
            root.AppendChild(node);

            node = configFile.CreateElement("LineInPage");
            attr = configFile.CreateAttribute("value");
            node.Attributes.Append(attr);
            root.AppendChild(node);

            node = configFile.CreateElement("PreView");
            attr = configFile.CreateAttribute("value");
            node.Attributes.Append(attr);
            root.AppendChild(node);

            configFile.AppendChild(root);
            configFile.Save(filePath);
        }
    }
}
