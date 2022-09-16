// See https://aka.ms/new-console-template for more information
using skrin_test;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Net;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Linq;

namespace MyApp // Note: actual namespace depends on the project name.
{
    internal class Program
    {   
        // Добавление в XML файл из базы
        public static void addXML()
        {
            string query = "SELECT users.user_second_name, users.user_surname, p.product_title, pur.purchase_date,  pp.product_count as _count, p.product_price as Price, SUM(p.product_price)*pp.product_count as total_price,  users.id, p.id " +
            "FROM purchase as pur " +
            "JOIN users ON pur.user_id = users.id " +
            "JOIN product_purchase as pp ON pp.purchase_id = pur.id " +
            "JOIN products as p ON pp.product_id = p.id " +
            "GROUP BY users.user_second_name,users.user_surname, pur.purchase_date, p.product_title, pp.product_count, p.product_price, users.id, p.id";


            SqlDataReader sqldata = DataBase.RunQuery(query);

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(@"D:\Visual studio\projects\skrin_test\XMLFile1.xml");
            XmlElement xRoot = xmlDocument.DocumentElement;



            if (sqldata.HasRows)
            {
                string name, surname, product, date;
                int count, price, totalprice, userID, productID;
                while (sqldata.Read())
                {
                    // создание элемента
                    XmlElement purchaseELem = xmlDocument.CreateElement("purchase");
                    // создание аттрибутов
                    XmlAttribute nameAttr = xmlDocument.CreateAttribute("name");
                    XmlAttribute nameAttr2 = xmlDocument.CreateAttribute("surname");
                    XmlAttribute nameAttr3 = xmlDocument.CreateAttribute("userID");
                    // создание элементов 
                    XmlElement productELem = xmlDocument.CreateElement("product");
                    XmlAttribute nameAttr4 = xmlDocument.CreateAttribute("productID");

                    XmlElement dateELem = xmlDocument.CreateElement("date");
                    XmlElement countELem = xmlDocument.CreateElement("count");
                    XmlElement priceELem = xmlDocument.CreateElement("price");
                    XmlElement totalpriceELem = xmlDocument.CreateElement("totalprice");

                    name = sqldata.GetString(0);
                    surname = sqldata.GetString(1);
                    product = sqldata.GetString(2);
                    date = String.Format("{0:dd/MM/yyyy HH:mm}", sqldata.GetDateTime(3));
                    count = sqldata.GetInt32(4);
                    price = sqldata.GetInt32(5);
                    totalprice = sqldata.GetInt32(6);
                    userID = sqldata.GetInt32(7);
                    productID = sqldata.GetInt32(8);

                    XmlText nameText = xmlDocument.CreateTextNode(name);
                    XmlText surnameText = xmlDocument.CreateTextNode(surname);
                    XmlText userIDText = xmlDocument.CreateTextNode(Convert.ToString(userID));
                    XmlText productText = xmlDocument.CreateTextNode(product);
                    XmlText productIDText = xmlDocument.CreateTextNode(Convert.ToString(productID));
                    XmlText dateText = xmlDocument.CreateTextNode(date);
                    XmlText countText = xmlDocument.CreateTextNode(Convert.ToString(count));
                    XmlText priceText = xmlDocument.CreateTextNode(Convert.ToString(price));
                    XmlText totalpriceText = xmlDocument.CreateTextNode(Convert.ToString(totalprice));

                    nameAttr.AppendChild(nameText);
                    nameAttr2.AppendChild(surnameText);
                    nameAttr3.AppendChild(userIDText);
                    nameAttr4.AppendChild(productIDText);

                    dateELem.AppendChild(dateText);
                    productELem.AppendChild(productText);
                    countELem.AppendChild(countText);
                    priceELem.AppendChild(priceText);
                    totalpriceELem.AppendChild(totalpriceText);

                    purchaseELem.Attributes.Append(nameAttr);
                    purchaseELem.Attributes.Append(nameAttr2);
                    purchaseELem.Attributes.Append(nameAttr3);

                    purchaseELem.AppendChild(dateELem);
                    purchaseELem.AppendChild(productELem);
                    purchaseELem.AppendChild(countELem);
                    purchaseELem.AppendChild(priceELem);
                    purchaseELem.AppendChild(totalpriceELem);

                    productELem.Attributes.Append(nameAttr4);

                    xRoot.AppendChild(purchaseELem);
                    xmlDocument.Save(@"D:\Visual studio\projects\skrin_test\XMLFile1.xml");
                }

            }
        }
        // Добавление из XML в БД
        public static void WritetoDB()
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(@"D:\Visual studio\projects\skrin_test\XMLFile2.xml");
            XmlElement element = xml.DocumentElement;
            foreach (XmlNode xnode in element)
            {
                DateTime date = DateTime.Now;
                int userID, count = 0, productID = 0, purchaseID = 0;

                XmlNode attruserID = xnode.Attributes.GetNamedItem("userID");
                userID = Convert.ToInt32(attruserID.Value);


                foreach (XmlNode childnode in xnode.ChildNodes)
                {
                    if (childnode.Name == "count")
                        count = Convert.ToInt32(childnode.InnerText);

                    if (childnode.Name == "date")
                        date = Convert.ToDateTime(childnode.InnerText);

                    if (childnode.Name == "product")
                    { 
                        XmlNode attrproductID = childnode.Attributes.GetNamedItem("productID");
                        productID = Convert.ToInt32(attrproductID.Value);

                    }

                }
                string query = "INSERT INTO purchase (user_id, purchase_date) VALUES(@param0, @param1)";
                List<Object> a = new List<Object>();
                a.Add(userID);
                a.Add(date);
                DataBase.XMLsave(query, a);

                string query2 = "SELECT TOP 1 id FROM purchase ORDER BY id DESC ";
                SqlDataReader sqlquery = DataBase.RunQuery(query2);

                while (sqlquery.Read())
                {
                    purchaseID = sqlquery.GetInt32(0);
                }
                string query3 = "INSERT INTO product_purchase (product_id, product_count, purchase_id) VALUES(@param0, @param1, @param2)";
                List<Object> b = new List<Object>();
                b.Add(productID);
                b.Add(count);
                b.Add(purchaseID);

                DataBase.XMLsave(query3, b);

            }
        }
        static void Main(string[] args)
        {
            // Добавление записей в XML файл
            addXML();
            //Чтение XML-файла и запись в базу
            WritetoDB();
           
        }
    }
}

