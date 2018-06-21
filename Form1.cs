using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace Komiwojazer
{
    public partial class Form1 : Form
    {
        string[] NazwyMiast;
        int[,] odleglosci;
        // bool[] dobry_odcinek;
        // int[,] najblizszysasiad ;
        int[][] pokolenie ;
        int[] DrogaOsobnika;
        int id_startowego = 100;
        int mutacje = 0;
        int iteracje = 0;
        int krzyzowanie = 0;
        int osobnicy = 0;
        string kraj;
        int miasta;
        int polowa_miast;
        bool moje;
        public Form1()
        {
            InitializeComponent();
            //NazwyMiast = new string[69];
            //odleglosci = new int[69, 69];
            //// dobry_odcinek= new bool[69];
            ////   najblizszysasiad = new int[69, 2];
            //pokolenie = new int[69][];
            //DrogaOsobnika = new int[69];


            //for(int i=0; i<69; i++)
            //{
            //    pokolenie[i] = new int[69];
            //    for(int j=0; j<69; j++)
            //    {
            //        pokolenie[i][j] = j;

            //    }

            //}



        }

        private void CzytajNazwy()
        {
            string wiersz;
            int id =0;
            StreamReader file = new System.IO.StreamReader("..\\..\\nazwy_miast_"+kraj);
            while ((wiersz = file.ReadLine()) != null)
            {
                NazwyMiast[id] = wiersz;
                id++;
            }

        }

        private void CzytajOdleglosci()
        {
            if (radioButtonPoland.Checked) { kraj = "poland.txt"; miasta = 69; }
            else if (radioButtonUsa.Checked) { kraj = "Usa.txt"; miasta = 48; }

            if (radioButtonMoje.Checked) moje = true;
            else if (radioButtonNieMoje.Checked) moje = false;

            polowa_miast = miasta / 2;
            NazwyMiast = new string[miasta];
            odleglosci = new int[miasta, miasta];

            mutacje = Convert.ToInt32(textBox3.Text);
            iteracje = Convert.ToInt32(textBox2.Text);
            krzyzowanie = Convert.ToInt32(textBox4.Text);
            osobnicy = Convert.ToInt32(textBox5.Text);



            
            //odleglosci = new int[69, 69];
            // dobry_odcinek= new bool[69];
            //   najblizszysasiad = new int[69, 2];
            pokolenie = new int[osobnicy][];
            DrogaOsobnika = new int[osobnicy];


            for (int i = 0; i < osobnicy; i++)
            {
                pokolenie[i] = new int[miasta];
                for (int j = 0; j < miasta; j++)
                {
                    pokolenie[i][j] = j;

                }

            }


            int id_miasta=0;
            using (StreamReader SR = new StreamReader("..\\..\\"+kraj))
            {
                string wiersz;
                while ((wiersz = SR.ReadLine()) != null)
                {
                    string[] WierszPodzielony = wiersz.Split(',');
                    for(int i=0; i<WierszPodzielony.Length;i++)
                    {
                        int zmienna = Convert.ToInt32(WierszPodzielony[i]);
                        if (zmienna == 0) break;
                        odleglosci[id_miasta, i] = zmienna;
                    }
                    id_miasta++;
                }
            }

            for (int i=0; i<miasta;i++)
            {
                for(int j=i; j<miasta;j++)
                {
                    odleglosci[i, j] = odleglosci[j, i];
                }
            }




        }





        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = null;

            string komunikat="";
            for(int i=0; i<NazwyMiast.Length; i++)
            {
                komunikat += NazwyMiast[i] + System.Environment.NewLine;
            }

            richTextBox1.Text = komunikat;
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            

            richTextBox1.Text = null;
            string komunikat="";
            for(int i=0; i<miasta; i++)
            {
                for(int j=0; j<miasta; j++)
                {

                    komunikat += Convert.ToString(odleglosci[i, j]) + " ";

                }

                komunikat += System.Environment.NewLine;
            }

            richTextBox1.Text = komunikat;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            CzytajOdleglosci();
            CzytajNazwy();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string nazwa_miasta_startowego = textBox1.Text;
            



            for (int i = 0; i < miasta; i++)
            {
                if (nazwa_miasta_startowego == NazwyMiast[i]) { id_startowego = i; break; }
            }
            string komunikat = "";
            if (id_startowego != 100) komunikat = nazwa_miasta_startowego + "    " + Convert.ToString(id_startowego);
            else { komunikat = "Nie znaleziono takiej nazwy ! - sprawdz poprawność i upewnij sie ze nie uzywasz polskich liter i ze wczytales dane"; richTextBox2.Text = komunikat; return; }
            richTextBox2.Text = komunikat;
            richTextBox1.Text = "";
         



           
       
            tasowanie( );


            //for (int i = 0; i < 69; i++)
            //{

            //    for (int j = 0; j < 69; j++)
            //    {
            //        richTextBox2.Text += pokolenie[i][j] + "  ";

            //    }
            //    richTextBox2.Text += Environment.NewLine;
            //}

            licz_droge();

            for (int i = 0; i < osobnicy; i++)
            {

                richTextBox1.Text += DrogaOsobnika[i] + Environment.NewLine;

            }
            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < iteracje; i++)
            {
                nowepokolenie();
                mutacja();
                licz_droge();
            }
            for (int i = 0; i < osobnicy; i++)
            {
                richTextBox1.Text += Environment.NewLine + DrogaOsobnika[i];
            }
            richTextBox1.Text += Environment.NewLine + sw.Elapsed;

            wypisz_trase();
        }

        private void tasowanie()
        {
            Random los = new Random();
            for (int k = 0; k < osobnicy; k++)
            {
                for (int i = 0; i < miasta; i++)
                {
                    int losowa = los.Next(0, miasta-1);
                    int temp = pokolenie[k][i];
                    pokolenie[k][i] = pokolenie[k][losowa];
                    pokolenie[k][losowa] = temp;

                }

                for (int i = 0; i < miasta; i++)
                {

                    if (pokolenie[k][i] == id_startowego)
                    {
                        int temp = pokolenie[k][0];
                        pokolenie[k][0] = id_startowego;
                        pokolenie[k][i] = temp;
                    }

                }
            }

            //return doprzetasowania;
        }

        private void licz_droge()
        {
            for (int k = 0; k < osobnicy; k++)
            {
                int suma = 0;
                int od = pokolenie[k][0];
                int doo;

                for (int i = 0; i < miasta-1; i++)
                {
                    doo = pokolenie[k][i + 1];
                    //for(int j=0; j<69; j++)
                    //{
                    //    if(doo==)
                    //}

                    suma += odleglosci[od, doo];
                    od = doo;

                }
                suma += odleglosci[od, pokolenie[k][0]];
                DrogaOsobnika[k] = suma;
            }
        }

        void nowepokolenie()
        {
            int[][] nowe = new int[krzyzowanie][];
            Random rng= new Random();
            int Ipolowa, IIpolowa, IIIpolowa , IVpolowa;

            for(int i=0; i<krzyzowanie; i++)
            {
                nowe[i] = new int[miasta];
                Ipolowa =rng.Next(0, osobnicy-1);
                IIpolowa = rng.Next(0, osobnicy-1);
                IIIpolowa = rng.Next(0, osobnicy - 1);
                IVpolowa = rng.Next(0, osobnicy - 1);
                if (DrogaOsobnika[Ipolowa] > DrogaOsobnika[IIIpolowa]) Ipolowa = IIIpolowa;
                if (DrogaOsobnika[IIpolowa] > DrogaOsobnika[IVpolowa]) IIpolowa = IVpolowa;


                while (Ipolowa==IIpolowa)IIpolowa = rng.Next(0, osobnicy-1);

                if(moje) nowe[i] = krzyzowanie_moje(nowe[i] , Ipolowa , IIpolowa);
                else nowe[i] = Crossover(pokolenie[Ipolowa], pokolenie[IIpolowa]);

                //nowe[i][0] = id_startowego;
                //for(int j=1; j<31; j++)
                //{
                //    nowe[i][j] = pokolenie[Ipolowa][j];
                //  //  richTextBox1.Text += nowe[i][j] + " ";
                //}


                //for(int j=31; j<miasta; j++)
                //{
                //    for(int k=1; k<miasta;k++)
                //    {
                //        bool wystapil = false;
                //        for (int l = 0; l < j; l++)
                //        {
                //            if (pokolenie[IIpolowa][k] == nowe[i][l])
                //            { wystapil = true; break; };
                //        }
                //        if (!wystapil) { //porownac
                //            nowe[i][j] = pokolenie[IIpolowa][k];
                //         //   richTextBox1.Text += nowe[i][j] + " ";
                //            break;
                //        }
                //    }
                //    //nowe[i][j] = pokolenie[IIpolowa][j];
                //    //richTextBox1.Text += nowe[i][j] + " ";
                //}

              //  richTextBox1.Text += Environment.NewLine ;
            }
            ///////////////////////Droga//////////////////////////
            int[] DrogaNowegoOsobnika = new int[krzyzowanie];
            for (int k = 0; k < krzyzowanie; k++)
            {
                int suma = 0;
                int od =nowe[k][0];
                int doo;

                for (int i = 0; i < miasta-1; i++)
                {
                    doo = nowe[k][i + 1];
                    suma += odleglosci[od, doo];
                    od = doo;

                }
                suma += odleglosci[od, nowe[k][0]];
                DrogaNowegoOsobnika[k] = suma;
              //  richTextBox1.Text += Environment.NewLine+ DrogaNowegoOsobnika[k];
            }
            int[] maksy = new int[krzyzowanie];
            int[] id_maksow = new int[krzyzowanie];
            //int maksimum = 0;
            //int id_maksimum = 2000;

            int[] polaczony =new int[osobnicy+krzyzowanie];
            int[] polaczonyOdleglosci = new int[osobnicy+krzyzowanie];
            // Array.Sort(DrogaOsobnika);
            //  Array.Sort(DrogaNowegoOsobnika);

            //Array.Sort(polaczony, DrogaOsobnika);
           // polaczony[i].Sort(DrogaNowegoOsobnika , DrogaOsobnika);
            for(int i=0; i< osobnicy; i++)
            {
                polaczony[i] = i;
                polaczonyOdleglosci[i] = DrogaOsobnika[i];

            }

            for(int i= osobnicy; i< osobnicy + krzyzowanie; i++)
            {
                polaczony[i] = i+(10000- osobnicy);
                polaczonyOdleglosci[i] = DrogaNowegoOsobnika[i- osobnicy];

            }
            Array.Sort( polaczonyOdleglosci, polaczony );
            
            for(int i=0; i<krzyzowanie; i++)
            {
                maksy[i] = polaczonyOdleglosci[(osobnicy + krzyzowanie-1) - i];
                id_maksow[i] = polaczony[(osobnicy + krzyzowanie-1) - i];
            }


            //    for (int i = 0; i < 69; i++)
            //    {
            //        if (DrogaOsobnika[i] > maksimum) { maksimum = DrogaOsobnika[i]; id_maksimum = i; }

            //    }

            //    for (int j = 0; j < 20; j++)
            //    {

            //        if (DrogaNowegoOsobnika[j] > maksimum) { maksimum = DrogaNowegoOsobnika[j]; id_maksimum = 100 + j; }
            //    }

            //    maksy[0] = maksimum;
            //    id_maksow[0] = id_maksimum;

            //   // richTextBox1.Text += "   "+id_maksow[0] +"  "+ maksy[0];
            //    for(int j=1; j<20; j++)
            //    {
            //        int maksimus = 0;
            //        for (int i = 0; i < 69; i++)
            //        {
            //           if(maksy[j-1]>DrogaOsobnika[i] &&DrogaOsobnika[i]>maksimus)
            //            {
            //                //bool kolejna_bezsensowna_zmienna = false;
            //                //for (int k = 0; k < 20; k++)
            //                //{
            //                //    if (i == id_maksow[k]) { kolejna_bezsensowna_zmienna = true; break; }

            //                //}
            //                //if (!kolejna_bezsensowna_zmienna)
            //                //{
            //                    maksimus = DrogaOsobnika[i];
            //                    id_maksimum = i;
            //                // }

            //                Array.Sort(DrogaOsobnika);
            //            }

            //        }

            //        for (int i = 0; i < 20; i++)
            //        {
            //            if (maksy[j - 1] > DrogaNowegoOsobnika[i] && DrogaNowegoOsobnika[i] > maksimus)
            //            {
            //                maksimus = DrogaNowegoOsobnika[i];
            //                id_maksimum = i+100;
            //            }

            //        }
            //        maksy[j] = maksimus;
            //        id_maksow[j] = id_maksimum;
            //   //     richTextBox1.Text += Environment.NewLine +id_maksow[j] + "  " + maksy[j];

            //    }

            //    //////////////////////Wymiana///////////////////////

            for (int i = 0; i < krzyzowanie; i++)
            {
                bool do_wywalenia = false;
                for (int j = 0; j < krzyzowanie; j++)
                {
                    if (i + 10000 == id_maksow[j]) { do_wywalenia = true; break; }
                }
                if (!do_wywalenia)
                {
                    for (int j = 0; j < krzyzowanie; j++)
                    {
                        if (id_maksow[j] < 10000)
                        {
                            for (int k = 1; k < miasta; k++)
                            {
                                int temp = nowe[i][k];
                                pokolenie[id_maksow[j]][k] = temp;


                            }
                            id_maksow[j] = 100000;
                            break;
                        }

                    }


                }

            }
            //    //licz_droge();

            //    //for(int i=0; i<69; i++)
            //    //{
            //    //    richTextBox1.Text += Environment.NewLine + DrogaOsobnika[i];
            //    //}
        }


        void mutacja()
        {

            Random los = new Random();
            for (int k = 0; k < mutacje; k++)
            {
                int losowanie_osobnika = los.Next(1, osobnicy);
                int losowanie_miasta = los.Next(1, miasta);
                int losowanie_drugiego_miasta = los.Next(1, miasta);
                int temp = pokolenie[losowanie_osobnika][losowanie_miasta];
                pokolenie[losowanie_osobnika][losowanie_miasta] = pokolenie[losowanie_osobnika][losowanie_drugiego_miasta];
                pokolenie[losowanie_osobnika][losowanie_drugiego_miasta] = temp;
            }

        }


        private void wypisz_trase()
        {
            int minimalna_droga = 100000000;
            int id_najlepszego=1000000;
            for(int i=0; i< osobnicy; i++)
            {
                if (DrogaOsobnika[i] < minimalna_droga) { id_najlepszego = i; minimalna_droga = DrogaOsobnika[i]; }



            }
            int suma = 0;
            for(int i=0; i< miasta; i++)
            {
                if (i != miasta-1)
                {
                    richTextBox2.Text += Environment.NewLine +"Z "+ NazwyMiast[pokolenie[id_najlepszego][i]]+ " do "+NazwyMiast[pokolenie[id_najlepszego][i+1]];
                    richTextBox2.Text += "   "+suma+" + " + odleglosci[pokolenie[id_najlepszego][i], pokolenie[id_najlepszego][i + 1]];
                    suma += odleglosci[pokolenie[id_najlepszego][i], pokolenie[id_najlepszego][i + 1]];
                    richTextBox2.Text +=" = "+suma;
                }
                else
                {
                    richTextBox2.Text+= Environment.NewLine + "Z " + NazwyMiast[pokolenie[id_najlepszego][i]] + " do " + NazwyMiast[pokolenie[id_najlepszego][0]];
                    richTextBox2.Text += "   " + suma + " + " + odleglosci[pokolenie[id_najlepszego][i], pokolenie[id_najlepszego][0]];
                    suma += odleglosci[pokolenie[id_najlepszego][i], pokolenie[id_najlepszego][0]];
                    richTextBox2.Text += " = "+suma;
                }
                
            }

            

            }
        //for (int i = 0; i < 69; i++)
        //{
        //    richTextBox1.Text += pokolenie[i][0] + Environment.NewLine;

        //}
        //int nastepny = pokolenie[startowy][0];
        //for (int i = 0; i < 69; i++) if (dobry_odcinek[i]) richTextBox2.Text += Environment.NewLine + NazwyMiast[i];
        //while (nastepny != startowy)
        //{
        //    richTextBox2.Text += Environment.NewLine + NazwyMiast[nastepny] + "  " + najblizszysasiad[nastepny, 1];
        //    //for (int i = 0; i < 69 ;i++ )
        //    //{
        //    //    if(i =)

        //    //}
        //    nastepny = najblizszysasiad[nastepny, 0];

        //}


        public int[] krzyzowanie_moje(int[] nowe, int Ipolowa, int IIpolowa)
        {

            nowe[0] = id_startowego;
            for (int j = 1; j < polowa_miast; j++)
            {
                nowe[j] = pokolenie[Ipolowa][j];
                //  richTextBox1.Text += nowe[i][j] + " ";
            }


            for (int j = polowa_miast; j < miasta; j++)
            {
                for (int k = 1; k < miasta; k++)
                {
                    bool wystapil = false;
                    for (int l = 0; l < j; l++)
                    {
                        if (pokolenie[IIpolowa][k] == nowe[l])
                        { wystapil = true; break; };
                    }
                    if (!wystapil)
                    { //porownac
                        nowe[j] = pokolenie[IIpolowa][k];
                        //   richTextBox1.Text += nowe[i][j] + " ";
                        break;
                    }
                }
 
            }
            return nowe;
        }


        public int[] Crossover(int[] parent1, int[] parent2)
        {
            int L = parent1.Length;
            int[] child = new int[L];
            for (int i = 0; i < L; i++)
                child[i] = -1;
            Random R = new Random();
            int startValue = R.Next(L);
            int k = 0; //liczba k


            child[k] = startValue;

            while (k < L)
            {

                for (int i = 0; i < L; i++)
                    if (parent2[i] == startValue)
                    {
                        if (i < L - 1)
                            child[k] = parent2[i + 1];
                        else
                            child[k] = parent2[0];

                        startValue = child[k++];
                        break;
                    }

                if (k > L - 1)
                    break;

                for (int i = 0; i < L; i++)
                    if (parent1[i] == startValue)
                    {
                        if (i < L - 1)
                            child[k] = parent1[i + 1];
                        else
                            child[k] = parent1[0];

                        startValue = child[k++];
                        break;
                    }
            }

            bool[] brakujace = new bool[miasta];
           // int brakujacy=0;

            for (int j = 0; j < miasta; j++)
            {
                bool tak = true;
                for (int i = 0; i < miasta; i++)
                {
                    if (j == child[i]) {tak = false;break; }

                }
                if (tak) brakujace[j]= true;
            }

            int powtorzony = 0;
            bool[] powtorzony_id= new bool[miasta];
            for (int i = 0; i < miasta; i++)
                powtorzony_id[i] = false;

            for (int i = 0; i < miasta; i++)
            {
                powtorzony = i;
                bool byl = false;
                for (int j = 0; j < miasta; j++)
                {

                    if (powtorzony == child[j] && byl){ powtorzony_id[j] = true; }
                    else if (powtorzony == child[j]) byl = true;

                }

            }

         //   child[powtorzony_id] = brakujacy;

            for(int i=0; i<miasta; i++)
            {

                if(powtorzony_id[i])
                {
                    for(int j=0; j<miasta; j++)
                        if(brakujace[j])
                        {
                            child[i] = j;
                            brakujace[j] = false;
                            break;
                        }

                }

            }


            for (int i = 0; i < miasta; i++)
            {
                if (child[i] == id_startowego)
                {
                    int temp = child[0];
                    child[0] = child[i];
                    child[i] = temp;
                    break;
                }
            }


            return child;
        }


        //Dramat......
        //    for(int i=0; i<69; i++)
        //    {
        //        int minimalna_droga = 1000;
        //        int id_sasiada = 0;
        //        for(int j=0; j<69; j++)
        //        {
        //            if (i == j) continue;
        //            if (odleglosci[i, j] < minimalna_droga) { minimalna_droga = odleglosci[i, j]; id_sasiada = j; }

        //        }

        //        najblizszysasiad[i,0] = id_sasiada;
        //        najblizszysasiad[i,1] = minimalna_droga;

        //    }

        //    richTextBox2.Text += Environment.NewLine;

        //    for(int i=0; i<69; i++)
        //    {
        //        richTextBox2.Text += NazwyMiast[i] + "   " + NazwyMiast[najblizszysasiad[i, 0]] + "    " 
        //            + Convert.ToString(najblizszysasiad[i, 1])+ Environment.NewLine;

        //    }
        //    //miasta które nie wystapiły
        //    //for(int i=0; i<69; i++)
        //    //{
        //    //    bool byl = false;
        //    //    for(int j=0; j<69; j++)
        //    //    {
        //    //        if (i == najblizszysasiad[j, 0]) byl = true ;


        //    //    }
        //    //    if(!byl) richTextBox2.Text += NazwyMiast[i] + Environment.NewLine;
        //    // }
        //    //martwe punkty

        //    // for (int j = 0; j < 69; j++)
        //    // {
        //    // if (j == najblizszysasiad[najblizszysasiad[j, 0], 0])
        //    // {
        //    //     // richTextBox1.Text += NazwyMiast[j] + Environment.NewLine;
        //    //     // ilemartwych++;
        //    // }
        //    // // else dobry_odcinek[najblizszysasiad[najblizszysasiad[j, 0], 0]] = true;
        //    //// else dobry_odcinek[j] = true;

        //    // }


        //    for (int i = 0; i < 69; i++)
        //    {
        //        bool byl = false;
        //        for (int j = 0; j < 69; j++)
        //        {
        //            if (i == najblizszysasiad[j, 0] && byl){ byl = false; break; }
        //            else if (i == najblizszysasiad[j, 0]) byl = true;


        //        }
        //        if (byl) richTextBox2.Text += NazwyMiast[i] + Environment.NewLine;
        //        if (byl) dobry_odcinek[i] = true;
        //    }


        //    for (int j = 0; j < 69; j++)
        //    {
        //            if(!dobry_odcinek[j]) richTextBox1.Text += NazwyMiast[j] + Environment.NewLine;
        //    }


        //    for (int i = 0; i < 69; i++)
        //    {
        //        if (i == najblizszysasiad[najblizszysasiad[i, 0], 0]) RozwiazProblemMartwych(i, najblizszysasiad[i, 0]);
        //    }


        //    for (int i = 0; i < 69; i++)
        //    {
        //        int pierwszewystapienie = 100;
        //        for (int j = 0; j < 69; j++)
        //        {
        //            if (najblizszysasiad[j, 0] == i && pierwszewystapienie != 100) pierwszewystapienie =RozwiazProblemPowtorek(pierwszewystapienie, j);
        //            else if (najblizszysasiad[j, 0] == i) pierwszewystapienie = j;
        //        }
        //        if(pierwszewystapienie!=100)dobry_odcinek[najblizszysasiad[pierwszewystapienie,0]] = true;
        //    }



        //    wypisz_trase(id_startowego);


        //}

        //private void RozwiazProblemMartwych(int jeden , int dwa)
        //{
        //    int minimum_pierwszego=1000, minimum_drugiego=1000;
        //    int id_pierwszego=0, id_drugiego=0;

        //        for(int i=0; i<69; i++)
        //        {

        //            if (dobry_odcinek[i]) continue;
        //            if (i == jeden || i==dwa) continue;

        //            if (odleglosci[jeden, i] < minimum_pierwszego)
        //            {
        //            minimum_pierwszego = odleglosci[jeden, i];
        //                id_pierwszego = i;
        //            }

        //            if (odleglosci[dwa, i] < minimum_drugiego)
        //            {
        //                minimum_drugiego = odleglosci[dwa, i];
        //                id_drugiego = i;
        //            }


        //        }


        //        if(minimum_pierwszego<minimum_drugiego)
        //        {
        //        najblizszysasiad[jeden, 0] = id_pierwszego;
        //        najblizszysasiad[jeden, 1] = minimum_pierwszego;
        //        dobry_odcinek[id_pierwszego] = true;

        //        najblizszysasiad[dwa, 0] = jeden;
        //        najblizszysasiad[dwa, 1] = odleglosci[dwa, jeden];
        //        dobry_odcinek[jeden] = true;
        //        dobry_odcinek[dwa] = false;


        //        }
        //    else
        //    {
        //        najblizszysasiad[dwa, 0] = id_drugiego;
        //        najblizszysasiad[dwa, 1] = minimum_drugiego;
        //        dobry_odcinek[id_drugiego] = true;

        //        najblizszysasiad[jeden, 0] = dwa;
        //        najblizszysasiad[jeden, 1] = odleglosci[jeden, dwa];
        //        dobry_odcinek[dwa] = true;
        //        dobry_odcinek[jeden] = false;
        //    }

        //}


        //private int RozwiazProblemPowtorek(int jeden, int dwa)
        //{
        //    int minimum_pierwszego = 1000, minimum_drugiego = 1000;
        //    int id_pierwszego=0 , id_drugiego =0;
        //    int pierwotny = najblizszysasiad[jeden, 0];

        //    for (int i = 0; i < 69; i++)
        //    {

        //        if (dobry_odcinek[i]) continue;
        //     //   if (i == jeden || i == dwa) continue;

        //        if (odleglosci[jeden, i] < minimum_pierwszego && i!=jeden)
        //        {
        //            minimum_pierwszego = odleglosci[jeden, i];
        //            id_pierwszego = i;
        //        }

        //        if (odleglosci[dwa, i] < minimum_drugiego && i!=dwa)
        //        {
        //            minimum_drugiego = odleglosci[dwa, i];
        //            id_drugiego = i;
        //        }


        //    }


        //    if (minimum_pierwszego < minimum_drugiego)
        //    {
        //        najblizszysasiad[jeden, 0] = id_pierwszego;
        //        najblizszysasiad[jeden, 1] = minimum_pierwszego;
        //        dobry_odcinek[id_pierwszego] = true;
        //        return  dwa;
        //    }
        //    else
        //    {
        //        najblizszysasiad[dwa, 0] = id_drugiego;
        //        najblizszysasiad[dwa, 1] = minimum_drugiego;
        //        dobry_odcinek[id_drugiego] = true;
        //        return jeden;
        //    }

        //}








    }
    }
