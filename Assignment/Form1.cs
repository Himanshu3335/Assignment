using ScrapingHelp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace googleReview
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string[] array_room_category;
        string[] array_alternate_hotel;
        private void button2_Click(object sender, EventArgs e)
        {
            string fileName = "supportFiles/Task 1-20210531T134048Z-001/Task 1/task 1 - Kempinski Hotel Bristol Berlin, Germany - Booking.com.HTML";

            var source = File.ReadAllText(Path.GetFullPath(fileName));

            string name = (HtmlHelper.ReturnValue(Regex.Replace(source, @"\s+", "^"), "", "<span^class=\"fn\"^id=\"hp_hotel_name\">", "</span>")).Replace("^", " ");

            string addres = (HtmlHelper.ReturnValue(Regex.Replace(source, @"\s+", "^"), "", "<span^id=\"hp_address_subtitle\"", "</span>")).Replace("^", " ");

            string address = (addres.Substring(addres.IndexOf(">"), (addres.Length - addres.IndexOf(">")))).Replace(">", "");

            string stars = (HtmlHelper.ReturnValue(Regex.Replace(source, @"\s+", "^"), "", "<i^class=\"b-sprite^stars", "star_track")).Replace("^", " ");

            string reviews_point = (HtmlHelper.ReturnValue(Regex.Replace(source, @"\s+", "^"), "", "<span^class=\"average^js--hp-scorecard-scoreval\">", "</span>")).Replace("^", " ");

            string reviews_count = (HtmlHelper.ReturnValue(Regex.Replace(source, @"\s+", "^"), "", "Score^from^<strong^class=\"count\">", "</strong>")).Replace("^", " ");


            string description = Regex.Replace((HtmlHelper.ReturnValue(Regex.Replace(source, @"\s+", "^"), "", "<div^class=\"hotel_description_wrapper_exp", "<div^class=\"property_hightlights_wrapper")).Replace("^", " "), "<.*?>", String.Empty);

            string room_category_detail = (HtmlHelper.ReturnValue(Regex.Replace(source, @"\s+", "^"), "", "<table^class=\"roomstable^rt_no_dates^__big-buttons^rt_lightbox_enabled^\"^id=\"maxotel_rooms\"^cellspacing=\"0\"^border=\"2\">", "</table>")).Replace("^", " ");

            array_room_category = HtmlHelper.CollectUrl(room_category_detail, "", "<td class=\"ftd\">", "</td>");


            string alternate_hotel_detail = (HtmlHelper.ReturnValue(Regex.Replace(source, @"\s+", "^"), "", "<table^cellspacing=\"0\"^id=\"althotelsTable\"^style=\"table-layout:fixed;width:100%;\">", "</table>")).Replace("^", " ");

            array_alternate_hotel = HtmlHelper.CollectUrl(alternate_hotel_detail, "", "<a class=\"althotel_link\"", "</a>");

            List<Hotel_detail> Hotel_detail = new List<Hotel_detail>();
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            Hotel_detail h_detail = new Hotel_detail
            {
                name = name,
                address = address,
                stars = stars,
                reviews_point = reviews_point,
                reviews_count = reviews_count,
                description = description,
                room_category = room_cat_data(),
                alternate_hotel = alternate_hotel_data()
            };
          

            Hotel_detail.Add(h_detail);

            var json = serializer.Serialize(Hotel_detail);
            TextWriter txt = new StreamWriter("supportFiles/Result.txt");
            txt.Write(json);
            txt.Close();


            MessageBox.Show("Done");
            Application.Exit();
        }

        public List<room_category> room_cat_data()
        {
            List<room_category> r_category = new List<room_category>();
            for (int i = 0; i < array_room_category.Length; i++)
            {
                r_category.Add(new room_category
                {
                   category_name = array_room_category[i].ToString()

                });
            }
            return r_category;
        }

        public List<alternate_hotel> alternate_hotel_data()
        {
            List<alternate_hotel> a_hotel = new List<alternate_hotel>();
            for (int i = 0; i < array_alternate_hotel.Length; i++)
            {
                a_hotel.Add(new alternate_hotel
                {
                    alternate_hotel_name = array_alternate_hotel[i].ToString().Substring(array_alternate_hotel[i].ToString().IndexOf(">"), (array_alternate_hotel[i].ToString().Length - array_alternate_hotel[i].ToString().IndexOf(">")))             

            });
            }
            return a_hotel;
        }
        public class Hotel_detail
        {
            public string name { get; set; }
            public string address { get; set; }
            public string stars { get; set; }
            public string reviews_point { get; set; }
            public string reviews_count { get; set; }
            public string description { get; set; }
            public List<room_category> room_category { get; set; }
            public List<alternate_hotel> alternate_hotel { get; set; }

        }
        public class room_category
        {
            public string category_name { get; set; }
        }

        public class alternate_hotel
        {
            public string alternate_hotel_name { get; set; }
        }
    }
}
