using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;
/// @author ansakatu
/// @version 04.12.2020
/// <summary>
///  Tofu-nimisen norjalaisen metsäkissan lempiruokaa ovat kärpäset. Pelissä autetaan Tofua syömään kaikki
///  kärpäset, jotka tulevat eteen.
/// </summary>
public class KarpasenMetsastys : PhysicsGame
{
    private const int KARPASIA = 25;
    private PhysicsObject Pelaaja1;
    private List<double> pisteet = new List<double>();
    private bool kaikkiSyoty = false;


    /// <summary>
    /// Aloitetaan peli
    /// </summary>
    public override void Begin()
    {
        AlustaKentta();
        Pelaaja1 = LuoPelaaja(this, 0, "pelaaja1");
        AlustaKarpaset();
        AddCollisionHandler(Pelaaja1, "karpanen", Tormays);
        AsetaOhjaimet();
    }


    /// <summary>
    /// Alustaa kentän pelille
    /// </summary>
    private void AlustaKentta()
    {
        Background tausta = Level.Background;
        tausta.Image = LoadImage("tausta");
        tausta.ScaleToLevelFull();

        Level.CreateBorders();
        Camera.ZoomToLevel();
    }


    /// <summary>
    /// Alustaa kärpäset peliin
    /// </summary>
    private void AlustaKarpaset()
    {
        BoundingRectangle oikea = new BoundingRectangle(new Vector(Level.Right, 300), Level.BoundingRect.BottomRight);

        for (int i = 0; i <= KARPASIA; i++)
            LuoSatunnainenKarpanen(this, oikea, 250, "karpanen");
    }


    /// <summary>
    /// Asetetaan ohjaimet pelille
    /// </summary>
    private void AsetaOhjaimet()
    {
        Keyboard.Listen(Key.F1, ButtonState.Pressed, ShowControlHelp, "Näytä info");
        Keyboard.Listen(Key.F5, ButtonState.Pressed, Begin, "Uusi peli");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, Exit, "Lopeta peli");
        Keyboard.Listen(Key.W, ButtonState.Pressed, Liiku, "Pelaaja ylös", Pelaaja1, new Vector(0, 200));
        Keyboard.Listen(Key.S, ButtonState.Pressed, Liiku, "Pelaaja alas", Pelaaja1, new Vector(0, -200));
        Keyboard.Listen(Key.A, ButtonState.Pressed, Liiku, "Pelaaja vasemmalle", Pelaaja1, new Vector(-200, 0));
        Keyboard.Listen(Key.D, ButtonState.Pressed, Liiku, "Pelaaja oikealle", Pelaaja1, new Vector(200, 0));
        Keyboard.Listen(Key.Q, ButtonState.Pressed, OnkoKaikkiSyoty, "Kärpäset syöty");
    }


    /// <summary>
    /// Luo uuden pelaajan
    /// </summary>
    /// <param name="peli">peli, johon kolmio luodaan</param>
    /// <param name="vauhti">pelaajan vauhti alussa</param>
    /// <returns>pelaaja</returns>
    private PhysicsObject LuoPelaaja(PhysicsGame peli, double vauhti, string tunniste)
    {
        double leveys = 100.0;
        double korkeus = 150.0;

        PhysicsObject pelaaja = new PhysicsObject(leveys, korkeus, Shape.Circle);
        pelaaja.Position = new Vector(peli.Level.Left + 100, 50);

        Vector suunta = RandomGen.NextVector(0, vauhti);
        pelaaja.Hit(suunta);
        pelaaja.Tag = tunniste;
        pelaaja.Image = LoadImage("tofu");
        peli.Add(pelaaja);
        return pelaaja;
    }


    /// <summary>
    /// Luodaan Kärpäsiä satunnaisesti
    /// </summary>
    /// <param name="peli">peli, johon kärpäset luodaan</param>
    /// <param name="rect">suorakaide johon kärpäset luodaan</param>
    /// <param name="vauhti">kärpäsen vauhti</param>
    /// <returns>kärpänen</returns>
    private PhysicsObject LuoSatunnainenKarpanen(PhysicsGame peli, BoundingRectangle rect, double vauhti, string tunniste)
    {
        double leveys = RandomGen.NextDouble(5, 25);
        double korkeus = leveys;

        PhysicsObject karpanen = new PhysicsObject(leveys, korkeus, Shape.Circle);
        karpanen.Position = RandomGen.NextVector(rect);
        karpanen.Angle = RandomGen.NextAngle();

        Vector suunta = RandomGen.NextVector(0, vauhti);
        karpanen.Hit(suunta);
        karpanen.Tag = tunniste;
        karpanen.Image = LoadImage("sirkarpanen");
        peli.Add(karpanen);
        return karpanen;
    }


    /// <summary>
    /// Tofu liikkuu
    /// </summary>
    /// <param name="pelaaja">Tofu</param>
    /// <param name="suunta">suunta, johon Tofu liikkuu</param>
    private void Liiku(PhysicsObject pelaaja, Vector suunta)
    {
        pelaaja.Hit(suunta);
    }


    /// <summary>
    /// Tofu törmää kärpäseen, syö sen ja saa pisteitä
    /// </summary>
    /// <param name="pelaaja">Tofu</param>
    /// <param name="karpanen">Kärpänen</param>
    private void Tormays(PhysicsObject pelaaja, PhysicsObject karpanen)
    {
        this.pisteet.Add(karpanen.Height);
        CollisionHandler.DestroyTarget(pelaaja, karpanen);
    }


    /// <summary>
    /// Tarkistetaan, onko kaikki kärpäset syöty
    /// </summary>
    private void OnkoKaikkiSyoty()
    {
        if (this.pisteet.Count >= KARPASIA) kaikkiSyoty = true;

        if (kaikkiSyoty == true) NaytaPisteet(this);
    }


    /// <summary>
    /// Laskee saadut pisteet
    /// </summary>
    /// <param name="pisteet">lista, jolta pisteet haetaan</param>
    /// <returns></returns>
    private double LaskePisteet(List<double> p)
    {
        double summa = 0;
        for (int i = 0; i < p.Count; i++)
            summa += p[i];
        return summa;
    }


    /// <summary>
    /// Näyttää pelaajan saamat pisteet
    /// </summary>
    /// <param name="peli">tämä peli</param>
    private void NaytaPisteet(PhysicsGame peli)
    {
        double p = LaskePisteet(this.pisteet);

        String pisteita = p.ToString("#.0");

        Label label = new Label("Pisteet: " + pisteita);
        label.Color = Color.White;
        label.BorderColor = Color.Transparent;
        label.Position = Level.Center;
        label.Font = new Font(70, true);
        peli.Add(label);
    }


}

